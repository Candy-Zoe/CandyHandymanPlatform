package com.candy.handyman.ui.screen.home

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.candy.handyman.data.remote.dto.CategoryDto
import com.candy.handyman.data.remote.dto.ServiceDto
import com.candy.handyman.data.remote.ApiService
import com.candy.handyman.data.repository.ServiceRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class HomeViewModel @Inject constructor(
    private val serviceRepository: ServiceRepository,
    private val api: ApiService
) : ViewModel() {

    private val _services = MutableStateFlow<List<ServiceDto>>(emptyList())
    val services = _services.asStateFlow()

    private val _categories = MutableStateFlow<List<CategoryDto>>(emptyList())
    val categories = _categories.asStateFlow()

    fun loadServices(categoryId: String? = null, keyword: String? = null) {
        viewModelScope.launch {
            serviceRepository.getServices(categoryId, keyword).onSuccess {
                _services.value = it
            }
        }
    }

    fun loadCategories() {
        viewModelScope.launch {
            try {
                val response = api.getCategories()
                if (response.isSuccessful) {
                    _categories.value = response.body() ?: emptyList()
                }
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }
    }
}