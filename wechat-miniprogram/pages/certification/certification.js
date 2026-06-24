const api = require('../../services/api');
Page({
  data: { certifications: [] },
  onLoad() { this.loadCerts(); },
  async loadCerts() {
    try {
      const certs = await api.getMyCertifications();
      const formatted = certs.map(c => ({
        ...c,
        statusText: c.status === 'Approved' ? '已通过' : c.status === 'Pending' ? '审核中' : '已拒绝'
      }));
      this.setData({ certifications: formatted });
    } catch (e) {}
  },
  showAddDialog() {
    wx.showModal({
      title: '新增认证',
      editable: true,
      placeholderText: '技能名称',
      success: (res) => {
        if (res.confirm && res.content) {
          api.submitCertification({ skillName: res.content, certificateName: '', certificateNo: '', certificateUrl: '' })
            .then(() => this.loadCerts());
        }
      }
    });
  }
})