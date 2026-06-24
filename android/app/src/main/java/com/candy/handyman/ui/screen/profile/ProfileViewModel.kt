package com.candy.handyman.ui.screen.profile

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.candy.handyman.data.remote.dto.UserDto
import com.candy.handyman.data.repository.UserRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class ProfileViewModel @Inject constructor(
    private val userRepository: UserRepository
) : ViewModel() {
    private val _user = MutableStateFlow<UserDto?>(null)
    val user = _user.asStateFlow()

    fun loadProfile() {
        viewModelScope.launch {
            userRepository.getMe().onSuccess { _user.value = it }
        }
    }
}