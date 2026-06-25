const i18nData = require('./i18n');

const app = getApp();

function t(key) {
  const lang = app.globalData?.lang || 'zh-CN';
  return i18nData[lang]?.[key] || i18nData['zh-CN']?.[key] || key;
}

function setLang(lang) {
  if (app.globalData) {
    app.globalData.lang = lang;
  }
  wx.setStorageSync('lang', lang);
}

function getLang() {
  return app.globalData?.lang || wx.getStorageSync('lang') || 'zh-CN';
}

module.exports = { t, setLang, getLang };
