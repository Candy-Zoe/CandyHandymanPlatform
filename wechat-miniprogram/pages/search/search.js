const api = require('../../services/api');
Page({
  data: { results: [] },
  async onSearch(e) {
    const keyword = e.detail;
    if (!keyword) return;
    const results = await api.getServices({ keyword });
    this.setData({ results });
  },
  goDetail(e) {
    wx.navigateTo({ url: `/pages/service-detail/service-detail?id=${e.currentTarget.dataset.id}` });
  }
})