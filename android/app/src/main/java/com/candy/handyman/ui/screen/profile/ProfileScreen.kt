package com.candy.handyman.ui.screen.profile

import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.candy.handyman.ui.theme.Primary

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun ProfileScreen(navController: NavController, viewModel: ProfileViewModel = hiltViewModel()) {
    val user by viewModel.user.collectAsState()

    LaunchedEffect(Unit) { viewModel.loadProfile() }

    Column(modifier = Modifier.fillMaxSize()) {
        TopAppBar(title = { Text("个人中心", fontWeight = FontWeight.Bold) })

        Card(modifier = Modifier.fillMaxWidth().padding(16.dp)) {
            Column(modifier = Modifier.padding(16.dp)) {
                Text(user?.nickname ?: "未登录", fontWeight = FontWeight.Bold, fontSize = 18.sp)
                Spacer(modifier = Modifier.height(4.dp))
                Text(user?.phone ?: "", fontSize = 14.sp, color = MaterialTheme.colorScheme.onSurfaceVariant)
                Spacer(modifier = Modifier.height(4.dp))
                Text("余额: ¥${user?.balance ?: 0}", fontSize = 14.sp, color = Primary)
            }
        }

        Spacer(modifier = Modifier.height(8.dp))

        ProfileMenuItem(Icons.Default.Build, "发布服务") { navController.navigate("publish") }
        ProfileMenuItem(Icons.Default.List, "我的订单") { navController.navigate("orders") }
        ProfileMenuItem(Icons.Default.Star, "我的评价") { }
        ProfileMenuItem(Icons.Default.Settings, "设置") { }
    }
}

@Composable
fun ProfileMenuItem(icon: ImageVector, title: String, onClick: () -> Unit) {
    ListItem(
        headlineContent = { Text(title) },
        leadingContent = { Icon(icon, contentDescription = null, tint = Primary) },
        modifier = Modifier.clickable(onClick = onClick)
    )
    HorizontalDivider()
}

@Composable
private fun Modifier.clickable(onClick: () -> Unit): Modifier {
    return this.then(androidx.compose.foundation.clickable(onClick = onClick) as Modifier)
}