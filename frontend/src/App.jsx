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
  padding: '6px 12px',
  borderRadius: '4px',
  transition: 'background-color 0.2s',
  backgroundColor: isActive ? 'rgba(255, 255, 255, 0.2)' : 'transparent'
})

function Home() {
  return (
    <div style={{ backgroundColor: '#fff', padding: '40px 20px', borderRadius: '8px', textAlign: 'center', boxShadow: '0 4px 12px rgba(0,0,0,0.05)' }}>
      <img src="/arkas-logo.png" alt="Arkas Logo" style={{ height: '80px', marginBottom: '20px' }} />
      <h2 style={{ color: '#0a1c3e', marginTop: 0, fontSize: '28px' }}>Liman Gemi Takip Sistemine Hoş Geldiniz</h2>
      <p style={{ color: '#5a6570', fontSize: '18px', maxWidth: '600px', margin: '0 auto 30px' }}>
        Arkas Holding bünyesindeki gemi, liman, yük ve mürettebat operasyonlarını kolayca yönetebilirsiniz.
      </p>
      
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))', gap: '20px', textAlign: 'left', marginTop: '30px' }}>
        <div style={{ padding: '20px', backgroundColor: '#f8f9fa', borderRadius: '8px', boxShadow: '0 2px 5px rgba(0,0,0,0.04)' }}>
          <h3 style={{ margin: '0 0 10px 0', color: '#0a1c3e' }}>Gemiler & Limanlar</h3>
          <p style={{ margin: 0, fontSize: '14px', color: '#6c757d' }}>Filo kayıtlarını ve liman bilgilerini sistemde yönetin.</p>
        </div>
        <div style={{ padding: '20px', backgroundColor: '#f8f9fa', borderRadius: '8px', boxShadow: '0 2px 5px rgba(0,0,0,0.04)' }}>
          <h3 style={{ margin: '0 0 10px 0', color: '#0a1c3e' }}>Ziyaret Kayıtları</h3>
          <p style={{ margin: 0, fontSize: '14px', color: '#6c757d' }}>Gemilerin limanlara varış ve ayrılış takvimlerini takip edin.</p>
        </div>
        <div style={{ padding: '20px', backgroundColor: '#f8f9fa', borderRadius: '8px', boxShadow: '0 2px 5px rgba(0,0,0,0.04)' }}>
          <h3 style={{ margin: '0 0 10px 0', color: '#0a1c3e' }}>Yük & Operasyon</h3>
          <p style={{ margin: 0, fontSize: '14px', color: '#6c757d' }}>Taşınan kargoları ve ağırlık detaylarını güncelleyin.</p>
        </div>
        <div style={{ padding: '20px', backgroundColor: '#f8f9fa', borderRadius: '8px', boxShadow: '0 2px 5px rgba(0,0,0,0.04)' }}>
          <h3 style={{ margin: '0 0 10px 0', color: '#0a1c3e' }}>Mürettebat</h3>
          <p style={{ margin: 0, fontSize: '14px', color: '#6c757d' }}>Personel listesini ve gemilere atanma durumlarını izleyin.</p>
        </div>
      </div>
    </div>
  )
}

function App() {
  return (
    <Router>
      <div style={{ fontFamily: 'sans-serif', maxWidth: '1100px', margin: '0 auto', padding: '10px' }}>

        {/* ÜST MENÜ (NAVBAR) */}
        <nav style={{ backgroundColor: '#0a1c3e', padding: '12px 20px', borderRadius: '8px', marginBottom: '25px', display: 'flex', gap: '15px', flexWrap: 'wrap', alignItems: 'center', boxShadow: '0 4px 10px rgba(10, 28, 62, 0.2)' }}>
          <img src="/arkas-logo.png" alt="Arkas Logo" style={{ height: '30px', marginRight: '10px', filter: 'brightness(0) invert(1)' }} />
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
