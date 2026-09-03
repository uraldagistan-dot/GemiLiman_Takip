import { useState, useEffect } from 'react'
import api, { getErrorMessage } from '../api'
import { Page, Alerts, FormCard, Button, Table } from '../components/ui'
import { inputStyle } from '../styles'

const today = () => new Date().toISOString().slice(0, 10)

const emptyForm = { shipId: '', crewId: '', assignmentDate: today() }

export default function Assignments() {
  const [assignments, setAssignments] = useState([])
  const [ships, setShips] = useState([])
  const [crewList, setCrewList] = useState([])
  const [errorMsg, setErrorMsg] = useState('')
  const [successMsg, setSuccessMsg] = useState('')
  const [formData, setFormData] = useState(emptyForm)

  // Listeyi gemiye göre daraltmak için
  const [filterShipId, setFilterShipId] = useState('')

  const fetchAssignments = () => {
    api.get('/ShipCrewAssignments')
      .then(response => setAssignments(response.data))
      .catch(error => console.error('Atamalar çekilirken hata oluştu:', error))
  }

  useEffect(() => {
    fetchAssignments()
    api.get('/Ships').then(r => setShips(r.data)).catch(e => console.error(e))
    api.get('/CrewMembers').then(r => setCrewList(r.data)).catch(e => console.error(e))
  }, [])

  const handleInputChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value })
  }

  const handleSubmit = (e) => {
    e.preventDefault()
    setErrorMsg('')
    setSuccessMsg('')

    if (!formData.shipId || !formData.crewId || !formData.assignmentDate) {
      setErrorMsg('Lütfen gemi, personel ve atama tarihini seçin!')
      return
    }

    api.post('/ShipCrewAssignments', {
      ShipId: Number(formData.shipId),
      CrewId: Number(formData.crewId),
      AssignmentDate: formData.assignmentDate
    })
      .then(() => {
        setSuccessMsg('Mürettebat gemiye başarıyla atandı!')
        setFormData({ ...emptyForm, assignmentDate: formData.assignmentDate })
        fetchAssignments()
      })
      .catch(error => {
        // EKRAN GEREKSİNİMİ: Aynı mürettebat aynı gemide aynı tarihte birden fazla kez atanamaz.
        // Bu kuralın mesajı backend'den geliyor.
        setErrorMsg(getErrorMessage(error, 'Atama yapılırken bir hata oluştu.'))
      })
  }

  const handleDelete = (assignment) => {
    if (!window.confirm('Bu atamayı kaldırmak istediğinize emin misiniz?')) return

    api.delete(`/ShipCrewAssignments/${assignment.assignmentId}`)
      .then(() => {
        setSuccessMsg('Atama başarıyla kaldırıldı!')
        fetchAssignments()
      })
      .catch(error => setErrorMsg(getErrorMessage(error, 'Atama kaldırılamadı.')))
  }

  const eksikVeri = ships.length === 0 || crewList.length === 0

  const visibleAssignments = filterShipId
    ? assignments.filter(a => String(a.shipId) === filterShipId)
    : assignments

  return (
    <Page icon="🔗" title="Gemi-Mürettebat Atamaları" description="Personeli gemilere atayın. Aynı personel aynı gemiye aynı tarihte iki kez atanamaz.">

      <FormCard editing={false} title="Yeni Atama Ekle">
        <Alerts error={errorMsg} success={successMsg} />

        {eksikVeri ? (
          <p style={{ margin: 0, fontStyle: 'italic', color: '#6c757d' }}>
            Atama yapabilmek için sistemde en az bir gemi ve bir personel olmalı.
            {ships.length === 0 && ' Önce Gemi Yönetimi sayfasından gemi ekleyin.'}
            {crewList.length === 0 && ' Önce Mürettebat sayfasından personel ekleyin.'}
          </p>
        ) : (
          <form onSubmit={handleSubmit} style={{ display: 'flex', gap: '10px', flexWrap: 'wrap' }}>
            <select name="shipId" value={formData.shipId} onChange={handleInputChange} style={{ ...inputStyle, minWidth: '200px' }}>
              <option value="">-- Gemi Seçin --</option>
              {ships.map(s => (
                <option key={s.shipId} value={s.shipId}>{s.name} ({s.imo})</option>
              ))}
            </select>

            <select name="crewId" value={formData.crewId} onChange={handleInputChange} style={{ ...inputStyle, minWidth: '220px' }}>
              <option value="">-- Personel Seçin --</option>
              {crewList.map(c => (
                <option key={c.crewId} value={c.crewId}>{c.firstName} {c.lastName} ({c.role})</option>
              ))}
            </select>

            <label style={{ display: 'flex', flexDirection: 'column', fontSize: '12px', color: '#495057', flex: '1', minWidth: '160px' }}>
              Atama Tarihi
              <input type="date" name="assignmentDate" value={formData.assignmentDate} onChange={handleInputChange} style={{ padding: '8px' }} />
            </label>

            <Button type="submit" variant="info">Ata</Button>
          </form>
        )}
      </FormCard>

      {/* GEMİYE GÖRE FİLTRE */}
      <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginBottom: '15px', flexWrap: 'wrap' }}>
        <strong>Gemiye göre filtrele:</strong>
        <select value={filterShipId} onChange={(e) => setFilterShipId(e.target.value)} style={{ padding: '8px', minWidth: '220px' }}>
          <option value="">Tüm gemiler</option>
          {ships.map(s => (
            <option key={s.shipId} value={s.shipId}>{s.name} ({s.imo})</option>
          ))}
        </select>
        <span style={{ color: '#6c757d' }}>{visibleAssignments.length} atama</span>
      </div>

      {visibleAssignments.length === 0 ? (
        <p>{filterShipId ? 'Bu gemiye ait atama bulunmuyor...' : 'Sistemde henüz atama bulunmuyor...'}</p>
      ) : (
        <Table headers={['ID', 'Gemi', 'Personel', 'Görev', 'Atama Tarihi', 'İşlemler']}>
          {visibleAssignments.map(a => (
            <tr key={a.assignmentId}>
              <td>{a.assignmentId}</td>
              <td>{a.ship?.name ?? `#${a.shipId}`}</td>
              <td><strong>{a.crewMember?.firstName} {a.crewMember?.lastName}</strong></td>
              <td>{a.crewMember?.role}</td>
              <td>{new Date(a.assignmentDate).toLocaleDateString('tr-TR')}</td>
              <td>
                <Button variant="danger" onClick={() => handleDelete(a)} style={{ padding: '5px 10px' }}>Kaldır</Button>
              </td>
            </tr>
          ))}
        </Table>
      )}
    </Page>
  )
}
