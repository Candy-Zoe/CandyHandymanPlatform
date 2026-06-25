const api = require('../../services/api');

Page({
  data: { announcements: [] },
  onLoad() { this.loadAnnouncements(); },

  async loadAnnouncements() {
    try {
      const announcements = await api.getAnnouncements();
      this.setData({ announcements: announcements || [] });
    } catch (e) { console.error(e); }
  }
});
