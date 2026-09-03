import { useState, useEffect } from 'react'
import api, { getErrorMessage } from '../api'
import { Page, Alerts, FormCard, Button, Table, RowActions } from '../components/ui'
import { inputStyle } from '../styles'

const emptyForm = { shipId: '', portId: '', arrivalDate: '', departureDate: '', purpose: '' }

// datetime-local input'u "2026-01-05T14:30" formatı ister.
// Backend'den gelen ISO tarihi bu formata kısaltıyoruz.
const toInputValue = (isoDate) => (isoDate ? isoDate.slice(0, 16) : '')

const formatDate = (isoDate) =>
  isoDate ? new Date(isoDate).toLocaleString('tr-TR', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit', hour12: false }) : '-'

export default function Visits() {
  const [visits, setVisits] = useState([])
  const [ships, setShips] = useState([])
  const [ports, setPorts] = useState([])
  const [errorMsg, setErrorMsg] = useState('')
  const [successMsg, setSuccessMsg] = useState('')
  const [editingId, setEditingId] = useState(null)
  const [formData, setFormData] = useState(emptyForm)

  const fetchVisits = () => {
    api.get('/ShipVisits')
      .then(response => setVisits(response.data))
      .catch(error => console.error('Ziyaretler çekilirken hata oluştu:', error))
  }

  useEffect(() => {
    fetchVisits()
    // Gemi ve liman seçim kutularını doldurmak için
    api.get('/Ships').then(r => setShips(r.data)).catch(e => console.error(e))
    api.get('/Ports').then(r => setPorts(r.data)).catch(e => console.error(e))
  }, [])

  const handleInputChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value })
  }

  const resetForm = () => {
    setEditingId(null)
    setFormData(emptyForm)
    setErrorMsg('')
  }

  const handleEditClick = (visit) => {
    setEditingId(visit.visitId)
    setFormData({
      shipId: String(visit.shipId),
      portId: String(visit.portId),
      arrivalDate: toInputValue(visit.arrivalDate),
      departureDate: toInputValue(visit.departureDate),
      purpose: visit.purpose
    })
    setErrorMsg('')
    setSuccessMsg('')
  }

  const handleSubmit = (e) => {
    e.preventDefault()
    setErrorMsg('')
    setSuccessMsg('')

    if (!formData.shipId || !formData.portId || !formData.arrivalDate || !formData.departureDate || !formData.purpose) {
      setErrorMsg('Lütfen tüm zorunlu alanları doldurun!')
      return
    }

    // EKRAN GEREKSİNİMİ: Geliş tarihi, ayrılış tarihinden önce olmalı.
    // Backend de aynı kuralı kontrol ediyor; bu kontrol kullanıcıya anında geri bildirim için.
    if (new Date(formData.arrivalDate) >= new Date(formData.departureDate)) {
      setErrorMsg('Geliş tarihi, ayrılış tarihinden önce olmalıdır!')
      return
    }

    const payload = {
      ShipId: Number(formData.shipId),
      PortId: Number(formData.portId),
      ArrivalDate: formData.arrivalDate,
      DepartureDate: formData.departureDate,
      Purpose: formData.purpose
    }

    const request = editingId
      ? api.put(`/ShipVisits/${editingId}`, payload)
      : api.post('/ShipVisits', payload)

    request
      .then(() => {
        setSuccessMsg(editingId ? 'Ziyaret kaydı güncellendi!' : 'Gemi ziyareti başarıyla kaydedildi!')
        resetForm()
        fetchVisits()
      })
      .catch(error => setErrorMsg(getErrorMessage(error, 'Ziyaret kaydedilirken bir hata oluştu.')))
  }

  const handleDelete = (visit) => {
    if (!window.confirm('Bu ziyaret kaydını silmek istediğinize emin misiniz?')) return

    api.delete(`/ShipVisits/${visit.visitId}`)
      .then(() => {
        setSuccessMsg('Ziyaret kaydı başarıyla silindi!')
        if (editingId === visit.visitId) resetForm()
        fetchVisits()
      })
      .catch(error => setErrorMsg(getErrorMessage(error, 'Silme işlemi başarısız oldu.')))
  }

  const eksikVeri = ships.length === 0 || ports.length === 0

  return (
    <Page icon="📅" title="Ziyaret Kayıtları" description="Gemilerin limanlara geliş ve ayrılış kayıtlarını yönetin.">

      <FormCard editing={editingId} title={editingId ? 'Ziyareti Güncelle' : 'Yeni Ziyaret Ekle'}>
        <Alerts error={errorMsg} success={successMsg} />

        {eksikVeri ? (
          <p style={{ margin: 0, fontStyle: 'italic', color: '#466581' }}>
            Ziyaret kaydı için sistemde en az bir gemi ve bir liman olmalı.
            {ships.length === 0 && ' Önce Gemi Yönetimi sayfasından gemi ekleyin.'}
            {ports.length === 0 && ' Önce Liman Yönetimi sayfasından liman ekleyin.'}
          </p>
        ) : (
          <form onSubmit={handleSubmit} style={{ display: 'flex', gap: '10px', flexWrap: 'wrap' }}>
            <select name="shipId" value={formData.shipId} onChange={handleInputChange} style={{ ...inputStyle, minWidth: '180px' }}>
              <option value="">-- Gemi Seçin --</option>
              {ships.map(s => (
                <option key={s.shipId} value={s.shipId}>{s.name} ({s.imo})</option>
              ))}
            </select>

            <select name="portId" value={formData.portId} onChange={handleInputChange} style={{ ...inputStyle, minWidth: '180px' }}>
              <option value="">-- Liman Seçin --</option>
              {ports.map(p => (
                <option key={p.portId} value={p.portId}>{p.name} - {p.city}</option>
              ))}
            </select>

            <label style={{ display: 'flex', flexDirection: 'column', fontSize: '13px', color: '#495057', flex: '1', minWidth: '190px' }}>
              Geliş Tarihi
              <input type="datetime-local" name="arrivalDate" value={formData.arrivalDate} onChange={handleInputChange} style={{ ...inputStyle, marginTop: '5px' }} />
            </label>

            <label style={{ display: 'flex', flexDirection: 'column', fontSize: '13px', color: '#495057', flex: '1', minWidth: '190px' }}>
              Ayrılış Tarihi
              <input type="datetime-local" name="departureDate" value={formData.departureDate} onChange={handleInputChange} style={{ ...inputStyle, marginTop: '5px' }} />
            </label>

            <input type="text" name="purpose" placeholder="Ziyaret Amacı (Örn: Yükleme, Bakım)" value={formData.purpose} onChange={handleInputChange} style={{ ...inputStyle, minWidth: '220px' }} />

            <Button type="submit" variant={editingId ? 'update' : 'primary'}>
              {editingId ? 'Güncelle' : 'Kaydet'}
            </Button>

            {editingId && (
              <Button type="button" variant="neutral" onClick={resetForm}>İptal</Button>
            )}
          </form>
        )}
      </FormCard>

      {visits.length === 0 ? (
        <p>Sistemde henüz ziyaret kaydı bulunmuyor...</p>
      ) : (
        <Table headers={['ID', 'Gemi', 'Liman', 'Geliş', 'Ayrılış', 'Amaç', 'İşlemler']}>
          {visits.map(visit => (
            <tr key={visit.visitId}>
              <td>{visit.visitId}</td>
              <td>{visit.ship?.name ?? `#${visit.shipId}`}</td>
              <td>{visit.port ? `${visit.port.name} (${visit.port.city})` : `#${visit.portId}`}</td>
              <td>{formatDate(visit.arrivalDate)}</td>
              <td>{formatDate(visit.departureDate)}</td>
              <td>{visit.purpose}</td>
              <RowActions onEdit={() => handleEditClick(visit)} onDelete={() => handleDelete(visit)} />
            </tr>
          ))}
        </Table>
      )}
    </Page>
  )
}
