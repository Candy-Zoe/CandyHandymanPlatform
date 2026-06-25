const api = require('../../services/api');

Page({
  data: {
    rankings: [],
    loading: false
  },

  onLoad() {
    this.loadRanking();
  },

  async loadRanking() {
    this.setData({ loading: true });
    try {
      const rankings = await api.getHandymenRanking({ top: 20 });
      this.setData({ rankings });
    } catch (e) {
      console.error('Failed to load ranking', e);
    }
    this.setData({ loading: false });
  }
});
