const api = require('../../services/api');
Page({
  data: { disputes: [] },
  onLoad() { this.loadDisputes(); },
  async loadDisputes() {
    try {
      const disputes = await api.getMyDisputes();
      const formatted = disputes.map(d => ({
        ...d,
        statusText: d.status === 'Open' ? '待处理' : d.status === 'UnderReview' ? '审核中' : d.status === 'Resolved' ? '已解决' : '已拒绝'
      }));
      this.setData({ disputes: formatted });
    } catch (e) {}
  }
})