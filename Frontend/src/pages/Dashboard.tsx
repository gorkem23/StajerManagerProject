import { useState, useEffect } from 'react'
import stajerService from '../services/stajerService.js'
import api from '../lib/api.js'
import { DashboardStats } from '../types/dashboard.types'

function Dashboard() {
  const [stats, setStats] = useState<DashboardStats>({
    totalStajers: 0,
    activeStajers: 0,
    totalDepartments: 0,
    totalUniversities: 0,
    thisMonthStajers: 0
  })
  const [loading, setLoading] = useState<boolean>(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    loadStats()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const loadStats = async (): Promise<void> => {
    try {
      setLoading(true)
      setError(null)  

      // Dashboard verilerini backend'den al
      const [dashboardRes, stajersRes] = await Promise.all([
        api.get<{ success: boolean; data: DashboardStats }>('/DashboardApi'),
        stajerService.getAll().catch(() => null) // Hata durumunda null dönsün
      ])

      if (dashboardRes?.data?.success && dashboardRes.data.data) {
        const data = dashboardRes.data.data
        setStats({
          totalStajers: data.totalStajers || 0,
          activeStajers: data.activeStajers || 0,
          totalDepartments: data.totalDepartments || 0,
          totalUniversities: data.totalUniversities || 0,
          thisMonthStajers: data.thisMonthStajers || 0
        })
      } else if (stajersRes) {
        // Eğer dashboard API çalışmazsa, sadece stajer sayısını al
        setStats(prev => ({
          ...prev,
          totalStajers: Array.isArray(stajersRes) ? stajersRes.length : 0
        }))
      }
    } catch (err: any) {
      console.error('İstatistikler yüklenirken hata:', err)
      setError(err.message || 'İstatistikler yüklenemedi')
    } finally {
      setLoading(false)
    }
  }

  if (loading) {
    return (
      <div className="dashboard">
        <div className="loading">Yükleniyor...</div>
      </div>
    )
  }

  if (error) {
    return (
      <div className="dashboard">
        <div className="error">Hata: {error}</div>
        <button onClick={loadStats}>Tekrar Dene</button>
      </div>
    )
  }

  return (
    <div className="container mt-4">
      <h1 className="mb-4">Dashboard</h1>

      <div className="row g-4">
        
        <div className="col-md-3">
          <div className="card text-center shadow-sm">
            <div className="card-body">
              <h5 className="card-title">Toplam Stajer</h5>
              <p className="display-6">{stats.totalStajers}</p>
            </div>
          </div>
        </div>

        <div className="col-md-3">
          <div className="card text-center shadow-sm">
            <div className="card-body">
              <h5 className="card-title">Aktif Stajer</h5>
              <p className="display-6">{stats.activeStajers}</p>
            </div>
          </div>
        </div>

        <div className="col-md-3">
          <div className="card text-center shadow-sm">
            <div className="card-body">
              <h5 className="card-title">Departman Sayısı</h5>
              <p className="display-6">{stats.totalDepartments}</p>
            </div>
          </div>
        </div>

        <div className="col-md-3">
          <div className="card text-center shadow-sm">
            <div className="card-body">
              <h5 className="card-title">Üniversite Sayısı</h5>
              <p className="display-6">{stats.totalUniversities}</p>
            </div>
          </div>
        </div>

        <div className="col-md-3">
          <div className="card text-center shadow-sm">
            <div className="card-body">
              <h5 className="card-title">Bu Ay Stajer</h5>
              <p className="display-6">{stats.thisMonthStajers}</p>
            </div>
          </div>
        </div>

      </div>
    </div>
  )
}

export default Dashboard

