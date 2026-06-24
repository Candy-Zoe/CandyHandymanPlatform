package com.candy.handyman.data.remote.dto

data class AuthResponseDto(
    val token: String,
    val refreshToken: String,
    val expiresAt: String,
    val user: UserDto
)

data class UserDto(
    val id: String,
    val nickname: String,
    val phone: String,
    val avatar: String?,
    val role: String,
    val balance: Double,
    val bio: String?
)

data class RegisterDto(
    val nickname: String,
    val phone: String,
    val password: String,
    val bio: String? = null
)

data class LoginDto(
    val phone: String,
    val password: String
)