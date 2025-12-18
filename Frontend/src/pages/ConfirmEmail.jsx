import { useEffect, useState } from 'react'
import { useSearchParams, Link } from 'react-router-dom'
import api from '../lib/api'
import './LoginPage.css'

function ConfirmEmail() {
  const [searchParams] = useSearchParams()
  const [status, setStatus] = useState({ loading: true, success: false, message: '' })

  useEffect(() => {
    const userId = searchParams.get('userId')
    const token = searchParams.get('token')

    const confirm = async () => {
      if (!userId || !token) {
        setStatus({ loading: false, success: false, message: 'Geçersiz veya eksik doğrulama bilgileri' })
        return
      }

      try {
        const response = await api.get('/AccountApi/ConfirmEmail', {
          params: { userId, token }
        })

        if (response.data?.success) {
          setStatus({ loading: false, success: true, message: response.data.message || 'E-posta başarıyla doğrulandı.' })
        } else {
          setStatus({ loading: false, success: false, message: response.data?.message || 'E-posta doğrulama başarısız.' })
        }
      } catch (err) {
        const message = err.response?.data?.message || err.message || 'E-posta doğrulama sırasında bir hata oluştu.'
        setStatus({ loading: false, success: false, message })
      }
    }

    confirm()
  }, [searchParams])

  return (
    <div className="login-container">
      <div className="login-card">
        <div className="login-header">
          <h2>
            <i className="fas fa-envelope-open-text"></i> E-posta Doğrulama
          </h2>
        </div>

        <div className="login-form">
          {status.loading ? (
            <div className="alert alert-success">
              <i className="fas fa-spinner fa-spin"></i> E-posta doğrulanıyor...
            </div>
          ) : (
            <div className={`alert ${status.success ? 'alert-success' : 'alert-danger'}`}>
              {status.message}
            </div>
          )}

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

export default ConfirmEmail


