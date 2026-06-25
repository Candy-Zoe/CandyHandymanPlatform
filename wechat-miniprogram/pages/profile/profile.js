const api = require('../../services/api');
const app = getApp();

Page({
  data: { userInfo: {} },
  onShow() {
    if (app.globalData.userInfo) {
      this.setData({ userInfo: app.globalData.userInfo });
    }
  },
  goNotifications() { wx.navigateTo({ url: '/pages/notifications/notifications' }); },
  goWallet() { wx.navigateTo({ url: '/pages/wallet/wallet' }); },
  goCoupons() { wx.navigateTo({ url: '/pages/coupon/coupon' }); },
  goFavorites() { wx.navigateTo({ url: '/pages/favorites/favorites' }); },
  goHistory() { wx.navigateTo({ url: '/pages/history/history' }); },
  goAnnouncements() { wx.navigateTo({ url: '/pages/announcements/announcements' }); },
  goHelp() { wx.navigateTo({ url: '/pages/help/help' }); },
  goFeedback() { wx.navigateTo({ url: '/pages/feedback/feedback' }); },
  goNearby() { wx.navigateTo({ url: '/pages/nearby/nearby' }); },
  goVerification() { wx.navigateTo({ url: '/pages/verification/verification' }); },
  goCertification() { wx.navigateTo({ url: '/pages/certification/certification' }); },
  goProvider() { wx.navigateTo({ url: '/pages/provider/provider' }); },
  goPublish() { wx.navigateTo({ url: '/pages/publish/publish' }); },
  goOrders() { wx.switchTab({ url: '/pages/order-list/order-list' }); },
  goPayment() { wx.navigateTo({ url: '/pages/payment/payment' }); },
  goDisputes() { wx.navigateTo({ url: '/pages/dispute/dispute' }); }
})