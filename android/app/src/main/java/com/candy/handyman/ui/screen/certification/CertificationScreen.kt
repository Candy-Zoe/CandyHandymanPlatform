package com.candy.handyman.ui.screen.certification

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
import com.candy.handyman.data.remote.dto.CraftsmanCertificationDto
import com.candy.handyman.ui.theme.Primary

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun CertificationScreen(navController: NavController, viewModel: CertificationViewModel = hiltViewModel()) {
    val certifications by viewModel.certifications.collectAsState()
    var showSubmitDialog by remember { mutableStateOf(false) }

    LaunchedEffect(Unit) { viewModel.loadCertifications() }

    Column(modifier = Modifier.fillMaxSize()) {
        TopAppBar(
            title = { Text("技能认证") },
            actions = {
                TextButton(onClick = { showSubmitDialog = true }) { Text("新增认证") }
            }
        )

        if (certifications.isEmpty()) {
            Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                Text("暂无认证记录", color = MaterialTheme.colorScheme.onSurfaceVariant)
            }
        } else {
            LazyColumn(
                contentPadding = PaddingValues(16.dp),
                verticalArrangement = Arrangement.spacedBy(8.dp)
            ) {
                items(certifications) { cert ->
                    CertificationItem(cert)
                }
            }
        }
    }

    if (showSubmitDialog) {
        SubmitCertificationDialog(
            onDismiss = { showSubmitDialog = false },
            onSubmit = { skillName, certName, certNo ->
                viewModel.submitCertification(skillName, certName, certNo)
                showSubmitDialog = false
            }
        )
    }
}

@Composable
fun CertificationItem(cert: CraftsmanCertificationDto) {
    Card(modifier = Modifier.fillMaxWidth()) {
        Column(modifier = Modifier.padding(16.dp)) {
            Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                Text(cert.skillName, fontWeight = FontWeight.Bold, fontSize = 16.sp)
                Text(
                    when (cert.status) { "Approved" -> "已通过"; "Pending" -> "审核中"; else -> "已拒绝" },
                    fontSize = 13.sp,
                    color = when (cert.status) { "Approved" -> Primary; "Pending" -> MaterialTheme.colorScheme.onSurfaceVariant; else -> MaterialTheme.colorScheme.error }
                )
            }
            Spacer(modifier = Modifier.height(4.dp))
            Text("证书: ${cert.certificateName}", fontSize = 13.sp, color = MaterialTheme.colorScheme.onSurfaceVariant)
            Text("编号: ${cert.certificateNo}", fontSize = 13.sp, color = MaterialTheme.colorScheme.onSurfaceVariant)
            if (cert.rejectReason != null) {
                Text("拒绝原因: ${cert.rejectReason}", fontSize = 13.sp, color = MaterialTheme.colorScheme.error)
            }
        }
    }
}

@Composable
fun SubmitCertificationDialog(onDismiss: () -> Unit, onSubmit: (String, String, String) -> Unit) {
    var skillName by remember { mutableStateOf("") }
    var certName by remember { mutableStateOf("") }
    var certNo by remember { mutableStateOf("") }

    AlertDialog(
        onDismissRequest = onDismiss,
        title = { Text("提交技能认证") },
        text = {
            Column {
                OutlinedTextField(value = skillName, onValueChange = { skillName = it }, label = { Text("技能名称") }, modifier = Modifier.fillMaxWidth())
                Spacer(modifier = Modifier.height(8.dp))
                OutlinedTextField(value = certName, onValueChange = { certName = it }, label = { Text("证书名称") }, modifier = Modifier.fillMaxWidth())
                Spacer(modifier = Modifier.height(8.dp))
                OutlinedTextField(value = certNo, onValueChange = { certNo = it }, label = { Text("证书编号") }, modifier = Modifier.fillMaxWidth())
            }
        },
        confirmButton = {
            Button(onClick = { onSubmit(skillName, certName, certNo) }) { Text("提交") }
        },
        dismissButton = {
            TextButton(onClick = onDismiss) { Text("取消") }
        }
    )
}