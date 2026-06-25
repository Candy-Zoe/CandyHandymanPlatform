const api = require('../../services/api');

Page({
  data: { feedbacks: [], content: '', contactInfo: '', type: 'Suggestion' },
  onLoad() { this.loadFeedbacks(); },

  async loadFeedbacks() {
    try {
      const feedbacks = await api.getMyFeedbacks();
      this.setData({ feedbacks: feedbacks || [] });
    } catch (e) { console.error(e); }
  },

  onContentInput(e) { this.setData({ content: e.detail.value }); },
  onContactInput(e) { this.setData({ contactInfo: e.detail.value }); },
  onTypeChange(e) { this.setData({ type: ['Bug', 'Suggestion', 'Complaint', 'Other'][e.detail.value] }); },

  async submit() {
    if (!this.data.content.trim()) {
      wx.showToast({ title: '请输入反馈内容', icon: 'error' });
      return;
    }
    try {
      await api.submitFeedback({ content: this.data.content, contactInfo: this.data.contactInfo, type: this.data.type });
      wx.showToast({ title: '提交成功', icon: 'success' });
      this.setData({ content: '', contactInfo: '' });
      this.loadFeedbacks();
    } catch (e) {
      wx.showToast({ title: '提交失败', icon: 'error' });
    }
  }
});
