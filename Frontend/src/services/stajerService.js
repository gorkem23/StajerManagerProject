import { api } from '../lib/api.js'

export const stajerService = {
  // Tüm stajerleri getir (arama ve sıralama ile)
  getAll: async (sortBy = 'StajerID', sortOrder = 'desc', searchText = '') => {
    const response = await api.get('/StajersApi', {
      params: {
        sortBy,
        sortOrder,
        searchText: searchText || ''
      }
    })
    return response.data
  },

  // Tek bir stajer getir
  getById: async (id) => {
    const response = await api.get(`/StajersApi/${id}`)
    return response.data?.data || response.data
  },

  // Yeni stajer oluştur
  create: async (stajer) => {
    const response = await api.post('/StajersApi', stajer)
    return response.data
  },

  // Stajer güncelle
  update: async (id, stajer) => {
    const response = await api.put(`/StajersApi/${id}`, stajer)
    return response.data
  },

  // Stajer sil
  delete: async (id) => {
    const response = await api.delete(`/StajersApi/${id}`)
    return response.data
  },

  // Üniversiteye göre bölümleri getir (cascade dropdown için)
  getBolumlerByUniversite: async (universiteId) => {
    try {
      const response = await api.get(`/StajersApi/bolumler/${universiteId}`)
      const data = response.data
      if (data.success && data.bolumler) {
        return data.bolumler.map(bol => ({
          bolumID: bol.bolumID || bol.BolumID,
          bolumAdi: bol.bolumAdi || bol.BolumAdi,
          universiteID: bol.universiteID || bol.UniversiteID,
          aktif: bol.aktif !== undefined ? bol.aktif : (bol.Aktif !== undefined ? bol.Aktif : true)
        }))
      }
      return []
    } catch (err) {
      console.error('Bölümler yüklenirken hata:', err)
      return []
    }
  },

  // Dropdown'lar için verileri getir
  getDropdownData: async () => {
    let departmanlar = []
    let universiteler = []
    
    try {
      const departmanRes = await api.get('/DepartmanApi')
      departmanlar = (departmanRes.data || []).map(dep => ({
        departmanID: dep.DepartmanID || dep.departmanID,
        departmanAdi: dep.DepartmanAdi || dep.departmanAdi,
        aciklama: dep.Aciklama || dep.aciklama
      }))
    } catch (err) {
      console.error('Departmanlar yüklenirken hata:', err)
      departmanlar = []
    }
    
    try {
      const universiteRes = await api.get('/UniversiteApi')
      universiteler = (universiteRes.data || []).map(uni => ({
        universiteID: uni.UniversiteID || uni.universiteID,
        universiteAdi: uni.UniversiteAdi || uni.universiteAdi,
        aktif: uni.Aktif !== undefined ? uni.Aktif : (uni.aktif !== undefined ? uni.aktif : true)
      }))
    } catch (err) {
      console.error('Üniversiteler yüklenirken hata:', err)
      universiteler = []
    }
    
    return {
      departmanlar,
      universiteler
    }
  }
}

export default stajerService



