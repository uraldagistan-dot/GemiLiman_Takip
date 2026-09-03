import { useState, useEffect } from 'react'
import api, { getErrorMessage } from '../api'
import { Page, Alerts, FormCard, Button, Table, RowActions } from '../components/ui'
import { inputStyle } from '../styles'

const emptyForm = { name: '', country: '', city: '' }

export default function Ports() {
  const [ports, setPorts] = useState([])
  const [errorMsg, setErrorMsg] = useState('')
  const [successMsg, setSuccessMsg] = useState('')
  const [editingId, setEditingId] = useState(null)
  const [formData, setFormData] = useState(emptyForm)

  const fetchPorts = () => {
    api.get('/Ports')
      .then(response => setPorts(response.data))
      .catch(error => console.error('Limanlar çekilirken hata oluştu:', error))
  }

  useEffect(() => {
    fetchPorts()
  }, [])

  const handleInputChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value })
  }

  const resetForm = () => {
    setEditingId(null)
    setFormData(emptyForm)
    setErrorMsg('')
  }

  const handleEditClick = (port) => {
    setEditingId(port.portId)
    setFormData({ name: port.name, country: port.country, city: port.city })
    setErrorMsg('')
    setSuccessMsg('')
  }

  const handleSubmit = (e) => {
    e.preventDefault()
    setErrorMsg('')
    setSuccessMsg('')

    if (!formData.name || !formData.country || !formData.city) {
      setErrorMsg('Lütfen tüm zorunlu alanları doldurun!')
      return
    }

    const payload = {
      Name: formData.name,
      Country: formData.country,
      City: formData.city
    }

    const request = editingId
      ? api.put(`/Ports/${editingId}`, payload)
      : api.post('/Ports', payload)

    request
      .then(() => {
        setSuccessMsg(editingId ? 'Liman başarıyla güncellendi!' : 'Liman başarıyla eklendi!')
        resetForm()
        fetchPorts()
      })
      .catch(error => setErrorMsg(getErrorMessage(error, 'Liman kaydedilirken bir hata oluştu.')))
  }

  const handleDelete = (port) => {
    const uyari = `"${port.name}" limanını silmek istediğinize emin misiniz?\n\nBu limana ait ziyaret kayıtları da silinecek.`
    if (!window.confirm(uyari)) return

    api.delete(`/Ports/${port.portId}`)
      .then(() => {
        setSuccessMsg('Liman başarıyla silindi!')
        if (editingId === port.portId) resetForm()
        fetchPorts()
      })
      .catch(error => setErrorMsg(getErrorMessage(error, 'Silme işlemi başarısız oldu.')))
  }

  return (
    <Page icon="⚓" title="Liman Yönetimi" description="Sistemdeki limanları ekleyebilir, güncelleyebilir veya silebilirsiniz.">

      <FormCard editing={editingId} title={editingId ? 'Limanı Güncelle' : 'Yeni Liman Ekle'}>
        <Alerts error={errorMsg} success={successMsg} />

        <form onSubmit={handleSubmit} style={{ display: 'flex', gap: '10px', flexWrap: 'wrap' }}>
          <input type="text" name="name" placeholder="Liman Adı" value={formData.name} onChange={handleInputChange} style={inputStyle} />
          <input type="text" name="country" placeholder="Ülke (Örn: Türkiye)" value={formData.country} onChange={handleInputChange} style={inputStyle} />
          <input type="text" name="city" placeholder="Şehir (Örn: İzmir)" value={formData.city} onChange={handleInputChange} style={inputStyle} />

          <Button type="submit" variant={editingId ? 'update' : 'primary'}>
            {editingId ? 'Güncelle' : 'Kaydet'}
          </Button>

          {editingId && (
            <Button type="button" variant="neutral" onClick={resetForm}>İptal</Button>
          )}
        </form>
      </FormCard>

      {ports.length === 0 ? (
        <p>Sistemde henüz liman bulunmuyor...</p>
      ) : (
        <Table headers={['ID', 'Liman Adı', 'Ülke', 'Şehir', 'İşlemler']}>
          {ports.map(port => (
            <tr key={port.portId}>
              <td>{port.portId}</td>
              <td>{port.name}</td>
              <td>{port.country}</td>
              <td>{port.city}</td>
              <RowActions onEdit={() => handleEditClick(port)} onDelete={() => handleDelete(port)} />
            </tr>
          ))}
        </Table>
      )}
    </Page>
  )
}
