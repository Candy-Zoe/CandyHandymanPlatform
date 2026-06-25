package com.candy.handyman.data.remote.dto

data class CouponDto(
    val id: String,
    val code: String,
    val name: String,
    val type: String,
    val discountValue: Double,
    val minAmount: Double,
    val maxUses: Int,
    val usedCount: Int,
    val expiresAt: String?,
    val isActive: Boolean
)

data class CreateCouponDto(
    val code: String,
    val name: String,
    val type: String,
    val discountValue: Double,
    val minAmount: Double,
    val maxUses: Int,
    val expiresAt: String? = null
)

data class CouponValidationResult(
    val isValid: Boolean,
    val couponId: String?,
    val discountAmount: Double,
    val message: String
)

data class UserCouponDto(
    val id: String,
    val code: String,
    val name: String,
    val type: String,
    val discountValue: Double,
    val minAmount: Double,
    val expiresAt: String?,
    val isUsed: Boolean,
    val usedAt: String?
)

data class WalletTransactionDto(
    val id: String,
    val amount: Double,
    val type: String,
    val balance: Double,
    val description: String?,
    val createdAt: String
)

data class WalletBalanceDto(
    val balance: Double
)
