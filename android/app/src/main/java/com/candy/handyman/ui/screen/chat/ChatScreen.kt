package com.candy.handyman.ui.screen.chat

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.lazy.rememberLazyListState
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Send
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.candy.handyman.data.remote.dto.MessageDto
import com.candy.handyman.data.remote.dto.SendMessageDto

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun ChatScreen(navController: NavController, conversationId: String, viewModel: ChatViewModel = hiltViewModel()) {
    val messages by viewModel.messages.collectAsState()
    var inputText by remember { mutableStateOf("") }
    val listState = rememberLazyListState()

    LaunchedEffect(conversationId) { viewModel.loadMessages(conversationId) }

    Scaffold(
        topBar = {
            TopAppBar(title = { Text("聊天") },
                navigationIcon = { TextButton(onClick = { navController.popBackStack() }) { Text("返回") } })
        },
        bottomBar = {
            Row(modifier = Modifier.padding(8.dp), verticalAlignment = Alignment.CenterVertically) {
                OutlinedTextField(
                    value = inputText, onValueChange = { inputText = it },
                    modifier = Modifier.weight(1f), placeholder = { Text("输入消息...") }, singleLine = true
                )
                IconButton(onClick = {
                    if (inputText.isNotEmpty()) {
                        viewModel.sendMessage(SendMessageDto("", inputText))
                        inputText = ""
                    }
                }) {
                    Icon(Icons.Default.Send, contentDescription = "发送")
                }
            }
        }
    ) { padding ->
        LazyColumn(
            state = listState,
            modifier = Modifier.padding(padding).fillMaxSize().padding(horizontal = 8.dp),
            verticalArrangement = Arrangement.spacedBy(4.dp),
            contentPadding = PaddingValues(vertical = 8.dp)
        ) {
            items(messages) { msg ->
                MessageBubble(msg)
            }
        }
    }
}

@Composable
fun MessageBubble(message: MessageDto) {
    val isMe = message.senderId == "" // Will be set from preferences
    Row(
        modifier = Modifier.fillMaxWidth(),
        horizontalArrangement = if (isMe) Arrangement.End else Arrangement.Start
    ) {
        Card(modifier = Modifier.widthIn(max = 280.dp)) {
            Column(modifier = Modifier.padding(10.dp)) {
                if (!isMe) {
                    Text(message.senderName, style = MaterialTheme.typography.labelSmall, color = MaterialTheme.colorScheme.primary)
                }
                Text(message.content, style = MaterialTheme.typography.bodyMedium)
            }
        }
    }
}