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
  getPaymentHistory: () => api.get('/api/payments/history')
};