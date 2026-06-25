package com.candy.handyman.ui.screen.ranking

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.candy.handyman.data.remote.dto.HandymanRankingDto
import com.candy.handyman.data.repository.ScheduleRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class RankingViewModel @Inject constructor(
    private val repository: ScheduleRepository
) : ViewModel() {

    private val _rankings = MutableStateFlow<List<HandymanRankingDto>>(emptyList())
    val rankings: StateFlow<List<HandymanRankingDto>> = _rankings

    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    init {
        loadRanking()
    }

    fun loadRanking(categoryId: String? = null) {
        viewModelScope.launch {
            _isLoading.value = true
            repository.getRanking(categoryId)
                .onSuccess { _rankings.value = it }
            _isLoading.value = false
        }
    }
}
