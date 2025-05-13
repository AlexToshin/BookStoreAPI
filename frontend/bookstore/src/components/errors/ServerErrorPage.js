import React from 'react';
import { Link } from 'react-router-dom';

function ServerErrorPage() {
  return (
    <div className="http-error-page">
      <div className="http-error-content">
        <div className="error-code">500</div>
        <h1>Ошибка сервера</h1>
        <p>
          Произошла внутренняя ошибка сервера. Наши специалисты уже работают над её устранением.
          Пожалуйста, попробуйте обновить страницу или вернитесь позже.
        </p>
        <div className="http-error-actions">
          <button 
            className="btn btn-secondary" 
            onClick={() => window.location.reload()}
          >
            Обновить страницу
          </button>
          <Link to="/" className="btn btn-primary">
            На главную
          </Link>
        </div>
      </div>
    </div>
  );
}

export default ServerErrorPage; 