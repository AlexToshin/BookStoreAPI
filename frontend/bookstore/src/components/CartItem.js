import React, { useState } from 'react';
import { toast } from 'react-toastify';

function CartItem({ item, onUpdateQuantity, onRemove }) {
  const [isRemoving, setIsRemoving] = useState(false);
  const [isUpdating, setIsUpdating] = useState(false);

  const handleQuantityChange = (e) => {
    const newQuantity = parseInt(e.target.value, 10);
    if (newQuantity > 0) {
      setIsUpdating(true);
      try {
        onUpdateQuantity(item.id, newQuantity);
        toast.success('Количество обновлено');
      } catch (error) {
        toast.error('Ошибка при обновлении количества');
        console.error('Ошибка при обновлении количества:', error);
      } finally {
        setIsUpdating(false);
      }
    }
  };

  const handleRemove = () => {
    if (window.confirm(`Вы уверены, что хотите удалить "${item.bookTitle}" из корзины?`)) {
      setIsRemoving(true);
      try {
        onRemove(item.id);
        toast.success('Товар удален из корзины');
      } catch (error) {
        toast.error('Ошибка при удалении товара');
        console.error('Ошибка при удалении товара:', error);
      } finally {
        setIsRemoving(false);
      }
    }
  };

  return (
    <div className="card mb-3">
      <div className="row g-0">
        <div className="col-md-2">
          {item.bookImageUrl ? (
            <img 
              src={`http://localhost:5059${item.bookImageUrl}`} 
              className="img-fluid rounded-start" 
              alt={item.bookTitle}
              style={{ maxHeight: '150px', objectFit: 'contain' }}
            />
          ) : (
            <div className="bg-light d-flex justify-content-center align-items-center" style={{ height: '150px' }}>
              <i className="bi bi-book" style={{ fontSize: '3rem' }}></i>
            </div>
          )}
        </div>
        <div className="col-md-8">
          <div className="card-body">
            <h5 className="card-title">{item.bookTitle}</h5>
            <p className="card-text text-muted">Автор: {item.authorName}</p>
            <p className="card-text text-muted">Категория: {item.categoryName}</p>
            <p className="card-text">
              <small className="text-muted">
                Добавлено: {new Date(item.dateAdded).toLocaleDateString()}
              </small>
            </p>
          </div>
        </div>
        <div className="col-md-2 d-flex flex-column justify-content-center align-items-center">
          <h5 className="text-primary mb-3">{item.bookPrice.toFixed(2)} ₽</h5>
          <div className="input-group mb-3" style={{ maxWidth: '120px' }}>
            <span className="input-group-text">Кол-во</span>
            <input 
              type="number" 
              className="form-control" 
              value={item.quantity} 
              min="1"
              onChange={handleQuantityChange}
              disabled={isUpdating}
            />
            {isUpdating && (
              <div className="position-absolute top-0 end-0 mt-1 me-1">
                <span className="spinner-border spinner-border-sm text-primary" role="status" aria-hidden="true"></span>
              </div>
            )}
          </div>
          <button 
            className="btn btn-sm btn-outline-danger" 
            onClick={handleRemove}
            disabled={isRemoving}
          >
            {isRemoving ? (
              <>
                <span className="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>
                Удаление...
              </>
            ) : (
              <>
                <i className="bi bi-trash me-1"></i> Удалить
              </>
            )}
          </button>
        </div>
      </div>
    </div>
  );
}

export default CartItem;
