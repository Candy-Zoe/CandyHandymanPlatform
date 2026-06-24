package com.candy.handyman.ui.screen.order

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.candy.handyman.data.remote.dto.OrderDto
import com.candy.handyman.data.repository.OrderRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class OrderDetailViewModel @Inject constructor(
    private val orderRepository: OrderRepository
) : ViewModel() {
    private val _order = MutableStateFlow<OrderDto?>(null)
    val order = _order.asStateFlow()

    fun loadOrder(id: String) {
        viewModelScope.launch {
            orderRepository.getOrderById(id).onSuccess { _order.value = it }
        }
    }

    fun acceptOrder(id: String) {
        viewModelScope.launch { orderRepository.acceptOrder(id).onSuccess { loadOrder(id) } }
    }

    fun startOrder(id: String) {
        viewModelScope.launch { orderRepository.startOrder(id).onSuccess { loadOrder(id) } }
    }

    fun completeOrder(id: String) {
        viewModelScope.launch { orderRepository.completeOrder(id).onSuccess { loadOrder(id) } }
    }
}