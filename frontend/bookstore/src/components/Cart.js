import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { toast } from 'react-toastify';
import CartItem from './CartItem';

function Cart() {
  const [cartItems, setCartItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [totalPrice, setTotalPrice] = useState(0);

  useEffect(() => {
    fetchCartItems();
  }, []);

  useEffect(() => {
    // Вычисляем общую стоимость при изменении корзины
    const total = cartItems.reduce((sum, item) => sum + (item.bookPrice * item.quantity), 0);
    setTotalPrice(total);
  }, [cartItems]);

  const fetchCartItems = async () => {
    try {
      setLoading(true);
      
      // Проверяем наличие токена
      const token = localStorage.getItem('token');
      if (!token) {
        setError('Необходима авторизация');
        setLoading(false);
        return;
      }
      
      const response = await fetch('http://localhost:5059/cart', {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        }
      });

      if (!response.ok) {
        if (response.status === 401) {
          setError('Необходима авторизация');
          localStorage.removeItem('token');
          localStorage.removeItem('userRole');
          window.location.href = '/login';
          return;
        }
        throw new Error(`Ошибка HTTP: ${response.status}`);
      }

      const data = await response.json();
      setCartItems(data || []);
      setLoading(false);
    } catch (err) {
      setError(err.message);
      setLoading(false);
      console.error('Ошибка при получении корзины:', err);
      toast.error(`Ошибка при загрузке корзины: ${err.message}`);
    }
  };

  const handleUpdateQuantity = async (itemId, quantity) => {
    try {
      const response = await fetch(`http://localhost:5059/cart/items/${itemId}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify({
          bookId: cartItems.find(item => item.id === itemId).bookId,
          quantity: quantity
        })
      });

      if (!response.ok) {
        throw new Error(`Ошибка HTTP: ${response.status}`);
      }

      // Обновляем состояние корзины
      setCartItems(prevItems =>
        prevItems.map(item =>
          item.id === itemId ? { ...item, quantity } : item
        )
      );
    } catch (err) {
      console.error('Ошибка при обновлении количества:', err);
      throw err;
    }
  };

  const handleRemoveItem = async (itemId) => {
    try {
      const response = await fetch(`http://localhost:5059/cart/items/${itemId}`, {
        method: 'DELETE',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) {
        throw new Error(`Ошибка HTTP: ${response.status}`);
      }

      // Удаляем товар из состояния
      setCartItems(prevItems => prevItems.filter(item => item.id !== itemId));
    } catch (err) {
      console.error('Ошибка при удалении товара:', err);
      throw err;
    }
  };

  const handleClearCart = async () => {
    if (!window.confirm('Вы уверены, что хотите очистить корзину?')) {
      return;
    }

    try {
      const response = await fetch('http://localhost:5059/cart', {
        method: 'DELETE',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) {
        throw new Error(`Ошибка HTTP: ${response.status}`);
      }

      // Очищаем корзину в состоянии
      setCartItems([]);
      toast.success('Корзина очищена');
    } catch (err) {
      console.error('Ошибка при очистке корзины:', err);
      toast.error(`Ошибка при очистке корзины: ${err.message}`);
    }
  };

  if (loading) {
    return (
      <div className="container mt-5 d-flex justify-content-center">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Загрузка...</span>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mt-5">
        <div className="alert alert-danger" role="alert">
          Ошибка: {error}
        </div>
      </div>
    );
  }

  return (
    <div className="container mt-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>Корзина</h2>
        <div>
          <Link to="/" className="btn btn-outline-primary me-2">
            <i className="bi bi-arrow-left"></i> Продолжить покупки
          </Link>
          {cartItems.length > 0 && (
            <button 
              className="btn btn-outline-danger" 
              onClick={handleClearCart}
            >
              <i className="bi bi-trash"></i> Очистить корзину
            </button>
          )}
        </div>
      </div>

      {cartItems.length === 0 ? (
        <div className="alert alert-info">
          Ваша корзина пуста. <Link to="/">Перейти к покупкам</Link>
        </div>
      ) : (
        <>
          <div className="card-body">
            {cartItems.map(item => (
              <CartItem 
                key={item.id} 
                item={item} 
                onUpdateQuantity={handleUpdateQuantity} 
                onRemove={handleRemoveItem} 
              />
            ))}
          </div>

          <div className="card mt-4 mb-5">
            <div className="card-body">
              <div className="d-flex justify-content-between align-items-center">
                <h4>Итого:</h4>
                <h4 className="text-primary">{totalPrice.toFixed(2)} ₽</h4>
              </div>
              <div className="d-grid gap-2 mt-3">
                <button className="btn btn-success">
                  <i className="bi bi-credit-card"></i> Оформить заказ
                </button>
              </div>
            </div>
          </div>
        </>
      )}
    </div>
  );
}

export default Cart;
