/* ── SEARCH PAGE JS ── */

let rawTrips    = [];
let filteredTrips = [];
let selectedTime  = 'all';
let selectedCarriers = new Set();

function initSearch() {
  if (window.TRIPS_DATA) {
    rawTrips = window.TRIPS_DATA;
    buildCarrierFilters();
    buildDateNav();
    applyFilters();
  }
}

function getLocalDateString(date) {
  const yyyy = date.getFullYear();
  const mm = String(date.getMonth() + 1).padStart(2, '0');
  const dd = String(date.getDate()).padStart(2, '0');
  return `${yyyy}-${mm}-${dd}`;
}

function buildDateNav() {
  const nav = document.createElement('div');
  nav.className = 'date-nav';
  
  // Try to use the searched date if available, otherwise today
  const searchParams = new URLSearchParams(window.location.search);
  let baseDateStr = searchParams.get('ngayDi');
  let baseDate = new Date();
  if (baseDateStr) {
    const parts = baseDateStr.split('-');
    baseDate = new Date(parseInt(parts[0]), parseInt(parts[1]) - 1, parseInt(parts[2]));
  }
  baseDate.setHours(0,0,0,0);

  const today = new Date();
  today.setHours(0,0,0,0);

  // Calculate start date (center baseDate if possible, but do not go into the past)
  let start = new Date(baseDateStr ? baseDate : today);
  if (baseDateStr) {
    start.setDate(baseDate.getDate() - 3);
  }
  start.setHours(0,0,0,0);
  
  if (start < today) {
    start = new Date(today);
  }
  
  for (let i = 0; i < 7; i++) {
    const d = new Date(start);
    d.setDate(start.getDate() + i);
    d.setHours(0,0,0,0);

    const iso = getLocalDateString(d);
    const isToday = (d.getTime() === today.getTime());
    
    let label = isToday ? 'Hôm nay' : d.toLocaleDateString('vi-VN',{weekday:'short',day:'2-digit',month:'2-digit'});
    
    const btn = document.createElement('div');
    btn.className = 'date-btn' + (iso === baseDateStr ? ' active' : (isToday && !baseDateStr ? ' active' : ''));
    btn.textContent = label;
    btn.dataset.date = iso;
    btn.onclick = () => {
      document.getElementById('sbc-ngay-di').value = iso;
      // Just submit the form
      document.querySelector('.compact-form')?.submit();
    };
    nav.appendChild(btn);
  }
  const results = document.querySelector('.search-results');
  results?.insertBefore(nav, results.firstChild);
}

function buildCarrierFilters() {
  const carriers = [...new Set(rawTrips.map(t => t.tenNhaXe))];
  const div = document.getElementById('carrier-filters');
  if (!div) return;
  div.innerHTML = carriers.map(c => `
    <div class="carrier-check">
      <input type="checkbox" id="c-${c}" value="${c}" checked onchange="toggleCarrier('${c}',this.checked)"/>
      <label for="c-${c}">${c}</label>
    </div>
  `).join('');
  selectedCarriers = new Set(carriers);
}

function toggleCarrier(name, checked) {
  checked ? selectedCarriers.add(name) : selectedCarriers.delete(name);
  applyFilters();
}

function selectTime(el, time) {
  document.querySelectorAll('.chip').forEach(c=>c.classList.remove('active'));
  el.classList.add('active');
  selectedTime = time;
  applyFilters();
}

function applyFilters() {
  const sort    = document.getElementById('sort-select')?.value || 'gio';
  const pMin    = parseFloat(document.getElementById('price-min')?.value)||0;
  const pMax    = parseFloat(document.getElementById('price-max')?.value)||Infinity;

  filteredTrips = rawTrips.filter(t => {
    if (!selectedCarriers.has(t.tenNhaXe)) return false;
    if (t.giaVe < pMin || t.giaVe > pMax) return false;
    if (selectedTime !== 'all') {
      const h = parseInt(t.gioDi);
      if (selectedTime === 'early'     && (h < 0  || h >= 6))  return false;
      if (selectedTime === 'morning'   && (h < 6  || h >= 12)) return false;
      if (selectedTime === 'afternoon' && (h < 12 || h >= 18)) return false;
      if (selectedTime === 'evening'   && (h < 18 || h >= 24)) return false;
    }
    return true;
  });

  filteredTrips.sort((a,b) => {
    if (sort === 'gio')      return a.gioDi.localeCompare(b.gioDi);
    if (sort === 'gia_asc')  return a.giaVe - b.giaVe;
    if (sort === 'gia_desc') return b.giaVe - a.giaVe;
    if (sort === 'ghe')      return b.soGheTrong - a.soGheTrong;
    return 0;
  });

  renderResults();
}

function renderResults() {
  const list  = document.getElementById('trip-list');
  const empty = document.getElementById('empty-state');
  const summ  = document.getElementById('results-summary');

  if (summ) {
    summ.innerHTML = filteredTrips.length > 0
      ? `Tìm thấy <strong>${filteredTrips.length}</strong> chuyến xe`
      : 'Không tìm thấy chuyến xe nào phù hợp với bộ lọc';
  }

  if (!filteredTrips.length) {
    list.innerHTML = '';
    empty?.classList.remove('hidden');
    return;
  }
  empty?.classList.add('hidden');

  list.innerHTML = filteredTrips.map(t => {
    const seats = t.soGheTrong;
    const seatsColor = seats === 0 ? 'var(--danger)' : seats <= 5 ? 'var(--warning)' : 'var(--success)';
    const seatsLabel = seats === 0 ? 'Hết ghế' : `${seats} ghế trống`;
    const canBook = seats > 0;
    
    // Status mock (since DB might not expose trangThaiChuyen in V_CHUYENXE)
    const statusLabel = canBook ? 'Sắp đi' : 'Hết chỗ';
    const statusBadge = canBook ? 'badge-primary' : 'badge-gray';
    
    return `
    <div class="trip-result-card">
      <div class="trc-img">
        <img src="${window.CTX}/assets/img/${t.imgXe || 'bus-default.jpg'}" alt="${t.loaiXe}"
             onerror="this.onerror=null;this.src='${window.CTX}/assets/img/bus-default.jpg'"/>
        <div class="trc-img-badge">${t.loaiXe || 'Xe khách'}</div>
      </div>
      <div class="trc-main">
        <div class="trc-info">
          <div class="trc-time-row">
            <span class="trc-time">${t.gioDiShort}</span>
            <span class="trc-arrow">→</span>
            <span class="trc-time trc-time-dest">${t.gioDenShort}</span>
            <span class="trc-duration">${formatDuration(t.thoiGian)}</span>
          </div>
          <div class="trc-station-row">
            <span class="trc-station" title="${t.benDi}">${t.benDi}</span>
            <span class="trc-station-sep">→</span>
            <span class="trc-station" title="${t.benDen}">${t.benDen}</span>
          </div>
          <div class="trc-carrier-row">
            <span class="trc-carrier-name" title="${t.tenNhaXe}">🏢 ${t.tenNhaXe}</span>
            <span class="trc-dot">●</span>
            <span class="trc-date-tag">📅 ${formatDate(t.ngayDi)}</span>
          </div>
        </div>

        <div class="trc-buy">
          <div class="trc-price">${formatCurrency(t.giaVe)}</div>
          <div class="trc-seats" style="color:${seatsColor}">${seatsLabel}</div>
          <button class="btn btn-primary trc-btn"
            onclick="location.href='${t.bookUrl}'"
            ${canBook ? '' : 'disabled'}>
            ${canBook ? 'Chọn ghế' : 'Hết vé'}
          </button>
        </div>
      </div>
    </div>`;
  }).join('');

}

function calcArrival(gioDi, phut) {
  const [h, m] = gioDi.split(':').map(Number);
  const totalMin = h * 60 + m + (phut || 0);
  const ah = Math.floor(totalMin/60) % 24;
  const am = totalMin % 60;
  return `${String(ah).padStart(2,'0')}:${String(am).padStart(2,'0')}`;
}

function resetFilters() {
  selectedTime = 'all';
  document.querySelectorAll('.chip').forEach((c,i)=>c.classList.toggle('active',i===0));
  document.getElementById('price-min').value = '';
  document.getElementById('price-max').value = '';
  document.querySelectorAll('#carrier-filters input').forEach(cb=>{
    cb.checked = true; selectedCarriers.add(cb.value);
  });
  applyFilters();
}

document.addEventListener('DOMContentLoaded', initSearch);
