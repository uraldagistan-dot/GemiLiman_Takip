// Altı ekranın ortak kullandığı küçük sunum bileşenleri.
// Her sayfada aynı inline style'ları tekrar yazmamak için ayrıldı.

export function Page({ icon, title, description, children }) {
  return (
    <div style={{ backgroundColor: '#fff', padding: '20px', borderRadius: '8px' }}>
      {/* Renkler açıkça veriliyor: beyaz kartın üzerindeki okunabilirlik
          global stylesheet'e bağlı kalmasın. */}
      <h2 style={{ color: '#2c3e50', marginTop: 0 }}>{icon} {title}</h2>
      {description && <p style={{ color: '#5a6570', marginBottom: '20px' }}>{description}</p>}
      {children}
    </div>
  )
}

export function Alerts({ error, success }) {
  return (
    <>
      {error && <div style={{ color: 'red', marginBottom: '10px' }}><strong>Hata:</strong> {error}</div>}
      {success && <div style={{ color: 'green', marginBottom: '10px' }}><strong>Başarılı:</strong> {success}</div>}
    </>
  )
}

// Ekleme/güncelleme formlarının ortak kabı. Güncelleme modunda sarıya döner.
export function FormCard({ editing, title, children }) {
  return (
    <div style={{
      backgroundColor: editing ? '#fff3cd' : '#f0f4f8',
      padding: '20px',
      borderRadius: '8px',
      marginBottom: '30px',
      border: editing ? '2px solid #ffeeba' : 'none'
    }}>
      <h3 style={{ marginTop: 0, color: '#2c3e50' }}>{title}</h3>
      {children}
    </div>
  )
}

export function Button({ variant = 'primary', children, ...rest }) {
  const colors = {
    primary: { backgroundColor: '#28a745', color: 'white' },
    update: { backgroundColor: '#ffc107', color: 'black' },
    danger: { backgroundColor: '#dc3545', color: 'white' },
    neutral: { backgroundColor: '#6c757d', color: 'white' },
    info: { backgroundColor: '#17a2b8', color: 'white' }
  }
  return (
    <button
      {...rest}
      style={{ ...colors[variant], border: 'none', padding: '8px 20px', cursor: 'pointer', borderRadius: '4px', ...rest.style }}
    >
      {children}
    </button>
  )
}

export function Table({ headers, children }) {
  return (
    <table border="1" cellPadding="10" style={{ borderCollapse: 'collapse', width: '100%', textAlign: 'left' }}>
      <thead style={{ backgroundColor: '#2c3e50', color: 'white' }}>
        <tr>{headers.map(h => <th key={h}>{h}</th>)}</tr>
      </thead>
      <tbody>{children}</tbody>
    </table>
  )
}

export function RowActions({ onEdit, onDelete }) {
  return (
    <td style={{ display: 'flex', gap: '5px' }}>
      <Button variant="update" onClick={onEdit} style={{ padding: '5px 10px', fontWeight: 'bold' }}>Düzenle</Button>
      <Button variant="danger" onClick={onDelete} style={{ padding: '5px 10px' }}>Sil</Button>
    </td>
  )
}
