package com.candy.handyman.ui.screen.profile

import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
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

        ProfileMenuItem(Icons.Default.Notifications, "消息通知") { navController.navigate("notifications") }
        ProfileMenuItem(Icons.Default.AccountBalanceWallet, "我的钱包") { navController.navigate("wallet") }
        ProfileMenuItem(Icons.Default.ConfirmationNumber, "优惠券") { navController.navigate("coupons") }
        ProfileMenuItem(Icons.Default.Leaderboard, "工匠排行") { navController.navigate("ranking") }
        ProfileMenuItem(Icons.Default.Build, "发布服务") { navController.navigate("publish") }
        ProfileMenuItem(Icons.Default.List, "我的订单") { navController.navigate("orders") }
        ProfileMenuItem(Icons.Default.VerifiedUser, "实名认证") { navController.navigate("verification") }
        ProfileMenuItem(Icons.Default.CardMembership, "技能认证") { navController.navigate("certifications") }
        ProfileMenuItem(Icons.Default.Favorite, "我的收藏") { }
        ProfileMenuItem(Icons.Default.History, "浏览历史") { }
        ProfileMenuItem(Icons.Default.Campaign, "平台公告") { }
        ProfileMenuItem(Icons.Default.Help, "帮助中心") { }
        ProfileMenuItem(Icons.Default.Feedback, "意见反馈") { }
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
