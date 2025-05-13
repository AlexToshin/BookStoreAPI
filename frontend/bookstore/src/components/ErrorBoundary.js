import React, { Component } from 'react';

class ErrorBoundary extends Component {
  constructor(props) {
    super(props);
    this.state = { 
      hasError: false,
      error: null,
      errorInfo: null
    };
  }

  static getDerivedStateFromError(error) {
    // Обновляем состояние, чтобы при следующем рендере показать запасной UI
    return { hasError: true };
  }

  componentDidCatch(error, errorInfo) {
    // Можно логировать ошибку в сервисы аналитики
    console.error('Ошибка перехвачена ErrorBoundary:', error, errorInfo);
    this.setState({
      error: error,
      errorInfo: errorInfo
    });
  }

  render() {
    if (this.state.hasError) {
      // Отображаем специальный UI для ошибки
      return (
        <div className="error-boundary-container">
          <div className="error-boundary-content">
            <h2>Что-то пошло не так.</h2>
            <p>Приносим извинения за неудобства. Пожалуйста, попробуйте обновить страницу или вернитесь на главную.</p>
            
            {this.props.showDetails && this.state.error && (
              <div className="error-details">
                <h3>Детали ошибки:</h3>
                <p className="error-message">{this.state.error.toString()}</p>
                <pre className="error-stack">
                  {this.state.errorInfo && this.state.errorInfo.componentStack}
                </pre>
              </div>
            )}
            
            <div className="error-actions">
              <button 
                className="btn btn-primary me-2" 
                onClick={() => window.location.reload()}
              >
                Обновить страницу
              </button>
              <button 
                className="btn btn-secondary" 
                onClick={() => window.location.href = '/'}
              >
                На главную
              </button>
            </div>
          </div>
        </div>
      );
    }

    return this.props.children;
  }
}

export default ErrorBoundary; 