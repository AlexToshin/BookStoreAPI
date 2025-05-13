import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';

const BookEdit = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [price, setPrice] = useState('');
  const [yearOfRelease, setYearOfRelease] = useState('');
  const [authors, setAuthors] = useState([]);
  const [categories, setCategories] = useState([]);
  const [selectedAuthorId, setSelectedAuthorId] = useState('');
  const [selectedCategoryId, setSelectedCategoryId] = useState('');
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);
  
  // Состояния для работы с изображением
  const [imageFile, setImageFile] = useState(null);
  const [imagePreview, setImagePreview] = useState('');
  const [currentImageUrl, setCurrentImageUrl] = useState('');
  const [uploadingImage, setUploadingImage] = useState(false);
  const [removeCurrentImage, setRemoveCurrentImage] = useState(false);

  useEffect(() => {
    const fetchData = async () => {
      try {
        console.log('Начинаем загрузку данных для книги с ID:', id);
        
        // Сначала загружаем данные книги
        const bookResponse = await fetch(`http://localhost:5059/Books/${id}`, {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        });

        if (!bookResponse.ok) {
          throw new Error(`Ошибка загрузки книги: ${bookResponse.status}`);
        }

        const bookData = await bookResponse.json();
        console.log('Получены данные книги:', bookData);

        // Загружаем списки авторов и категорий
        const authorsResponse = await fetch('http://localhost:5059/Authors', {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        });

        if (!authorsResponse.ok) {
          throw new Error(`Ошибка загрузки авторов: ${authorsResponse.status}`);
        }

        const authorsData = await authorsResponse.json();
        console.log('Получены данные авторов:', authorsData);

        const categoriesResponse = await fetch('http://localhost:5059/Categories', {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        });

        if (!categoriesResponse.ok) {
          throw new Error(`Ошибка загрузки категорий: ${categoriesResponse.status}`);
        }

        const categoriesData = await categoriesResponse.json();
        console.log('Получены данные категорий:', categoriesData);

        // Заполняем форму данными книги
        setTitle(bookData.title || '');
        setDescription(bookData.description || '');
        setPrice(bookData.price ? bookData.price.toString() : '0');
        setYearOfRelease(bookData.yearOfRelease ? bookData.yearOfRelease.toString() : new Date().getFullYear().toString());
        
        // Проверяем наличие автора и категории и устанавливаем их ID
        if (bookData.author && bookData.author.id) {
          setSelectedAuthorId(bookData.author.id);
        } else if (bookData.authorId) {
          setSelectedAuthorId(bookData.authorId);
        }
        
        if (bookData.category && bookData.category.id) {
          setSelectedCategoryId(bookData.category.id);
        } else if (bookData.categoryId) {
          setSelectedCategoryId(bookData.categoryId);
        }

        // Устанавливаем данные изображения, если оно есть
        if (bookData.imageUrl) {
          setCurrentImageUrl(bookData.imageUrl);
          setImagePreview(`http://localhost:5059${bookData.imageUrl}`);
        }

        // Сохраняем списки авторов и категорий
        setAuthors(authorsData);
        setCategories(categoriesData);
        
        console.log('Данные успешно загружены и установлены');
      } catch (err) {
        console.error('Детальная ошибка при загрузке данных:', err);
        setError(`Ошибка при загрузке данных: ${err.message}`);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [id]);

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
      setRemoveCurrentImage(false);
      setError(null);
    }
  };

  // Обработчик удаления текущего изображения
  const handleRemoveCurrentImage = () => {
    setCurrentImageUrl('');
    setImagePreview('');
    setImageFile(null);
    setRemoveCurrentImage(true);
  };

  // Обработчик отмены выбора нового изображения
  const handleCancelNewImage = () => {
    if (currentImageUrl) {
      setImageFile(null);
      setImagePreview(`http://localhost:5059${currentImageUrl}`);
      setRemoveCurrentImage(false);
    } else {
      setImageFile(null);
      setImagePreview('');
    }
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

  // Функция для удаления текущего изображения с сервера
  const deleteCurrentImage = async () => {
    if (!currentImageUrl || !removeCurrentImage) return;
    
    try {
      const response = await fetch(`http://localhost:5059/Books/${id}/image`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });
      
      if (!response.ok) {
        throw new Error(`Ошибка удаления изображения: ${response.status}`);
      }
    } catch (err) {
      console.error('Ошибка при удалении изображения:', err);
      // Продолжаем процесс, даже если не удалось удалить изображение
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);
    
    try {
      // Управление изображением
      let newImageUrl = currentImageUrl;
      
      // Если нужно удалить текущее изображение
      if (removeCurrentImage) {
        await deleteCurrentImage();
        newImageUrl = null;
      }
      
      // Если есть новое изображение для загрузки
      if (imageFile) {
        newImageUrl = await uploadImage();
      }
      
      // Преобразуем цену и год в числа
      const priceNumber = parseFloat(price);
      const yearNumber = parseInt(yearOfRelease, 10);

      const bookData = {
        id,
        title,
        description,
        price: priceNumber,
        yearOfRelease: yearNumber,
        authorId: selectedAuthorId,
        categoryId: selectedCategoryId,
        imageUrl: newImageUrl
      };

      console.log('Отправляем данные на сервер:', bookData);

      const response = await fetch(`http://localhost:5059/Books/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(bookData)
      });

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }

      // Переходим на главную страницу после успешного обновления
      navigate('/');
    } catch (err) {
      console.error('Детальная ошибка при обновлении книги:', err);
      setError(`Ошибка при обновлении книги: ${err.message}`);
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div className="container d-flex justify-content-center align-items-center" style={{ minHeight: '50vh' }}>
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Загрузка...</span>
        </div>
      </div>
    );
  }

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
      <h2 className="text-center mb-4">Редактирование книги</h2>
      
      <div className="card">
        <div className="card-body">
          <form onSubmit={handleSubmit}>
            {/* Блок для управления изображением */}
            <div className="mb-3">
              <label htmlFor="imageUpload" className="form-label">Изображение книги</label>
              <div className="image-upload-container">
                {imagePreview ? (
                  <div className="text-center">
                    <img src={imagePreview} alt="Изображение книги" className="image-preview" />
                    <div className="image-actions">
                      {imageFile ? (
                        // Если выбрано новое изображение
                        <button 
                          type="button" 
                          className="btn btn-sm btn-secondary me-2"
                          onClick={handleCancelNewImage}
                        >
                          Отменить выбор
                        </button>
                      ) : (
                        // Если отображается текущее изображение
                        <button 
                          type="button" 
                          className="btn btn-sm btn-danger"
                          onClick={handleRemoveCurrentImage}
                        >
                          Удалить изображение
                        </button>
                      )}
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
                disabled={saving || uploadingImage}
              >
                {saving || uploadingImage ? (
                  <>
                    <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                    Сохранение...
                  </>
                ) : 'Сохранить изменения'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default BookEdit; 