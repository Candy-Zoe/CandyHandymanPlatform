package com.candy.handyman.data.local

import android.content.Context
import android.content.SharedPreferences
import dagger.hilt.android.qualifiers.ApplicationContext
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class AppPreferences @Inject constructor(
    @ApplicationContext context: Context
) {
    private val prefs: SharedPreferences = context.getSharedPreferences("candy_handyman", Context.MODE_PRIVATE)

    fun saveToken(token: String) = prefs.edit().putString("token", token).apply()

    fun getToken(): String? = prefs.getString("token", null)

    fun saveRefreshToken(token: String) = prefs.edit().putString("refresh_token", token).apply()

    fun getRefreshToken(): String? = prefs.getString("refresh_token", null)

    fun saveUserId(id: String) = prefs.edit().putString("user_id", id).apply()

    fun getUserId(): String? = prefs.getString("user_id", null)

    fun clearAll() = prefs.edit().clear().apply()

    fun isLoggedIn(): Boolean = getToken() != null
}