const api = require('../../services/api');
Page({
  data: { services: [], categoryId: '' },
  onLoad(options) {
    this.setData({ categoryId: options.id });
    this.loadServices();
  },
  async loadServices() {
    const services = await api.getServices({ categoryId: this.data.categoryId });
    this.setData({ services });
  },
  goDetail(e) {
    wx.navigateTo({ url: `/pages/service-detail/service-detail?id=${e.currentTarget.dataset.id}` });
  }
})