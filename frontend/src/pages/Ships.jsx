import { useState, useEffect } from 'react'
import api, { getErrorMessage } from '../api'
import { Page, Alerts, FormCard, Button } from '../components/ui'
import { inputStyle } from '../styles'

const emptyForm = { name: '', imo: '', yearBuilt: '', flag: '', type: '' }

export default function Ships() {
  const [ships, setShips] = useState([])
  const [errorMsg, setErrorMsg] = useState('')
  const [successMsg, setSuccessMsg] = useState('')

  // Hangi geminin detaylarının açık olduğunu tutar (Sadece 1 gemi açık kalır)
  const [expandedShipId, setExpandedShipId] = useState(null)

  // Düzenleme modunda hangi geminin güncellendiğini tutar
  const [editingId, setEditingId] = useState(null)

  const [formData, setFormData] = useState(emptyForm)

  // MÜRETTEBAT ATAMA STATE'LERİ
  const [crewList, setCrewList] = useState([])
  const [selectedCrewId, setSelectedCrewId] = useState('')
  const [assignDate, setAssignDate] = useState(new Date().toISOString().slice(0, 10))
  const [assignError, setAssignError] = useState('')
  const [assignSuccess, setAssignSuccess] = useState('')

  const fetchShips = () => {
    api.get('/Ships/details')
      .then(response => setShips(response.data))
      .catch(error => console.error('Gemiler çekilirken hata oluştu:', error))
  }

  const fetchCrew = () => {
    api.get('/CrewMembers')
      .then(response => setCrewList(response.data))
      .catch(error => console.error('Mürettebat çekilirken hata oluştu:', error))
  }

  useEffect(() => {
    fetchShips()
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

  const handleEditClick = (ship) => {
    setEditingId(ship.shipId)
    setFormData({
      name: ship.name,
      imo: ship.imo,
      yearBuilt: ship.yearBuilt,
      flag: ship.flag,
      type: ship.type
    })
    setErrorMsg('')
    setSuccessMsg('')
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  const handleSubmit = (e) => {
    e.preventDefault()
    setErrorMsg('')
    setSuccessMsg('')

    if (!formData.name || !formData.imo || !formData.yearBuilt || !formData.flag || !formData.type) {
      setErrorMsg('Lütfen tüm zorunlu alanları doldurun!')
      return
    }

    const currentYear = new Date().getFullYear()
    const builtYear = Number(formData.yearBuilt)

    if (builtYear < 1900 || builtYear > currentYear) {
      setErrorMsg(`Yapım yılı 1900 ile ${currentYear} arasında olmalıdır!`)
      return
    }

    const payload = {
      Name: formData.name,
      IMO: formData.imo,
      YearBuilt: builtYear,
      Flag: formData.flag,
      Type: formData.type
    }

    const request = editingId
      ? api.put(`/Ships/${editingId}`, payload)
      : api.post('/Ships', payload)

    request
      .then(() => {
        setSuccessMsg(editingId ? 'Gemi başarıyla güncellendi!' : 'Gemi başarıyla eklendi!')
        resetForm()
        fetchShips()
      })
      .catch(error => {
        // IMO benzersizlik hatası gibi iş kuralı mesajlarını backend'den olduğu gibi gösteriyoruz
        setErrorMsg(getErrorMessage(error, 'Gemi kaydedilirken bir hata oluştu.'))
      })
  }

  const handleDelete = (ship) => {
    const uyari = `"${ship.name}" gemisini silmek istediğinize emin misiniz?\n\nGeminin yükleri, ziyaretleri ve mürettebat atamaları da silinecek.`
    if (!window.confirm(uyari)) return

    api.delete(`/Ships/${ship.shipId}`)
      .then(() => {
        setSuccessMsg('Gemi başarıyla silindi!')
        if (editingId === ship.shipId) resetForm()
        if (expandedShipId === ship.shipId) setExpandedShipId(null)
        fetchShips()
      })
      .catch(error => setErrorMsg(getErrorMessage(error, 'Silme işlemi başarısız oldu.')))
  }

  const toggleDetails = (shipId) => {
    // Gemi değiştiğinde önceki geminin atama formu/mesajları temizlenir
    setSelectedCrewId('')
    setAssignError('')
    setAssignSuccess('')

    setExpandedShipId(expandedShipId === shipId ? null : shipId)
  }

  // Seçilen personeli açık olan gemiye atar
  const handleAssignCrew = (shipId) => {
    setAssignError('')
    setAssignSuccess('')

    if (!selectedCrewId) {
      setAssignError('Lütfen atanacak personeli seçin!')
      return
    }
    if (!assignDate) {
      setAssignError('Lütfen atama tarihini seçin!')
      return
    }

    api.post('/ShipCrewAssignments', {
      ShipId: shipId,
      CrewId: Number(selectedCrewId),
      AssignmentDate: assignDate
    })
      .then(() => {
        setAssignSuccess('Mürettebat gemiye başarıyla atandı!')
        setSelectedCrewId('')
        fetchShips()
      })
      .catch(error => {
        // Aynı tarihte mükerrer atama gibi iş kuralı mesajları buradan gelir
        setAssignError(getErrorMessage(error, 'Mürettebat atanırken bir hata oluştu.'))
      })
  }

  const handleRemoveAssignment = (assignmentId) => {
    if (!window.confirm('Bu atamayı kaldırmak istediğinize emin misiniz?')) return

    api.delete(`/ShipCrewAssignments/${assignmentId}`)
      .then(() => {
        setAssignSuccess('Atama kaldırıldı!')
        fetchShips()
      })
      .catch(error => setAssignError(getErrorMessage(error, 'Atama kaldırılamadı.')))
  }

  return (
    <Page icon="🚢" title="Gemi Yönetimi" description="Gemi ekleyebilir, düzenleyebilir, silebilir ve detaylarını görmek için üzerlerine tıklayabilirsiniz.">

      {/* GEMİ EKLEME / GÜNCELLEME FORMU */}
      <FormCard editing={editingId} title={editingId ? 'Gemiyi Güncelle' : 'Yeni Gemi Ekle'}>
        <Alerts error={errorMsg} success={successMsg} />

        <form onSubmit={handleSubmit} style={{ display: 'flex', gap: '10px', flexWrap: 'wrap' }}>
          <input type="text" name="name" placeholder="Gemi Adı" value={formData.name} onChange={handleInputChange} style={inputStyle} />
          <input type="text" name="imo" placeholder="IMO Numarası" value={formData.imo} onChange={handleInputChange} style={inputStyle} />
          <input type="number" name="yearBuilt" placeholder="Yapım Yılı" value={formData.yearBuilt} onChange={handleInputChange} style={{ ...inputStyle, minWidth: '100px' }} />
          <input type="text" name="flag" placeholder="Bayrak (Örn: TR)" value={formData.flag} onChange={handleInputChange} style={{ ...inputStyle, minWidth: '100px' }} />
          <input type="text" name="type" placeholder="Gemi Tipi (Örn: Kargo)" value={formData.type} onChange={handleInputChange} style={inputStyle} />

          <Button type="submit" variant={editingId ? 'update' : 'primary'}>
            {editingId ? 'Güncelle' : 'Gemiyi Kaydet'}
          </Button>

          {editingId && (
            <Button type="button" variant="neutral" onClick={resetForm}>İptal</Button>
          )}
        </form>
      </FormCard>

      {/* DİNAMİK GEMİLER LİSTESİ (AKORDEON YAPI) */}
      {ships.length === 0 ? (
        <p>Sistemde henüz gemi bulunmuyor...</p>
      ) : (
        ships.map(ship => (
          <div key={ship.shipId} style={{ border: '2px solid #2c3e50', margin: '15px 0', borderRadius: '8px', overflow: 'hidden', backgroundColor: '#fdfdfd' }}>

            {/* TIKLANABİLİR ÖZET ALANI */}
            <div
              onClick={() => toggleDetails(ship.shipId)}
              style={{ padding: '15px', backgroundColor: expandedShipId === ship.shipId ? '#eaf2f8' : '#fff', cursor: 'pointer', display: 'flex', justifyContent: 'space-between', alignItems: 'center', gap: '10px' }}
            >
              <div>
                <h3 style={{ color: '#2c3e50', margin: '0 0 5px 0' }}>{ship.name}</h3>
                <span style={{ fontSize: '14px', color: '#555' }}>
                  IMO: {ship.imo} | Bayrak: {ship.flag} | Tip: {ship.type} | Yıl: {ship.yearBuilt}
                </span>
              </div>

              <div style={{ display: 'flex', gap: '5px', alignItems: 'center' }}>
                {/* Butonlar akordeonu açıp kapatmasın diye tıklama yayılımını durduruyoruz */}
                <Button variant="update" onClick={(e) => { e.stopPropagation(); handleEditClick(ship) }} style={{ padding: '5px 10px', fontWeight: 'bold' }}>Düzenle</Button>
                <Button variant="danger" onClick={(e) => { e.stopPropagation(); handleDelete(ship) }} style={{ padding: '5px 10px' }}>Sil</Button>
                <span style={{ fontSize: '18px', color: '#007bff', fontWeight: 'bold' }}>
                  {expandedShipId === ship.shipId ? '▲' : '▼'}
                </span>
              </div>
            </div>

            {/* SADECE TIKLANDIĞINDA AÇILAN DETAY ALANI */}
            {expandedShipId === ship.shipId && (
              <div style={{ padding: '15px', borderTop: '2px solid #e9ecef', backgroundColor: '#f8f9fa' }}>

                {/* YÜKLER BÖLÜMÜ */}
                <h4 style={{ color: '#007bff', marginTop: 0 }}>Gemideki Yükler</h4>
                {ship.cargoes && ship.cargoes.length > 0 ? (
                  <ul style={{ lineHeight: '1.6', marginBottom: '20px' }}>
                    {ship.cargoes.map(cargo => (
                      <li key={cargo.cargoId}>
                        <strong>{cargo.description}</strong> - {cargo.weightTon} Ton <em>({cargo.cargoType})</em>
                      </li>
                    ))}
                  </ul>
                ) : (
                  <p style={{ color: '#e74c3c', fontStyle: 'italic', marginBottom: '20px' }}>Bu gemiye henüz bir yük atanmamış.</p>
                )}

                {/* MÜRETTEBAT BÖLÜMÜ */}
                <h4 style={{ color: '#17a2b8', marginTop: 0 }}>Mürettebat Bilgisi</h4>

                {ship.crewAssignments && ship.crewAssignments.length > 0 ? (
                  <ul style={{ lineHeight: '1.8', marginBottom: '20px' }}>
                    {ship.crewAssignments.map(assignment => (
                      <li key={assignment.assignmentId}>
                        <strong>{assignment.crewMember?.firstName} {assignment.crewMember?.lastName}</strong>
                        <span style={{ color: '#6c757d', marginLeft: '5px' }}>
                          - {assignment.crewMember?.role}
                        </span>
                        {assignment.assignmentDate && (
                          <span style={{ color: '#95a5a6', marginLeft: '8px', fontSize: '13px' }}>
                            (Atama: {new Date(assignment.assignmentDate).toLocaleDateString('tr-TR')})
                          </span>
                        )}
                        <Button variant="danger" onClick={() => handleRemoveAssignment(assignment.assignmentId)} style={{ padding: '2px 8px', fontSize: '12px', marginLeft: '10px' }}>Kaldır</Button>
                      </li>
                    ))}
                  </ul>
                ) : (
                  <p style={{ color: '#e74c3c', fontStyle: 'italic', marginBottom: '20px' }}>Bu gemiye henüz mürettebat atanmamış.</p>
                )}

                {/* MÜRETTEBAT ATAMA FORMU */}
                <div style={{ backgroundColor: '#e8f6f8', padding: '15px', borderRadius: '6px', border: '1px solid #17a2b8' }}>
                  <strong style={{ display: 'block', marginBottom: '10px', color: '#0f6674' }}>Bu Gemiye Mürettebat Ata</strong>

                  <Alerts error={assignError} success={assignSuccess} />

                  {crewList.length === 0 ? (
                    <p style={{ margin: 0, fontStyle: 'italic', color: '#6c757d' }}>
                      Sistemde kayıtlı personel yok. Önce Mürettebat sayfasından personel ekleyin.
                    </p>
                  ) : (
                    <div style={{ display: 'flex', gap: '10px', flexWrap: 'wrap' }}>
                      <select value={selectedCrewId} onChange={(e) => setSelectedCrewId(e.target.value)} style={{ ...inputStyle, minWidth: '220px' }}>
                        <option value="">-- Personel Seçin --</option>
                        {crewList.map(c => (
                          <option key={c.crewId} value={c.crewId}>
                            {c.firstName} {c.lastName} ({c.role})
                          </option>
                        ))}
                      </select>
                      <input type="date" value={assignDate} onChange={(e) => setAssignDate(e.target.value)} style={{ ...inputStyle, minWidth: '150px' }} />
                      <Button type="button" variant="info" onClick={() => handleAssignCrew(ship.shipId)}>Gemiye Ata</Button>
                    </div>
                  )}
                </div>
              </div>
            )}
          </div>
        ))
      )}
    </Page>
  )
}
