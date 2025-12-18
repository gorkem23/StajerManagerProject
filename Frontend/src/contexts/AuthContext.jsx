import { createContext, useContext, useEffect, useState } from 'react'
import { api } from '../lib/api.js'

const AuthContext = createContext({
  user: null,
  loading: true,
  setUser: () => {},
  refreshUser: async () => {},
  logout: async () => {}
})

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null)
  const [loading, setLoading] = useState(true)

  const refreshUser = async () => {
    try {
      const res = await api.get('/AccountApi/GetCurrentUser')
      if (res.data.isAuthenticated) {
        setUser({
          email: res.data.email,
          userName: res.data.userName,
          role: res.data.role,
          isAdmin: res.data.isAdmin          
        })
      } else {
        setUser(null)
      }
    } catch (err) {
      console.error('Auth kontrolü hata verdi:', err)
      setUser(null)
    } finally {
      setLoading(false)
    }
  }

  const logout = async () => {
    try {
      await api.post('/AccountApi/Logout')
    } catch (err) {
      console.error('Logout hatası:', err)
    } finally {
      setUser(null)
      setLoading(false)
    }
  }

  useEffect(() => {
    refreshUser()
  }, [])

  return (
    <AuthContext.Provider value={{ user, loading, setUser, refreshUser, logout }}>
      {children}
    </AuthContext.Provider>
  )
}

export const useAuth = () => useContext(AuthContext)