import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

const AuthorsList = () => {
  const [authors, setAuthors] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();
  const userRole = localStorage.getItem('userRole');
  const isAdmin = userRole === 'admin';

  const fetchAuthors = async () => {
    try {
      setLoading(true);
      const response = await fetch('http://localhost:5059/Authors', {
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
        throw new Error(`Ошибка HTTP: ${response.status}`);
      }

      const data = await response.json();
      setAuthors(data);
      setError(null);
    } catch (err) {
      setError(err.message);
      console.error('Ошибка при получении авторов:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAuthors();
  }, []);

  const handleDelete = async (id) => {
    if (window.confirm('Вы уверены, что хотите удалить этого автора?')) {
      try {
        const response = await fetch(`http://localhost:5059/Authors/${id}`, {
          method: 'DELETE',
          headers: {
            'Content-Type': 'application/json',
            ...(localStorage.getItem('token') ? {
              'Authorization': `Bearer ${localStorage.getItem('token')}`
            } : {})
          }
        });

        if (!response.ok) {
          throw new Error(`Ошибка HTTP: ${response.status}`);
        }

        // Обновить список авторов после успешного удаления
        fetchAuthors();
      } catch (err) {
        setError(err.message);
        console.error('Ошибка при удалении автора:', err);
      }
    }
  };

  const handleViewDetails = (id) => {
    navigate(`/authors/${id}`);
  };

  const handleEditAuthor = (id) => {
    navigate(`/authors/edit/${id}`);
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
          Ошибка: {error}
        </div>
      </div>
    );
  }

  return (
    <div className="container mt-4">
      <h2 className="text-center my-4">Авторы</h2>
      
      {isAdmin && (
        <div className="d-flex justify-content-center mb-3">
          <button 
            className="btn btn-sm btn-primary"
            onClick={() => navigate('/authors/create')}
            style={{ width: '150px' }}
          >
            Добавить автора
          </button>
        </div>
      )}

      {authors.length === 0 ? (
        <div className="alert alert-info">Авторы не найдены</div>
      ) : (
        <div className="table-responsive">
          <table className="table table-striped table-hover">
            <thead className="table-dark">
              <tr>
                <th>Имя</th>
                <th>Фамилия</th>
                <th>Действия</th>
              </tr>
            </thead>
            <tbody>
              {authors.map(author => (
                <tr key={author.id}>
                  <td>{author.name}</td>
                  <td>{author.surname}</td>
                  <td>
                    <div className="btn-group" role="group">
                      <button 
                        className="btn btn-sm btn-info me-2"
                        onClick={() => handleViewDetails(author.id)}
                      >
                        Просмотр
                      </button>
                      {isAdmin && (
                        <>
                          <button 
                            className="btn btn-sm btn-warning me-2"
                            onClick={() => handleEditAuthor(author.id)}
                          >
                            Редактировать
                          </button>
                          <button
                            className="btn btn-sm btn-danger"
                            onClick={() => handleDelete(author.id)}
                          >
                            Удалить
                          </button>
                        </>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default AuthorsList;