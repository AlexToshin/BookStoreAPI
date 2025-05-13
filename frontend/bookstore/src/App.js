import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import './App.css';
import Header from './components/Header';
import ProductList from './components/ProductList';
import Login from './components/Login';
import Register from './components/Register';
import PrivateRoute from './components/PrivateRoute';
import AuthorsList from './components/AuthorsList';
import AuthorCreate from './components/AuthorCreate';
import AuthorEdit from './components/AuthorEdit';
import AuthorDetails from './components/AuthorDetails';
import CategoriesList from './components/CategoriesList';
import CategoryCreate from './components/CategoryCreate';
import CategoryEdit from './components/CategoryEdit';
import CategoryDetails from './components/CategoryDetails';
import BookCreate from './components/BookCreate';
import BookEdit from './components/BookEdit';
import BookDetails from './components/BookDetails';
import Cart from './components/Cart';
import ErrorBoundary from './components/ErrorBoundary';
import NotFoundPage from './components/errors/NotFoundPage';
import ServerErrorPage from './components/errors/ServerErrorPage';
import UnauthorizedPage from './components/errors/UnauthorizedPage';

function App() {
  const [books, setBooks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [userRole, setUserRole] = useState('user');

  useEffect(() => {
    // Проверяем, есть ли токен при загрузке приложения
    const token = localStorage.getItem('token');
    const savedRole = localStorage.getItem('userRole');
    
    if (token) {
      setIsAuthenticated(true);
      if (savedRole) {
        setUserRole(savedRole);
      }
    }
    
    fetchBooks();
  }, []);

  const fetchBooks = async () => {
    try {
      console.log('Начало запроса списка книг к API...');
      const response = await fetch('http://localhost:5059/books', {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
          // Добавляем токен авторизации, если пользователь авторизован
          ...(localStorage.getItem('token') ? {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          } : {})
        }
      });
      
      console.log('Статус ответа API:', response.status);
      
      if (!response.ok) {
        throw new Error(`Ошибка HTTP: ${response.status}`);
      }
      
      const data = await response.json();
      console.log('Получены книги от API:', data);
      
      // Детальная проверка изображений книг
      data.forEach(book => {
        console.log(`Книга ${book.title}:`, {
          id: book.id,
          hasImageUrl: !!book.imageUrl,
          imageUrl: book.imageUrl || 'отсутствует'
        });
        
        // Добавляем проверку на книгу "All quiet on western front"
        if (book.title && book.title.toLowerCase().includes('all quiet on western front')) {
          console.log('НАЙДЕНА ИСКОМАЯ КНИГА:', book);
          console.log('Изображение искомой книги:', book.imageUrl);
        }
      });
      
      setBooks(data);
      setLoading(false);
    } catch (err) {
      setError(err.message);
      setLoading(false);
      console.error('Детали ошибки при получении книг:', err);
    }
  };

  const handleDeleteBook = async (bookId) => {
    try {
      const response = await fetch(`http://localhost:5059/books/${bookId}`, {
        method: 'DELETE',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }

      // Обновляем список книг после удаления
      setBooks(books.filter(book => book.id !== bookId));
    } catch (err) {
      console.error('Ошибка при удалении книги:', err);
      alert(`Ошибка при удалении книги: ${err.message}`);
    }
  };

  return (
    <Router>
      <ErrorBoundary showDetails={process.env.NODE_ENV === 'development'}>
        <div className="App bg-light min-vh-100">
          <Header isAuthenticated={isAuthenticated} setIsAuthenticated={setIsAuthenticated} userRole={userRole} />
          <ToastContainer 
            position="top-right"
            autoClose={3000}
            hideProgressBar={false}
            newestOnTop
            closeOnClick
            rtl={false}
            pauseOnFocusLoss
            draggable
            pauseOnHover
            theme="light"
          />
          
          <Routes>
            <Route path="/login" element={
              isAuthenticated ? 
                <Navigate to="/" /> : 
                <Login setIsAuthenticated={setIsAuthenticated} setUserRole={setUserRole} />
            } />
            
            <Route path="/register" element={
              isAuthenticated ? 
                <Navigate to="/" /> : 
                <Register />
            } />
            
            <Route path="/" element={
              <PrivateRoute isAuthenticated={isAuthenticated}>
                {loading ? (
                  <div className="container d-flex justify-content-center align-items-center" style={{ minHeight: '80vh' }}>
                    <div className="spinner-border text-primary" role="status">
                      <span className="visually-hidden">Загрузка...</span>
                    </div>
                  </div>
                ) : error ? (
                  <div className="container mt-5">
                    <div className="alert alert-danger" role="alert">
                      Ошибка: {error}
                    </div>
                  </div>
                ) : (
                  <ProductList books={books} onDeleteBook={handleDeleteBook} />
                )}
              </PrivateRoute>
            } />

            {/* Маршруты для авторов */}
            <Route path="/authors" element={
              <PrivateRoute isAuthenticated={isAuthenticated}>
                <AuthorsList />
              </PrivateRoute>
            } />
            
            <Route path="/authors/create" element={
              <PrivateRoute isAuthenticated={isAuthenticated} requiredRole="admin">
                <AuthorCreate />
              </PrivateRoute>
            } />
            
            <Route path="/authors/edit/:id" element={
              <PrivateRoute isAuthenticated={isAuthenticated} requiredRole="admin">
                <AuthorEdit />
              </PrivateRoute>
            } />
            
            <Route path="/authors/:id" element={
              <PrivateRoute isAuthenticated={isAuthenticated}>
                <AuthorDetails />
              </PrivateRoute>
            } />

            {/* Маршруты для категорий */}
            <Route path="/categories" element={
              <PrivateRoute isAuthenticated={isAuthenticated}>
                <CategoriesList />
              </PrivateRoute>
            } />
            
            <Route path="/categories/create" element={
              <PrivateRoute isAuthenticated={isAuthenticated} requiredRole="admin">
                <CategoryCreate />
              </PrivateRoute>
            } />
            
            <Route path="/categories/edit/:id" element={
              <PrivateRoute isAuthenticated={isAuthenticated} requiredRole="admin">
                <CategoryEdit />
              </PrivateRoute>
            } />
            
            <Route path="/categories/:id" element={
              <PrivateRoute isAuthenticated={isAuthenticated}>
                <CategoryDetails />
              </PrivateRoute>
            } />

            {/* Маршруты для книг */}
            <Route path="/books/create" element={
              <PrivateRoute isAuthenticated={isAuthenticated} requiredRole="admin">
                <BookCreate />
              </PrivateRoute>
            } />
            
            <Route path="/books/edit/:id" element={
              <PrivateRoute isAuthenticated={isAuthenticated} requiredRole="admin">
                <BookEdit />
              </PrivateRoute>
            } />
            
            <Route path="/books/:id" element={
              <PrivateRoute isAuthenticated={isAuthenticated}>
                <BookDetails />
              </PrivateRoute>
            } />

            {/* Маршрут для корзины - недоступен для администраторов */}
            <Route path="/cart" element={
              <PrivateRoute isAuthenticated={isAuthenticated}>
                {userRole === 'admin' ? 
                  <Navigate to="/" /> : 
                  <Cart />
                }
              </PrivateRoute>
            } />
            
            {/* Маршруты для страниц ошибок */}
            <Route path="/error/404" element={<NotFoundPage />} />
            <Route path="/error/500" element={<ServerErrorPage />} />
            <Route path="/error/unauthorized" element={<UnauthorizedPage />} />
            
            {/* Обработка всех других маршрутов */}
            <Route path="*" element={<NotFoundPage />} />
          </Routes>
        </div>
      </ErrorBoundary>
    </Router>
  );
}

export default App;