const api = require('../../services/api');

Page({
  data: { orders: [], currentTab: 0 },
  onLoad() { this.loadOrders(); },

  switchTab(e) {
    const tab = parseInt(e.currentTarget.dataset.tab);
    this.setData({ currentTab: tab });
    this.loadOrders();
  },

  async loadOrders() {
    const statuses = [null, 'Pending', 'InProgress', 'Completed'];
    const status = statuses[this.data.currentTab];
    const orders = await api.getOrders(status);
    this.setData({ orders });
  },

  goDetail(e) {
    wx.navigateTo({ url: `/pages/order-detail/order-detail?id=${e.currentTarget.dataset.id}` });
  }
})