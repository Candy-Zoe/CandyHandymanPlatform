package com.candy.handyman.data.repository

import com.candy.handyman.data.local.AppPreferences
import com.candy.handyman.data.remote.ApiService
import com.candy.handyman.data.remote.dto.*
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class AuthRepository @Inject constructor(
    private val api: ApiService,
    private val preferences: AppPreferences
) {
    suspend fun register(dto: RegisterDto): Result<AuthResponseDto> {
        return try {
            val response = api.register(dto)
            if (response.isSuccessful) {
                val body = response.body()!!
                preferences.saveToken(body.token)
                preferences.saveRefreshToken(body.refreshToken)
                preferences.saveUserId(body.user.id)
                Result.success(body)
            } else {
                Result.failure(Exception("注册失败"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun login(dto: LoginDto): Result<AuthResponseDto> {
        return try {
            val response = api.login(dto)
            if (response.isSuccessful) {
                val body = response.body()!!
                preferences.saveToken(body.token)
                preferences.saveRefreshToken(body.refreshToken)
                preferences.saveUserId(body.user.id)
                Result.success(body)
            } else {
                Result.failure(Exception("登录失败"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    fun logout() = preferences.clearAll()

    fun isLoggedIn() = preferences.isLoggedIn()
}