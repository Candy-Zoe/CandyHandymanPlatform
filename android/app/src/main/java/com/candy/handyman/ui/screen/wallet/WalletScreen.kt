package com.candy.handyman.ui.screen.wallet

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
import com.candy.handyman.data.remote.dto.WalletTransactionDto
import com.candy.handyman.ui.theme.Primary

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun WalletScreen(
    navController: NavController,
    viewModel: WalletViewModel = hiltViewModel()
) {
    val balance by viewModel.balance.collectAsState()
    val transactions by viewModel.transactions.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()
    var showRechargeDialog by remember { mutableStateOf(false) }

    Column(modifier = Modifier.fillMaxSize()) {
        TopAppBar(title = { Text("我的钱包", fontWeight = FontWeight.Bold) })

        Card(
            modifier = Modifier
                .fillMaxWidth()
                .padding(16.dp)
        ) {
            Column(
                modifier = Modifier.padding(24.dp),
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                Text("账户余额", fontSize = 14.sp, color = MaterialTheme.colorScheme.onSurfaceVariant)
                Text(
                    "¥${String.format("%.2f", balance)}",
                    fontSize = 32.sp,
                    fontWeight = FontWeight.Bold,
                    color = Primary
                )
                Spacer(modifier = Modifier.height(16.dp))
                Button(onClick = { showRechargeDialog = true }) {
                    Text("充值")
                }
            }
        }

        Text(
            "交易记录",
            fontWeight = FontWeight.Bold,
            fontSize = 16.sp,
            modifier = Modifier.padding(horizontal = 16.dp, vertical = 8.dp)
        )

        LazyColumn(
            modifier = Modifier.fillMaxSize(),
            contentPadding = PaddingValues(16.dp),
            verticalArrangement = Arrangement.spacedBy(8.dp)
        ) {
            items(transactions) { transaction ->
                TransactionItem(transaction = transaction)
            }

            if (isLoading) {
                item {
                    Box(modifier = Modifier.fillMaxWidth(), contentAlignment = Alignment.Center) {
                        CircularProgressIndicator(modifier = Modifier.padding(16.dp))
                    }
                }
            }
        }
    }

    if (showRechargeDialog) {
        RechargeDialog(
            onDismiss = { showRechargeDialog = false },
            onConfirm = { amount ->
                viewModel.recharge(amount)
                showRechargeDialog = false
            }
        )
    }
}

@Composable
fun TransactionItem(transaction: WalletTransactionDto) {
    Card(modifier = Modifier.fillMaxWidth()) {
        Row(
            modifier = Modifier.padding(12.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            Column(modifier = Modifier.weight(1f)) {
                Text(
                    text = transaction.description ?: transaction.type,
                    fontWeight = FontWeight.Bold,
                    fontSize = 14.sp
                )
                Text(
                    text = transaction.createdAt,
                    fontSize = 12.sp,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
            }
            Text(
                text = if (transaction.amount >= 0) "+¥${transaction.amount}" else "-¥${-transaction.amount}",
                fontWeight = FontWeight.Bold,
                fontSize = 16.sp,
                color = if (transaction.amount >= 0) Primary else MaterialTheme.colorScheme.error
            )
        }
    }
}

@Composable
fun RechargeDialog(onDismiss: () -> Unit, onConfirm: (Double) -> Unit) {
    var amount by remember { mutableStateOf("") }

    AlertDialog(
        onDismissRequest = onDismiss,
        title = { Text("充值") },
        text = {
            OutlinedTextField(
                value = amount,
                onValueChange = { amount = it },
                label = { Text("充值金额") },
                prefix = { Text("¥") }
            )
        },
        confirmButton = {
            TextButton(onClick = {
                amount.toDoubleOrNull()?.let { onConfirm(it) }
            }) {
                Text("确认充值")
            }
        },
        dismissButton = {
            TextButton(onClick = onDismiss) {
                Text("取消")
            }
        }
    )
}
