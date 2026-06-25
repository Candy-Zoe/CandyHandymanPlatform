package com.candy.handyman.ui.navigation

import androidx.compose.foundation.layout.padding
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Chat
import androidx.compose.material.icons.filled.Home
import androidx.compose.material.icons.filled.List
import androidx.compose.material.icons.filled.Person
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.navigation.NavType
import androidx.navigation.compose.*
import androidx.navigation.navArgument
import com.candy.handyman.ui.screen.auth.LoginScreen
import com.candy.handyman.ui.screen.auth.RegisterScreen
import com.candy.handyman.ui.screen.certification.CertificationScreen
import com.candy.handyman.ui.screen.chat.ChatScreen
import com.candy.handyman.ui.screen.chat.ConversationListScreen
import com.candy.handyman.ui.screen.dispute.DisputeScreen
import com.candy.handyman.ui.screen.home.HomeScreen
import com.candy.handyman.ui.screen.insurance.InsuranceScreen
import com.candy.handyman.ui.screen.nearby.NearbyScreen
import com.candy.handyman.ui.screen.order.CreateOrderScreen
import com.candy.handyman.ui.screen.order.OrderDetailScreen
import com.candy.handyman.ui.screen.order.OrderListScreen
import com.candy.handyman.ui.screen.payment.PaymentScreen
import com.candy.handyman.ui.screen.profile.ProfileScreen
import com.candy.handyman.ui.screen.publish.PublishServiceScreen
import com.candy.handyman.ui.screen.service.ServiceDetailScreen
import com.candy.handyman.ui.screen.verification.VerificationScreen
import com.candy.handyman.ui.screen.notification.NotificationListScreen
import com.candy.handyman.ui.screen.schedule.ScheduleManageScreen
import com.candy.handyman.ui.screen.schedule.SlotPickerScreen
import com.candy.handyman.ui.screen.ranking.RankingScreen
import com.candy.handyman.ui.screen.wallet.WalletScreen
import com.candy.handyman.ui.screen.coupon.CouponListScreen

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun NavGraph() {
    val navController = rememberNavController()
    val currentBackStackEntry by navController.currentBackStackEntryAsState()
    val currentRoute = currentBackStackEntry?.destination?.route

    val bottomBarRoutes = listOf("home", "orders", "chat", "profile")
    val showBottomBar = currentRoute in bottomBarRoutes

    Scaffold(
        bottomBar = {
            if (showBottomBar) {
                NavigationBar {
                    NavigationBarItem(
                        icon = { Icon(Icons.Default.Home, contentDescription = null) },
                        label = { Text("首页") },
                        selected = currentRoute == "home",
                        onClick = { navController.navigate("home") { launchSingleTop = true } }
                    )
                    NavigationBarItem(
                        icon = { Icon(Icons.Default.List, contentDescription = null) },
                        label = { Text("订单") },
                        selected = currentRoute == "orders",
                        onClick = { navController.navigate("orders") { launchSingleTop = true } }
                    )
                    NavigationBarItem(
                        icon = { Icon(Icons.Default.Chat, contentDescription = null) },
                        label = { Text("消息") },
                        selected = currentRoute == "chat",
                        onClick = { navController.navigate("chat") { launchSingleTop = true } }
                    )
                    NavigationBarItem(
                        icon = { Icon(Icons.Default.Person, contentDescription = null) },
                        label = { Text("我的") },
                        selected = currentRoute == "profile",
                        onClick = { navController.navigate("profile") { launchSingleTop = true } }
                    )
                }
            }
        }
    ) { padding ->
        NavHost(
            navController = navController,
            startDestination = "home",
            modifier = Modifier.padding(padding)
        ) {
            composable("home") { HomeScreen(navController) }
            composable("login") { LoginScreen(navController) }
            composable("register") { RegisterScreen(navController) }
            composable("nearby") { NearbyScreen(navController) }
            composable("verification") { VerificationScreen(navController) }
            composable("certifications") { CertificationScreen(navController) }
            composable("payment") { PaymentScreen(navController) }
            composable("disputes") { DisputeScreen(navController) }
            composable(
                "insurance/{orderId}",
                arguments = listOf(navArgument("orderId") { type = NavType.StringType })
            ) { backStackEntry ->
                InsuranceScreen(navController, backStackEntry.arguments?.getString("orderId") ?: "")
            }
            composable(
                "serviceDetail/{serviceId}",
                arguments = listOf(navArgument("serviceId") { type = NavType.StringType })
            ) { backStackEntry ->
                ServiceDetailScreen(navController, backStackEntry.arguments?.getString("serviceId") ?: "")
            }
            composable(
                "createOrder/{serviceId}",
                arguments = listOf(navArgument("serviceId") { type = NavType.StringType })
            ) { backStackEntry ->
                CreateOrderScreen(navController, backStackEntry.arguments?.getString("serviceId") ?: "")
            }
            composable("orders") { OrderListScreen(navController) }
            composable(
                "orderDetail/{orderId}",
                arguments = listOf(navArgument("orderId") { type = NavType.StringType })
            ) { backStackEntry ->
                OrderDetailScreen(navController, backStackEntry.arguments?.getString("orderId") ?: "")
            }
            composable("chat") { ConversationListScreen(navController) }
            composable(
                "chatDetail/{conversationId}",
                arguments = listOf(navArgument("conversationId") { type = NavType.StringType })
            ) { backStackEntry ->
                ChatScreen(navController, backStackEntry.arguments?.getString("conversationId") ?: "")
            }
            composable("profile") { ProfileScreen(navController) }
            composable("publish") { PublishServiceScreen(navController) }
            composable("notifications") { NotificationListScreen(navController) }
            composable("ranking") { RankingScreen(navController) }
            composable(
                "schedule/{handymanId}",
                arguments = listOf(navArgument("handymanId") { type = NavType.StringType })
            ) { backStackEntry ->
                ScheduleManageScreen(navController, backStackEntry.arguments?.getString("handymanId") ?: "")
            }
            composable(
                "slotPicker/{handymanId}",
                arguments = listOf(navArgument("handymanId") { type = NavType.StringType })
            ) { backStackEntry ->
                SlotPickerScreen(
                    navController,
                    backStackEntry.arguments?.getString("handymanId") ?: "",
                    onSlotSelected = { }
                )
            }
            composable("wallet") { WalletScreen(navController) }
            composable("coupons") { CouponListScreen(navController) }
        }
    }
}