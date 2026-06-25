const api = require('../../services/api');

Page({
  data: { favorites: [], loading: false },
  onLoad() { this.loadFavorites(); },
  onShow() { this.loadFavorites(); },

  async loadFavorites() {
    this.setData({ loading: true });
    try {
      const favorites = await api.getMyFavorites();
      this.setData({ favorites: favorites || [] });
    } catch (e) { console.error(e); }
    this.setData({ loading: false });
  },

  async removeFavorite(e) {
    const { serviceId, handymanId } = e.currentTarget.dataset;
    await api.removeFavorite({ serviceId: serviceId || null, handymanProfileId: handymanId || null });
    this.loadFavorites();
  },

  goDetail(e) {
    const { serviceId, handymanId } = e.currentTarget.dataset;
    if (serviceId) wx.navigateTo({ url: `/pages/service-detail/service-detail?id=${serviceId}` });
  }
});
