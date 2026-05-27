/* ── HOME PAGE JS ── */

function initHome() {
  renderStats();
}

function renderStats() {
  const chuyenText = document.getElementById('stat-chuyen')?.textContent?.replace(/\D/g, '') || 128;
  animateNumber('stat-chuyen', parseInt(chuyenText));
  animateNumber('stat-tuyen',  47);
  animateNumber('stat-nhaxe',  12);
}

function animateNumber(id, target) {
  const el = document.getElementById(id);
  if (!el || isNaN(target) || target <= 0) return;
  let current = 0;
  const step = Math.max(1, Math.ceil(target / 40));
  const timer = setInterval(() => {
    current = Math.min(current + step, target);
    el.textContent = current.toLocaleString('vi-VN') + '+';
    if (current >= target) clearInterval(timer);
  }, 30);
}

document.addEventListener('DOMContentLoaded', initHome);
