import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import { api } from '../lib/api.js'

const registerSchema = z
  .object({
    firstName: z.string().min(1, 'Ad alanı zorunludur'),
    lastName: z.string().min(1, 'Soyad alanı zorunludur'),
    email: z.string().min(1, 'E-posta alanı zorunludur').email('Geçerli bir e-posta adresi giriniz'),
    password: z.string().min(6, 'Şifre en az 6 karakter olmalıdır'),
    confirmPassword: z.string().min(6, 'Şifre tekrar alanı zorunludur')
  })
  .refine(data => data.password === data.confirmPassword, {
    message: 'Şifreler eşleşmiyor',
    path: ['confirmPassword']
  })

function Register() {
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')
  const [loading, setLoading] = useState(false)
  const navigate = useNavigate()

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors }
  } = useForm({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      firstName: '',
      lastName: '',
      email: '',
      password: '',
      confirmPassword: ''
    }
  })

  const onSubmit = async (formData) => {
    setError('')
    setSuccess('')
    setLoading(true)

    try {
      const response = await api.post('/AccountApi/Register', {
        firstName: formData.firstName,
        lastName: formData.lastName,
        email: formData.email,
        password: formData.password,
        confirmPassword: formData.confirmPassword
      })

      if (response.data.success) {
        setSuccess(response.data.message || 'Kayıt başarılı! Lütfen e-posta kutunuzu kontrol edin.')
        reset()
        setTimeout(() => navigate('/login', { replace: true }), 2000)      
      } else {
        setError(response.data.message || 'Kayıt başarısız.')
      }
    } catch (err) {
      console.error('Register error:', err)
      const apiError =
        err.response?.data?.message ||
        err.response?.data?.errors?.join(', ') ||
        'Kayıt sırasında bir hata oluştu. Lütfen tekrar deneyin.'
      setError(apiError)
    } finally {
      setLoading(false)
      navigate('/login',{replace: true})
    }
  }

  return (
    <div className="container d-flex justify-content-center align-items-center min-vh-100">
      <div className="card shadow" style={{ maxWidth: '500px', width: '100%' }}>
        <div className="card-body p-4">
          <h2 className="text-center mb-4">
            <i className="bi bi-person-plus me-2"></i>Kayıt Ol
          </h2>

          <form onSubmit={handleSubmit(onSubmit)}>
            {error && (
              <div className="alert alert-danger" role="alert">
                <i className="bi bi-exclamation-triangle me-2"></i>{error}
              </div>
            )}

            {success && (
              <div className="alert alert-success" role="alert">
                <i className="bi bi-check-circle me-2"></i>{success}
              </div>
            )}

            <div className="row mb-3">
              <div className="col-md-6">
                <label htmlFor="firstName" className="form-label">
                  Ad
                </label>
                <input
                  type="text"
                  id="firstName"
                  className={`form-control ${errors.firstName ? 'is-invalid' : ''}`}
                  {...register('firstName')}
                  placeholder="Adınızı giriniz"
                  autoComplete="given-name"
                />
                {errors.firstName && (
                  <div className="invalid-feedback">
                    {errors.firstName.message}
                  </div>
                )}
              </div>

              <div className="col-md-6">
                <label htmlFor="lastName" className="form-label">
                  Soyad
                </label>
                <input
                  type="text"
                  id="lastName"
                  className={`form-control ${errors.lastName ? 'is-invalid' : ''}`}
                  {...register('lastName')}
                  placeholder="Soyadınızı giriniz"
                  autoComplete="family-name"
                />
                {errors.lastName && (
                  <div className="invalid-feedback">
                    {errors.lastName.message}
                  </div>
                )}
              </div>
            </div>

            <div className="mb-3">
              <label htmlFor="email" className="form-label">
                E-posta
              </label>
              <input
                type="email"
                id="email"
                className={`form-control ${errors.email ? 'is-invalid' : ''}`}
                {...register('email')}
                placeholder="E-posta adresinizi giriniz"
                autoComplete="email"
              />
              {errors.email && (
                <div className="invalid-feedback">
                  {errors.email.message}
                </div>
              )}
            </div>

            <div className="row mb-3">
              <div className="col-md-6">
                <label htmlFor="password" className="form-label">
                  Şifre
                </label>
                <input
                  type="password"
                  id="password"
                  className={`form-control ${errors.password ? 'is-invalid' : ''}`}
                  {...register('password')}
                  placeholder="Şifrenizi giriniz"
                  autoComplete="new-password"
                />
                {errors.password && (
                  <div className="invalid-feedback">
                    {errors.password.message}
                  </div>
                )}
              </div>

              <div className="col-md-6">
                <label htmlFor="confirmPassword" className="form-label">
                  Şifre Tekrar
                </label>
                <input
                  type="password"
                  id="confirmPassword"
                  className={`form-control ${errors.confirmPassword ? 'is-invalid' : ''}`}
                  {...register('confirmPassword')}
                  placeholder="Şifrenizi tekrar giriniz"
                  autoComplete="new-password"
                />
                {errors.confirmPassword && (
                  <div className="invalid-feedback">
                    {errors.confirmPassword.message}
                  </div>
                )}
              </div>
            </div>

            <button
              type="submit"
              className="btn btn-primary w-100"
              disabled={loading}
            >
              {loading ? (
                <>
                  <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                  Kayıt yapılıyor...
                </>
              ) : (
                <>
                  <i className="bi bi-person-plus me-2"></i>Kayıt Ol
                </>
              )}
            </button>
          </form>

          <div className="text-center mt-3">
            <span className="text-muted">Zaten hesabınız var mı? </span>
            <a href="/login" className="text-decoration-none">
              <i className="bi bi-box-arrow-in-right me-1"></i>Giriş Yap
            </a>
          </div>
        </div>
      </div>
    </div>
  )
}

export default Register