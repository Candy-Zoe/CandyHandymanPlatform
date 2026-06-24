package com.candy.handyman.ui.screen.certification

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.candy.handyman.data.remote.FeatureApiService
import com.candy.handyman.data.remote.dto.CraftsmanCertificationDto
import com.candy.handyman.data.remote.dto.SubmitCertificationDto
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class CertificationViewModel @Inject constructor(
    private val featureApi: FeatureApiService
) : ViewModel() {

    private val _certifications = MutableStateFlow<List<CraftsmanCertificationDto>>(emptyList())
    val certifications = _certifications.asStateFlow()

    fun loadCertifications() {
        viewModelScope.launch {
            try {
                val response = featureApi.getMyCertifications()
                if (response.isSuccessful) {
                    _certifications.value = response.body() ?: emptyList()
                }
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }
    }

    fun submitCertification(skillName: String, certName: String, certNo: String) {
        viewModelScope.launch {
            try {
                featureApi.submitCertification(
                    SubmitCertificationDto(skillName, certName, certNo, "")
                )
                loadCertifications()
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }
    }
}