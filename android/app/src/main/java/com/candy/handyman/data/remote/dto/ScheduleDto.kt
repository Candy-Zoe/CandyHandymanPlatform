package com.candy.handyman.data.remote.dto

data class ScheduleDto(
    val id: String?,
    val dayOfWeek: Int,
    val startTime: String,
    val endTime: String,
    val isAvailable: Boolean
)

data class AppointmentSlotDto(
    val id: String,
    val date: String,
    val startTime: String,
    val endTime: String,
    val isBooked: Boolean
)

data class HandymanRankingDto(
    val handymanId: String,
    val nickname: String,
    val avatar: String?,
    val level: String,
    val averageRating: Double,
    val totalCompletedOrders: Int,
    val totalReviews: Int
)
