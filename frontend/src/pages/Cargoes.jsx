import { useState, useEffect } from 'react'
import api, { getErrorMessage } from '../api'
import { Page, Alerts, FormCard, Button, Table, RowActions } from '../components/ui'
import { inputStyle } from '../styles'

const emptyForm = { description: '', weightTon: '', cargoType: '', shipId: '' }

export default function Cargoes() {
  const [cargoes, setCargoes] = useState([])
  const [ships, setShips] = useState([])
  const [errorMsg, setErrorMsg] = useState('')
  const [successMsg, setSuccessMsg] = useState('')

  // Hangi yükü güncellediğimizi tutar
  const [editingId, setEditingId] = useState(null)

  // EKRAN GEREKSİNİMİ: "Gemiye ait yüklerin listesi" - listeyi gemiye göre filtreler
  const [filterShipId, setFilterShipId] = useState('')

  const [formData, setFormData] = useState(emptyForm)

  const fetchCargoes = (shipId = filterShipId) => {
    // Filtre seçiliyse backend'in gemiye özel endpoint'ini kullanıyoruz
    const url = shipId ? `/Cargoes/ship/${shipId}` : '/Cargoes'
    api.get(url)
      .then(response => setCargoes(response.data))
      .catch(error => console.error('Veri çekilemedi:', error))
  }

  useEffect(() => {
    // İlk açılışta filtresiz tüm liste. fetchCargoes'u bağımlılık yapmamak için
    // çağrı burada doğrudan yapılıyor.
    api.get('/Cargoes').then(r => setCargoes(r.data)).catch(e => console.error(e))
    api.get('/Ships').then(r => setShips(r.data)).catch(e => console.error(e))
  }, [])

  const handleFilterChange = (e) => {
    const value = e.target.value
    setFilterShipId(value)
    fetchCargoes(value)
  }

  const handleInputChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value })
  }

  const resetForm = () => {
    setEditingId(null)
    setFormData(emptyForm)
    setErrorMsg('')
  }

  const handleEditClick = (cargo) => {
    setEditingId(cargo.cargoId)
    setFormData({
      description: cargo.description,
      weightTon: cargo.weightTon,
      cargoType: cargo.cargoType,
      shipId: String(cargo.shipId)
    })
    setErrorMsg('')
    setSuccessMsg('')
  }

  const handleSubmit = (e) => {
    e.preventDefault()
    setErrorMsg('')
    setSuccessMsg('')

    if (!formData.description || !formData.weightTon || !formData.cargoType || !formData.shipId) {
      setErrorMsg('Lütfen tüm zorunlu alanları doldurun!')
      return
    }

    // EKRAN GEREKSİNİMİ: Ağırlık > 0 olmalı
    if (Number(formData.weightTon) <= 0) {
      setErrorMsg('Ağırlık 0 veya negatif olamaz!')
      return
    }

    const payload = {
      Description: formData.description,
      WeightTon: Number(formData.weightTon),
      CargoType: formData.cargoType,
      ShipId: Number(formData.shipId)
    }

    const request = editingId
      ? api.put(`/Cargoes/${editingId}`, payload)
      : api.post('/Cargoes', payload)

    request
      .then(() => {
        setSuccessMsg(editingId ? 'Yük başarıyla güncellendi!' : 'Yük başarıyla eklendi!')
        resetForm()
        fetchCargoes()
      })
      .catch(error => setErrorMsg(getErrorMessage(error, 'Yük kaydedilirken bir hata oluştu.')))
  }

  const handleDelete = (cargo) => {
    if (!window.confirm('Bu yükü sistemden silmek istediğinize emin misiniz?')) return

    api.delete(`/Cargoes/${cargo.cargoId}`)
      .then(() => {
        setSuccessMsg('Yük başarıyla silindi!')
        if (editingId === cargo.cargoId) resetForm()
        fetchCargoes()
      })
      .catch(error => setErrorMsg(getErrorMessage(error, 'Silme işlemi başarısız oldu.')))
  }

  return (
    <Page icon="📦" title="Yük Yönetimi" description="Sistemdeki yükleri ekleyebilir, güncelleyebilir veya silebilirsiniz.">

      <FormCard editing={editingId} title={editingId ? '✏️ Yükü Güncelle' : '➕ Yeni Yük Ekle'}>
        <Alerts error={errorMsg} success={successMsg} />

        {ships.length === 0 ? (
          <p style={{ margin: 0, fontStyle: 'italic', color: '#6c757d' }}>
            Yük eklemek için önce Gemi Yönetimi sayfasından bir gemi ekleyin.
          </p>
        ) : (
          <form onSubmit={handleSubmit} style={{ display: 'flex', gap: '10px', flexWrap: 'wrap' }}>
            <input type="text" name="description" placeholder="Yük Açıklaması" value={formData.description} onChange={handleInputChange} style={inputStyle} />
            <input type="number" name="weightTon" step="0.01" placeholder="Ağırlık (Ton)" value={formData.weightTon} onChange={handleInputChange} style={inputStyle} />
            <input type="text" name="cargoType" placeholder="Tür (Konteyner/Dökme)" value={formData.cargoType} onChange={handleInputChange} style={inputStyle} />

            {/* Gemi ID'sini elle yazmak yerine listeden seçtiriyoruz */}
            <select name="shipId" value={formData.shipId} onChange={handleInputChange} style={{ ...inputStyle, minWidth: '180px' }}>
              <option value="">-- Gemi Seçin --</option>
              {ships.map(s => (
                <option key={s.shipId} value={s.shipId}>{s.name} ({s.imo})</option>
              ))}
            </select>

            <Button type="submit" variant={editingId ? 'update' : 'primary'}>
              {editingId ? 'Güncelle' : 'Kaydet'}
            </Button>

            {editingId && (
              <Button type="button" variant="neutral" onClick={resetForm}>İptal</Button>
            )}
          </form>
        )}
      </FormCard>

      {/* GEMİYE GÖRE FİLTRE */}
      <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginBottom: '15px', flexWrap: 'wrap' }}>
        <strong>Gemiye göre filtrele:</strong>
        <select value={filterShipId} onChange={handleFilterChange} style={{ padding: '8px', minWidth: '220px' }}>
          <option value="">Tüm gemiler</option>
          {ships.map(s => (
            <option key={s.shipId} value={s.shipId}>{s.name} ({s.imo})</option>
          ))}
        </select>
        <span style={{ color: '#6c757d' }}>{cargoes.length} kayıt</span>
      </div>

      {cargoes.length === 0 ? (
        <p>{filterShipId ? 'Bu gemiye ait yük bulunmuyor...' : 'Sistemde henüz yük bulunmuyor...'}</p>
      ) : (
        <Table headers={['ID', 'Açıklama', 'Ağırlık (Ton)', 'Tür', 'Gemi', 'İşlemler']}>
          {cargoes.map(cargo => (
            <tr key={cargo.cargoId}>
              <td>{cargo.cargoId}</td>
              <td>{cargo.description}</td>
              <td>{cargo.weightTon}</td>
              <td>{cargo.cargoType}</td>
              <td>{cargo.ship?.name ?? `#${cargo.shipId}`}</td>
              <RowActions onEdit={() => handleEditClick(cargo)} onDelete={() => handleDelete(cargo)} />
            </tr>
          ))}
        </Table>
      )}
    </Page>
  )
}
