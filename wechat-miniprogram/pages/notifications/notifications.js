const api = require('../../services/api');

Page({
  data: {
    notifications: [],
    page: 1,
    hasMore: true,
    loading: false
  },

  onLoad() {
    this.loadNotifications();
  },

  onShow() {
    this.loadNotifications();
  },

  async loadNotifications() {
    if (this.data.loading) return;
    this.setData({ loading: true });

    try {
      const result = await api.getNotifications({ page: 1, pageSize: 20 });
      this.setData({
        notifications: result.items || [],
        page: 1,
        hasMore: (result.items || []).length === 20
      });
    } catch (e) {
      console.error('Failed to load notifications', e);
    }

    this.setData({ loading: false });
  },

  async loadMore() {
    if (this.data.loading || !this.data.hasMore) return;
    this.setData({ loading: true });

    try {
      const nextPage = this.data.page + 1;
      const result = await api.getNotifications({ page: nextPage, pageSize: 20 });
      this.setData({
        notifications: this.data.notifications.concat(result.items || []),
        page: nextPage,
        hasMore: (result.items || []).length === 20
      });
    } catch (e) {
      console.error('Failed to load more', e);
    }

    this.setData({ loading: false });
  },

  async markAsRead(e) {
    const id = e.currentTarget.dataset.id;
    try {
      await api.markAsRead(id);
      const notifications = this.data.notifications.map(n => {
        if (n.id === id) return Object.assign({}, n, { isRead: true });
        return n;
      });
      this.setData({ notifications });
    } catch (e) {
      console.error('Failed to mark as read', e);
    }
  },

  async markAllAsRead() {
    try {
      await api.markAllAsRead();
      const notifications = this.data.notifications.map(n =>
        Object.assign({}, n, { isRead: true })
      );
      this.setData({ notifications });
      wx.showToast({ title: '全部已读', icon: 'success' });
    } catch (e) {
      console.error('Failed to mark all as read', e);
    }
  },

  async deleteNotification(e) {
    const id = e.currentTarget.dataset.id;
    try {
      await api.deleteNotification(id);
      this.setData({
        notifications: this.data.notifications.filter(n => n.id !== id)
      });
    } catch (e) {
      console.error('Failed to delete', e);
    }
  }
});
