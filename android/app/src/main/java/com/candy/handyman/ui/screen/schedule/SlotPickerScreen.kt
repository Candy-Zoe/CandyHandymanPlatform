package com.candy.handyman.ui.screen.schedule

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
import com.candy.handyman.data.remote.dto.AppointmentSlotDto
import com.candy.handyman.ui.theme.Primary
import java.time.LocalDate

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun SlotPickerScreen(
    navController: NavController,
    handymanId: String,
    onSlotSelected: (AppointmentSlotDto) -> Unit,
    viewModel: ScheduleViewModel = hiltViewModel()
) {
    val availableSlots by viewModel.availableSlots.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()
    var selectedDate by remember { mutableStateOf(LocalDate.now().toString()) }

    LaunchedEffect(handymanId, selectedDate) {
        viewModel.loadAvailableSlots(handymanId, selectedDate)
    }

    Column(modifier = Modifier.fillMaxSize()) {
        TopAppBar(title = { Text("选择预约时段", fontWeight = FontWeight.Bold) })

        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(16.dp),
            horizontalArrangement = Arrangement.SpaceEvenly
        ) {
            for (i in 0..6) {
                val date = LocalDate.now().plusDays(i.toLong())
                val isSelected = date.toString() == selectedDate
                FilterChip(
                    selected = isSelected,
                    onClick = { selectedDate = date.toString() },
                    label = {
                        Column(horizontalAlignment = Alignment.CenterHorizontally) {
                            Text("${date.monthValue}/${date.dayOfMonth}", fontSize = 12.sp)
                            Text(
                                when (date.dayOfWeek.value) {
                                    1 -> "一"
                                    2 -> "二"
                                    3 -> "三"
                                    4 -> "四"
                                    5 -> "五"
                                    6 -> "六"
                                    7 -> "日"
                                    else -> ""
                                },
                                fontSize = 10.sp
                            )
                        }
                    }
                )
            }
        }

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
                items(availableSlots) { slot ->
                    SlotItem(slot = slot, onClick = { onSlotSelected(slot) })
                }

                if (availableSlots.isEmpty()) {
                    item {
                        Box(
                            modifier = Modifier.fillMaxWidth().padding(32.dp),
                            contentAlignment = Alignment.Center
                        ) {
                            Text("该日期暂无可用时段", color = MaterialTheme.colorScheme.onSurfaceVariant)
                        }
                    }
                }
            }
        }
    }
}

@Composable
fun SlotItem(slot: AppointmentSlotDto, onClick: () -> Unit) {
    Card(
        modifier = Modifier
            .fillMaxWidth()
            .clickable(onClick = onClick)
    ) {
        Row(
            modifier = Modifier.padding(16.dp),
            verticalAlignment = Alignment.CenterVertically,
            horizontalArrangement = Arrangement.SpaceBetween
        ) {
            Column {
                Text(
                    text = "${slot.startTime} - ${slot.endTime}",
                    fontWeight = FontWeight.Bold,
                    fontSize = 16.sp
                )
                Text(
                    text = slot.date,
                    fontSize = 13.sp,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
            }

            Button(
                onClick = onClick,
                colors = ButtonDefaults.buttonColors(containerColor = Primary)
            ) {
                Text("选择")
            }
        }
    }
}
