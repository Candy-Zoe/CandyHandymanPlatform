package com.candy.handyman.ui.screen.wallet

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.candy.handyman.data.remote.dto.UserCouponDto
import com.candy.handyman.data.remote.dto.WalletTransactionDto
import com.candy.handyman.data.repository.WalletRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class WalletViewModel @Inject constructor(
    private val repository: WalletRepository
) : ViewModel() {

    private val _balance = MutableStateFlow(0.0)
    val balance: StateFlow<Double> = _balance

    private val _transactions = MutableStateFlow<List<WalletTransactionDto>>(emptyList())
    val transactions: StateFlow<List<WalletTransactionDto>> = _transactions

    private val _coupons = MutableStateFlow<List<UserCouponDto>>(emptyList())
    val coupons: StateFlow<List<UserCouponDto>> = _coupons

    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    private var currentPage = 1
    private var hasMore = true

    init {
        loadBalance()
        loadTransactions()
        loadCoupons()
    }

    fun loadBalance() {
        viewModelScope.launch {
            repository.getBalance()
                .onSuccess { _balance.value = it.balance }
        }
    }

    fun loadTransactions() {
        viewModelScope.launch {
            _isLoading.value = true
            currentPage = 1
            repository.getTransactions(page = currentPage)
                .onSuccess {
                    _transactions.value = it
                    hasMore = it.size == 20
                }
            _isLoading.value = false
        }
    }

    fun loadMoreTransactions() {
        if (_isLoading.value || !hasMore) return
        viewModelScope.launch {
            _isLoading.value = true
            currentPage++
            repository.getTransactions(page = currentPage)
                .onSuccess {
                    _transactions.value = _transactions.value + it
                    hasMore = it.size == 20
                }
            _isLoading.value = false
        }
    }

    fun loadCoupons() {
        viewModelScope.launch {
            repository.getMyCoupons()
                .onSuccess { _coupons.value = it }
        }
    }

    fun recharge(amount: Double) {
        viewModelScope.launch {
            _isLoading.value = true
            repository.recharge(amount)
                .onSuccess { loadBalance() }
            _isLoading.value = false
        }
    }

    fun withdraw(amount: Double) {
        viewModelScope.launch {
            _isLoading.value = true
            repository.withdraw(amount)
                .onSuccess { loadBalance() }
            _isLoading.value = false
        }
    }
}
