const api = require('../../services/api');
const app = getApp();

Page({
  data: { phone: '', password: '' },
  onPhoneInput(e) { this.setData({ phone: e.detail.value }); },
  onPasswordInput(e) { this.setData({ password: e.detail.value }); },

  async login() {
    const { phone, password } = this.data;
    if (!phone || !password) {
      wx.showToast({ title: '请输入手机号和密码', icon: 'none' });
      return;
    }
    try {
      const res = await api.login({ phone, password });
      app.globalData.token = res.token;
      app.globalData.userInfo = res.user;
      wx.setStorageSync('token', res.token);
      wx.setStorageSync('userInfo', res.user);
      wx.showToast({ title: '登录成功' });
      setTimeout(() => wx.navigateBack(), 1500);
    } catch (e) {
      wx.showToast({ title: e.message || '登录失败', icon: 'none' });
    }
  }
})