// ── Seat Selection Logic ──────────────────────────────────────────────────────
const selectedSeats = []; // [{maGhe, soGhe}]
let TRIP_DATA = window.TRIP_DATA || { giaVe: 0, maChuyen: '' };

function toggleSeat(maGhe, soGhe) {
    const el = document.getElementById('seat-' + maGhe);
    if (!el) return;
    const idx = selectedSeats.findIndex(s => s.maGhe === maGhe);
    if (idx >= 0) {
        selectedSeats.splice(idx, 1);
        el.classList.remove('selected');
        el.classList.add('trong');
    } else {
        if (selectedSeats.length >= 5) {
            showToast('Bạn chỉ được chọn tối đa 5 ghế 1 lần', 'error');
            return;
        }
        selectedSeats.push({ maGhe, soGhe });
        el.classList.add('selected');
        el.classList.remove('trong');
    }
    updateSeatUI();
}

function updateSeatUI() {
    const total     = selectedSeats.length * TRIP_DATA.giaVe;
    const sumCount  = document.getElementById('sum-seat-count');
    const sumNames  = document.getElementById('sum-seat-names');
    const sumTotal  = document.getElementById('sum-total');
    const infoBar   = document.getElementById('seat-info-bar');
    const confirmBtn= document.getElementById('btn-confirm');

    if (sumCount) sumCount.textContent = selectedSeats.length + ' ghế';
    if (sumNames) sumNames.textContent = selectedSeats.map(s => s.soGhe).join(', ');
    if (sumTotal) sumTotal.textContent = formatMoney(total);
    if (infoBar)  infoBar.style.display = selectedSeats.length > 0 ? 'flex' : 'none';
    if (confirmBtn) confirmBtn.disabled = selectedSeats.length === 0;
}

function showFloor(floor, btn) {
    document.querySelectorAll('.bus-floor').forEach(f => f.style.display = 'none');
    const el = document.getElementById('floor-' + floor);
    if (el) el.style.display = 'block';
    document.querySelectorAll('.floor-tab').forEach(t => t.classList.remove('active'));
    if (btn) btn.classList.add('active');
}

function formatMoney(n) {
    return new Intl.NumberFormat('vi-VN').format(n) + '₫';
}

// ── Submit booking form ──────────────────────────────────────────────────────
function submitBooking(tenNguoiDi, sdtNguoiDi, ghiChu) {
    if (selectedSeats.length === 0) {
        showToast('Vui lòng chọn ít nhất 1 ghế', 'error');
        return;
    }
    const form = document.getElementById('form-chon-ghe');
    if (form) {
        document.getElementById('hdn-dsGhe').value  = JSON.stringify(selectedSeats.map(s => s.maGhe));
        document.getElementById('hdn-ten').value    = tenNguoiDi;
        document.getElementById('hdn-sdt').value    = sdtNguoiDi;
        document.getElementById('hdn-ghichu').value = ghiChu;
        form.submit();
    }
}

// ── Toast notifications ──────────────────────────────────────────────────────
function showToast(msg, type = 'info') {
    const container = document.getElementById('toast-container') || createToastContainer();
    const toast = document.createElement('div');
    toast.className = 'toast ' + type;
    toast.textContent = msg;
    container.appendChild(toast);
    setTimeout(() => toast.remove(), 3500);
}

function createToastContainer() {
    const c = document.createElement('div');
    c.id = 'toast-container';
    c.className = 'toast-container';
    document.body.appendChild(c);
    return c;
}

// ── Confirm booking (ThanhToan page) ─────────────────────────────────────────
async function confirmBooking(maChuyen, dsGhe, tenNguoiDi, sdtNguoiDi, phuongThuc, ghiChu, maKM) {
    const btn = document.getElementById('btn-submit');
    if (btn) { btn.disabled = true; btn.textContent = 'Đang xử lý...'; }

    try {
        const res = await fetch('/DatVe/XacNhanDatVe', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ maChuyen, dsGhe, tenNguoiDi, sdtNguoiDi, phuongThuc, ghiChu, maKM })
        });
        const data = await res.json();
        if (data.thanhCong) {
            document.getElementById('modal-ma-don').textContent = data.maDon;
            openModal('modal-success');
        } else {
            showToast(data.thongBao || 'Đặt vé thất bại', 'error');
            if (btn) { btn.disabled = false; btn.textContent = '🎫 Xác nhận đặt vé'; }
        }
    } catch (e) {
        showToast('Lỗi kết nối, vui lòng thử lại', 'error');
        if (btn) { btn.disabled = false; btn.textContent = '🎫 Xác nhận đặt vé'; }
    }
}

// ── Modal ─────────────────────────────────────────────────────────────────────
function openModal(id)  { const m = document.getElementById(id); if (m) m.classList.add('active'); }
function closeModal(id) { const m = document.getElementById(id); if (m) m.classList.remove('active'); }

// ── Swap tỉnh đi / tỉnh đến ──────────────────────────────────────────────────
function swapTinh() {
    const a = document.getElementById('select-tinh-di');
    const b = document.getElementById('select-tinh-den');
    if (a && b) { const tmp = a.value; a.value = b.value; b.value = tmp; }
}

// ── Auto dismiss TempData alerts ──────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {
    const alerts = document.querySelectorAll('.alert[data-autodismiss]');
    alerts.forEach(a => setTimeout(() => a.style.display = 'none', 4000));

    // Set min date for date inputs to today
    const dateInputs = document.querySelectorAll('input[type="date"][data-mintoday]');
    const today = new Date().toISOString().split('T')[0];
    dateInputs.forEach(i => { i.min = today; if (!i.value) i.value = today; });
});
