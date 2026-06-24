package com.candy.handyman.ui.screen.insurance

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.candy.handyman.data.remote.FeatureApiService
import com.candy.handyman.data.remote.dto.PurchaseInsuranceDto
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class InsuranceViewModel @Inject constructor(
    private val featureApi: FeatureApiService
) : ViewModel() {

    private val _insuranceInfo = MutableStateFlow<Map<String, Any>?>(null)
    val insuranceInfo = _insuranceInfo.asStateFlow()

    private val _isLoading = MutableStateFlow(false)
    val isLoading = _isLoading.asStateFlow()

    private val _message = MutableStateFlow<String?>(null)
    val message = _message.asStateFlow()

    fun checkInsurance(orderId: String) {
        viewModelScope.launch {
            try {
                val response = featureApi.getOrderInsurance(orderId)
                if (response.isSuccessful) {
                    _insuranceInfo.value = response.body()
                }
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }
    }

    fun purchase(orderId: String, type: String) {
        viewModelScope.launch {
            _isLoading.value = true
            try {
                val response = featureApi.purchaseInsurance(PurchaseInsuranceDto(orderId, type))
                if (response.isSuccessful) {
                    _message.value = "购买成功"
                    checkInsurance(orderId)
                }
            } catch (e: Exception) {
                _message.value = "购买失败"
            }
            _isLoading.value = false
        }
    }
}