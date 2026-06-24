package com.candy.handyman.ui.screen.dispute

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
import com.candy.handyman.data.remote.dto.DisputeDto
import com.candy.handyman.ui.theme.Primary

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun DisputeScreen(navController: NavController, viewModel: DisputeViewModel = hiltViewModel()) {
    val disputes by viewModel.disputes.collectAsState()
    var showCreateDialog by remember { mutableStateOf(false) }

    LaunchedEffect(Unit) { viewModel.loadDisputes() }

    Column(modifier = Modifier.fillMaxSize()) {
        TopAppBar(title = { Text("纠纷仲裁") })

        if (disputes.isEmpty()) {
            Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                Text("暂无争议记录", color = MaterialTheme.colorScheme.onSurfaceVariant)
            }
        } else {
            LazyColumn(
                contentPadding = PaddingValues(16.dp),
                verticalArrangement = Arrangement.spacedBy(8.dp)
            ) {
                items(disputes) { dispute ->
                    DisputeItem(dispute)
                }
            }
        }
    }
}

@Composable
fun DisputeItem(dispute: DisputeDto) {
    Card(modifier = Modifier.fillMaxWidth()) {
        Column(modifier = Modifier.padding(16.dp)) {
            Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                Text(dispute.reason, fontWeight = FontWeight.Bold, fontSize = 15.sp)
                Text(
                    when (dispute.status) { "Open" -> "待处理"; "UnderReview" -> "审核中"; "Resolved" -> "已解决"; else -> "已拒绝" },
                    fontSize = 13.sp,
                    color = when (dispute.status) { "Resolved" -> Primary; "Rejected" -> MaterialTheme.colorScheme.error; else -> MaterialTheme.colorScheme.onSurfaceVariant }
                )
            }
            if (dispute.description != null) {
                Spacer(modifier = Modifier.height(4.dp))
                Text(dispute.description, fontSize = 13.sp, color = MaterialTheme.colorScheme.onSurfaceVariant)
            }
            if (dispute.resolution != null) {
                Spacer(modifier = Modifier.height(4.dp))
                Text("处理结果: ${dispute.resolution}", fontSize = 13.sp, color = Primary)
            }
        }
    }
}