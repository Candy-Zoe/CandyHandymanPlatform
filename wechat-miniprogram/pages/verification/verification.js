const api = require('../../services/api');
const app = getApp();

Page({
  data: { status: 'NotSubmitted', realName: '', idCard: '', frontUrl: '', backUrl: '' },
  onLoad() { this.loadStatus(); },
  async loadStatus() {
    try {
      const res = await api.getVerificationStatus();
      this.setData({ status: res.status, realName: res.realName || '' });
    } catch (e) {}
  },
  onNameInput(e) { this.setData({ realName: e.detail.value }); },
  onIdCardInput(e) { this.setData({ idCard: e.detail.value }); },
  uploadFront() {
    wx.chooseMedia({ count: 1, mediaType: ['image'], success: (res) => {
      this.setData({ frontUrl: res.tempFiles[0].tempFilePath });
    }});
  },
  uploadBack() {
    wx.chooseMedia({ count: 1, mediaType: ['image'], success: (res) => {
      this.setData({ backUrl: res.tempFiles[0].tempFilePath });
    }});
  },
  async submit() {
    const { realName, idCard, frontUrl, backUrl } = this.data;
    if (!realName || !idCard) {
      wx.showToast({ title: '请填写完整信息', icon: 'none' });
      return;
    }
    try {
      await api.submitVerification({ realName, idCardNumber: idCard, idCardFrontUrl: frontUrl, idCardBackUrl: backUrl });
      wx.showToast({ title: '提交成功' });
      this.loadStatus();
    } catch (e) {
      wx.showToast({ title: '提交失败', icon: 'none' });
    }
  }
})