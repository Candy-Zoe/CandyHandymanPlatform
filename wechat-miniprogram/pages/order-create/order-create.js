const api = require('../../services/api');
const app = getApp();

Page({
  data: { serviceId: '', quantity: 1, address: '', phone: '', description: '', price: 0, unit: '', total: 0 },

  onLoad(options) {
    this.setData({
      serviceId: options.serviceId,
      price: parseFloat(options.price),
      unit: options.unit,
      total: parseFloat(options.price)
    });
  },

  onQuantityInput(e) {
    const quantity = parseInt(e.detail.value) || 1;
    this.setData({ quantity, total: (this.data.price * quantity).toFixed(2) });
  },
  onAddressInput(e) { this.setData({ address: e.detail.value }); },
  onPhoneInput(e) { this.setData({ phone: e.detail.value }); },
  onDescInput(e) { this.setData({ description: e.detail.value }); },

  async submitOrder() {
    const { serviceId, quantity, address, phone, description } = this.data;
    if (!address || !phone) {
      wx.showToast({ title: '请填写地址和电话', icon: 'none' });
      return;
    }
    try {
      await api.createOrder({ serviceId, quantity, address, contactPhone: phone, description: description || null });
      wx.showToast({ title: '下单成功', icon: 'success' });
      setTimeout(() => wx.navigateBack(), 1500);
    } catch (e) {
      wx.showToast({ title: e.message || '下单失败', icon: 'none' });
    }
  }
})