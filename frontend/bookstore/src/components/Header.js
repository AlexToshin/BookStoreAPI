import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import CartIcon from './CartIcon';

const Header = ({ isAuthenticated, setIsAuthenticated, userRole }) => {
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('userRole');
    setIsAuthenticated(false);
    navigate('/login');
  };

  return (
    <header className="header">
      <div className="container">
        <div className="header-content">
          <h1><Link to="/" className="logo">Book Store</Link></h1>
          <nav className="nav-links">
            {isAuthenticated ? (
              <>
                <Link to="/" className="nav-link">Книги</Link>
                <Link to="/authors" className="nav-link">Авторы</Link>
                <Link to="/categories" className="nav-link">Категории</Link>
                {userRole !== 'admin' && <CartIcon />}
                {userRole === 'admin' && (
                  <span className="nav-link admin-badge">Администратор</span>
                )}
                <button onClick={handleLogout} className="btn btn-logout">
                  Выйти
                </button>
              </>
            ) : (
              <>
                <Link to="/login" className="nav-link">Войти</Link>
                <Link to="/register" className="nav-link">Регистрация</Link>
              </>
            )}
          </nav>
        </div>
      </div>
    </header>
  );
};

export default Header;