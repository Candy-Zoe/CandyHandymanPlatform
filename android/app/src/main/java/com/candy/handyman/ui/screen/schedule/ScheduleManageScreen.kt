package com.candy.handyman.ui.screen.schedule

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
import com.candy.handyman.data.remote.dto.ScheduleDto
import com.candy.handyman.ui.theme.Primary

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun ScheduleManageScreen(
    navController: NavController,
    handymanId: String,
    viewModel: ScheduleViewModel = hiltViewModel()
) {
    val schedules by viewModel.schedules.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()

    val dayNames = listOf("周日", "周一", "周二", "周三", "周四", "周五", "周六")

    LaunchedEffect(handymanId) {
        viewModel.loadSchedules(handymanId)
    }

    Column(modifier = Modifier.fillMaxSize()) {
        TopAppBar(title = { Text("排班管理", fontWeight = FontWeight.Bold) })

        if (isLoading) {
            Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                CircularProgressIndicator()
            }
        } else {
            LazyColumn(
                modifier = Modifier.fillMaxSize(),
                contentPadding = PaddingValues(16.dp),
                verticalArrangement = Arrangement.spacedBy(8.dp)
            ) {
                items(7) { dayOfWeek ->
                    val schedule = schedules.find { it.dayOfWeek == dayOfWeek }
                    ScheduleDayItem(
                        dayName = dayNames[dayOfWeek],
                        schedule = schedule
                    )
                }

                item {
                    Spacer(modifier = Modifier.height(16.dp))
                    Button(
                        onClick = { viewModel.generateSlots(handymanId) },
                        modifier = Modifier.fillMaxWidth()
                    ) {
                        Text("生成未来14天预约时段")
                    }
                }
            }
        }
    }
}

@Composable
fun ScheduleDayItem(dayName: String, schedule: ScheduleDto?) {
    Card(modifier = Modifier.fillMaxWidth()) {
        Row(
            modifier = Modifier.padding(16.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            Text(
                text = dayName,
                fontWeight = FontWeight.Bold,
                fontSize = 16.sp,
                modifier = Modifier.width(50.dp)
            )

            Spacer(modifier = Modifier.width(16.dp))

            if (schedule != null && schedule.isAvailable) {
                Text(
                    text = "${schedule.startTime} - ${schedule.endTime}",
                    fontSize = 14.sp,
                    color = Primary
                )
            } else {
                Text(
                    text = "休息",
                    fontSize = 14.sp,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
            }
        }
    }
}
