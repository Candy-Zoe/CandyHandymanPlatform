const api = require('../../services/api');

Page({
  data: {
    coupons: [],
    loading: false
  },

  onLoad() {
    this.loadCoupons();
  },

  onShow() {
    this.loadCoupons();
  },

  async loadCoupons() {
    this.setData({ loading: true });
    try {
      const coupons = await api.getMyCoupons();
      this.setData({ coupons: coupons || [] });
    } catch (e) {
      console.error('Failed to load coupons', e);
    }
    this.setData({ loading: false });
  }
});
