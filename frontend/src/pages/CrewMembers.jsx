import { useState, useEffect } from 'react'
import api, { getErrorMessage } from '../api'
import { Page, Alerts, FormCard, Button, Table, RowActions } from '../components/ui'
import { inputStyle } from '../styles'

const emptyForm = { firstName: '', lastName: '', email: '', phoneNumber: '', role: '' }

// EKRAN GEREKSİNİMİ: E-posta ve telefon validasyonu.
// Backend'de de aynı kurallar var; buradakiler kullanıcıya anında geri bildirim için.
const EMAIL_PATTERN = /^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$/
const PHONE_PATTERN = /^(\+90|0)?\s*5\d{2}\s*\d{3}\s*\d{2}\s*\d{2}$/

export default function CrewMembers() {
  const [crew, setCrew] = useState([])
  const [errorMsg, setErrorMsg] = useState('')
  const [successMsg, setSuccessMsg] = useState('')
  const [editingId, setEditingId] = useState(null)
  const [formData, setFormData] = useState(emptyForm)

  const fetchCrew = () => {
    api.get('/CrewMembers')
      .then(response => setCrew(response.data))
      .catch(error => console.error('Mürettebat çekilirken hata oluştu:', error))
  }

  useEffect(() => {
    fetchCrew()
  }, [])

  const handleInputChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value })
  }

  const resetForm = () => {
    setEditingId(null)
    setFormData(emptyForm)
    setErrorMsg('')
  }

  const handleEditClick = (c) => {
    setEditingId(c.crewId)
    setFormData({
      firstName: c.firstName,
      lastName: c.lastName,
      email: c.email ?? '',
      phoneNumber: c.phoneNumber ?? '',
      role: c.role
    })
    setErrorMsg('')
    setSuccessMsg('')
  }

  const handleSubmit = (e) => {
    e.preventDefault()
    setErrorMsg('')
    setSuccessMsg('')

    // Zorunlu alan kontrolleri (görev alanı dahil)
    if (!formData.firstName || !formData.lastName || !formData.role) {
      setErrorMsg('Ad, soyad ve görev alanları zorunludur!')
      return
    }
    if (!formData.email) {
      setErrorMsg('E-posta zorunludur!')
      return
    }
    if (!EMAIL_PATTERN.test(formData.email)) {
      setErrorMsg('Geçersiz e-posta formatı! (örnek: ali@firma.com)')
      return
    }
    if (!formData.phoneNumber) {
      setErrorMsg('Telefon numarası zorunludur!')
      return
    }
    if (!PHONE_PATTERN.test(formData.phoneNumber)) {
      setErrorMsg('Geçersiz telefon formatı! (örnek: +90 532 123 45 67)')
      return
    }

    const payload = {
      FirstName: formData.firstName,
      LastName: formData.lastName,
      Email: formData.email,
      PhoneNumber: formData.phoneNumber,
      Role: formData.role
    }

    const request = editingId
      ? api.put(`/CrewMembers/${editingId}`, payload)
      : api.post('/CrewMembers', payload)

    request
      .then(() => {
        setSuccessMsg(editingId ? 'Personel başarıyla güncellendi!' : 'Mürettebat başarıyla eklendi!')
        resetForm()
        fetchCrew()
      })
      .catch(error => setErrorMsg(getErrorMessage(error, 'Personel kaydedilirken bir hata oluştu.')))
  }

  const handleDelete = (c) => {
    const uyari = `${c.firstName} ${c.lastName} kaydını silmek istediğinize emin misiniz?\n\nBu personelin gemi atamaları da silinecek.`
    if (!window.confirm(uyari)) return

    api.delete(`/CrewMembers/${c.crewId}`)
      .then(() => {
        setSuccessMsg('Personel başarıyla silindi!')
        if (editingId === c.crewId) resetForm()
        fetchCrew()
      })
      .catch(error => setErrorMsg(getErrorMessage(error, 'Silme işlemi başarısız oldu.')))
  }

  return (
    <Page icon="👨‍✈️" title="Mürettebat Yönetimi" description="Sisteme personel ekleyebilir, bilgilerini güncelleyebilir veya silebilirsiniz.">

      <FormCard editing={editingId} title={editingId ? '✏️ Personeli Güncelle' : '➕ Yeni Mürettebat Ekle'}>
        <Alerts error={errorMsg} success={successMsg} />

        {/* noValidate: tarayıcının kendi (İngilizce) uyarısı devreye girip formu kesmesin;
            doğrulama mesajları tek yerden, Türkçe olarak yukarıdaki kurallardan gelsin. */}
        <form onSubmit={handleSubmit} noValidate style={{ display: 'flex', gap: '10px', flexWrap: 'wrap' }}>
          <input type="text" name="firstName" placeholder="Ad" value={formData.firstName} onChange={handleInputChange} style={inputStyle} />
          <input type="text" name="lastName" placeholder="Soyad" value={formData.lastName} onChange={handleInputChange} style={inputStyle} />
          <input type="email" name="email" placeholder="E-posta (ali@firma.com)" value={formData.email} onChange={handleInputChange} style={inputStyle} />
          <input type="tel" name="phoneNumber" placeholder="Telefon (+90 532 123 45 67)" value={formData.phoneNumber} onChange={handleInputChange} style={inputStyle} />
          <input type="text" name="role" placeholder="Görevi (Örn: Kaptan, Mühendis)" value={formData.role} onChange={handleInputChange} style={inputStyle} />

          <Button type="submit" variant={editingId ? 'update' : 'primary'}>
            {editingId ? 'Güncelle' : 'Kaydet'}
          </Button>

          {editingId && (
            <Button type="button" variant="neutral" onClick={resetForm}>İptal</Button>
          )}
        </form>
      </FormCard>

      {crew.length === 0 ? (
        <p>Sistemde henüz mürettebat bulunmuyor...</p>
      ) : (
        <Table headers={['ID', 'Ad Soyad', 'Görev', 'E-posta', 'Telefon', 'İşlemler']}>
          {crew.map(c => (
            <tr key={c.crewId}>
              <td>{c.crewId}</td>
              <td><strong>{c.firstName} {c.lastName}</strong></td>
              <td>
                <span style={{ padding: '4px 8px', backgroundColor: '#e9ecef', borderRadius: '4px', fontSize: '14px', color: '#495057' }}>
                  {c.role}
                </span>
              </td>
              <td>{c.email || <em style={{ color: '#adb5bd' }}>-</em>}</td>
              <td>{c.phoneNumber || <em style={{ color: '#adb5bd' }}>-</em>}</td>
              <RowActions onEdit={() => handleEditClick(c)} onDelete={() => handleDelete(c)} />
            </tr>
          ))}
        </Table>
      )}
    </Page>
  )
}
