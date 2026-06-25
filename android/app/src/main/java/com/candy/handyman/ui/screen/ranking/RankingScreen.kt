package com.candy.handyman.ui.screen.ranking

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.itemsIndexed
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.candy.handyman.data.remote.dto.HandymanRankingDto
import com.candy.handyman.ui.theme.Primary

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun RankingScreen(
    navController: NavController,
    viewModel: RankingViewModel = hiltViewModel()
) {
    val rankings by viewModel.rankings.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()

    Column(modifier = Modifier.fillMaxSize()) {
        TopAppBar(title = { Text("工匠排行榜", fontWeight = FontWeight.Bold) })

        if (isLoading) {
            Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                CircularProgressIndicator()
            }
        } else {
            LazyColumn(
                modifier = Modifier.fillMaxSize(),
                contentPadding = PaddingValues(16.dp),
                verticalArrangement = Arrangement.spacedBy(8.dp)
            ) {
                itemsIndexed(rankings) { index, ranking ->
                    RankingItem(rank = index + 1, ranking = ranking)
                }
            }
        }
    }
}

@Composable
fun RankingItem(rank: Int, ranking: HandymanRankingDto) {
    Card(modifier = Modifier.fillMaxWidth()) {
        Row(
            modifier = Modifier.padding(16.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            Text(
                text = "$rank",
                fontWeight = FontWeight.Bold,
                fontSize = 20.sp,
                color = when (rank) {
                    1 -> Primary
                    2 -> MaterialTheme.colorScheme.secondary
                    3 -> MaterialTheme.colorScheme.tertiary
                    else -> MaterialTheme.colorScheme.onSurfaceVariant
                },
                modifier = Modifier.width(40.dp)
            )

            Column(modifier = Modifier.weight(1f).padding(start = 12.dp)) {
                Text(
                    text = ranking.nickname,
                    fontWeight = FontWeight.Bold,
                    fontSize = 16.sp
                )
                Text(
                    text = "${ranking.level} | 完成${ranking.totalCompletedOrders}单",
                    fontSize = 13.sp,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
            }

            Column(horizontalAlignment = Alignment.End) {
                Text(
                    text = "★ ${String.format("%.1f", ranking.averageRating)}",
                    fontWeight = FontWeight.Bold,
                    fontSize = 16.sp,
                    color = Primary
                )
                Text(
                    text = "${ranking.totalReviews}条评价",
                    fontSize = 12.sp,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
            }
        }
    }
}
