const api = require('../../services/api');

Page({
  data: { topics: [], selectedTopic: null },
  onLoad() { this.loadTopics(); },

  async loadTopics() {
    try {
      const topics = await api.getHelpTopics();
      this.setData({ topics: topics || [] });
    } catch (e) { console.error(e); }
  },

  selectTopic(e) {
    const topic = this.data.topics.find(t => t.id === e.currentTarget.dataset.id);
    this.setData({ selectedTopic: topic });
  },

  goBack() { this.setData({ selectedTopic: null }); }
});
