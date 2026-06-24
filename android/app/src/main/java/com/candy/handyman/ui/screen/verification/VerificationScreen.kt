package com.candy.handyman.ui.screen.verification

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
fun VerificationScreen(navController: NavController, viewModel: VerificationViewModel = hiltViewModel()) {
    val status by viewModel.status.collectAsState()
    val realName by remember { mutableStateOf("") }
    val idCard by remember { mutableStateOf("") }
    val isLoading by viewModel.isLoading.collectAsState()
    val message by viewModel.message.collectAsState()

    LaunchedEffect(Unit) { viewModel.loadStatus() }

    Column(
        modifier = Modifier.fillMaxSize().padding(16.dp),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        TopAppBar(title = { Text("实名认证") })

        Spacer(modifier = Modifier.height(24.dp))

        when (status?.status) {
            "Approved" -> {
                Card(modifier = Modifier.fillMaxWidth()) {
                    Column(modifier = Modifier.padding(24.dp), horizontalAlignment = Alignment.CenterHorizontally) {
                        Text("✓ 认证已通过", fontSize = 20.sp, fontWeight = FontWeight.Bold, color = Primary)
                        Spacer(modifier = Modifier.height(8.dp))
                        Text("真实姓名: ${status?.realName}", fontSize = 14.sp)
                    }
                }
            }
            "Pending" -> {
                Card(modifier = Modifier.fillMaxWidth()) {
                    Column(modifier = Modifier.padding(24.dp), horizontalAlignment = Alignment.CenterHorizontally) {
                        Text("审核中", fontSize = 20.sp, fontWeight = FontWeight.Bold)
                        Spacer(modifier = Modifier.height(8.dp))
                        Text("您的实名认证正在审核中，请耐心等待", fontSize = 14.sp, color = MaterialTheme.colorScheme.onSurfaceVariant)
                    }
                }
            }
            else -> {
                Card(modifier = Modifier.fillMaxWidth()) {
                    Column(modifier = Modifier.padding(16.dp)) {
                        Text("提交实名认证", fontWeight = FontWeight.Bold, fontSize = 16.sp)
                        Spacer(modifier = Modifier.height(16.dp))

                        OutlinedTextField(
                            value = realName, onValueChange = {},
                            label = { Text("真实姓名") }, modifier = Modifier.fillMaxWidth(), singleLine = true
                        )
                        Spacer(modifier = Modifier.height(12.dp))
                        OutlinedTextField(
                            value = idCard, onValueChange = {},
                            label = { Text("身份证号") }, modifier = Modifier.fillMaxWidth(), singleLine = true
                        )
                        Spacer(modifier = Modifier.height(16.dp))

                        Text("上传身份证正面", fontSize = 14.sp)
                        Button(onClick = { }, modifier = Modifier.fillMaxWidth().height(48.dp)) {
                            Text("选择图片")
                        }
                        Spacer(modifier = Modifier.height(8.dp))
                        Text("上传身份证反面", fontSize = 14.sp)
                        Button(onClick = { }, modifier = Modifier.fillMaxWidth().height(48.dp)) {
                            Text("选择图片")
                        }

                        if (message != null) {
                            Spacer(modifier = Modifier.height(8.dp))
                            Text(message ?: "", color = Primary, fontSize = 13.sp)
                        }

                        Spacer(modifier = Modifier.height(16.dp))
                        Button(
                            onClick = { viewModel.submit(realName, idCard) },
                            modifier = Modifier.fillMaxWidth().height(48.dp),
                            enabled = !isLoading && realName.isNotEmpty() && idCard.isNotEmpty()
                        ) {
                            if (isLoading) CircularProgressIndicator(modifier = Modifier.size(20.dp))
                            else Text("提交认证")
                        }
                    }
                }
            }
        }
    }
}