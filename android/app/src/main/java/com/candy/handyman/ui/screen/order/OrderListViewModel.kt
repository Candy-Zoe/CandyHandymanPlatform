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
class OrderListViewModel @Inject constructor(
    private val orderRepository: OrderRepository
) : ViewModel() {
    private val _orders = MutableStateFlow<List<OrderDto>>(emptyList())
    val orders = _orders.asStateFlow()

    fun loadOrders(status: String? = null) {
        viewModelScope.launch {
            orderRepository.getOrders(status).onSuccess { _orders.value = it }
        }
    }
}