package com.candy.handyman.ui.screen.chat

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.candy.handyman.data.remote.dto.*
import com.candy.handyman.data.repository.ChatRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class ChatViewModel @Inject constructor(
    private val chatRepository: ChatRepository
) : ViewModel() {
    private val _conversations = MutableStateFlow<List<ConversationDto>>(emptyList())
    val conversations = _conversations.asStateFlow()

    private val _messages = MutableStateFlow<List<MessageDto>>(emptyList())
    val messages = _messages.asStateFlow()

    fun loadConversations() {
        viewModelScope.launch { chatRepository.getConversations().onSuccess { _conversations.value = it } }
    }

    fun loadMessages(conversationId: String) {
        viewModelScope.launch { chatRepository.getMessages(conversationId).onSuccess { _messages.value = it } }
    }

    fun sendMessage(dto: SendMessageDto) {
        viewModelScope.launch {
            chatRepository.sendMessage(dto).onSuccess { msg ->
                _messages.value = _messages.value + msg
            }
        }
    }
}