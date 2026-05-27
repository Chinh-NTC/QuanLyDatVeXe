/* =====================================================
   BUSGO — Shared JavaScript Utilities
===================================================== */

// ── Toast Notifications ──
function showToast(message, type = 'info', duration = 3500) {
  let container = document.getElementById('toast-container');
  if (!container) {
    container = document.createElement('div');
    container.id = 'toast-container';
    document.body.appendChild(container);
  }
  const icons = { success: '✅', error: '❌', warning: '⚠️', info: 'ℹ️' };
  const toast = document.createElement('div');
  toast.className = `toast ${type}`;
  toast.innerHTML = `<span style="font-size:1.1rem">${icons[type]||'ℹ️'}</span><span>${message}</span>`;
  container.appendChild(toast);
  setTimeout(() => { toast.style.opacity='0'; toast.style.transform='translateX(100%)'; toast.style.transition='all .3s'; setTimeout(()=>toast.remove(), 300); }, duration);
}

// ── Modal helpers ──
function openModal(id) { document.getElementById(id)?.classList.add('active'); }
function closeModal(id) { document.getElementById(id)?.classList.remove('active'); }
document.addEventListener('click', e => {
  if (e.target.classList.contains('modal-overlay')) e.target.classList.remove('active');
});

// ── Format currency ──
function formatCurrency(amount) {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
}

// ── Format date ──
function formatDate(dateStr) {
  if (!dateStr) return '';
  const d = new Date(dateStr);
  return d.toLocaleDateString('vi-VN', { day:'2-digit', month:'2-digit', year:'numeric' });
}

// ── Format datetime ──
function formatDateTime(dateStr) {
  if (!dateStr) return '';
  const d = new Date(dateStr);
  return d.toLocaleDateString('vi-VN',{day:'2-digit',month:'2-digit',year:'numeric',hour:'2-digit',minute:'2-digit'});
}

// ── Hamburger menu ──
function initHamburger() {
  const btn = document.querySelector('.hamburger');
  const menu = document.querySelector('.navbar-menu');
  if (!btn || !menu) return;
  btn.addEventListener('click', () => menu.classList.toggle('open'));
}

// ── Set active nav link ──
function setActiveNav() {
  const page = location.pathname.split('/').pop() || 'index.html';
  document.querySelectorAll('.navbar-menu a').forEach(a => {
    if (a.getAttribute('href') === page) a.classList.add('active');
  });
}

// ── Trip status labels ──
const TRIP_STATUS = {
  SAPDI: { label: 'Sắp đi', badge: 'badge-primary' },
  DANGTHANH: { label: 'Đang chạy', badge: 'badge-warning' },
  HOANTHANH: { label: 'Hoàn thành', badge: 'badge-success' },
  HOAN: { label: 'Bị hoãn', badge: 'badge-warning' },
  HUY: { label: 'Đã hủy', badge: 'badge-danger' }
};

const ORDER_STATUS = {
  CHOXULY:   { label: 'Chờ xử lý',  badge: 'badge-warning' },
  DAXACNHAN: { label: 'Đã xác nhận',badge: 'badge-success' },
  DAHUY:     { label: 'Đã hủy',     badge: 'badge-danger' },
  HOANTHANH: { label: 'Hoàn thành', badge: 'badge-gray' }
};

const TICKET_STATUS = {
  DADAT:    { label: 'Đã đặt',   badge: 'badge-primary' },
  DAHUY:    { label: 'Đã hủy',   badge: 'badge-danger' },
  DASUDUNG: { label: 'Đã dùng',  badge: 'badge-success' }
};

// ── Seat status helpers ──
function isSeatBookable(seat, bookedMaGheSet) {
  return seat.trangThai === 'TRONG' && !bookedMaGheSet.has(seat.maGhe);
}

// ── Format duration (phút → "Xh Ym") ──
function formatDuration(minutes) {
  const h = Math.floor(minutes / 60);
  const m = minutes % 60;
  return h > 0 ? `${h}h ${m > 0 ? m+'m' : ''}`.trim() : `${m}m`;
}

// ── Number utils ──
function parseVND(str) { return parseInt((str||'0').toString().replace(/\D/g,''))||0; }

// ── Search form validation ──
function validateSearchForm(form) {
  const tinhDi = form.elements['maTinhDi']?.value;
  const tinhDen = form.elements['maTinhDen']?.value;
  
  if (tinhDi && tinhDen && tinhDi === tinhDen) {
    showToast('Điểm đi và điểm đến không được trùng nhau', 'warning');
    return false;
  }
  return true;
}

// ── Search params ──
function getSearchParams() {
  const p = new URLSearchParams(location.search);
  return {
    maBenDi:  p.get('maBenDi')  || '',
    maBenDen: p.get('maBenDen') || '',
    ngayDi:   p.get('ngayDi')   || '',
    maChuyen: p.get('maChuyen') || ''
  };
}

// ── DOM ready ──
document.addEventListener('DOMContentLoaded', () => {
  initHamburger();
  setActiveNav();
});
