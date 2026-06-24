package com.candy.handyman.ui.screen.verification

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.candy.handyman.data.remote.FeatureApiService
import com.candy.handyman.data.remote.dto.SubmitVerificationDto
import com.candy.handyman.data.remote.dto.VerificationStatusDto
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class VerificationViewModel @Inject constructor(
    private val featureApi: FeatureApiService
) : ViewModel() {

    private val _status = MutableStateFlow<VerificationStatusDto?>(null)
    val status = _status.asStateFlow()

    private val _isLoading = MutableStateFlow(false)
    val isLoading = _isLoading.asStateFlow()

    private val _message = MutableStateFlow<String?>(null)
    val message = _message.asStateFlow()

    fun loadStatus() {
        viewModelScope.launch {
            try {
                val response = featureApi.getVerificationStatus()
                if (response.isSuccessful) {
                    _status.value = response.body()
                }
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }
    }

    fun submit(realName: String, idCard: String) {
        viewModelScope.launch {
            _isLoading.value = true
            try {
                val response = featureApi.submitVerification(
                    SubmitVerificationDto(realName, idCard, "", "")
                )
                if (response.isSuccessful) {
                    _message.value = "提交成功，等待审核"
                    loadStatus()
                }
            } catch (e: Exception) {
                _message.value = "提交失败: ${e.message}"
            }
            _isLoading.value = false
        }
    }
}