package com.candy.handyman.data.repository

import com.candy.handyman.data.remote.ApiService
import com.candy.handyman.data.remote.dto.*
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class ServiceRepository @Inject constructor(
    private val api: ApiService
) {
    suspend fun getServices(categoryId: String? = null, keyword: String? = null): Result<List<ServiceDto>> {
        return try {
            val response = api.getServices(categoryId = categoryId, keyword = keyword)
            if (response.isSuccessful) Result.success(response.body() ?: emptyList())
            else Result.failure(Exception("获取服务列表失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun getServiceById(id: String): Result<ServiceDto> {
        return try {
            val response = api.getServiceById(id)
            if (response.isSuccessful) Result.success(response.body()!!)
            else Result.failure(Exception("获取服务详情失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun createService(dto: CreateServiceDto): Result<ServiceDto> {
        return try {
            val response = api.createService(dto)
            if (response.isSuccessful) Result.success(response.body()!!)
            else Result.failure(Exception("发布服务失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
}