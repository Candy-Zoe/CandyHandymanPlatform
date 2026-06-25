package com.candy.handyman.ui.screen.notification

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.candy.handyman.data.remote.dto.NotificationDto
import com.candy.handyman.data.repository.NotificationRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class NotificationViewModel @Inject constructor(
    private val repository: NotificationRepository
) : ViewModel() {

    private val _notifications = MutableStateFlow<List<NotificationDto>>(emptyList())
    val notifications: StateFlow<List<NotificationDto>> = _notifications

    private val _unreadCount = MutableStateFlow(0)
    val unreadCount: StateFlow<Int> = _unreadCount

    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    private var currentPage = 1
    private var hasMore = true

    init {
        loadNotifications()
        loadUnreadCount()
    }

    fun loadNotifications() {
        viewModelScope.launch {
            _isLoading.value = true
            currentPage = 1
            repository.getNotifications(page = currentPage)
                .onSuccess { result ->
                    _notifications.value = result.items
                    hasMore = result.items.size == result.pageSize
                }
            _isLoading.value = false
        }
    }

    fun loadMore() {
        if (_isLoading.value || !hasMore) return
        viewModelScope.launch {
            _isLoading.value = true
            currentPage++
            repository.getNotifications(page = currentPage)
                .onSuccess { result ->
                    _notifications.value = _notifications.value + result.items
                    hasMore = result.items.size == result.pageSize
                }
            _isLoading.value = false
        }
    }

    fun loadUnreadCount() {
        viewModelScope.launch {
            repository.getUnreadCount()
                .onSuccess { _unreadCount.value = it }
        }
    }

    fun markAsRead(id: String) {
        viewModelScope.launch {
            repository.markAsRead(id)
                .onSuccess {
                    _notifications.value = _notifications.value.map {
                        if (it.id == id) it.copy(isRead = true) else it
                    }
                    loadUnreadCount()
                }
        }
    }

    fun markAllAsRead() {
        viewModelScope.launch {
            repository.markAllAsRead()
                .onSuccess {
                    _notifications.value = _notifications.value.map { it.copy(isRead = true) }
                    _unreadCount.value = 0
                }
        }
    }

    fun deleteNotification(id: String) {
        viewModelScope.launch {
            repository.deleteNotification(id)
                .onSuccess {
                    _notifications.value = _notifications.value.filter { it.id != id }
                    loadUnreadCount()
                }
        }
    }
}
