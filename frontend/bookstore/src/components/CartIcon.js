import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';

function CartIcon() {
  const [itemCount, setItemCount] = useState(0);

  useEffect(() => {
    // Получаем количество товаров в корзине при загрузке компонента
    fetchCartItemCount();

    // Устанавливаем интервал для периодического обновления
    const interval = setInterval(fetchCartItemCount, 30000);
    
    // Очищаем интервал при размонтировании компонента
    return () => clearInterval(interval);
  }, []);

  const fetchCartItemCount = async () => {
    // Проверяем, авторизован ли пользователь
    const token = localStorage.getItem('token');
    if (!token) {
      setItemCount(0);
      return;
    }

    try {
      const response = await fetch('http://localhost:5059/cart', {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        }
      });

      if (!response.ok) {
        throw new Error(`Ошибка HTTP: ${response.status}`);
      }

      const data = await response.json();
      // Подсчитываем общее количество товаров в корзине
      const totalItems = data.reduce((sum, item) => sum + item.quantity, 0);
      setItemCount(totalItems);
    } catch (err) {
      console.error('Ошибка при получении данных корзины:', err);
      // В случае ошибки не обновляем счетчик
    }
  };

  return (
    <Link to="/cart" className="nav-link position-relative">
      <i className="bi bi-cart3" style={{ fontSize: '1.2rem' }}></i>
    </Link>
  );
}

export default CartIcon;
