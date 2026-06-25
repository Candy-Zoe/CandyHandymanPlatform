package com.candy.handyman.ui.screen.schedule

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.candy.handyman.data.remote.dto.AppointmentSlotDto
import com.candy.handyman.data.remote.dto.ScheduleDto
import com.candy.handyman.data.repository.ScheduleRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class ScheduleViewModel @Inject constructor(
    private val repository: ScheduleRepository
) : ViewModel() {

    private val _schedules = MutableStateFlow<List<ScheduleDto>>(emptyList())
    val schedules: StateFlow<List<ScheduleDto>> = _schedules

    private val _availableSlots = MutableStateFlow<List<AppointmentSlotDto>>(emptyList())
    val availableSlots: StateFlow<List<AppointmentSlotDto>> = _availableSlots

    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    fun loadSchedules(handymanId: String) {
        viewModelScope.launch {
            _isLoading.value = true
            repository.getSchedules(handymanId)
                .onSuccess { _schedules.value = it }
            _isLoading.value = false
        }
    }

    fun updateSchedules(schedules: List<ScheduleDto>) {
        viewModelScope.launch {
            _isLoading.value = true
            repository.updateSchedules(schedules)
                .onSuccess { _schedules.value = schedules }
            _isLoading.value = false
        }
    }

    fun loadAvailableSlots(handymanId: String, date: String) {
        viewModelScope.launch {
            _isLoading.value = true
            repository.getAvailableSlots(handymanId, date)
                .onSuccess { _availableSlots.value = it }
            _isLoading.value = false
        }
    }

    fun generateSlots(handymanId: String, days: Int = 14) {
        viewModelScope.launch {
            _isLoading.value = true
            repository.generateSlots(handymanId, days)
            _isLoading.value = false
        }
    }
}
