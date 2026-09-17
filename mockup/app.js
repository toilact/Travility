const navItems = document.querySelectorAll('.nav-item');
const viewPanels = document.querySelectorAll('[data-view-panel]');
const categoryCards = document.querySelectorAll('.category-card');
const placeCards = document.querySelectorAll('.place-card');
const resultTitle = document.getElementById('results-title');
const resultCount = document.getElementById('result-count');
const emptyMessage = document.getElementById('empty-message');
const toast = document.getElementById('toast');
let toastTimer;

const categoryNames = {
  hotel: 'Nơi ở được yêu thích',
  food: 'Quán ăn đáng thử',
  cafe: 'Cà phê có gu',
  fun: 'Trải nghiệm nổi bật'
};

const sampleNames = {
  hotel: ['The Myst Dong Khoi', 'M Village Hai Bà Trưng', 'Maison De Camille'],
  food: ['Bánh Mì Huynh Hoa', 'Cục Gạch Quán', 'Ốc Đào Nguyễn Trãi'],
  cafe: ['The Workshop Coffee', 'Shin Heritage', 'Cộng Cà Phê Đinh Tiên Hoàng'],
  fun: ['Bưu điện Thành phố', 'Chợ Bến Thành', 'Landmark 81 SkyView']
};

function showToast(message) {
  toast.textContent = message;
  toast.classList.add('is-visible');
  clearTimeout(toastTimer);
  toastTimer = setTimeout(() => toast.classList.remove('is-visible'), 2400);
}

function showView(viewName) {
  navItems.forEach((item) => item.classList.toggle('is-active', item.dataset.view === viewName));
  viewPanels.forEach((panel) => {
    panel.hidden = panel.dataset.viewPanel !== viewName;
  });
}

function filterPlaces(category) {
  categoryCards.forEach((card) => card.classList.toggle('is-selected', card.dataset.filter === category));
  resultTitle.textContent = categoryNames[category];
  const names = sampleNames[category];
  let visibleCount = 0;
  placeCards.forEach((card, index) => {
    const matches = card.dataset.category === category;
    card.hidden = !matches;
    if (matches) {
      visibleCount += 1;
      card.querySelector('h3').textContent = names[index] || names[0];
      card.querySelector('.place-meta span').textContent = category === 'hotel' ? 'KHÁCH SẠN' : category.toUpperCase();
    }
  });
  resultCount.textContent = `${visibleCount ? 24 : 0} địa điểm`;
  emptyMessage.hidden = visibleCount > 0;
  showToast(`Đã lọc: ${categoryNames[category]}`);
}

navItems.forEach((item) => {
  item.addEventListener('click', () => showView(item.dataset.view));
});

categoryCards.forEach((card) => {
  card.addEventListener('click', () => {
    showView('explore');
    filterPlaces(card.dataset.filter);
  });
});

document.querySelectorAll('.day-tab').forEach((tab) => {
  tab.addEventListener('click', () => {
    document.querySelectorAll('.day-tab').forEach((item) => item.classList.remove('is-active'));
    tab.classList.add('is-active');
    showToast(`Đang xem ${tab.textContent.trim().replace(/\s+/g, ' ')}`);
  });
});

document.querySelectorAll('.save-button').forEach((button) => {
  button.addEventListener('click', (event) => {
    event.stopPropagation();
    button.textContent = button.textContent === '♥' ? '♡' : '♥';
    showToast(button.textContent === '♥' ? 'Đã lưu địa điểm' : 'Đã bỏ lưu địa điểm');
  });
});

document.querySelectorAll('.map-pin').forEach((pin) => {
  pin.addEventListener('click', () => showToast(`Đang xem ${pin.dataset.place}`));
});

document.querySelectorAll('[data-action="new-trip"]').forEach((button) => {
  button.addEventListener('click', () => showToast('Trip Wizard sẽ được mở ở bước tiếp theo'));
});

document.querySelectorAll('[data-action="view-all"]').forEach((button) => {
  button.addEventListener('click', () => showToast('Đang mở toàn bộ địa điểm'));
});

document.querySelectorAll('[data-action="explore"]').forEach((button) => {
  button.addEventListener('click', () => showView('explore'));
});

document.querySelector('.map-control').addEventListener('click', () => showToast('Đã lấy vị trí hiện tại trên bản đồ'));
document.querySelector('.map-zoom').addEventListener('click', (event) => {
  if (event.target.matches('button')) showToast(event.target.textContent === '+' ? 'Phóng to bản đồ' : 'Thu nhỏ bản đồ');
});
