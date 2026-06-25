const api = require('../../services/api');

Page({
  data: {
    slots: [],
    dates: [],
    selectedDate: '',
    loading: false
  },

  onLoad(options) {
    this.handymanId = options.handymanId || '';
    this.initDates();
    this.loadSlots();
  },

  initDates() {
    const dates = [];
    const dayNames = ['日', '一', '二', '三', '四', '五', '六'];
    for (let i = 0; i < 7; i++) {
      const date = new Date();
      date.setDate(date.getDate() + i);
      dates.push({
        date: this.formatDate(date),
        month: date.getMonth() + 1,
        day: date.getDate(),
        dayName: dayNames[date.getDay()]
      });
    }
    this.setData({
      dates,
      selectedDate: dates[0].date
    });
  },

  formatDate(date) {
    const y = date.getFullYear();
    const m = String(date.getMonth() + 1).padStart(2, '0');
    const d = String(date.getDate()).padStart(2, '0');
    return `${y}-${m}-${d}`;
  },

  selectDate(e) {
    this.setData({ selectedDate: e.currentTarget.dataset.date });
    this.loadSlots();
  },

  async loadSlots() {
    if (!this.handymanId || !this.data.selectedDate) return;
    this.setData({ loading: true });
    try {
      const slots = await api.getAvailableSlots(this.handymanId, this.data.selectedDate);
      this.setData({ slots });
    } catch (e) {
      console.error('Failed to load slots', e);
    }
    this.setData({ loading: false });
  },

  selectSlot(e) {
    const slot = e.currentTarget.dataset.slot;
    const pages = getCurrentPages();
    const prevPage = pages[pages.length - 2];
    if (prevPage) {
      prevPage.setSelectedSlot && prevPage.setSelectedSlot(slot);
    }
    wx.navigateBack();
  }
});
