package com.candy.handyman.ui.screen.order

import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.candy.handyman.data.remote.dto.OrderDto
import com.candy.handyman.ui.theme.Primary

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun OrderListScreen(navController: NavController, viewModel: OrderListViewModel = hiltViewModel()) {
    val orders by viewModel.orders.collectAsState()
    var selectedTab by remember { mutableIntStateOf(0) }
    val tabs = listOf("全部", "待接单", "进行中", "已完成")

    LaunchedEffect(selectedTab) {
        val status = when (selectedTab) { 1 -> "Pending"; 2 -> "InProgress"; 3 -> "Completed"; else -> null }
        viewModel.loadOrders(status)
    }

    Column(modifier = Modifier.fillMaxSize()) {
        TopAppBar(title = { Text("我的订单", fontWeight = FontWeight.Bold) })
        TabRow(selectedTabIndex = selectedTab) {
            tabs.forEachIndexed { index, title ->
                Tab(selected = selectedTab == index, onClick = { selectedTab = index }, text = { Text(title) })
            }
        }
        LazyColumn(
            modifier = Modifier.fillMaxSize(),
            contentPadding = PaddingValues(16.dp),
            verticalArrangement = Arrangement.spacedBy(8.dp)
        ) {
            items(orders) { order ->
                OrderItem(order) { navController.navigate("orderDetail/${order.id}") }
            }
        }
    }
}

@Composable
fun OrderItem(order: OrderDto, onClick: () -> Unit) {
    Card(modifier = Modifier.fillMaxWidth().clickable(onClick = onClick)) {
        Column(modifier = Modifier.padding(12.dp)) {
            Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                Text(order.orderNo, fontWeight = FontWeight.Bold, fontSize = 14.sp)
                Text(order.status, color = Primary, fontSize = 13.sp)
            }
            Spacer(modifier = Modifier.height(4.dp))
            Text("¥${order.totalAmount}", fontWeight = FontWeight.Bold, fontSize = 16.sp, color = Primary)
            Spacer(modifier = Modifier.height(4.dp))
            Text(order.address, fontSize = 13.sp, color = MaterialTheme.colorScheme.onSurfaceVariant)
        }
    }
}