package com.candy.handyman.data.remote.dto

data class OrderDto(
    val id: String,
    val orderNo: String,
    val service: ServiceDto? = null,
    val provider: HandymanDto? = null,
    val customer: UserDto? = null,
    val quantity: Int,
    val unitPrice: Double,
    val totalAmount: Double,
    val status: String,
    val address: String,
    val contactPhone: String,
    val description: String?,
    val scheduledAt: String?,
    val createdAt: String,
    val completedAt: String?,
    val review: ReviewDto? = null
)

data class CreateOrderDto(
    val serviceId: String,
    val quantity: Int = 1,
    val address: String,
    val contactPhone: String,
    val description: String? = null,
    val scheduledAt: String? = null
)

data class ReviewDto(
    val id: String,
    val rating: Int,
    val content: String,
    val customer: UserDto? = null,
    val createdAt: String
)

data class CreateReviewDto(
    val orderId: String,
    val rating: Int,
    val content: String
)