const api = require('../../services/api');

Page({
  data: { history: [], loading: false },
  onLoad() { this.loadHistory(); },

  async loadHistory() {
    this.setData({ loading: true });
    try {
      const history = await api.getMyHistory({ limit: 50 });
      this.setData({ history: history || [] });
    } catch (e) { console.error(e); }
    this.setData({ loading: false });
  },

  async clearHistory() {
    wx.showModal({
      title: '确认清空',
      content: '确定清空浏览历史？',
      success: async (res) => {
        if (res.confirm) {
          await api.clearHistory();
          this.setData({ history: [] });
          wx.showToast({ title: '已清空', icon: 'success' });
        }
      }
    });
  },

  goDetail(e) {
    const { serviceId, handymanId } = e.currentTarget.dataset;
    if (serviceId) wx.navigateTo({ url: `/pages/service-detail/service-detail?id=${serviceId}` });
  }
});
