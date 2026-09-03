import { BrowserRouter as Router, Routes, Route, NavLink } from 'react-router-dom'
import Ships from './pages/Ships'
import Ports from './pages/Ports'
import Visits from './pages/Visits'
import Cargoes from './pages/Cargoes'
import CrewMembers from './pages/CrewMembers'
import Assignments from './pages/Assignments'

// Dokümandaki 6 ekran. Menü ve rotalar tek yerden üretiliyor ki
// yeni ekran eklerken iki yeri birden güncellemek gerekmesin.
const screens = [
  { path: '/ships', label: ' Gemiler', element: <Ships /> },
  { path: '/ports', label: ' Limanlar', element: <Ports /> },
  { path: '/visits', label: ' Ziyaretler', element: <Visits /> },
  { path: '/cargoes', label: ' Yükler', element: <Cargoes /> },
  { path: '/crew', label: ' Mürettebat', element: <CrewMembers /> },
  { path: '/assignments', label: ' Atamalar', element: <Assignments /> }
]

const linkStyle = ({ isActive }) => ({
  color: 'white',
  textDecoration: 'none',
  fontWeight: 'bold',
  padding: '6px 10px',
  borderRadius: '4px',
  backgroundColor: isActive ? '#1a252f' : 'transparent'
})

function Home() {
  return (
    <div style={{ backgroundColor: '#fff', padding: '20px', borderRadius: '8px' }}>
      <h2 style={{ color: '#2c3e50', marginTop: 0 }}>⚓ Liman Gemi Takip Sistemine Hoş Geldiniz</h2>
      <p>Yukarıdaki menüden ilgili ekrana geçebilirsiniz.</p>
      <ul style={{ lineHeight: '1.8' }}>
        <li><strong>Gemiler</strong> — Gemi listeleme, ekleme, düzenleme, silme (IMO benzersiz)</li>
        <li><strong>Limanlar</strong> — Liman CRUD işlemleri</li>
        <li><strong>Ziyaretler</strong> — Gemi ve liman seçimiyle ziyaret kaydı (geliş &lt; ayrılış)</li>
        <li><strong>Yükler</strong> — Gemiye ait yük listesi (ağırlık &gt; 0)</li>
        <li><strong>Mürettebat</strong> — Personel yönetimi (e-posta ve telefon validasyonu)</li>
        <li><strong>Atamalar</strong> — Gemi-mürettebat atamaları (aynı gemi/personel/tarih tekrar edemez)</li>
      </ul>
    </div>
  )
}

function App() {
  return (
    <Router>
      <div style={{ fontFamily: 'sans-serif', maxWidth: '1100px', margin: '0 auto', padding: '10px' }}>

        {/* ÜST MENÜ (NAVBAR) */}
        <nav style={{ backgroundColor: '#2c3e50', padding: '12px 15px', borderRadius: '8px', marginBottom: '20px', display: 'flex', gap: '8px', flexWrap: 'wrap', alignItems: 'center' }}>
          <NavLink to="/" style={linkStyle} end>Ana Sayfa</NavLink>
          {screens.map(s => (
            <NavLink key={s.path} to={s.path} style={linkStyle}>{s.label}</NavLink>
          ))}
        </nav>

        {/* SAYFALARIN GÖSTERİLECEĞİ ALAN */}
        <Routes>
          <Route path="/" element={<Home />} />
          {screens.map(s => (
            <Route key={s.path} path={s.path} element={s.element} />
          ))}
          <Route path="*" element={<p>Sayfa bulunamadı.</p>} />
        </Routes>

      </div>
    </Router>
  )
}

export default App
