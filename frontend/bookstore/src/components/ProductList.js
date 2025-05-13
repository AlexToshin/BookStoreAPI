import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import ProductCard from './ProductCard';

function ProductList({ books, onDeleteBook }) {
  const navigate = useNavigate();
  const userRole = localStorage.getItem('userRole');
  const isAdmin = userRole === 'admin';

  // Добавляем логирование для отладки
  useEffect(() => {
    console.log('Список всех книг:', books);
    
    // Проверяем наличие imageUrl в каждой книге
    books.forEach(book => {
      console.log(`Книга "${book.title}" имеет imageUrl: ${book.imageUrl || 'отсутствует'}`);
    });
  }, [books]);

  const handleAddBook = () => {
    navigate('/books/create');
  };

  return (
    <div className="container mt-4">
      <h2 className="text-center my-4">Книги</h2>
      
      {isAdmin && (
        <div className="d-flex justify-content-center mb-3">
          <button 
            className="btn btn-sm btn-primary"
            onClick={handleAddBook}
            style={{ width: '150px' }}
          >
            Добавить книгу
          </button>
        </div>
      )}

      {books.length === 0 ? (
        <div className="alert alert-info">Книги не найдены</div>
      ) : (
        <div className="books-grid">
          {books.map(book => (
            <ProductCard 
              key={book.id} 
              book={book} 
              onDelete={onDeleteBook} 
              isAdmin={isAdmin}
            />
          ))}
        </div>
      )}
    </div>
  );
}

export default ProductList;