const api = require('../../services/api');

Page({
  data: { conversations: [] },
  onShow() { this.loadConversations(); },
  async loadConversations() {
    const conversations = await api.getConversations();
    this.setData({ conversations });
  },
  goChat(e) {
    wx.navigateTo({ url: `/pages/chat/chat?id=${e.currentTarget.dataset.id}` });
  }
})