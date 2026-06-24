const api = require('../../services/api');
Page({
  data: { payments: [] },
  onLoad() { this.loadHistory(); },
  async loadHistory() {
    try {
      const payments = await api.getPaymentHistory();
      const formatted = payments.map(p => ({
        ...p,
        statusText: p.status === 'Paid' ? '已支付' : '待支付'
      }));
      this.setData({ payments: formatted });
    } catch (e) {}
  }
})