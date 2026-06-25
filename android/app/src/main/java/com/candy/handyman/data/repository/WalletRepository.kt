package com.candy.handyman.data.repository

import com.candy.handyman.data.remote.FeatureApiService
import com.candy.handyman.data.remote.dto.CouponValidationResult
import com.candy.handyman.data.remote.dto.UserCouponDto
import com.candy.handyman.data.remote.dto.WalletBalanceDto
import com.candy.handyman.data.remote.dto.WalletTransactionDto
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class WalletRepository @Inject constructor(
    private val api: FeatureApiService
) {
    suspend fun getBalance(): Result<WalletBalanceDto> {
        return try {
            val response = api.getWalletBalance()
            if (response.isSuccessful) Result.success(response.body()!!)
            else Result.failure(Exception("获取余额失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun recharge(amount: Double): Result<Map<String, Any>> {
        return try {
            val response = api.rechargeWallet(mapOf("amount" to amount))
            if (response.isSuccessful) Result.success(response.body()!!)
            else Result.failure(Exception("充值失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun withdraw(amount: Double): Result<Unit> {
        return try {
            val response = api.withdrawWallet(mapOf("amount" to amount))
            if (response.isSuccessful) Result.success(Unit)
            else Result.failure(Exception("提现失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun getTransactions(page: Int = 1, pageSize: Int = 20): Result<List<WalletTransactionDto>> {
        return try {
            val response = api.getWalletTransactions(page, pageSize)
            if (response.isSuccessful) Result.success(response.body()!!)
            else Result.failure(Exception("获取交易记录失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun validateCoupon(code: String, orderAmount: Double): Result<CouponValidationResult> {
        return try {
            val response = api.validateCoupon(mapOf("code" to code, "orderAmount" to orderAmount))
            if (response.isSuccessful) Result.success(response.body()!!)
            else Result.failure(Exception("验证优惠码失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun getMyCoupons(): Result<List<UserCouponDto>> {
        return try {
            val response = api.getMyCoupons()
            if (response.isSuccessful) Result.success(response.body()!!)
            else Result.failure(Exception("获取优惠券失败"))
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
}
