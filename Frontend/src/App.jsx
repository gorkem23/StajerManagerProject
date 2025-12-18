import { Routes, Route, Link, useLocation, Navigate } from 'react-router-dom'
import Departmanlar from './pages/Departmanlar'
import StajersPage from './pages/StajersPage'
import Dashboard from './pages/Dashboard'
import LoginForm from './components/LoginForm'
import Register from './pages/Register'
import { useAuth } from './contexts/AuthContext'
import Universities from './pages/Universities'
import { useState, useEffect } from 'react'

/**
 * Kullanıcının admin olup olmadığını kontrol eder
 * @param {Object} user - Kullanıcı objesi
 * @param {boolean} authLoading - Authentication yükleme durumu
 * @returns {boolean} - Admin ise true, değilse false
 */
function checkIsAdmin(user, authLoading) {
  if (authLoading || !user) return false
  const role = (user?.role ?? '').trim().toLowerCase()
  const email = (user?.email ?? '').trim().toLowerCase()
  return user?.isAdmin || role === 'admin' || email === 'admin@stajermanager.com'
}

function Navigation() {
  const location = useLocation()
  const { user, logout, loading: authLoading } = useAuth()
  const [isScrolled, setIsScrolled] = useState(false)


  useEffect(() => {
    const handleScroll = () => {
      const scrollPosition = window.scrollY || window.pageYOffset
      setIsScrolled(scrollPosition > 10)
    }

    window.addEventListener('scroll', handleScroll)
    return () => window.removeEventListener('scroll', handleScroll)
  }, [])
  
  // Admin kontrolü - utility function kullan
  const isAdmin = checkIsAdmin(user, authLoading)

  if (!user) {
    return null
  }

  return (

    <nav className={`navbar navbar-expand-lg fixed-top ${isScrolled ? 'bg-primary-light' : 'bg-body-tertiary'}`}
    style={{
      transition: 'background-color 1s ease',
      ...(isScrolled && { backgroundColor: 'rgba(143, 185, 228, 0.95)' })
    }}>
      <div className="container-fluid">
        <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
          <a className="navbar-brand d-lg-none" href="#">Menu</a>
          <span className="navbar-toggler-icon"></span>
        </button>
        <div className="collapse navbar-collapse" id="navbarNav">
          <ul className="navbar-nav">
            <li className="nav-item">
              <Link
                to="/stajers"
                className={['/','/stajers'].includes(location.pathname) ? 'nav-link active' : 'nav-link'}
              >
                Stajerler
              </Link>
            </li>
            <li className="nav-item">
              <Link
                to="/departmanlar"
                className={location.pathname === '/departmanlar' ? 'nav-link active' : 'nav-link'}
              >
                Departmanlar
              </Link>
            </li>
          </ul>
          <ul className="navbar-nav ms-auto">
            {isAdmin && (
              <li className="nav-item d-flex align-items-center">
                <span className="badge bg-success d-flex align-items-center gap-1">
                  <i className="bi bi-person-arms-up"></i>
                  Admin
                </span>
              </li>
            )}
            <li className="nav-item">
              <button type="button" className="btn btn-outline-danger btn-sm d-flex align-items-center gap-1" onClick={logout}>
                <i className="bi bi-person-walking"></i>
                <i className="bi bi-door-open"></i>
                Çıkış Yap
              </button>
            </li>
          </ul> 
        </div>
      </div>
    </nav>   
  )
}

function PrivateRoute({ children }) {
  const { user, loading } = useAuth()

  if (loading) {
    return <div>Yükleniyor...</div>
  }
  return user ? children : <Navigate to="/login" replace />
}

function DashboardRoute() {
  const { user, loading: authLoading } = useAuth()
  
  if (authLoading) {
    return <div>Yükleniyor...</div>
  }
  
  // Admin kontrolü - utility function kullan
  const isAdmin = checkIsAdmin(user, authLoading)
  
  return isAdmin ? <Dashboard /> : <Navigate to="/stajers" replace />
}

function App() {
  return (
    <div className="min-vh-100 bg-body-tertiary d-flex flex-column">
      <Navigation />
      <main className="container-fluid py-4 flex-grow-1">
        <Routes>
          <Route
            path="/"
            element={
              <PrivateRoute>
                <Navigate to="/stajers" replace />
              </PrivateRoute>
            }
          />
          <Route
            path="/stajers"
            element={
              <PrivateRoute>
                <StajersPage />
              </PrivateRoute>
            }
          />
          <Route
            path="/departmanlar"
            element={
              <PrivateRoute>
                <Departmanlar />
              </PrivateRoute>
            }
          />
          <Route
            path="/universities"
            element={
              <PrivateRoute>
                <Universities />
              </PrivateRoute>
            }
          />
          <Route path="/login" element={<LoginForm />} />
          <Route path="/register" element={<Register />} />
        </Routes>
      </main>
      <footer className="bg-dark text-light py-3 mt-auto">
        <div className="marquee-container">
          <div className="marquee-text">
            🎉 Stajer Manager'a Hoş Geldiniz! • Tüm hakları saklıdır • © 2024 Stajer Manager • 🎉 Stajer Manager'a Hoş Geldiniz! • Tüm hakları saklıdır • © 2024 Stajer Manager
          </div>
        </div>
      </footer>
    </div>
  )
}

export default App
