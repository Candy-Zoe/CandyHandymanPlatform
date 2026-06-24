package com.candy.handyman.data.remote

import com.candy.handyman.data.remote.dto.*
import retrofit2.Response
import retrofit2.http.*

interface ApiService {

    @POST("api/auth/register")
    suspend fun register(@Body dto: RegisterDto): Response<AuthResponseDto>

    @POST("api/auth/login")
    suspend fun login(@Body dto: LoginDto): Response<AuthResponseDto>

    @GET("api/users/me")
    suspend fun getMe(): Response<UserDto>

    @PUT("api/users/me")
    suspend fun updateMe(@Body dto: Map<String, Any?>): Response<UserDto>

    @GET("api/categories")
    suspend fun getCategories(): Response<List<CategoryDto>>

    @GET("api/services")
    suspend fun getServices(
        @Query("categoryId") categoryId: String? = null,
        @Query("keyword") keyword: String? = null,
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 10
    ): Response<List<ServiceDto>>

    @GET("api/services/{id}")
    suspend fun getServiceById(@Path("id") id: String): Response<ServiceDto>

    @POST("api/services")
    suspend fun createService(@Body dto: CreateServiceDto): Response<ServiceDto>

    @POST("api/orders")
    suspend fun createOrder(@Body dto: CreateOrderDto): Response<OrderDto>

    @GET("api/orders")
    suspend fun getOrders(@Query("status") status: String? = null): Response<List<OrderDto>>

    @GET("api/orders/{id}")
    suspend fun getOrderById(@Path("id") id: String): Response<OrderDto>

    @PUT("api/orders/{id}/accept")
    suspend fun acceptOrder(@Path("id") id: String): Response<Unit>

    @PUT("api/orders/{id}/start")
    suspend fun startOrder(@Path("id") id: String): Response<Unit>

    @PUT("api/orders/{id}/complete")
    suspend fun completeOrder(@Path("id") id: String): Response<Unit>

    @PUT("api/orders/{id}/cancel")
    suspend fun cancelOrder(@Path("id") id: String): Response<Unit>

    @GET("api/chat/conversations")
    suspend fun getConversations(): Response<List<ConversationDto>>

    @GET("api/chat/conversations/{id}/messages")
    suspend fun getMessages(@Path("id") conversationId: String): Response<List<MessageDto>>

    @POST("api/chat/messages")
    suspend fun sendMessage(@Body dto: SendMessageDto): Response<MessageDto>

    @POST("api/reviews")
    suspend fun createReview(@Body dto: CreateReviewDto): Response<ReviewDto>

    @GET("api/reviews/handyman/{handymanId}")
    suspend fun getReviews(@Path("handymanId") handymanId: String): Response<List<ReviewDto>>
}