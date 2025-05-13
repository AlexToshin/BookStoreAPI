import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

const BookCreate = () => {
  const navigate = useNavigate();
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [price, setPrice] = useState('');
  const [yearOfRelease, setYearOfRelease] = useState('');
  const [authors, setAuthors] = useState([]);
  const [categories, setCategories] = useState([]);
  const [selectedAuthorId, setSelectedAuthorId] = useState('');
  const [selectedCategoryId, setSelectedCategoryId] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  
  // Состояния для работы с изображением
  const [imageFile, setImageFile] = useState(null);
  const [imagePreview, setImagePreview] = useState('');
  const [imageUrl, setImageUrl] = useState('');
  const [uploadingImage, setUploadingImage] = useState(false);

  useEffect(() => {
    // Загружаем списки авторов и категорий
    const fetchData = async () => {
      try {
        const [authorsResponse, categoriesResponse] = await Promise.all([
          fetch('http://localhost:5059/Authors', {
            method: 'GET',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
          }),
          fetch('http://localhost:5059/Categories', {
            method: 'GET',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
          })
        ]);

        if (!authorsResponse.ok || !categoriesResponse.ok) {
          throw new Error('Ошибка при загрузке данных');
        }

        const authorsData = await authorsResponse.json();
        const categoriesData = await categoriesResponse.json();

        setAuthors(authorsData);
        setCategories(categoriesData);

        // Устанавливаем первое значение в списках по умолчанию, если они есть
        if (authorsData.length > 0) {
          setSelectedAuthorId(authorsData[0].id);
        }
        if (categoriesData.length > 0) {
          setSelectedCategoryId(categoriesData[0].id);
        }
      } catch (err) {
        setError(`Ошибка при загрузке данных: ${err.message}`);
        console.error('Ошибка при загрузке данных:', err);
      }
    };

    fetchData();
  }, []);

  // Обработчик выбора файла изображения
  const handleImageChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      // Проверка типа файла
      const allowedTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/jpg'];
      if (!allowedTypes.includes(file.type)) {
        setError('Поддерживаются только изображения формата JPG, JPEG, PNG и GIF');
        return;
      }
      
      // Проверка размера файла (не более 5 МБ)
      if (file.size > 5 * 1024 * 1024) {
        setError('Размер файла не должен превышать 5 МБ');
        return;
      }
      
      setImageFile(file);
      setImagePreview(URL.createObjectURL(file));
      setError(null);
    }
  };

  // Обработчик удаления выбранного изображения
  const handleRemoveImage = () => {
    setImageFile(null);
    setImagePreview('');
    setImageUrl('');
  };

  // Функция для загрузки изображения на сервер
  const uploadImage = async () => {
    if (!imageFile) return null;
    
    setUploadingImage(true);
    
    try {
      const formData = new FormData();
      formData.append('file', imageFile);
      
      const response = await fetch('http://localhost:5059/Books/upload-image', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: formData
      });
      
      if (!response.ok) {
        throw new Error(`Ошибка загрузки изображения: ${response.status}`);
      }
      
      const data = await response.json();
      return data.imageUrl;
    } catch (err) {
      console.error('Ошибка при загрузке изображения:', err);
      throw err;
    } finally {
      setUploadingImage(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    
    try {
      // Сначала загружаем изображение, если оно выбрано
      let uploadedImageUrl = null;
      if (imageFile) {
        uploadedImageUrl = await uploadImage();
      }
      
      // Преобразуем цену в число
      const priceNumber = parseFloat(price);
      // Преобразуем год в число
      const yearNumber = parseInt(yearOfRelease, 10);

      const bookData = {
        title,
        description,
        price: priceNumber,
        yearOfRelease: yearNumber,
        authorId: selectedAuthorId,
        categoryId: selectedCategoryId,
        imageUrl: uploadedImageUrl
      };

      const response = await fetch('http://localhost:5059/Books', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(bookData)
      });

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }

      // Переходим на главную страницу после успешного создания
      navigate('/');
    } catch (err) {
      setError(`Ошибка при создании книги: ${err.message}`);
      console.error('Ошибка при создании книги:', err);
    } finally {
      setLoading(false);
    }
  };

  if (error) {
    return (
      <div className="container mt-4">
        <div className="alert alert-danger" role="alert">
          {error}
        </div>
        <button
          className="btn btn-primary"
          onClick={() => navigate('/')}
        >
          Вернуться к списку книг
        </button>
      </div>
    );
  }

  return (
    <div className="container mt-4">
      <h2 className="text-center mb-4">Создание новой книги</h2>
      
      <div className="card">
        <div className="card-body">
          <form onSubmit={handleSubmit}>
            {/* Блок для загрузки изображения */}
            <div className="mb-3">
              <label htmlFor="imageUpload" className="form-label">Изображение книги</label>
              <div className="image-upload-container">
                {imagePreview ? (
                  <div className="text-center">
                    <img src={imagePreview} alt="Предпросмотр" className="image-preview" />
                    <div className="image-actions">
                      <button 
                        type="button" 
                        className="btn btn-sm btn-danger"
                        onClick={handleRemoveImage}
                      >
                        Удалить изображение
                      </button>
                    </div>
                  </div>
                ) : (
                  <>
                    <input
                      type="file"
                      className="form-control"
                      id="imageUpload"
                      accept="image/jpeg,image/png,image/gif"
                      onChange={handleImageChange}
                    />
                    <small className="text-muted d-block mt-2">
                      Поддерживаемые форматы: JPG, JPEG, PNG, GIF. Максимальный размер файла: 5 МБ.
                    </small>
                  </>
                )}
              </div>
            </div>

            <div className="mb-3">
              <label htmlFor="title" className="form-label">Название книги</label>
              <input
                type="text"
                className="form-control"
                id="title"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                required
              />
            </div>
            
            <div className="mb-3">
              <label htmlFor="description" className="form-label">Описание</label>
              <textarea
                className="form-control"
                id="description"
                rows="3"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
              />
            </div>
            
            <div className="mb-3">
              <label htmlFor="price" className="form-label">Цена</label>
              <input
                type="number"
                className="form-control"
                id="price"
                min="0"
                step="0.01"
                value={price}
                onChange={(e) => setPrice(e.target.value)}
                required
              />
            </div>
            
            <div className="mb-3">
              <label htmlFor="yearOfRelease" className="form-label">Год издания</label>
              <input
                type="number"
                className="form-control"
                id="yearOfRelease"
                min="1500"
                max={new Date().getFullYear()}
                value={yearOfRelease}
                onChange={(e) => setYearOfRelease(e.target.value)}
                required
              />
            </div>
            
            <div className="mb-3">
              <label htmlFor="author" className="form-label">Автор</label>
              <select
                className="form-select"
                id="author"
                value={selectedAuthorId}
                onChange={(e) => setSelectedAuthorId(e.target.value)}
                required
              >
                <option value="">-- Выберите автора --</option>
                {authors.map(author => (
                  <option key={author.id} value={author.id}>
                    {author.name} {author.surname}
                  </option>
                ))}
              </select>
            </div>
            
            <div className="mb-3">
              <label htmlFor="category" className="form-label">Категория</label>
              <select
                className="form-select"
                id="category"
                value={selectedCategoryId}
                onChange={(e) => setSelectedCategoryId(e.target.value)}
                required
              >
                <option value="">-- Выберите категорию --</option>
                {categories.map(category => (
                  <option key={category.id} value={category.id}>
                    {category.name}
                  </option>
                ))}
              </select>
            </div>
            
            <div className="d-flex justify-content-end gap-2">
              <button
                type="button"
                className="btn btn-secondary"
                onClick={() => navigate('/')}
              >
                Отмена
              </button>
              <button
                type="submit"
                className="btn btn-primary"
                disabled={loading || uploadingImage}
              >
                {loading || uploadingImage ? (
                  <>
                    <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                    Сохранение...
                  </>
                ) : 'Создать книгу'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default BookCreate; 