import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';

function Login({ setIsAuthenticated, setUserRole }) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      console.log('Отправка запроса на вход с данными:', { username: email, password });
      
      // Проверка доступности сервера перед отправкой запроса
      try {
        const pingResponse = await fetch('http://localhost:5059/ping', { 
          method: 'GET',
          mode: 'cors',
          cache: 'no-cache',
          headers: { 'Content-Type': 'application/json' }
        });
        console.log('Статус проверки сервера:', pingResponse.status);
      } catch (pingError) {
        console.error('Ошибка при проверке сервера:', pingError);
      }
      
      const response = await fetch('http://localhost:5059/Auth/login', {
        method: 'POST',
        mode: 'cors',
        cache: 'no-cache',
        credentials: 'same-origin',
        headers: { 
          'Content-Type': 'application/json',
          'Accept': 'application/json'
        },
        body: JSON.stringify({ username: email, password })
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
          throw new Error(text || 'Ошибка при входе');
        }
        
        // Иначе просто создаем пустой объект
        data = {};
      }

      if (!response.ok) {
        throw new Error(data.error || 'Ошибка при входе');
      }

      if (data.token) {
        localStorage.setItem('token', data.token);
        
        // Извлекаем роль из токена JWT
        try {
          const payload = JSON.parse(atob(data.token.split('.')[1]));
          const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 'user';
          console.log('Роль пользователя:', role);
          
          // Сохраняем роль пользователя
          localStorage.setItem('userRole', role);
          if (setUserRole) setUserRole(role);
        } catch (e) {
          console.error('Ошибка при извлечении роли из токена:', e);
          localStorage.setItem('userRole', 'user'); // По умолчанию роль пользователя
          if (setUserRole) setUserRole('user');
        }
        
        setIsAuthenticated(true);
        navigate('/');
      } else {
        throw new Error('Токен не получен');
      }
    } catch (err) {
      console.error('Ошибка входа:', err);
      
      // Более детальная обработка ошибок
      if (err.name === 'TypeError' && err.message === 'Failed to fetch') {
        setError('Не удалось подключиться к серверу. Проверьте, что сервер запущен и доступен по адресу http://localhost:5059');
      } else if (err.message && err.message.includes('NetworkError')) {
        setError('Ошибка сети: возможно, сервер недоступен или есть проблемы с CORS');
      } else if (err.message && err.message.includes('CORS')) {
        setError('Ошибка CORS: сервер не разрешает запросы с этого домена');
      } else {
        setError(err.message || 'Произошла ошибка при входе');
      }
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
        <h2>Вход в систему</h2>
        {error && <div className="alert alert-danger">{error}</div>}
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="email">Email или имя пользователя</label>
            <input
              type="text"
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
          <button 
            type="submit" 
            className="btn btn-primary" 
            disabled={loading}
          >
            {loading ? 'Вход...' : 'Войти'}
          </button>
        </form>
        <div className="auth-links">
          <span>Нет аккаунта?</span>
          <Link to="/register">Зарегистрироваться</Link>
        </div>
      </div>
    </div>
  );
}

export default Login;