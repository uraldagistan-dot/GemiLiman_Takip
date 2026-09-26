// Altı ekranın ortak kullandığı küçük sunum bileşenleri.
// Her sayfada aynı inline style'ları tekrar yazmamak için ayrıldı.

export function Page({ title, description, children }) {
  return (
    <div style={{ backgroundColor: '#fff', padding: '30px', borderRadius: '12px', boxShadow: '0 4px 15px rgba(0,0,0,0.05)' }}>
      <h2 style={{ color: '#0a1c3e', marginTop: 0, fontSize: '24px', borderBottom: '2px solid #f0f4f8', paddingBottom: '15px' }}>{title}</h2>
      {description && <p style={{ color: '#5a6570', marginBottom: '25px', fontSize: '15px' }}>{description}</p>}
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
      backgroundColor: editing ? '#fffdf5' : '#e4ebf3',
      padding: '25px',
      borderRadius: '8px',
      marginBottom: '30px',
      border: '1px solid',
      borderColor: editing ? '#ffeeba' : '#d5dfe8',
      boxShadow: '0 2px 8px rgba(0,0,0,0.02)'
    }}>
      <h3 style={{ marginTop: 0, color: '#0a1c3e' }}>{title}</h3>
      {children}
    </div>
  )
}

export function Button({ variant = 'primary', children, ...rest }) {
  const colors = {
    primary: { backgroundColor: '#0a1c3e', color: 'white' }, // Lacivert
    update: { backgroundColor: '#ffc107', color: 'black' },
    danger: { backgroundColor: '#dc3545', color: 'white' },
    neutral: { backgroundColor: '#6c757d', color: 'white' },
    info: { backgroundColor: '#17a2b8', color: 'white' }
  }
  return (
    <button
      {...rest}
      style={{ 
        ...colors[variant], 
        border: 'none', 
        padding: '10px 22px', 
        cursor: 'pointer', 
        borderRadius: '6px', 
        fontWeight: '500',
        boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
        ...rest.style 
      }}
    >
      {children}
    </button>
  )
}

export function Table({ headers, children }) {
  return (
    <table border="1" cellPadding="10" style={{ borderCollapse: 'collapse', width: '100%', textAlign: 'left' }}>
      <thead style={{ backgroundColor: '#0a1c3e', color: 'white' }}>
        <tr>{headers.map(h => <th key={h} style={{ padding: '12px' }}>{h}</th>)}</tr>
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
