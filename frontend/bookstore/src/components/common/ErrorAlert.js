import React from 'react';
import PropTypes from 'prop-types';

/**
 * Компонент для отображения сообщений об ошибках в формах
 * @param {Object} props - Свойства компонента
 * @param {string|null} props.error - Сообщение об ошибке
 * @param {boolean} props.dismissible - Можно ли закрыть уведомление
 * @param {Function} props.onDismiss - Функция для закрытия уведомления
 */
function ErrorAlert({ error, dismissible = true, onDismiss }) {
  if (!error) return null;

  return (
    <div className="alert alert-danger d-flex align-items-center" role="alert">
      <svg 
        xmlns="http://www.w3.org/2000/svg" 
        width="24" 
        height="24" 
        fill="currentColor" 
        className="bi bi-exclamation-triangle-fill flex-shrink-0 me-2" 
        viewBox="0 0 16 16"
      >
        <path d="M8.982 1.566a1.13 1.13 0 0 0-1.96 0L.165 13.233c-.457.778.091 1.767.98 1.767h13.713c.889 0 1.438-.99.98-1.767L8.982 1.566zM8 5c.535 0 .954.462.9.995l-.35 3.507a.552.552 0 0 1-1.1 0L7.1 5.995A.905.905 0 0 1 8 5zm.002 6a1 1 0 1 1 0 2 1 1 0 0 1 0-2z"/>
      </svg>
      <div className="flex-grow-1">
        {error}
      </div>
      {dismissible && (
        <button 
          type="button" 
          className="btn-close" 
          aria-label="Close"
          onClick={onDismiss}
        ></button>
      )}
    </div>
  );
}

ErrorAlert.propTypes = {
  error: PropTypes.string,
  dismissible: PropTypes.bool,
  onDismiss: PropTypes.func
};

export default ErrorAlert; 