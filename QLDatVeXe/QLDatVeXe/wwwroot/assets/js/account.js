/* ── ACCOUNT PAGE JS ── */

let currentReviewChuyen = null;
let currentStarRating   = 0;

function initAccount() {
  const hash = location.hash.replace('#','') || window.ACC_DATA?.tab || 'orders';
  const tabEl = document.querySelector(`[data-tab="${hash}"]`);
  if (tabEl) switchTab(hash, tabEl);
}

function openReview(maDon, label) {
  currentReviewChuyen = maDon;
  const el = document.getElementById('review-trip-label');
  if (el) el.textContent = label;
  currentStarRating = 0;
  document.querySelectorAll('.star-btn').forEach(s=>s.classList.remove('active'));
  document.getElementById('review-comment').value = '';
  openModal('review-modal');
}

function setStar(v) {
  currentStarRating = v;
  document.querySelectorAll('.star-btn').forEach(s => {
    s.classList.toggle('active', parseInt(s.dataset.v) <= v);
  });
}

function submitReview() {
  if (!currentStarRating) { showToast('Vui lòng chọn số sao', 'warning'); return; }
  const comment = document.getElementById('review-comment')?.value?.trim();
  
  const form = document.createElement('form');
  form.method = 'POST';
  form.action = (window.CTX || '') + '/account';
  
  const addInput = (name, value) => {
    const inp = document.createElement('input');
    inp.type = 'hidden';
    inp.name = name;
    inp.value = value;
    form.appendChild(inp);
  };
  
  addInput('action', 'review');
  addInput('maDon', currentReviewChuyen);
  addInput('diemDanhGia', currentStarRating);
  addInput('binhLuan', comment || '');
  
  document.body.appendChild(form);
  form.submit();
}

function filterOrders() {
  const filter = document.getElementById('order-filter')?.value || '';
  const cards = document.querySelectorAll('.order-card');
  cards.forEach(card => {
    if (!filter || card.dataset.status === filter) {
      card.style.display = 'block';
    } else {
      card.style.display = 'none';
    }
  });
}

function switchTab(tabName, el) {
  document.querySelectorAll('.tab-panel').forEach(p=>p.classList.remove('active'));
  document.querySelectorAll('.acc-nav-item').forEach(a=>a.classList.remove('active'));
  const panel = document.getElementById(`tab-${tabName}`);
  if (panel) panel.classList.add('active');
  if (el) el.classList.add('active');
  history.pushState(null,'', '#'+tabName);
}

document.addEventListener('DOMContentLoaded', initAccount);
