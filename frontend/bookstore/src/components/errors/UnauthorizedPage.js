import React from 'react';
import { Link } from 'react-router-dom';

function UnauthorizedPage() {
  return (
    <div className="http-error-page">
      <div className="http-error-content">
        <div className="error-code">401</div>
        <h1>Требуется авторизация</h1>
        <p>
          У вас нет доступа к запрашиваемому ресурсу.
          Пожалуйста, войдите в систему и повторите попытку.
        </p>
        <div className="http-error-actions">
          <Link to="/login" className="btn btn-primary">
            Войти в систему
          </Link>
          <Link to="/" className="btn btn-secondary">
            На главную
          </Link>
        </div>
      </div>
    </div>
  );
}

export default UnauthorizedPage; 