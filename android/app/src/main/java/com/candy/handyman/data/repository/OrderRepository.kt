package com.candy.handyman.data.repository

import com.candy.handyman.data.remote.ApiService
import com.candy.handyman.data.remote.dto.*
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class OrderRepository @Inject constructor(
    private val api: ApiService
) {
    suspend fun createOrder(dto: CreateOrderDto): Result<OrderDto> {
        return try {
            val response = api.createOrder(dto)
            if (response.isSuccessful) Result.success(response.body()!!)
            else Result.failure(Exception("创建订单失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun getOrderById(id: String): Result<OrderDto> {
        return try {
            val response = api.getOrderById(id)
            if (response.isSuccessful) Result.success(response.body()!!)
            else Result.failure(Exception("获取订单详情失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun getOrders(status: String? = null): Result<List<OrderDto>> {
        return try {
            val response = api.getOrders(status)
            if (response.isSuccessful) Result.success(response.body() ?: emptyList())
            else Result.failure(Exception("获取订单列表失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun acceptOrder(id: String) = try { api.acceptOrder(id); Result.success(Unit) } catch (e: Exception) { Result.failure(e) }
    suspend fun startOrder(id: String) = try { api.startOrder(id); Result.success(Unit) } catch (e: Exception) { Result.failure(e) }
    suspend fun completeOrder(id: String) = try { api.completeOrder(id); Result.success(Unit) } catch (e: Exception) { Result.failure(e) }
    suspend fun cancelOrder(id: String) = try { api.cancelOrder(id); Result.success(Unit) } catch (e: Exception) { Result.failure(e) }
}