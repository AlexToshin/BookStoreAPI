import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

function ProductCard({ book, onDelete, isAdmin }) {
  const navigate = useNavigate();
  
  // Добавляем логирование для отладки
  useEffect(() => {
    console.log(`Карточка книги "${book.title}":`, book);
    console.log(`Изображение для "${book.title}":`, book.imageUrl);
  }, [book]);

  const handleEdit = () => {
    navigate(`/books/edit/${book.id}`);
  };

  const handleDelete = () => {
    if (window.confirm(`Вы уверены, что хотите удалить книгу "${book.title}"?`)) {
      onDelete(book.id);
    }
  };
  
  const handleCardClick = () => {
    navigate(`/books/${book.id}`);
  };

  return (
    <div className="card" onClick={handleCardClick} style={{ cursor: 'pointer' }}>
      {book.imageUrl && (
        <div className="book-image-container">
          <img 
            src={`http://localhost:5059${book.imageUrl}`}
            alt={book.title}
            className="book-image"
            onError={(e) => {
              console.log(`Ошибка загрузки изображения для "${book.title}"`);
              e.target.style.display = 'none';
            }}
          />
        </div>
      )}
      <div className="card-body">
        <h2 className="card-title">{book.title}</h2>
        <p className="card-text">{book.description}</p>
        <div className="book-details">
          <p className="category">Категория: {book.category ? book.category.name : 'Нет категории'}</p>
          <p className="author">Автор: {book.author ? `${book.author.name} ${book.author.surname}` : 'Нет автора'}</p>
          <p className="price">Цена: {book.price} ₽</p>
        </div>
        {isAdmin && (
          <div className="d-flex justify-content-end mt-3" onClick={(e) => e.stopPropagation()}>
            <button 
              className="btn btn-sm btn-warning me-2" 
              onClick={handleEdit}
              title="Редактировать"
            >
              <i className="bi bi-pencil"></i>
            </button>
            <button 
              className="btn btn-sm btn-danger" 
              onClick={handleDelete}
              title="Удалить"
            >
              <i className="bi bi-trash"></i>
            </button>
          </div>
        )}
      </div>
    </div>
  );
}

export default ProductCard;