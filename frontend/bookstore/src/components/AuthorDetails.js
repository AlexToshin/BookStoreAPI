import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';

const AuthorDetails = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [author, setAuthor] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchAuthorDetails = async () => {
      try {
        const response = await fetch(`http://localhost:5059/Authors/${id}`, {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            ...(localStorage.getItem('token') ? {
              'Authorization': `Bearer ${localStorage.getItem('token')}`
            } : {})
          }
        });

        if (!response.ok) {
          throw new Error(`HTTP error! Status: ${response.status}`);
        }

        const authorData = await response.json();
        setAuthor(authorData);
      } catch (err) {
        setError(`Ошибка при загрузке информации об авторе: ${err.message}`);
        console.error('Ошибка при загрузке информации об авторе:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchAuthorDetails();
  }, [id]);

  const handleViewBook = (bookId) => {
    navigate(`/books/${bookId}`);
  };

  if (loading) {
    return (
      <div className="container d-flex justify-content-center align-items-center" style={{ minHeight: '50vh' }}>
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Загрузка...</span>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mt-4">
        <div className="alert alert-danger" role="alert">
          {error}
        </div>
        <button
          className="btn btn-primary"
          onClick={() => navigate('/authors')}
        >
          Вернуться к списку авторов
        </button>
      </div>
    );
  }

  if (!author) {
    return (
      <div className="container mt-4">
        <div className="alert alert-warning" role="alert">
          Автор не найден
        </div>
        <button
          className="btn btn-primary"
          onClick={() => navigate('/authors')}
        >
          Вернуться к списку авторов
        </button>
      </div>
    );
  }

  return (
    <div className="container mt-4">
      <div className="card">
        <div className="card-header bg-info text-white">
          <div className="d-flex justify-content-between align-items-center">
            <h3 className="mb-0">Информация об авторе</h3>
            <div>
              <button 
                className="btn btn-warning me-2"
                onClick={() => navigate(`/authors/edit/${id}`)}
              >
                Редактировать
              </button>
              <button
                className="btn btn-primary"
                onClick={() => navigate('/authors')}
              >
                Назад к списку
              </button>
            </div>
          </div>
        </div>
        <div className="card-body">
          <div className="row mb-4">
            <div className="col-md-6">
              <h4>{author.name} {author.surname}</h4>
              <p className="text-muted">ID автора: {author.id}</p>
            </div>
          </div>

          <h5 className="mb-3">Книги автора</h5>
          {author.books && author.books.length > 0 ? (
            <div className="table-responsive">
              <table className="table table-striped">
                <thead>
                  <tr>
                    <th>Название</th>
                    <th>Категория</th>
                    <th>Год издания</th>
                    <th>Действия</th>
                  </tr>
                </thead>
                <tbody>
                  {author.books.map(book => (
                    <tr key={book.id}>
                      <td>{book.title}</td>
                      <td>{book.category ? book.category.name : 'Нет категории'}</td>
                      <td>{book.yearOfRelease}</td>
                      <td>
                        <button 
                          className="btn btn-sm btn-info"
                          onClick={() => handleViewBook(book.id)}
                        >
                          Подробнее
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : (
            <div className="alert alert-info">
              У этого автора пока нет книг
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default AuthorDetails; 