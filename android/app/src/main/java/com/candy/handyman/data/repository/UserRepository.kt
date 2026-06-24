package com.candy.handyman.data.repository

import com.candy.handyman.data.remote.ApiService
import com.candy.handyman.data.remote.dto.UserDto
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class UserRepository @Inject constructor(
    private val api: ApiService
) {
    suspend fun getMe() = try {
        val response = api.getMe()
        if (response.isSuccessful) Result.success(response.body()!!)
        else Result.failure(Exception("获取用户信息失败"))
    } catch (e: Exception) {
        Result.failure(e)
    }
}