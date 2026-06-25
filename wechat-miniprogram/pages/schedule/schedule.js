const api = require('../../services/api');

Page({
  data: {
    schedules: [],
    dayNames: ['周日', '周一', '周二', '周三', '周四', '周五', '周六'],
    loading: false
  },

  onLoad(options) {
    this.handymanId = options.handymanId || '';
    this.loadSchedules();
  },

  async loadSchedules() {
    if (!this.handymanId) return;
    this.setData({ loading: true });
    try {
      const schedules = await api.getSchedules(this.handymanId);
      this.setData({ schedules });
    } catch (e) {
      console.error('Failed to load schedules', e);
    }
    this.setData({ loading: false });
  },

  async generateSlots() {
    if (!this.handymanId) return;
    wx.showLoading({ title: '生成中...' });
    try {
      await api.generateSlots(this.handymanId, 14);
      wx.showToast({ title: '已生成14天时段', icon: 'success' });
    } catch (e) {
      wx.showToast({ title: '生成失败', icon: 'error' });
    }
    wx.hideLoading();
  }
});
