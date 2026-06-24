Page({
  goPublish() { wx.navigateTo({ url: '/pages/publish/publish' }); },
  goMyServices() {},
  goMyOrders() { wx.switchTab({ url: '/pages/order-list/order-list' }); },
  goReviews() {}
})