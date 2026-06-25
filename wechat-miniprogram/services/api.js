const api = require('../utils/request');

module.exports = {
  // Auth
  register: (data) => api.post('/api/auth/register', data),
  login: (data) => api.post('/api/auth/login', data),

  // Services
  getServices: (params) => api.get('/api/services', params),
  getServiceById: (id) => api.get(`/api/services/${id}`),
  createService: (data) => api.post('/api/services', data),

  // Categories
  getCategories: () => api.get('/api/categories'),

  // Orders
  createOrder: (data) => api.post('/api/orders', data),
  getOrders: (status) => api.get('/api/orders', { status }),
  getOrderById: (id) => api.get(`/api/orders/${id}`),
  acceptOrder: (id) => api.put(`/api/orders/${id}/accept`),
  startOrder: (id) => api.put(`/api/orders/${id}/start`),
  completeOrder: (id) => api.put(`/api/orders/${id}/complete`),
  cancelOrder: (id) => api.put(`/api/orders/${id}/cancel`),

  // Chat
  getConversations: () => api.get('/api/chat/conversations'),
  getMessages: (id) => api.get(`/api/chat/conversations/${id}/messages`),
  sendMessage: (data) => api.post('/api/chat/messages', data),

  // Reviews
  createReview: (data) => api.post('/api/reviews', data),
  getReviews: (handymanId) => api.get(`/api/reviews/handyman/${handymanId}`),

  // Users
  getMe: () => api.get('/api/users/me'),
  updateMe: (data) => api.put('/api/users/me', data),

  // Nearby
  getNearby: (params) => api.get('/api/nearby', params),

  // Verification
  submitVerification: (data) => api.post('/api/verification', data),
  getVerificationStatus: () => api.get('/api/verification/status'),

  // Certifications
  submitCertification: (data) => api.post('/api/certifications', data),
  getMyCertifications: () => api.get('/api/certifications/my'),

  // Insurance
  purchaseInsurance: (data) => api.post('/api/insurance', data),
  getOrderInsurance: (orderId) => api.get(`/api/insurance/order/${orderId}`),

  // Disputes
  createDispute: (data) => api.post('/api/disputes', data),
  getMyDisputes: () => api.get('/api/disputes/my'),

  // Payments
  createPayment: (data) => api.post('/api/payments/create', data),
  getPaymentHistory: () => api.get('/api/payments/history'),

  // Notifications
  getNotifications: (params) => api.get('/api/notifications', params),
  getUnreadCount: () => api.get('/api/notifications/unread-count'),
  markAsRead: (id) => api.put(`/api/notifications/${id}/read`),
  markAllAsRead: () => api.put('/api/notifications/read-all'),
  deleteNotification: (id) => api.delete(`/api/notifications/${id}`),
  getNotificationSettings: () => api.get('/api/notifications/settings'),
  updateNotificationSettings: (settings) => api.put('/api/notifications/settings', settings),

  // Schedule
  getSchedules: (handymanId) => api.get(`/api/schedule/${handymanId}`),
  updateSchedules: (schedules) => api.put('/api/schedule', schedules),
  getAvailableSlots: (handymanId, date) => api.get(`/api/schedule/${handymanId}/slots`, { date }),
  generateSlots: (handymanId, days) => api.post(`/api/schedule/${handymanId}/slots/generate`, null, { days }),

  // Rankings
  getHandymenRanking: (params) => api.get('/api/rankings/handymen', params),

  // Wallet
  getWalletBalance: () => api.get('/api/wallet/balance'),
  rechargeWallet: (data) => api.post('/api/wallet/recharge', data),
  withdrawWallet: (data) => api.post('/api/wallet/withdraw', data),
  getWalletTransactions: (params) => api.get('/api/wallet/transactions', params),

  // Payment Enhancement
  createWechatPayment: (data) => api.post('/api/payments/wechat/create', data),
  refundPayment: (data) => api.post('/api/payments/refund', data),
  walletPay: (data) => api.post('/api/payments/wallet/pay', data),

  // Coupons
  validateCoupon: (data) => api.post('/api/coupons/validate', data),
  getMyCoupons: () => api.get('/api/coupons/my'),

  // Favorites
  addFavorite: (data) => api.post('/api/favorites', data),
  removeFavorite: (data) => api.delete('/api/favorites', data),
  getMyFavorites: () => api.get('/api/favorites'),
  checkFavorite: (params) => api.get('/api/favorites/check', params),

  // Browsing History
  recordHistory: (data) => api.post('/api/browsinghistory', data),
  getMyHistory: (params) => api.get('/api/browsinghistory', params),
  clearHistory: () => api.delete('/api/browsinghistory'),

  // Order Templates
  createTemplate: (data) => api.post('/api/ordertemplates', data),
  getMyTemplates: () => api.get('/api/ordertemplates'),
  updateTemplate: (id, data) => api.put(`/api/ordertemplates/${id}`, data),
  deleteTemplate: (id) => api.delete(`/api/ordertemplates/${id}`),

  // Announcements
  getAnnouncements: () => api.get('/api/announcements'),

  // Feedback
  submitFeedback: (data) => api.post('/api/feedbacks', data),
  getMyFeedbacks: () => api.get('/api/feedbacks/my'),

  // Help
  getHelpTopics: () => api.get('/api/help'),
  getHelpTopic: (id) => api.get(`/api/help/${id}`),

  // Admin Stats
  getDailyStats: (days) => api.get('/api/admin/stats/daily', { days }),
  getOverviewStats: () => api.get('/api/admin/stats/overview'),
  getTopServices: (top) => api.get('/api/admin/stats/top-services', { top })
};