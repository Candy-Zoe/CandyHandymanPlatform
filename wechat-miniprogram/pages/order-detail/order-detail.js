const api = require('../../services/api');

Page({
  data: { order: {} },
  onLoad(options) { this.loadOrder(options.id); },

  async loadOrder(id) {
    const orders = await api.getOrders();
    const order = orders.find(o => o.id === id);
    this.setData({ order });
  },

  async acceptOrder() {
    await api.acceptOrder(this.data.order.id);
    this.loadOrder(this.data.order.id);
    wx.showToast({ title: '已接单' });
  },
  async startOrder() {
    await api.startOrder(this.data.order.id);
    this.loadOrder(this.data.order.id);
    wx.showToast({ title: '已开始' });
  },
  async completeOrder() {
    await api.completeOrder(this.data.order.id);
    this.loadOrder(this.data.order.id);
    wx.showToast({ title: '已完成' });
  }
})