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
}