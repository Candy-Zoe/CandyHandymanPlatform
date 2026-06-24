const api = require('../../services/api');
Page({
  data: { orderId: '', hasInsurance: false },
  onLoad(options) { this.setData({ orderId: options.orderId }); this.checkInsurance(); },
  async checkInsurance() {
    try {
      const res = await api.getOrderInsurance(this.data.orderId);
      if (res.hasInsurance) {
        this.setData({ hasInsurance: true, policyNo: res.policyNo, coverageAmount: res.coverageAmount, insuranceType: res.type });
      }
    } catch (e) {}
  },
  async purchase(e) {
    const type = e.currentTarget.dataset.type;
    try {
      await api.purchaseInsurance({ orderId: this.data.orderId, type });
      wx.showToast({ title: '购买成功' });
      this.checkInsurance();
    } catch (e) {
      wx.showToast({ title: '购买失败', icon: 'none' });
    }
  }
})