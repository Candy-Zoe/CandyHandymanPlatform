const api = require('../../services/api');

Page({
  data: { title: '', description: '', price: '', unit: '次' },
  onTitleInput(e) { this.setData({ title: e.detail.value }); },
  onDescInput(e) { this.setData({ description: e.detail.value }); },
  onPriceInput(e) { this.setData({ price: e.detail.value }); },
  onUnitInput(e) { this.setData({ unit: e.detail.value }); },

  async publish() {
    const { title, description, price, unit } = this.data;
    if (!title || !price) {
      wx.showToast({ title: '请填写标题和价格', icon: 'none' });
      return;
    }
    try {
      await api.createService({ title, description, categoryId: '', pricingType: 'PerJob', price: parseFloat(price), unit });
      wx.showToast({ title: '发布成功', icon: 'success' });
      setTimeout(() => wx.navigateBack(), 1500);
    } catch (e) {
      wx.showToast({ title: e.message || '发布失败', icon: 'none' });
    }
  }
})