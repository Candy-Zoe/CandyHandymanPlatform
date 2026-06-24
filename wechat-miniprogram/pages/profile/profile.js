const api = require('../../services/api');
const app = getApp();

Page({
  data: { userInfo: {} },
  onShow() {
    if (app.globalData.userInfo) {
      this.setData({ userInfo: app.globalData.userInfo });
    }
  },
  goNearby() { wx.navigateTo({ url: '/pages/nearby/nearby' }); },
  goVerification() { wx.navigateTo({ url: '/pages/verification/verification' }); },
  goCertification() { wx.navigateTo({ url: '/pages/certification/certification' }); },
  goProvider() { wx.navigateTo({ url: '/pages/provider/provider' }); },
  goPublish() { wx.navigateTo({ url: '/pages/publish/publish' }); },
  goOrders() { wx.switchTab({ url: '/pages/order-list/order-list' }); },
  goPayment() { wx.navigateTo({ url: '/pages/payment/payment' }); },
  goDisputes() { wx.navigateTo({ url: '/pages/dispute/dispute' }); }
})