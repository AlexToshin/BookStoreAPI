import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';

const CategoryDetails = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [category, setCategory] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchCategoryDetails = async () => {
      try {
        const response = await fetch(`http://localhost:5059/Categories/${id}`, {
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

        const categoryData = await response.json();
        setCategory(categoryData);
      } catch (err) {
        setError(`Ошибка при загрузке информации о категории: ${err.message}`);
        console.error('Ошибка при загрузке информации о категории:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchCategoryDetails();
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
          onClick={() => navigate('/categories')}
        >
          Вернуться к списку категорий
        </button>
      </div>
    );
  }

  if (!category) {
    return (
      <div className="container mt-4">
        <div className="alert alert-warning" role="alert">
          Категория не найдена
        </div>
        <button
          className="btn btn-primary"
          onClick={() => navigate('/categories')}
        >
          Вернуться к списку категорий
        </button>
      </div>
    );
  }

  return (
    <div className="container mt-4">
      <div className="card">
        <div className="card-header bg-info text-white">
          <div className="d-flex justify-content-between align-items-center">
            <h3 className="mb-0">Информация о категории</h3>
            <div>
              <button 
                className="btn btn-warning me-2"
                onClick={() => navigate(`/categories/edit/${id}`)}
              >
                Редактировать
              </button>
              <button
                className="btn btn-primary"
                onClick={() => navigate('/categories')}
              >
                Назад к списку
              </button>
            </div>
          </div>
        </div>
        <div className="card-body">
          <div className="row mb-4">
            <div className="col-md-6">
              <h4>{category.name}</h4>
              <p className="text-muted">ID категории: {category.id}</p>
              <div className="mt-3">
                <h5>Описание:</h5>
                <p>{category.description || 'Описание отсутствует'}</p>
              </div>
            </div>
          </div>

          <h5 className="mb-3">Книги в категории</h5>
          {category.books && category.books.length > 0 ? (
            <div className="table-responsive">
              <table className="table table-striped">
                <thead>
                  <tr>
                    <th>Название</th>
                    <th>Автор</th>
                    <th>Год издания</th>
                    <th>Действия</th>
                  </tr>
                </thead>
                <tbody>
                  {category.books.map(book => (
                    <tr key={book.id}>
                      <td>{book.title}</td>
                      <td>{book.author ? `${book.author.name} ${book.author.surname}` : 'Нет автора'}</td>
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
              В этой категории пока нет книг
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default CategoryDetails; 