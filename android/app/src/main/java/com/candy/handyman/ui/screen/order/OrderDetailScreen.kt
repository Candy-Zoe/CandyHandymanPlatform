package com.candy.handyman.ui.screen.order

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.candy.handyman.ui.theme.Primary

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun OrderDetailScreen(navController: NavController, orderId: String, viewModel: OrderDetailViewModel = hiltViewModel()) {
    val order by viewModel.order.collectAsState()

    LaunchedEffect(orderId) { viewModel.loadOrder(orderId) }

    order?.let { o ->
        Scaffold(
            topBar = {
                TopAppBar(title = { Text("订单详情") },
                    navigationIcon = { TextButton(onClick = { navController.popBackStack() }) { Text("返回") } })
            }
        ) { padding ->
            Column(modifier = Modifier.padding(padding).padding(16.dp).fillMaxSize()) {
                Text("订单号: ${o.orderNo}", fontWeight = FontWeight.Bold, fontSize = 16.sp)
                Spacer(modifier = Modifier.height(8.dp))
                Text("状态: ${o.status}", color = Primary, fontSize = 14.sp)
                Spacer(modifier = Modifier.height(8.dp))
                Text("金额: ¥${o.totalAmount}", fontWeight = FontWeight.Bold, fontSize = 20.sp, color = Primary)
                Spacer(modifier = Modifier.height(8.dp))
                Text("地址: ${o.address}", fontSize = 14.sp)
                Spacer(modifier = Modifier.height(8.dp))
                Text("电话: ${o.contactPhone}", fontSize = 14.sp)
                Spacer(modifier = Modifier.height(24.dp))

                if (o.status == "Pending") {
                    Button(onClick = { viewModel.acceptOrder(orderId, navController) }, modifier = Modifier.fillMaxWidth()) { Text("接单") }
                }
                if (o.status == "Accepted") {
                    Button(onClick = { viewModel.startOrder(orderId, navController) }, modifier = Modifier.fillMaxWidth()) { Text("开始服务") }
                }
                if (o.status == "InProgress") {
                    Button(onClick = { viewModel.completeOrder(orderId, navController) }, modifier = Modifier.fillMaxWidth()) { Text("完成服务") }
                }
            }
        }
    }
}