package com.candy.handyman.data.remote.dto

data class ServiceDto(
    val id: String,
    val title: String,
    val description: String,
    val pricingType: String,
    val price: Double,
    val unit: String,
    val status: String,
    val viewCount: Int,
    val categoryId: String,
    val categoryName: String? = null,
    val handyman: HandymanDto? = null,
    val media: List<ServiceMediaDto> = emptyList(),
    val createdAt: String
)

data class ServiceMediaDto(
    val id: String,
    val mediaType: String,
    val mediaUrl: String,
    val coverUrl: String?,
    val sortOrder: Int
)

data class HandymanDto(
    val id: String,
    val nickname: String,
    val avatar: String?,
    val yearsOfExperience: Int,
    val averageRating: Double,
    val totalReviews: Int,
    val isVerified: Boolean
)

data class CategoryDto(
    val id: String,
    val name: String,
    val icon: String?,
    val parentId: String?,
    val sortOrder: Int,
    val children: List<CategoryDto> = emptyList()
)

data class CreateServiceDto(
    val title: String,
    val description: String,
    val categoryId: String,
    val pricingType: String,
    val price: Double,
    val unit: String
)