// Frontend/src/components/LoginForm.jsx
import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Link, useNavigate } from 'react-router-dom'
import { api } from '../lib/api'
import { useAuth } from '../contexts/AuthContext.jsx'


// Zod validation schema
const loginSchema = z.object({
  email: z
    .string()
    .min(1, 'E-posta adresi gereklidir')
    .email('Geçerli bir e-posta adresi giriniz'),
  password: z
    .string()
    .min(1, 'Şifre gereklidir'),
  rememberMe: z.boolean().optional().default(false)
})


export default function LoginForm() {
  const navigate = useNavigate()
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const { refreshUser } = useAuth()

  const {
    register,
    handleSubmit,
    formState: { errors }
  } = useForm({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      email: '',
      password: '',
      rememberMe: false
    }
  })

  const onSubmit = async (data) => {
    setError('')
    setLoading(true)

    try {
      const response = await api.post('/AccountApi/Login', {
        Email: data.email,        
        Password: data.password,
        RememberMe: data.rememberMe
      })

      if (response.data.success) {
        await refreshUser()
        // Başarılı login - Dashboard'a yönlendir
        navigate('/',{replace: true})
      } else {
        setError(response.data.message || 'Giriş başarısız')
      }
    } catch (err) {
      if (err.response?.data?.message) {
        setError(err.response.data.message)
      } else if (err.response?.data?.errors) {
        // Backend validation hataları
        setError(err.response.data.errors.join(', '))
      } else {
        setError('Bir hata oluştu. Lütfen tekrar deneyin.')
      }
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="container d-flex justify-content-center align-items-center min-vh-100">
      <div className="card shadow" style={{ maxWidth: '400px', width: '100%' }}>
        <div className="card-body p-4">
          <h2 className="text-center mb-4">Giriş Yap</h2>

          {error && (
            <div className="alert alert-danger">
              {error}
            </div>
          )}

          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="mb-3">
              <label htmlFor="email" className="form-label">
                E-posta
              </label>
              <input
                id="email"
                type="email"
                {...register('email')}
                className={`form-control ${errors.email ? 'is-invalid' : ''}`}
                placeholder="E-posta adresinizi giriniz"
              />
              {errors.email && (
                <div className="invalid-feedback">
                  {errors.email.message}
                </div>
              )}
            </div>

            <div className="mb-3">
              <label htmlFor="password" className="form-label">
                Şifre
              </label>
              <input
                id="password"
                type="password"
                {...register('password')}
                className={`form-control ${errors.password ? 'is-invalid' : ''}`}
                placeholder="Şifrenizi giriniz"
              />
              {errors.password && (
                <div className="invalid-feedback">
                  {errors.password.message}
                </div>
              )}
            </div>

            <div className="mb-3 form-check">
              <input
                type="checkbox"
                className="form-check-input"
                id="rememberMe"
                {...register('rememberMe')}
              />
              <label className="form-check-label" htmlFor="rememberMe">
                Beni hatırla
              </label>
            </div>

            <button
              type="submit"
              disabled={loading}
              className="btn btn-primary w-100"
            >
              {loading ? 'Giriş yapılıyor...' : 'Giriş Yap'}
            </button>
          </form>

          <div className="text-center mt-3">
            Hesabınız yok mu? <Link to="/register" className="text-decoration-none">Kayıt Ol</Link>
          </div>
        </div>
      </div>
    </div>
  )
}