package com.candy.handyman.data.remote

import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.WebSocket
import okhttp3.WebSocketListener
import org.json.JSONObject
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class SignalRClient @Inject constructor(
    private val okHttpClient: OkHttpClient
) {

    private var webSocket: WebSocket? = null

    fun connect(token: String, onMessage: (String) -> Unit, onConnected: () -> Unit = {}, onDisconnected: () -> Unit = {}) {
        val request = Request.Builder()
            .url("ws://10.0.2.2:5000/hubs/chat?access_token=$token")
            .build()

        webSocket = okHttpClient.newWebSocket(request, object : WebSocketListener() {
            override fun onOpen(webSocket: WebSocket, response: okhttp3.Response) {
                onConnected()
            }

            override fun onMessage(webSocket: WebSocket, text: String) {
                onMessage(text)
            }

            override fun onClosing(webSocket: WebSocket, code: Int, reason: String) {
                webSocket.close(1000, null)
                onDisconnected()
            }

            override fun onFailure(webSocket: WebSocket, t: Throwable, response: okhttp3.Response?) {
                onDisconnected()
            }
        })
    }

    fun sendMessage(conversationId: String, receiverId: String, content: String, messageType: String = "Text") {
        val message = JSONObject().apply {
            put("type", 1)
            put("target", "SendMessage")
            put("arguments", org.json.JSONArray().apply {
                put(conversationId)
                put(receiverId)
                put(content)
                put(messageType)
            })
        }
        webSocket?.send(message.toString())
    }

    fun joinConversation(conversationId: String) {
        val message = JSONObject().apply {
            put("type", 1)
            put("target", "JoinConversation")
            put("arguments", org.json.JSONArray().apply { put(conversationId) })
        }
        webSocket?.send(message.toString())
    }

    fun typing(conversationId: String) {
        val message = JSONObject().apply {
            put("type", 1)
            put("target", "Typing")
            put("arguments", org.json.JSONArray().apply { put(conversationId) })
        }
        webSocket?.send(message.toString())
    }

    fun disconnect() {
        webSocket?.close(1000, "Client disconnect")
        webSocket = null
    }
}