package com.candy.handyman.ui.screen.service

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.candy.handyman.data.remote.dto.ServiceDto
import com.candy.handyman.data.repository.ServiceRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class ServiceDetailViewModel @Inject constructor(
    private val serviceRepository: ServiceRepository
) : ViewModel() {

    private val _service = MutableStateFlow<ServiceDto?>(null)
    val service = _service.asStateFlow()

    fun loadService(id: String) {
        viewModelScope.launch {
            serviceRepository.getServiceById(id).onSuccess { _service.value = it }
        }
    }
}