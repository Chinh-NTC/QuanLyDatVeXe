/* ── LOGIN / REGISTER JS ── */

function initLogin() {
  if (location.hash === '#register') {
    if (typeof switchAuthTab === 'function') {
      switchAuthTab('register');
    }
  }
}

document.addEventListener('DOMContentLoaded', initLogin);
