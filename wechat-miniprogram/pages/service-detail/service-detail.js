const api = require('../../services/api');
const app = getApp();

Page({
  data: { service: {} },

  onLoad(options) {
    this.loadService(options.id);
  },

  async loadService(id) {
    const service = await api.getServiceById(id);
    this.setData({ service });
  },

  goChat() {
    if (!app.globalData.token) {
      wx.navigateTo({ url: '/pages/login/login' });
      return;
    }
  },

  goOrder() {
    if (!app.globalData.token) {
      wx.navigateTo({ url: '/pages/login/login' });
      return;
    }
    wx.navigateTo({ url: `/pages/order-create/order-create?serviceId=${this.data.service.id}&price=${this.data.service.price}&unit=${this.data.service.unit}` });
  }
})