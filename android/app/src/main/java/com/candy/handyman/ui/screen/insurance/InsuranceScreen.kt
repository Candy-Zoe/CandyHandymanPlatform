package com.candy.handyman.ui.screen.insurance

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.candy.handyman.ui.theme.Primary

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun InsuranceScreen(navController: NavController, orderId: String, viewModel: InsuranceViewModel = hiltViewModel()) {
    val insuranceInfo by viewModel.insuranceInfo.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()
    val message by viewModel.message.collectAsState()

    LaunchedEffect(orderId) { viewModel.checkInsurance(orderId) }

    Column(
        modifier = Modifier.fillMaxSize().padding(16.dp),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        TopAppBar(title = { Text("保险服务") })

        Spacer(modifier = Modifier.height(24.dp))

        if (insuranceInfo?.get("hasInsurance") == true) {
            Card(modifier = Modifier.fillMaxWidth()) {
                Column(modifier = Modifier.padding(24.dp), horizontalAlignment = Alignment.CenterHorizontally) {
                    Text("已购买保险", fontSize = 20.sp, fontWeight = FontWeight.Bold, color = Primary)
                    Spacer(modifier = Modifier.height(12.dp))
                    Text("保单号: ${insuranceInfo?.get("policyNo")}", fontSize = 14.sp)
                    Text("保额: ¥${insuranceInfo?.get("coverageAmount")}", fontSize = 14.sp)
                    Text("类型: ${insuranceInfo?.get("type")}", fontSize = 14.sp)
                }
            }
        } else {
            Text("选择保险类型", fontWeight = FontWeight.Bold, fontSize = 18.sp)
            Spacer(modifier = Modifier.height(16.dp))

            InsuranceOption("人身意外险", "¥5.00", "保额¥500") {
                viewModel.purchase(orderId, "PersonalInjury")
            }
            Spacer(modifier = Modifier.height(12.dp))
            InsuranceOption("财产损失险", "¥8.00", "保额¥800") {
                viewModel.purchase(orderId, "PropertyDamage")
            }
            Spacer(modifier = Modifier.height(12.dp))
            InsuranceOption("综合保障险", "¥12.00", "保额¥1200") {
                viewModel.purchase(orderId, "Comprehensive")
            }

            if (message != null) {
                Spacer(modifier = Modifier.height(12.dp))
                Text(message ?: "", color = Primary, fontSize = 13.sp)
            }
        }
    }
}

@Composable
fun InsuranceOption(name: String, price: String, coverage: String, onClick: () -> Unit) {
    Card(modifier = Modifier.fillMaxWidth()) {
        Row(
            modifier = Modifier.padding(16.dp).fillMaxWidth(),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Column {
                Text(name, fontWeight = FontWeight.Bold, fontSize = 16.sp)
                Text(coverage, fontSize = 13.sp, color = MaterialTheme.colorScheme.onSurfaceVariant)
            }
            Button(onClick = onClick) { Text(price) }
        }
    }
}