const api = require('../../services/api');
const app = getApp();

Page({
  data: { conversationId: '', messages: [], inputText: '', currentUserId: '' },

  onLoad(options) {
    this.setData({
      conversationId: options.id,
      currentUserId: app.globalData.userInfo?.id || ''
    });
    this.loadMessages();
  },

  async loadMessages() {
    const messages = await api.getMessages(this.data.conversationId);
    this.setData({ messages, scrollTo: `msg-${messages[messages.length - 1]?.id}` });
  },

  onInput(e) { this.setData({ inputText: e.detail.value }); },

  async send() {
    if (!this.data.inputText) return;
    await api.sendMessage({ receiverId: '', content: this.data.inputText, messageType: 'Text' });
    this.setData({ inputText: '' });
    this.loadMessages();
  }
})