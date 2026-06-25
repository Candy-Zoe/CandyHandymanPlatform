const api = require('../../services/api');

Page({
  data: {
    balance: 0,
    transactions: [],
    page: 1,
    hasMore: true,
    loading: false
  },

  onLoad() {
    this.loadBalance();
    this.loadTransactions();
  },

  onShow() {
    this.loadBalance();
  },

  async loadBalance() {
    try {
      const result = await api.getWalletBalance();
      this.setData({ balance: result.balance || 0 });
    } catch (e) {
      console.error('Failed to load balance', e);
    }
  },

  async loadTransactions() {
    if (this.data.loading) return;
    this.setData({ loading: true });

    try {
      const result = await api.getWalletTransactions({ page: 1, pageSize: 20 });
      this.setData({
        transactions: result || [],
        page: 1,
        hasMore: (result || []).length === 20
      });
    } catch (e) {
      console.error('Failed to load transactions', e);
    }

    this.setData({ loading: false });
  },

  async loadMore() {
    if (this.data.loading || !this.data.hasMore) return;
    this.setData({ loading: true });

    try {
      const nextPage = this.data.page + 1;
      const result = await api.getWalletTransactions({ page: nextPage, pageSize: 20 });
      this.setData({
        transactions: this.data.transactions.concat(result || []),
        page: nextPage,
        hasMore: (result || []).length === 20
      });
    } catch (e) {
      console.error('Failed to load more', e);
    }

    this.setData({ loading: false });
  },

  showRecharge() {
    wx.showModal({
      title: '充值',
      editable: true,
      placeholderText: '请输入充值金额',
      success: async (res) => {
        if (res.confirm && res.content) {
          const amount = parseFloat(res.content);
          if (isNaN(amount) || amount <= 0) {
            wx.showToast({ title: '请输入有效金额', icon: 'error' });
            return;
          }
          try {
            await api.rechargeWallet({ amount });
            this.loadBalance();
            wx.showToast({ title: '充值成功', icon: 'success' });
          } catch (e) {
            wx.showToast({ title: '充值失败', icon: 'error' });
          }
        }
      }
    });
  }
});
