const api = require('../../services/api');

Page({
  data: {
    categories: [],
    services: []
  },

  onLoad() {
    this.loadData();
  },

  async loadData() {
    try {
      const [categories, services] = await Promise.all([
        api.getCategories(),
        api.getServices()
      ]);
      this.setData({ categories, services });
    } catch (e) {
      console.error(e);
    }
  },

  goSearch() {
    wx.navigateTo({ url: '/pages/search/search' });
  },

  goCategory(e) {
    const id = e.currentTarget.dataset.id;
    wx.navigateTo({ url: `/pages/category/category?id=${id}` });
  },

  goServiceDetail(e) {
    const id = e.currentTarget.dataset.id;
    wx.navigateTo({ url: `/pages/service-detail/service-detail?id=${id}` });
  }
})