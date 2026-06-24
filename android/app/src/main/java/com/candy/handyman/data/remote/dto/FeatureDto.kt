package com.candy.handyman.data.remote.dto

data class NearbyHandymanDto(
    val id: String,
    val nickname: String,
    val avatar: String?,
    val latitude: Double?,
    val longitude: Double?,
    val averageRating: Double,
    val totalReviews: Int,
    val isVerified: Boolean,
    val distance: Double
)

data class VerificationStatusDto(
    val status: String,
    val realName: String?,
    val verifiedAt: String?
)

data class SubmitVerificationDto(
    val realName: String,
    val idCardNumber: String,
    val idCardFrontUrl: String,
    val idCardBackUrl: String,
    val facePhotoUrl: String? = null
)

data class CraftsmanCertificationDto(
    val id: String,
    val skillName: String,
    val certificateName: String,
    val certificateNo: String,
    val status: String,
    val rejectReason: String?
)

data class SubmitCertificationDto(
    val skillName: String,
    val certificateName: String,
    val certificateNo: String,
    val certificateUrl: String,
    val issuingAuthority: String? = null,
    val issueDate: String? = null,
    val expiryDate: String? = null
)

data class InsurancePolicyDto(
    val id: String,
    val policyNo: String,
    val type: String,
    val premium: Double,
    val coverageAmount: Double,
    val status: String,
    val endDate: String
)

data class PurchaseInsuranceDto(
    val orderId: String,
    val type: String
)

data class DisputeDto(
    val id: String,
    val orderId: String,
    val reason: String,
    val description: String?,
    val status: String,
    val resolution: String?,
    val createdAt: String,
    val resolvedAt: String?
)

data class CreateDisputeDto(
    val orderId: String,
    val reason: String,
    val description: String? = null,
    val evidenceUrls: String? = null
)

data class PaymentHistoryDto(
    val id: String,
    val transactionId: String,
    val amount: Double,
    val paymentMethod: String,
    val status: String,
    val createdAt: String
)