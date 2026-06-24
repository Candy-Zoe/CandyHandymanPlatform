package com.candy.handyman.ui.screen.payment

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.candy.handyman.data.remote.FeatureApiService
import com.candy.handyman.data.remote.dto.PaymentHistoryDto
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class PaymentViewModel @Inject constructor(
    private val featureApi: FeatureApiService
) : ViewModel() {

    private val _payments = MutableStateFlow<List<PaymentHistoryDto>>(emptyList())
    val payments = _payments.asStateFlow()

    fun loadHistory() {
        viewModelScope.launch {
            try {
                val response = featureApi.getPaymentHistory()
                if (response.isSuccessful) {
                    _payments.value = response.body() ?: emptyList()
                }
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }
    }
}