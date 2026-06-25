package com.candy.handyman.ui.screen.coupon

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
import com.candy.handyman.data.remote.dto.UserCouponDto
import com.candy.handyman.ui.theme.Primary
import com.candy.handyman.ui.screen.wallet.WalletViewModel

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun CouponListScreen(
    navController: NavController,
    viewModel: WalletViewModel = hiltViewModel()
) {
    val coupons by viewModel.coupons.collectAsState()

    Column(modifier = Modifier.fillMaxSize()) {
        TopAppBar(title = { Text("我的优惠券", fontWeight = FontWeight.Bold) })

        LazyColumn(
            modifier = Modifier.fillMaxSize(),
            contentPadding = PaddingValues(16.dp),
            verticalArrangement = Arrangement.spacedBy(8.dp)
        ) {
            items(coupons) { coupon ->
                CouponItem(coupon = coupon)
            }

            if (coupons.isEmpty()) {
                item {
                    Box(
                        modifier = Modifier.fillMaxWidth().padding(32.dp),
                        contentAlignment = Alignment.Center
                    ) {
                        Text("暂无优惠券", color = MaterialTheme.colorScheme.onSurfaceVariant)
                    }
                }
            }
        }
    }
}

@Composable
fun CouponItem(coupon: UserCouponDto) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        colors = CardDefaults.cardColors(
            containerColor = if (coupon.isUsed) MaterialTheme.colorScheme.surfaceVariant else MaterialTheme.colorScheme.surface
        )
    ) {
        Row(
            modifier = Modifier.padding(16.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            Column(modifier = Modifier.weight(1f)) {
                Text(
                    text = coupon.name,
                    fontWeight = FontWeight.Bold,
                    fontSize = 16.sp
                )
                Text(
                    text = "满${coupon.minAmount}元可用",
                    fontSize = 13.sp,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
                coupon.expiresAt?.let {
                    Text(
                        text = "有效期至 $it",
                        fontSize = 12.sp,
                        color = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                }
            }

            Column(horizontalAlignment = Alignment.End) {
                Text(
                    text = if (coupon.type == "Percentage") "${coupon.discountValue}%OFF" else "¥${coupon.discountValue}",
                    fontWeight = FontWeight.Bold,
                    fontSize = 18.sp,
                    color = if (coupon.isUsed) MaterialTheme.colorScheme.onSurfaceVariant else Primary
                )
                if (coupon.isUsed) {
                    Text(
                        text = "已使用",
                        fontSize = 12.sp,
                        color = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                }
            }
        }
    }
}
