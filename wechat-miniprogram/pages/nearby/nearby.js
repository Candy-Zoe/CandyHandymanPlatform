const api = require('../../services/api');
const app = getApp();

Page({
  data: { handymen: [] },
  onLoad() { this.loadNearby(); },
  loadNearby() {
    wx.getLocation({
      type: 'gcj02',
      success: async (res) => {
        try {
          const handymen = await api.getNearby({
            latitude: res.latitude,
            longitude: res.longitude
          });
          const formatted = handymen.map(h => ({
            ...h,
            distanceText: h.distance.toFixed(1)
          }));
          this.setData({ handymen: formatted });
        } catch (e) {
          console.error(e);
        }
      }
    });
  },
  goDetail(e) {
    wx.navigateTo({ url: `/pages/service-detail/service-detail?id=${e.currentTarget.dataset.id}` });
  }
})