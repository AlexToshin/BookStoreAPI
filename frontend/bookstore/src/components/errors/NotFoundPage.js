import React from 'react';
import { Link } from 'react-router-dom';

function NotFoundPage() {
  return (
    <div className="http-error-page">
      <div className="http-error-content">
        <div className="error-code">404</div>
        <h1>Страница не найдена</h1>
        <p>
          Запрашиваемая страница не существует или была перемещена.
          Проверьте правильность URL или вернитесь на главную страницу.
        </p>
        <div className="http-error-actions">
          <button 
            className="btn btn-secondary" 
            onClick={() => window.history.back()}
          >
            Назад
          </button>
          <Link to="/" className="btn btn-primary">
            На главную
          </Link>
        </div>
      </div>
    </div>
  );
}

export default NotFoundPage; 