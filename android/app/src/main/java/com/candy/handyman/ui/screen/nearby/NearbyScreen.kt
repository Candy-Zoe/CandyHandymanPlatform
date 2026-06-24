package com.candy.handyman.ui.screen.nearby

import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.candy.handyman.data.remote.dto.NearbyHandymanDto
import com.candy.handyman.ui.theme.Primary
import com.candy.handyman.ui.theme.StarYellow

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun NearbyScreen(navController: NavController, viewModel: NearbyViewModel = hiltViewModel()) {
    val handymen by viewModel.handymen.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()

    LaunchedEffect(Unit) { viewModel.loadNearby() }

    Column(modifier = Modifier.fillMaxSize()) {
        TopAppBar(title = { Text("附近工匠", fontWeight = FontWeight.Bold) })

        if (isLoading) {
            Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                CircularProgressIndicator()
            }
        } else {
            LazyColumn(
                contentPadding = PaddingValues(16.dp),
                verticalArrangement = Arrangement.spacedBy(12.dp)
            ) {
                items(handymen) { handyman ->
                    HandymanCard(handyman) {
                        navController.navigate("serviceDetail/${handyman.id}")
                    }
                }
            }
        }
    }
}

@Composable
fun HandymanCard(handyman: NearbyHandymanDto, onClick: () -> Unit) {
    Card(
        modifier = Modifier.fillMaxWidth().clickable(onClick = onClick),
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp)
    ) {
        Row(
            modifier = Modifier.padding(16.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            Column(modifier = Modifier.weight(1f)) {
                Row(verticalAlignment = Alignment.CenterVertically) {
                    Text(handyman.nickname, fontWeight = FontWeight.Bold, fontSize = 16.sp)
                    if (handyman.isVerified) {
                        Text(" ✓", color = Primary, fontSize = 14.sp)
                    }
                }
                Spacer(modifier = Modifier.height(4.dp))
                Row {
                    Text("★ ${handyman.averageRating}", color = StarYellow, fontSize = 14.sp)
                    Text(" (${handyman.totalReviews}条评价)", fontSize = 12.sp, color = MaterialTheme.colorScheme.onSurfaceVariant)
                }
            }
            Column(horizontalAlignment = Alignment.End) {
                Text("${String.format("%.1f", handyman.distance)}km", fontSize = 14.sp, color = Primary)
            }
        }
    }
}