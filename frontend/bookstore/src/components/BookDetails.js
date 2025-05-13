import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';

const BookDetails = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [book, setBook] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [isAdmin, setIsAdmin] = useState(false);

  useEffect(() => {
    // Проверяем, является ли пользователь администратором
    const userRole = localStorage.getItem('userRole');
    setIsAdmin(userRole === 'admin');
  }, []);

  useEffect(() => {
    const fetchBookDetails = async () => {
      try {
        const response = await fetch(`http://localhost:5059/Books/${id}`, {
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

        const bookData = await response.json();
        setBook(bookData);
      } catch (err) {
        setError(`Ошибка при загрузке информации о книге: ${err.message}`);
        console.error('Ошибка при загрузке информации о книге:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchBookDetails();
  }, [id]);

  const handleEditBook = () => {
    navigate(`/books/edit/${id}`);
  };

  const handleDeleteBook = async () => {
    if (window.confirm(`Вы уверены, что хотите удалить книгу "${book.title}"?`)) {
      try {
        const response = await fetch(`http://localhost:5059/Books/${id}`, {
          method: 'DELETE',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        });

        if (!response.ok) {
          throw new Error(`HTTP error! Status: ${response.status}`);
        }

        navigate('/');
      } catch (err) {
        setError(`Ошибка при удалении книги: ${err.message}`);
        console.error('Ошибка при удалении книги:', err);
      }
    }
  };

  const handleViewAuthor = (authorId) => {
    navigate(`/authors/${authorId}`);
  };

  const handleViewCategory = (categoryId) => {
    navigate(`/categories/${categoryId}`);
  };

  const addToCart = async (quantity = 1) => {
    if (!localStorage.getItem('token')) {
      navigate('/login');
      return;
    }

    try {
      const response = await fetch('http://localhost:5059/cart/items', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify({
          bookId: book.id,
          quantity: quantity
        })
      });

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }

      toast.success('Книга добавлена в корзину!');
    } catch (err) {
      setError(`Ошибка при добавлении в корзину: ${err.message}`);
      console.error('Ошибка при добавлении в корзину:', err);
      toast.error('Не удалось добавить книгу в корзину');
    }
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
          onClick={() => navigate('/')}
        >
          Вернуться к списку книг
        </button>
      </div>
    );
  }

  if (!book) {
    return (
      <div className="container mt-4">
        <div className="alert alert-warning" role="alert">
          Книга не найдена
        </div>
        <button
          className="btn btn-primary"
          onClick={() => navigate('/')}
        >
          Вернуться к списку книг
        </button>
      </div>
    );
  }

  return (
    <div className="container mt-4">
      <div className="card">
        <div className="card-header d-flex justify-content-between align-items-center">
          <h3>Детали книги</h3>
          {isAdmin && (
            <div>
              <button 
                className="btn btn-warning me-2" 
                onClick={handleEditBook}
              >
                <i className="bi bi-pencil me-1"></i> Редактировать
              </button>
              <button 
                className="btn btn-danger" 
                onClick={handleDeleteBook}
              >
                <i className="bi bi-trash me-1"></i> Удалить
              </button>
            </div>
          )}
        </div>
        
        <div className="card-body">
          <div className="row mb-4">
            {book.imageUrl && (
              <div className="col-md-4">
                <div className="book-detail-image-container mb-3">
                  <img 
                    src={`http://localhost:5059${book.imageUrl}`}
                    alt={book.title}
                    className="book-detail-image"
                    onError={(e) => {
                      console.log(`Ошибка загрузки изображения для "${book.title}"`);
                      e.target.style.display = 'none';
                    }}
                  />
                </div>
              </div>
            )}
            <div className={book.imageUrl ? "col-md-8" : "col-md-12"}>
              <h4>{book.title}</h4>
              <p className="text-muted">ID книги: {book.id}</p>
              <div className="mt-3">
                <h5>Описание:</h5>
                <p>{book.description || 'Описание отсутствует'}</p>
              </div>
            </div>
          </div>
          
          <div className="row">
            <div className={book.imageUrl ? "col-md-8 offset-md-4" : "col-md-12"}>
              <div className="card bg-light">
                <div className="card-body">
                  <h5 className="card-title">Информация о книге</h5>
                  <p className="mb-1"><strong>Цена:</strong> {book.price} ₽</p>
                  <p className="mb-1"><strong>Год издания:</strong> {book.yearOfRelease}</p>
                  <p className="mb-1">
                    <strong>Автор:</strong>{' '}
                    {book.author ? (
                      <a 
                        href="#" 
                        onClick={(e) => {
                          e.preventDefault();
                          handleViewAuthor(book.author.id);
                        }}
                      >
                        {book.author.name} {book.author.surname}
                      </a>
                    ) : 'Нет автора'}
                  </p>
                  <p className="mb-1">
                    <strong>Категория:</strong>{' '}
                    {book.category ? (
                      <a 
                        href="#" 
                        onClick={(e) => {
                          e.preventDefault();
                          handleViewCategory(book.category.id);
                        }}
                      >
                        {book.category.name}
                      </a>
                    ) : 'Нет категории'}
                  </p>
                  
                  {localStorage.getItem('userRole') !== 'admin' && (
                    <div className="mt-4">
                      <button 
                        className="btn btn-success btn-lg w-100" 
                        onClick={() => addToCart(1)}
                      >
                        <i className="bi bi-cart-plus me-2"></i>
                        Добавить в корзину
                      </button>
                    </div>
                  )}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default BookDetails; 