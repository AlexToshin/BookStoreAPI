import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

const CategoriesList = () => {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();
  const userRole = localStorage.getItem('userRole');
  const isAdmin = userRole === 'admin';

  const fetchCategories = async () => {
    try {
      setLoading(true);
      const response = await fetch('http://localhost:5059/Categories', {
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
      setCategories(data);
      setError(null);
    } catch (err) {
      setError(err.message);
      console.error('Ошибка при получении категорий:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCategories();
  }, []);

  const handleDelete = async (id) => {
    if (window.confirm('Вы уверены, что хотите удалить эту категорию?')) {
      try {
        const response = await fetch(`http://localhost:5059/Categories/${id}`, {
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

        // Обновить список категорий после успешного удаления
        fetchCategories();
      } catch (err) {
        setError(err.message);
        console.error('Ошибка при удалении категории:', err);
      }
    }
  };

  const handleViewDetails = (id) => {
    navigate(`/categories/${id}`);
  };

  const handleEditCategory = (id) => {
    navigate(`/categories/edit/${id}`);
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
      <h2 className="text-center my-4">Категории</h2>
      
      {isAdmin && (
        <div className="d-flex justify-content-center mb-3">
          <button 
            className="btn btn-sm btn-primary"
            onClick={() => navigate('/categories/create')}
            style={{ width: '150px' }}
          >
            Добавить категорию
          </button>
        </div>
      )}

      {categories.length === 0 ? (
        <div className="alert alert-info">Категории не найдены</div>
      ) : (
        <div className="table-responsive">
          <table className="table table-striped table-hover">
            <thead className="table-dark">
              <tr>
                <th>Название</th>
                <th>Описание</th>
                <th>Действия</th>
              </tr>
            </thead>
            <tbody>
              {categories.map(category => (
                <tr key={category.id}>
                  <td>{category.name}</td>
                  <td>{category.description}</td>
                  <td>
                    <div className="btn-group" role="group">
                      <button 
                        className="btn btn-sm btn-info me-2"
                        onClick={() => handleViewDetails(category.id)}
                      >
                        Просмотр
                      </button>
                      {isAdmin && (
                        <>
                          <button 
                            className="btn btn-sm btn-warning me-2"
                            onClick={() => handleEditCategory(category.id)}
                          >
                            Редактировать
                          </button>
                          <button
                            className="btn btn-sm btn-danger"
                            onClick={() => handleDelete(category.id)}
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

export default CategoriesList;