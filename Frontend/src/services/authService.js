import { api } from '../lib/api.js'

export const authService = {
  // Kullanıcı kayıt fonksiyonu
  register: async (userData) => {
    try {
      const response = await api.post('/AccountApi/Register', {
        FirstName: userData.firstName,
        LastName: userData.lastName,
        Email: userData.email,
        Password: userData.password,
        ConfirmPassword: userData.confirmPassword
      })
      return response.data
    } catch (error) {
      // Hata durumunda backend'den gelen mesajı döndür
      if (error.response?.data) {
        throw error.response.data
      }
      throw { success: false, message: 'Bir hata oluştu. Lütfen tekrar deneyin.' }
    }
  }
}

export default authService

