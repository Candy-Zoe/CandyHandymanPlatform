App({
  globalData: {
    baseUrl: 'http://localhost:5000',
    token: '',
    userInfo: null,
    lang: 'zh-CN'
  },
  onLaunch() {
    const token = wx.getStorageSync('token');
    if (token) {
      this.globalData.token = token;
    }
    const userInfo = wx.getStorageSync('userInfo');
    if (userInfo) {
      this.globalData.userInfo = userInfo;
    }
    const lang = wx.getStorageSync('lang');
    if (lang) {
      this.globalData.lang = lang;
    }
  }
})