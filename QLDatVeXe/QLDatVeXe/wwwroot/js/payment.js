/* ── PAYMENT PAGE JS ──
   Tables used:
   - DONDATVE      → tạo đơn đặt vé (maKH, maNV=null, tongTien, tienCoc, trangThai, ghiChu)
   - CHITIETDATVE  → tạo chi tiết (maDon, maChuyen, maGhe, giaVeLucDat)
   - THANHTOAN     → tạo thanh toán (maDon, soTien, phuongThuc)
   - DONDATVE_KHUYENMAI → nếu có khuyến mãi
*/

let selectedMethod = 'TIENMAT';
let countdownSec = 600; // 10 phút

function initPayment() {
  startCountdown();
}

function selectMethod(method) {
  selectedMethod = method;
  document.querySelectorAll('.pay-method').forEach(el => el.classList.remove('active'));
  const map = { TIENMAT:'pm-tienmat', CHUYENKHOAN:'pm-chuyenkhoan' };
  document.getElementById(map[method])?.classList.add('active');
  const bankInfo = document.getElementById('bank-info');
  if (bankInfo) {
    method === 'CHUYENKHOAN' ? bankInfo.classList.remove('hidden') : bankInfo.classList.add('hidden');
  }
  const cashWarning = document.getElementById('cash-warning');
  if (cashWarning) {
    method === 'TIENMAT' ? cashWarning.style.display = 'block' : cashWarning.style.display = 'none';
  }
}

function checkReady() {
  const accepted = document.getElementById('accept-terms')?.checked;
  const btn = document.getElementById('btn-confirm');
  if (btn) btn.disabled = !accepted;
}

function confirmBooking() {
  const btn = document.getElementById('btn-confirm');
  if (btn) { btn.disabled = true; btn.textContent = '⏳ Đang xử lý...'; }

  const maChuyen = document.getElementById('hdn-maChuyen')?.value;
  const maGhes = document.getElementById('hdn-maGhes')?.value;
  const giaVe = document.getElementById('hdn-giaVe')?.value;
  const ghiChu = document.getElementById('pay-ghichu')?.value || '';
  const promoCode = document.getElementById('hdn-promoCode')?.value || '';

  if (!maChuyen || !maGhes) {
    showToast('Dữ liệu không hợp lệ', 'error');
    if (btn) { btn.disabled = false; btn.textContent = '🎫 Xác nhận đặt vé'; }
    return;
  }

  // Create form to POST to /payment
  const form = document.createElement('form');
  form.method = 'POST';
  form.action = (window.CTX || '') + '/payment';

  const addInput = (name, value) => {
    const inp = document.createElement('input');
    inp.type = 'hidden';
    inp.name = name;
    inp.value = value;
    form.appendChild(inp);
  };

  addInput('maChuyen', maChuyen);
  addInput('maGhes', maGhes);
  addInput('giaVe', giaVe);
  addInput('phuongThuc', selectedMethod);
  addInput('ghiChu', ghiChu);
  addInput('promoCode', promoCode);

  document.body.appendChild(form);
  form.submit();
}

function startCountdown() {
  const el = document.getElementById('countdown');
  if (!el) return;
  window.countdownTimer = setInterval(() => {
    countdownSec--;
    if (countdownSec <= 0) {
      clearInterval(window.countdownTimer);
      showToast('Hết thời gian giữ ghế! Vui lòng chọn lại.', 'error');
      setTimeout(() => location.href = (window.CTX || '') + '/search', 2000);
      return;
    }
    const m = Math.floor(countdownSec/60);
    const s = countdownSec % 60;
    el.textContent = `${String(m).padStart(2,'0')}:${String(s).padStart(2,'0')}`;
  }, 1000);
}

function copyText(text) {
  navigator.clipboard?.writeText(text).then(()=>showToast('Đã sao chép!','success'));
}

document.addEventListener('DOMContentLoaded', initPayment);
