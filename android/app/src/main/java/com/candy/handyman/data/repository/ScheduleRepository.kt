package com.candy.handyman.data.repository

import com.candy.handyman.data.remote.FeatureApiService
import com.candy.handyman.data.remote.dto.AppointmentSlotDto
import com.candy.handyman.data.remote.dto.HandymanRankingDto
import com.candy.handyman.data.remote.dto.ScheduleDto
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class ScheduleRepository @Inject constructor(
    private val api: FeatureApiService
) {
    suspend fun getSchedules(handymanId: String): Result<List<ScheduleDto>> {
        return try {
            val response = api.getSchedules(handymanId)
            if (response.isSuccessful) Result.success(response.body()!!)
            else Result.failure(Exception("获取排班失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun updateSchedules(schedules: List<ScheduleDto>): Result<Unit> {
        return try {
            val response = api.updateSchedules(schedules)
            if (response.isSuccessful) Result.success(Unit)
            else Result.failure(Exception("更新排班失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun getAvailableSlots(handymanId: String, date: String): Result<List<AppointmentSlotDto>> {
        return try {
            val response = api.getAvailableSlots(handymanId, date)
            if (response.isSuccessful) Result.success(response.body()!!)
            else Result.failure(Exception("获取时段失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun generateSlots(handymanId: String, days: Int = 14): Result<Unit> {
        return try {
            val response = api.generateSlots(handymanId, days)
            if (response.isSuccessful) Result.success(Unit)
            else Result.failure(Exception("生成时段失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun getRanking(categoryId: String? = null, top: Int = 20): Result<List<HandymanRankingDto>> {
        return try {
            val response = api.getHandymenRanking(categoryId, top)
            if (response.isSuccessful) Result.success(response.body()!!)
            else Result.failure(Exception("获取排行失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
}
