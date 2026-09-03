import axios from 'axios'

// Tüm sayfalar aynı adresi tekrar tekrar yazmasın diye tek noktadan yönetiyoruz.
// Backend portu değişirse sadece burası güncellenir.
const api = axios.create({
  baseURL: 'http://localhost:5184/api'
})

// Backend iş kuralı hatalarını düz metin olarak (BadRequest(ex.Message)) döndürüyor.
// Beklenmedik bir hata gelirse React'in nesne render etmeye çalışıp patlamaması için
// her zaman string'e çeviriyoruz.
export function getErrorMessage(error, fallback = 'Bir hata oluştu.') {
  const data = error?.response?.data
  if (typeof data === 'string' && data.trim()) return data
  if (data?.title) return data.title
  return fallback
}

export default api
