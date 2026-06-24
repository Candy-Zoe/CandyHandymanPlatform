package com.candy.handyman.ui.screen.service

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import coil.compose.AsyncImage
import com.candy.handyman.ui.theme.Gray500
import com.candy.handyman.ui.theme.Primary
import com.candy.handyman.ui.theme.StarYellow

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun ServiceDetailScreen(navController: NavController, serviceId: String, viewModel: ServiceDetailViewModel = hiltViewModel()) {
    val service by viewModel.service.collectAsState()

    LaunchedEffect(serviceId) { viewModel.loadService(serviceId) }

    service?.let { svc ->
        Scaffold(
            topBar = {
                TopAppBar(
                    title = { Text(svc.title) },
                    navigationIcon = {
                        TextButton(onClick = { navController.popBackStack() }) { Text("返回") }
                    }
                )
            },
            bottomBar = {
                Button(
                    onClick = { navController.navigate("createOrder/$serviceId") },
                    modifier = Modifier.fillMaxWidth().padding(16.dp).height(48.dp)
                ) {
                    Text("立即下单 ¥${svc.price}/${svc.unit}", fontSize = 16.sp)
                }
            }
        ) { padding ->
            Column(
                modifier = Modifier
                    .padding(padding)
                    .verticalScroll(rememberScrollState())
            ) {
                if (svc.media.isNotEmpty()) {
                    AsyncImage(
                        model = svc.media.first().mediaUrl,
                        contentDescription = null,
                        modifier = Modifier.fillMaxWidth().height(250.dp),
                        contentScale = ContentScale.Crop
                    )
                }

                Column(modifier = Modifier.padding(16.dp)) {
                    Text(svc.title, fontWeight = FontWeight.Bold, fontSize = 20.sp)
                    Spacer(modifier = Modifier.height(8.dp))
                    Text("¥${svc.price}/${svc.unit}", color = Primary, fontWeight = FontWeight.Bold, fontSize = 24.sp)
                    Spacer(modifier = Modifier.height(16.dp))
                    Text("服务描述", fontWeight = FontWeight.Bold, fontSize = 16.sp)
                    Spacer(modifier = Modifier.height(8.dp))
                    Text(svc.description, fontSize = 14.sp, color = Gray500, lineHeight = 22.sp)
                }
            }
        }
    }
}