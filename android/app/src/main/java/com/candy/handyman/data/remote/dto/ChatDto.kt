package com.candy.handyman.data.remote.dto

data class ConversationDto(
    val id: String,
    val otherUser: UserDto? = null,
    val lastMessage: MessageDto? = null,
    val unreadCount: Int = 0,
    val lastMessageAt: String
)

data class MessageDto(
    val id: String,
    val senderId: String,
    val senderName: String,
    val senderAvatar: String?,
    val content: String,
    val messageType: String,
    val isRead: Boolean,
    val createdAt: String
)

data class SendMessageDto(
    val receiverId: String,
    val content: String,
    val messageType: String = "Text"
)