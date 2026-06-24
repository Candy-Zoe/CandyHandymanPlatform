package com.candy.handyman.ui.screen.dispute

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.candy.handyman.data.remote.FeatureApiService
import com.candy.handyman.data.remote.dto.DisputeDto
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class DisputeViewModel @Inject constructor(
    private val featureApi: FeatureApiService
) : ViewModel() {

    private val _disputes = MutableStateFlow<List<DisputeDto>>(emptyList())
    val disputes = _disputes.asStateFlow()

    fun loadDisputes() {
        viewModelScope.launch {
            try {
                val response = featureApi.getMyDisputes()
                if (response.isSuccessful) {
                    _disputes.value = response.body() ?: emptyList()
                }
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }
    }
}