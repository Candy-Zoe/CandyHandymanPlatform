package com.candy.handyman.ui.screen.order

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import androidx.navigation.NavController
import com.candy.handyman.data.remote.dto.CreateOrderDto
import com.candy.handyman.data.repository.OrderRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class CreateOrderViewModel @Inject constructor(
    private val orderRepository: OrderRepository
) : ViewModel() {
    private val _isLoading = MutableStateFlow(false)
    val isLoading = _isLoading.asStateFlow()
    private val _error = MutableStateFlow<String?>(null)
    val error = _error.asStateFlow()

    fun createOrder(serviceId: String, quantity: Int, address: String, phone: String, description: String, navController: NavController) {
        viewModelScope.launch {
            _isLoading.value = true
            orderRepository.createOrder(CreateOrderDto(serviceId, quantity, address, phone, description.ifEmpty { null }))
                .onSuccess { navController.popBackStack() }
                .onFailure { _error.value = it.message }
            _isLoading.value = false
        }
    }
}