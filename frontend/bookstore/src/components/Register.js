import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';

function Register() {
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    // Проверка совпадения паролей
    if (password !== confirmPassword) {
      setError('Пароли не совпадают');
      return;
    }

    setLoading(true);

    try {
      console.log('Отправка запроса на регистрацию с данными:', { username, email, password });
      
      const response = await fetch('http://localhost:5059/Auth/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, email, password })
      });

      console.log('Получен ответ:', response);
      
      // Проверка содержимого ответа перед вызовом json()
      const contentType = response.headers.get('content-type');
      let data;
      
      if (contentType && contentType.indexOf('application/json') !== -1) {
        data = await response.json();
        console.log('Данные ответа (JSON):', data);
      } else {
        // Если ответ не JSON, получаем его как текст
        const text = await response.text();
        console.log('Данные ответа (текст):', text);
        
        // Если ответ не успешный, используем текст как сообщение об ошибке
        if (!response.ok) {
          throw new Error(text || 'Ошибка при регистрации');
        }
        
        // Иначе просто создаем пустой объект
        data = {};
      }

      if (!response.ok) {
        throw new Error(data.error || 'Ошибка при регистрации');
      }

      // При успешной регистрации перенаправляем на страницу входа
      navigate('/login');
    } catch (err) {
      console.error('Ошибка регистрации:', err);
      setError(err.message || 'Произошла ошибка при регистрации');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-header">
        <h1>Book Store</h1>
      </div>
      <div className="auth-form-container">
        <h2>Регистрация</h2>
        {error && <div className="alert alert-danger">{error}</div>}
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="username">Имя пользователя</label>
            <input
              type="text"
              id="username"
              className="form-control"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
            />
          </div>
          <div className="form-group">
            <label htmlFor="email">Email</label>
            <input
              type="email"
              id="email"
              className="form-control"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>
          <div className="form-group">
            <label htmlFor="password">Пароль</label>
            <input
              type="password"
              id="password"
              className="form-control"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>
          <div className="form-group">
            <label htmlFor="confirmPassword">Подтвердите пароль</label>
            <input
              type="password"
              id="confirmPassword"
              className="form-control"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              required
            />
          </div>
          <button 
            type="submit" 
            className="btn btn-primary" 
            disabled={loading}
          >
            {loading ? 'Регистрация...' : 'Зарегистрироваться'}
          </button>
        </form>
        <div className="auth-links">
          <span>Уже есть аккаунт?</span>
          <Link to="/login">Войти</Link>
        </div>
      </div>
    </div>
  );
}

export default Register; 