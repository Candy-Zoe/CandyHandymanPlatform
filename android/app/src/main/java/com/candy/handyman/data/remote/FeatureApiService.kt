package com.candy.handyman.data.remote

import com.candy.handyman.data.remote.dto.*
import retrofit2.Response
import retrofit2.http.*

interface FeatureApiService {

    @GET("api/nearby")
    suspend fun getNearbyHandymen(
        @Query("latitude") latitude: Double,
        @Query("longitude") longitude: Double,
        @Query("radiusKm") radiusKm: Double = 10.0,
        @Query("categoryId") categoryId: String? = null
    ): Response<List<NearbyHandymanDto>>

    @POST("api/verification")
    suspend fun submitVerification(@Body dto: SubmitVerificationDto): Response<Map<String, String>>

    @GET("api/verification/status")
    suspend fun getVerificationStatus(): Response<VerificationStatusDto>

    @POST("api/certifications")
    suspend fun submitCertification(@Body dto: SubmitCertificationDto): Response<Map<String, String>>

    @GET("api/certifications/my")
    suspend fun getMyCertifications(): Response<List<CraftsmanCertificationDto>>

    @POST("api/insurance")
    suspend fun purchaseInsurance(@Body dto: PurchaseInsuranceDto): Response<InsurancePolicyDto>

    @GET("api/insurance/order/{orderId}")
    suspend fun getOrderInsurance(@Path("orderId") orderId: String): Response<Map<String, Any>>

    @POST("api/disputes")
    suspend fun createDispute(@Body dto: CreateDisputeDto): Response<Map<String, String>>

    @GET("api/disputes/my")
    suspend fun getMyDisputes(): Response<List<DisputeDto>>

    @GET("api/disputes/{id}")
    suspend fun getDisputeById(@Path("id") id: String): Response<DisputeDto>

    @POST("api/payments/create")
    suspend fun createPayment(@Body dto: Map<String, String>): Response<Map<String, Any>>

    @GET("api/payments/history")
    suspend fun getPaymentHistory(): Response<List<PaymentHistoryDto>>

    @GET("api/notifications")
    suspend fun getNotifications(
        @Query("type") type: String? = null,
        @Query("isRead") isRead: Boolean? = null,
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 20
    ): Response<PagedNotificationResult>

    @GET("api/notifications/unread-count")
    suspend fun getUnreadCount(): Response<UnreadCountDto>

    @PUT("api/notifications/{id}/read")
    suspend fun markAsRead(@Path("id") id: String): Response<Unit>

    @PUT("api/notifications/read-all")
    suspend fun markAllAsRead(): Response<Unit>

    @DELETE("api/notifications/{id}")
    suspend fun deleteNotification(@Path("id") id: String): Response<Unit>

    @GET("api/notifications/settings")
    suspend fun getNotificationSettings(): Response<List<NotificationSettingDto>>

    @PUT("api/notifications/settings")
    suspend fun updateNotificationSettings(@Body settings: List<NotificationSettingDto>): Response<Unit>

    @POST("api/notifications/fcm-token")
    suspend fun registerFcmToken(@Body token: Map<String, String>): Response<Unit>

    @GET("api/schedule/{handymanId}")
    suspend fun getSchedules(@Path("handymanId") handymanId: String): Response<List<ScheduleDto>>

    @PUT("api/schedule")
    suspend fun updateSchedules(@Body schedules: List<ScheduleDto>): Response<Unit>

    @GET("api/schedule/{handymanId}/slots")
    suspend fun getAvailableSlots(
        @Path("handymanId") handymanId: String,
        @Query("date") date: String
    ): Response<List<AppointmentSlotDto>>

    @POST("api/schedule/{handymanId}/slots/generate")
    suspend fun generateSlots(
        @Path("handymanId") handymanId: String,
        @Query("days") days: Int = 14
    ): Response<Map<String, String>>

    @GET("api/rankings/handymen")
    suspend fun getHandymenRanking(
        @Query("categoryId") categoryId: String? = null,
        @Query("top") top: Int = 20
    ): Response<List<HandymanRankingDto>>

    @GET("api/wallet/balance")
    suspend fun getWalletBalance(): Response<WalletBalanceDto>

    @POST("api/wallet/recharge")
    suspend fun rechargeWallet(@Body dto: Map<String, Double>): Response<Map<String, Any>>

    @POST("api/wallet/withdraw")
    suspend fun withdrawWallet(@Body dto: Map<String, Double>): Response<Map<String, String>>

    @GET("api/wallet/transactions")
    suspend fun getWalletTransactions(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 20
    ): Response<List<WalletTransactionDto>>

    @POST("api/payments/wechat/create")
    suspend fun createWechatPayment(@Body dto: Map<String, String>): Response<Map<String, Any>>

    @POST("api/payments/refund")
    suspend fun refundPayment(@Body dto: Map<String, Any>): Response<Map<String, Any>>

    @POST("api/payments/wallet/pay")
    suspend fun walletPay(@Body dto: Map<String, Any>): Response<Map<String, Any>>

    @POST("api/coupons/validate")
    suspend fun validateCoupon(@Body dto: Map<String, Any>): Response<CouponValidationResult>

    @GET("api/coupons/my")
    suspend fun getMyCoupons(): Response<List<UserCouponDto>>

    @POST("api/favorites")
    suspend fun addFavorite(@Body dto: Map<String, String?>): Response<Unit>

    @HTTP(method = "DELETE", path = "api/favorites", hasBody = true)
    suspend fun removeFavorite(@Body dto: Map<String, String?>): Response<Unit>

    @GET("api/favorites")
    suspend fun getMyFavorites(): Response<List<Map<String, Any>>>

    @GET("api/favorites/check")
    suspend fun checkFavorite(@Query("serviceId") serviceId: String? = null, @Query("handymanId") handymanId: String? = null): Response<Map<String, Boolean>>

    @POST("api/browsinghistory")
    suspend fun recordHistory(@Body dto: Map<String, String?>): Response<Unit>

    @GET("api/browsinghistory")
    suspend fun getMyHistory(@Query("limit") limit: Int = 50): Response<List<Map<String, Any>>>

    @HTTP(method = "DELETE", path = "api/browsinghistory", hasBody = false)
    suspend fun clearHistory(): Response<Unit>

    @POST("api/ordertemplates")
    suspend fun createTemplate(@Body dto: Map<String, Any>): Response<Map<String, Any>>

    @GET("api/ordertemplates")
    suspend fun getMyTemplates(): Response<List<Map<String, Any>>>

    @PUT("api/ordertemplates/{id}")
    suspend fun updateTemplate(@Path("id") id: String, @Body dto: Map<String, Any>): Response<Unit>

    @DELETE("api/ordertemplates/{id}")
    suspend fun deleteTemplate(@Path("id") id: String): Response<Unit>

    @GET("api/announcements")
    suspend fun getAnnouncements(): Response<List<Map<String, Any>>>

    @POST("api/feedbacks")
    suspend fun submitFeedback(@Body dto: Map<String, String>): Response<Map<String, Any>>

    @GET("api/feedbacks/my")
    suspend fun getMyFeedbacks(): Response<List<Map<String, Any>>>

    @GET("api/help")
    suspend fun getHelpTopics(): Response<List<Map<String, Any>>>

    @GET("api/help/{id}")
    suspend fun getHelpTopic(@Path("id") id: String): Response<Map<String, Any>>

    @GET("api/admin/stats/daily")
    suspend fun getDailyStats(@Query("days") days: Int = 7): Response<List<Map<String, Any>>>

    @GET("api/admin/stats/overview")
    suspend fun getOverviewStats(): Response<Map<String, Any>>

    @GET("api/admin/stats/top-services")
    suspend fun getTopServices(@Query("top") top: Int = 10): Response<List<Map<String, Any>>>
}