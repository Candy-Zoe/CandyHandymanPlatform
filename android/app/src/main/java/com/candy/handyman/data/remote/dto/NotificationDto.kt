package com.candy.handyman.data.remote.dto

data class NotificationDto(
    val id: String,
    val title: String,
    val content: String,
    val type: String,
    val isRead: Boolean,
    val relatedId: String?,
    val relatedType: String?,
    val image: String?,
    val createdAt: String
)

data class PagedNotificationResult(
    val items: List<NotificationDto>,
    val total: Int,
    val page: Int,
    val pageSize: Int
)

data class NotificationSettingDto(
    val type: String,
    val enabled: Boolean
)

data class UnreadCountDto(
    val count: Int
)
