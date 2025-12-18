import { useEffect, useState } from 'react'
import { useSearchParams, Link, useNavigate } from 'react-router-dom'
import api from '../lib/api'
import './LoginPage.css'

function ResetPassword() {
  const [searchParams] = useSearchParams()
  const navigate = useNavigate()
  const [formData, setFormData] = useState({ password: '', confirmPassword: '' })
  const [status, setStatus] = useState({ loading: false, success: false, message: '' })
  const [validParams, setValidParams] = useState(true)

  useEffect(() => {
    const userId = searchParams.get('userId')
    const token = searchParams.get('token')
    if (!userId || !token) {
      setValidParams(false)
      setStatus({ loading: false, success: false, message: 'Geçersiz veya eksik sıfırlama bilgileri' })
    }
  }, [searchParams])

  const handleSubmit = async (e) => {
    e.preventDefault()

    const userId = searchParams.get('userId')
    const token = searchParams.get('token')

    if (!userId || !token) {
      setStatus({ loading: false, success: false, message: 'Geçersiz veya eksik sıfırlama bilgileri' })
      setValidParams(false)
      return
    }

    if (!formData.password || !formData.confirmPassword) {
      setStatus({ loading: false, success: false, message: 'Lütfen yeni şifrenizi giriniz' })
      return
    }

    if (formData.password !== formData.confirmPassword) {
      setStatus({ loading: false, success: false, message: 'Şifreler eşleşmiyor' })
      return
    }

    setStatus({ loading: true, success: false, message: '' })

    try {
      const response = await api.post('/AccountApi/ResetPassword', {
        userId,
        token,
        password: formData.password,
        confirmPassword: formData.confirmPassword
      })

      if (response.data?.success) {
        setStatus({ loading: false, success: true, message: response.data.message || 'Şifreniz başarıyla sıfırlandı.' })
        setTimeout(() => navigate('/login'), 3000)
      } else {
        setStatus({ loading: false, success: false, message: response.data?.message || 'Şifre sıfırlama başarısız.' })
      }
    } catch (err) {
      const message = err.response?.data?.message || err.message || 'Şifre sıfırlama sırasında bir hata oluştu.'
      setStatus({ loading: false, success: false, message })
    }
  }

  if (!validParams) {
    return (
      <div className="login-container">
        <div className="login-card">
          <div className="login-header">
            <h2>
              <i className="fas fa-key"></i> Şifre Sıfırlama
            </h2>
          </div>
          <div className="login-form">
            <div className="alert alert-danger">{status.message}</div>
            <div className="login-footer">
              <p>
                <Link to="/login" className="link">
                  <i className="fas fa-sign-in-alt"></i> Giriş sayfasına dön
                </Link>
              </p>
            </div>
          </div>
        </div>
      </div>
    )
  }

  return (
    <div className="login-container">
      <div className="login-card">
        <div className="login-header">
          <h2>
            <i className="fas fa-key"></i> Şifre Sıfırla
          </h2>
        </div>

        <form className="login-form" onSubmit={handleSubmit}>
          {status.message && (
            <div className={`alert ${status.success ? 'alert-success' : 'alert-danger'}`}>
              {status.message}
            </div>
          )}

          <div className="form-group">
            <label htmlFor="password">
              <i className="fas fa-lock"></i> Yeni Şifre
            </label>
            <input
              type="password"
              id="password"
              name="password"
              value={formData.password}
              onChange={(e) => setFormData({ ...formData, password: e.target.value })}
              placeholder="Yeni şifrenizi giriniz"
              required
              minLength={6}
            />
          </div>

          <div className="form-group">
            <label htmlFor="confirmPassword">
              <i className="fas fa-lock"></i> Şifre Tekrar
            </label>
            <input
              type="password"
              id="confirmPassword"
              name="confirmPassword"
              value={formData.confirmPassword}
              onChange={(e) => setFormData({ ...formData, confirmPassword: e.target.value })}
              placeholder="Yeni şifrenizi tekrar giriniz"
              required
              minLength={6}
            />
          </div>

          <button type="submit" className="btn btn-primary btn-block" disabled={status.loading}>
            {status.loading ? (
              <>
                <i className="fas fa-spinner fa-spin"></i> Şifre sıfırlanıyor...
              </>
            ) : (
              <>
                <i className="fas fa-check"></i> Şifreyi Sıfırla
              </>
            )}
          </button>
        </form>

        <div className="login-footer">
          <p>
            <Link to="/login" className="link">
              <i className="fas fa-sign-in-alt"></i> Giriş sayfasına dön
            </Link>
          </p>
        </div>
      </div>
    </div>
  )
}

export default ResetPassword


