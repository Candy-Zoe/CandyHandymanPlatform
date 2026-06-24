package com.candy.handyman.ui.screen.nearby

import android.Manifest
import android.content.Context
import android.location.LocationManager
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.candy.handyman.data.remote.FeatureApiService
import com.candy.handyman.data.remote.dto.NearbyHandymanDto
import dagger.hilt.android.lifecycle.HiltViewModel
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class NearbyViewModel @Inject constructor(
    @ApplicationContext private val context: Context,
    private val featureApi: FeatureApiService
) : ViewModel() {

    private val _handymen = MutableStateFlow<List<NearbyHandymanDto>>(emptyList())
    val handymen = _handymen.asStateFlow()

    private val _isLoading = MutableStateFlow(false)
    val isLoading = _isLoading.asStateFlow()

    fun loadNearby() {
        viewModelScope.launch {
            _isLoading.value = true
            try {
                val locationManager = context.getSystemService(Context.LOCATION_SERVICE) as LocationManager
                val location = locationManager.getLastKnownLocation(LocationManager.GPS_PROVIDER)
                    ?: locationManager.getLastKnownLocation(LocationManager.NETWORK_PROVIDER)

                if (location != null) {
                    val response = featureApi.getNearbyHandymen(location.latitude, location.longitude)
                    if (response.isSuccessful) {
                        _handymen.value = response.body() ?: emptyList()
                    }
                }
            } catch (e: Exception) {
                e.printStackTrace()
            }
            _isLoading.value = false
        }
    }
}