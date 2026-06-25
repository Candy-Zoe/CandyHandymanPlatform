package com.candy.handyman.data.repository

import com.candy.handyman.data.remote.FeatureApiService
import com.candy.handyman.data.remote.dto.NotificationDto
import com.candy.handyman.data.remote.dto.NotificationSettingDto
import com.candy.handyman.data.remote.dto.PagedNotificationResult
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class NotificationRepository @Inject constructor(
    private val api: FeatureApiService
) {
    suspend fun getNotifications(
        type: String? = null,
        isRead: Boolean? = null,
        page: Int = 1,
        pageSize: Int = 20
    ): Result<PagedNotificationResult> {
        return try {
            val response = api.getNotifications(type, isRead, page, pageSize)
            if (response.isSuccessful) Result.success(response.body()!!)
            else Result.failure(Exception("获取通知失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun getUnreadCount(): Result<Int> {
        return try {
            val response = api.getUnreadCount()
            if (response.isSuccessful) Result.success(response.body()!!.count)
            else Result.failure(Exception("获取未读数失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun markAsRead(id: String): Result<Unit> {
        return try {
            val response = api.markAsRead(id)
            if (response.isSuccessful) Result.success(Unit)
            else Result.failure(Exception("标记已读失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun markAllAsRead(): Result<Unit> {
        return try {
            val response = api.markAllAsRead()
            if (response.isSuccessful) Result.success(Unit)
            else Result.failure(Exception("全部已读失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun deleteNotification(id: String): Result<Unit> {
        return try {
            val response = api.deleteNotification(id)
            if (response.isSuccessful) Result.success(Unit)
            else Result.failure(Exception("删除通知失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun getSettings(): Result<List<NotificationSettingDto>> {
        return try {
            val response = api.getNotificationSettings()
            if (response.isSuccessful) Result.success(response.body()!!)
            else Result.failure(Exception("获取设置失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun updateSettings(settings: List<NotificationSettingDto>): Result<Unit> {
        return try {
            val response = api.updateNotificationSettings(settings)
            if (response.isSuccessful) Result.success(Unit)
            else Result.failure(Exception("更新设置失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
}
