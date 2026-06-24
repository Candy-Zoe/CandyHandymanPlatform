package com.candy.handyman.ui.screen.payment

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
import com.candy.handyman.data.remote.dto.PaymentHistoryDto
import com.candy.handyman.ui.theme.Primary

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun PaymentScreen(navController: NavController, viewModel: PaymentViewModel = hiltViewModel()) {
    val payments by viewModel.payments.collectAsState()

    LaunchedEffect(Unit) { viewModel.loadHistory() }

    Column(modifier = Modifier.fillMaxSize()) {
        TopAppBar(title = { Text("支付记录", fontWeight = FontWeight.Bold) })

        LazyColumn(
            contentPadding = PaddingValues(16.dp),
            verticalArrangement = Arrangement.spacedBy(8.dp)
        ) {
            items(payments) { payment ->
                PaymentItem(payment)
            }
        }
    }
}

@Composable
fun PaymentItem(payment: PaymentHistoryDto) {
    Card(modifier = Modifier.fillMaxWidth()) {
        Row(
            modifier = Modifier.padding(16.dp).fillMaxWidth(),
            horizontalArrangement = Arrangement.SpaceBetween
        ) {
            Column {
                Text(payment.paymentMethod, fontWeight = FontWeight.Bold, fontSize = 15.sp)
                Text(payment.transactionId, fontSize = 12.sp, color = MaterialTheme.colorScheme.onSurfaceVariant)
                Text(payment.createdAt.substring(0, 10), fontSize = 12.sp, color = MaterialTheme.colorScheme.onSurfaceVariant)
            }
            Column(horizontalAlignment = Alignment.End) {
                Text("¥${payment.amount}", fontWeight = FontWeight.Bold, fontSize = 16.sp, color = Primary)
                Text(payment.status, fontSize = 12.sp, color = if (payment.status == "Paid") Primary else MaterialTheme.colorScheme.error)
            }
        }
    }
}