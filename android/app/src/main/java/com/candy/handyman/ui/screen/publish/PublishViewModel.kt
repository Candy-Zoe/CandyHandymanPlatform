package com.candy.handyman.ui.screen.publish

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import androidx.navigation.NavController
import com.candy.handyman.data.remote.dto.CreateServiceDto
import com.candy.handyman.data.repository.ServiceRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class PublishViewModel @Inject constructor(
    private val serviceRepository: ServiceRepository
) : ViewModel() {
    private val _isLoading = MutableStateFlow(false)
    val isLoading = _isLoading.asStateFlow()
    private val _error = MutableStateFlow<String?>(null)
    val error = _error.asStateFlow()

    fun publish(dto: CreateServiceDto, navController: NavController) {
        viewModelScope.launch {
            _isLoading.value = true
            serviceRepository.createService(dto).onSuccess { navController.popBackStack() }.onFailure { _error.value = it.message }
            _isLoading.value = false
        }
    }
}