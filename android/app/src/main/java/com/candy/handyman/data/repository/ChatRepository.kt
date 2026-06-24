package com.candy.handyman.data.repository

import com.candy.handyman.data.remote.ApiService
import com.candy.handyman.data.remote.dto.*
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class ChatRepository @Inject constructor(
    private val api: ApiService
) {
    suspend fun getConversations(): Result<List<ConversationDto>> {
        return try {
            val response = api.getConversations()
            if (response.isSuccessful) Result.success(response.body() ?: emptyList())
            else Result.failure(Exception("获取会话列表失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun getMessages(conversationId: String): Result<List<MessageDto>> {
        return try {
            val response = api.getMessages(conversationId)
            if (response.isSuccessful) Result.success(response.body() ?: emptyList())
            else Result.failure(Exception("获取消息列表失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun sendMessage(dto: SendMessageDto): Result<MessageDto> {
        return try {
            val response = api.sendMessage(dto)
            if (response.isSuccessful) Result.success(response.body()!!)
            else Result.failure(Exception("发送消息失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
}