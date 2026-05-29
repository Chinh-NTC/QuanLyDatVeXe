/* ── BOOKING PAGE JS ── */

let tripData    = null;   
let selectedSeats = [];   
let appliedPromo  = null; 

function initBooking() {
  // tripData is injected by JSP
  if (window.TRIP_DATA) {
    tripData = window.TRIP_DATA;
    updateTotal();
    initWebSocket();
  }
}

function initWebSocket() {
  const protocol = location.protocol === 'https:' ? 'wss:' : 'ws:';
  const wsUrl = `${protocol}//${location.host}${window.CTX}/ws/ghe/${tripData.maChuyen}`;
  const socket = new WebSocket(wsUrl);

  socket.onmessage = (event) => {
    const msg = event.data;
    if (msg.startsWith('BOOKED:')) {
      const maGhe = msg.replace('BOOKED:', '');
      const seatEl = document.getElementById('seat-' + maGhe);
      if (seatEl) {
        seatEl.classList.remove('trong', 'selected');
        seatEl.classList.add('dadat');
        seatEl.onclick = null;
        seatEl.title = seatEl.dataset.so + ' (Vừa bị đặt)';
        
        // Remove from selected if user was selecting it
        const idx = selectedSeats.findIndex(s => s.maGhe === maGhe);
        if (idx >= 0) {
          selectedSeats.splice(idx, 1);
          updateSelectionInfo();
          updateTotal();
          showToast(`Ghế ${seatEl.dataset.so} vừa được người khác đặt`, 'warning');
        }
      }
    } else if (msg.startsWith('RELEASED:')) {
      const maGhe = msg.replace('RELEASED:', '');
      const seatEl = document.getElementById('seat-' + maGhe);
      if (seatEl && seatEl.classList.contains('dadat')) {
        seatEl.classList.remove('dadat');
        seatEl.classList.add('trong');
        seatEl.onclick = () => toggleSeat(maGhe, seatEl.dataset.so);
        seatEl.title = seatEl.dataset.so;
      }
    }
  };
}

// Seat is toggled from JSP's onclick="toggleSeat(maGhe, 'soGhe')"
function toggleSeat(maGhe, soGhe) {
  const seatEl = document.getElementById('seat-' + maGhe);
  if (!seatEl) return;

  const idx = selectedSeats.findIndex(s => s.maGhe === maGhe);
  if (idx >= 0) {
    // Deselect
    selectedSeats.splice(idx, 1);
    seatEl.classList.remove('selected');
    seatEl.classList.add('trong');
  } else {
    // Select
    if (selectedSeats.length >= 5) {
      showToast('Tối đa 5 ghế mỗi lần đặt', 'warning'); 
      return;
    }
    selectedSeats.push({ maGhe, soGhe });
    seatEl.classList.remove('trong');
    seatEl.classList.add('selected');
  }

  updateSelectionInfo();
  updateTotal();
}

function updateSelectionInfo() {
  const infoEl = document.getElementById('seat-selection-info');
  const tagsEl = document.getElementById('selected-tags');
  const btnBook = document.getElementById('btn-book');
  if (!infoEl || !tagsEl || !btnBook) return;

  if (selectedSeats.length === 0) {
    infoEl.style.display = 'none';
    btnBook.disabled = true;
    return;
  }
  infoEl.style.display = 'flex';
  tagsEl.innerHTML = selectedSeats.map(s => `<span class="selected-tag">${s.soGhe}</span>`).join('');
  btnBook.disabled = false;
}

function clearSeats() {
  selectedSeats.forEach(s => {
    const seatEl = document.getElementById('seat-' + s.maGhe);
    if (seatEl) {
      seatEl.classList.remove('selected');
      seatEl.classList.add('trong');
    }
  });
  selectedSeats = [];
  updateSelectionInfo();
  updateTotal();
}

function updateTotal() {
  const count   = selectedSeats.length;
  const priceP  = tripData?.giaVe || 0;
  const subTotal= count * priceP;
  let discount  = 0;

  if (appliedPromo) {
    if (appliedPromo.loaiKM === 'PHANTRAM') {
      discount = Math.round(subTotal * appliedPromo.giaTriGiam / 100);
    } else {
      discount = Math.min(appliedPromo.giaTriGiam, subTotal);
    }
  }

  const total   = subTotal - discount;
  const deposit = Math.round(total * 0.3);

  const countEl = document.getElementById('sum-seat-count');
  if (countEl) countEl.textContent = `${count} ghế`;
  
  const namesEl = document.getElementById('sum-seat-names');
  if (namesEl) namesEl.textContent = selectedSeats.map(s => s.soGhe).join(', ');
  
  const totalEl = document.getElementById('sum-total');
  if (totalEl) totalEl.textContent   = formatCurrency(total);
  
  const depositEl = document.getElementById('sum-deposit');
  if (depositEl) depositEl.textContent = formatCurrency(deposit);

  const discRow = document.getElementById('discount-row');
  if (discRow) {
    if (discount > 0) {
      discRow.style.display = '';
      document.getElementById('sum-discount').textContent = '- ' + formatCurrency(discount);
    } else {
      discRow.style.display = 'none';
    }
  }
}

function applyPromo() {
  const code = document.getElementById('promo-code')?.value?.trim();
  const result = document.getElementById('promo-result');
  if (!code) { showToast('Nhập mã khuyến mãi', 'warning'); return; }

  const url = (window.CTX || '') + '/promo?code=' + encodeURIComponent(code);
  fetch(url)
    .then(res => res.json())
    .then(promo => {
      if (promo.success) {
        appliedPromo = promo;
        if(result) result.innerHTML = `<span style="color:var(--success)">✅ ${promo.tenKhuyenMai}</span>`;
        showToast('Áp dụng mã khuyến mãi thành công!', 'success');
        updateTotal();
      } else {
        appliedPromo = null;
        if(result) result.innerHTML = `<span style="color:var(--danger)">❌ Mã không hợp lệ hoặc đã hết hạn</span>`;
        updateTotal();
      }
    })
    .catch(err => {
        showToast('Lỗi khi kiểm tra mã khuyến mãi', 'danger');
    });
}

function proceedToPayment() {
  if (!selectedSeats.length) { 
    showToast('Vui lòng chọn ít nhất 1 ghế', 'warning'); 
    return; 
  }

  const hoTen = document.getElementById('input-hoten')?.value?.trim();
  const sdt   = document.getElementById('input-sdt')?.value?.trim();
  if (!hoTen) { showToast('Vui lòng nhập họ tên hành khách', 'warning'); return; }
  if (!sdt)   { showToast('Vui lòng nhập số điện thoại',     'warning'); return; }

  const ghiChu = document.getElementById('input-ghichu')?.value || '';
  
  // Submit via POST to /booking
  const form = document.createElement('form');
  form.method = 'POST';
  form.action = (window.CTX || '') + '/booking';
  
  const inputChuyen = document.createElement('input');
  inputChuyen.type = 'hidden';
  inputChuyen.name = 'maChuyen';
  inputChuyen.value = tripData.maChuyen;
  form.appendChild(inputChuyen);
  
  selectedSeats.forEach(seat => {
    const inputGhe = document.createElement('input');
    inputGhe.type = 'hidden';
    inputGhe.name = 'maGhes';
    inputGhe.value = seat.maGhe;
    form.appendChild(inputGhe);
  });
  
  const inputGhiChu = document.createElement('input');
  inputGhiChu.type = 'hidden';
  inputGhiChu.name = 'ghiChu';
  inputGhiChu.value = ghiChu;
  form.appendChild(inputGhiChu);
  
  document.body.appendChild(form);
  form.submit();
}

document.addEventListener('DOMContentLoaded', initBooking);

