import React from 'react';
import { Navigate } from 'react-router-dom';

const PrivateRoute = ({ children, isAuthenticated, requiredRole }) => {
  // Проверяем наличие токена в localStorage
  const token = localStorage.getItem('token');
  const userRole = localStorage.getItem('userRole') || 'user';
  
  // Если токен есть или пользователь уже аутентифицирован, проверяем роль (если требуется)
  if (token || isAuthenticated) {
    // Если указана требуемая роль и роль пользователя не соответствует
    if (requiredRole && userRole !== requiredRole && userRole !== 'admin') {
      // Перенаправляем на страницу "Недостаточно прав"
      return <Navigate to="/unauthorized" />;
    }
    // Иначе показываем защищенное содержимое
    return children;
  }
  
  // Иначе перенаправляем на страницу входа
  return <Navigate to="/login" />;
};

export default PrivateRoute;