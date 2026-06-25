# 万能工匠平台 (Universal Handyman Platform)

一个连接技能工匠与顾客的O2O服务平台，支持多端访问。

## 技术栈

| 端 | 技术 |
|---|------|
| 后端API | .NET 9 Web API + Entity Framework Core + SQLite + SignalR |
| Android | Kotlin + Jetpack Compose + Hilt + Material 3 |
| 桌面管理端 | WPF (.NET 9) + CommunityToolkit.Mvvm |
| 微信小程序 | 原生 WXML/WXSS/JS |

## 项目结构

```
CandyHandymanPlatform/
├── src/                                # 后端项目
│   ├── CandyHandyman.Core/             # 领域层(实体/枚举)
│   │   ├── Entities/                   # 实体定义
│   │   └── Enums/                      # 枚举定义
│   ├── CandyHandyman.Application/      # 应用层(DTO/接口)
│   ├── CandyHandyman.Infrastructure/   # 基础设施层(EF Core/仓储/服务)
│   └── CandyHandyman.Api/             # API层(控制器/SignalR Hub)
├── android/                            # Android手机端
├── desktop/                            # WPF电脑管理端
└── wechat-miniprogram/                 # 微信小程序端
```

## 功能模块

### 用户角色
- **顾客**: 浏览服务、下单、支付、评价、申诉
- **工匠**: 发布服务、接单、管理订单、上传资质认证
- **管理员**: 仪表盘数据统计、用户管理、订单管理、服务审核

### 核心功能
- 技能展示（图片/视频媒体上传）
- 按时间/数量/固定价格收费
- 实时聊天沟通（SignalR WebSocket）
- 订单全生命周期管理（待接单→已接单→进行中→已完成/已取消/争议中）
- 评价评分系统
- 分类搜索发现（支持树形分类、关键词搜索）
- 附近工匠发现（基于地理位置 Haversine 算法）
- 在线状态实时通知
- 输入状态提示（Typing）
- 位置共享通知

### 通知系统
- 多渠道推送（FCM/微信订阅消息/应用内通知）
- 通知偏好设置（按类型开关）
- 实时 SignalR 推送
- 未读计数 + 全部已读

### 工匠运营
- 工匠等级体系（初级→中级→高级→专家，自动升级）
- 排班日历管理（每周可设置可用时段）
- 预约时段自动生成与锁定
- 工匠排行榜（按评分+完成订单数排序）

### 支付完善
- 微信支付 v3 对接（JSAPI/Native）
- 退款流程
- 优惠券系统（百分比/固定金额折扣）
- 钱包余额（充值/提现/交易流水）

### 安全与信任
- 身份实名认证（审核流程）
- 工匠资质证书认证
- 订单服务保险（人身伤害/财产损失/综合保障）
- 订单争议处理
- JWT + Refresh Token 认证体系

### 高价值功能
- 收藏工匠/服务
- 浏览历史记录
- 工匠服务范围区域设定
- 订单模板（常用地址/描述快捷填写）

### 后台管理增强
- 数据统计报表（日/周/月趋势、Top服务排行）
- 公告管理（发布/编辑/定时过期）
- 反馈管理（用户反馈+官方回复）
- 帮助中心管理

### 用户体验
- 深色模式（Android动态主题+系统跟随）
- 多语言支持框架（中文/英文）
- 帮助中心（分类浏览+搜索）
- 用户意见反馈

### 性能优化
- 数据库索引优化（15+复合索引覆盖高频查询）
- 响应压缩（Brotli/Gzip）
- 内存缓存（IMemoryCache + 自定义CacheService）
- 查询优化（延迟加载、投影查询）

### 安全加固
- XSS防护（HTML标签移除+编码）
- SQL注入防护（EF Core参数化查询+输入验证）
- 敏感词过滤（内容审核）
- 速率限制（内存限流器）
- 操作审计日志
- 强密码策略（8位+大小写+数字+特殊字符）
- 输入验证属性（Phone/NoHtml/NoSqlInjection）

### 测试覆盖
- 单元测试：27个（xUnit + FluentAssertions）
- 覆盖模块：密码服务、安全服务、缓存服务、限流器、验证属性
- 测试命令：`dotnet test`

## 快速启动

### 后端API
```bash
cd src/CandyHandyman.Api
dotnet run
# API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

首次启动自动创建数据库并填充种子数据。

### Android
用 Android Studio 打开 `android/` 目录，Gradle 同步后运行。

### 桌面管理端
```bash
cd desktop/CandyHandyman.Desktop
dotnet run
```

管理端包含：仪表盘、用户管理、订单管理、服务管理、评价管理、资质管理、保险管理、争议管理、分类管理、实名认证管理。

### 微信小程序
用微信开发者工具导入 `wechat-miniprogram/` 目录。

## API 端点

### 认证 `/api/auth`
| 方法 | 端点 | 说明 |
|------|------|------|
| POST | `/api/auth/register` | 注册 |
| POST | `/api/auth/login` | 登录 |
| POST | `/api/auth/refresh` | 刷新 Token |

### 用户 `/api/users`
| 方法 | 端点 | 说明 |
|------|------|------|
| GET | `/api/users/me` | 获取当前用户信息 |
| PUT | `/api/users/me` | 更新个人资料（昵称/简介/位置） |

### 服务 `/api/services`
| 方法 | 端点 | 说明 |
|------|------|------|
| GET | `/api/services` | 服务列表（支持分类/关键词/定价方式筛选+分页） |
| GET | `/api/services/{id}` | 服务详情（自动增加浏览量） |
| POST | `/api/services` | 发布服务（需工匠身份） |
| POST | `/api/services/{id}/media` | 上传服务媒体文件（图片/视频） |

### 分类 `/api/categories`
| 方法 | 端点 | 说明 |
|------|------|------|
| GET | `/api/categories` | 获取树形分类列表 |
| GET | `/api/categories/{id}/services` | 获取分类下的服务 |

### 订单 `/api/orders`
| 方法 | 端点 | 说明 |
|------|------|------|
| POST | `/api/orders` | 创建订单 |
| GET | `/api/orders` | 订单列表（支持状态筛选） |
| GET | `/api/orders/{id}` | 订单详情 |
| PUT | `/api/orders/{id}/accept` | 接单 |
| PUT | `/api/orders/{id}/start` | 开始服务 |
| PUT | `/api/orders/{id}/complete` | 完成服务 |
| PUT | `/api/orders/{id}/cancel` | 取消订单 |

### 支付 `/api/payments`
| 方法 | 端点 | 说明 |
|------|------|------|
| POST | `/api/payments/create` | 创建支付 |
| POST | `/api/payments/wechat/create` | 创建微信支付 |
| POST | `/api/payments/wechat/notify` | 微信支付回调 |
| POST | `/api/payments/refund` | 退款 |
| POST | `/api/payments/wallet/pay` | 余额支付 |
| GET | `/api/payments/{id}/status` | 查询支付状态 |
| GET | `/api/payments/history` | 支付记录 |

### 评价 `/api/reviews`
| 方法 | 端点 | 说明 |
|------|------|------|
| POST | `/api/reviews` | 提交评价（仅限已完成订单） |
| GET | `/api/reviews/handyman/{id}` | 获取工匠评价列表 |

### 聊天 `/api/chat`
| 方法 | 端点 | 说明 |
|------|------|------|
| GET | `/api/chat/conversations` | 会话列表 |
| GET | `/api/chat/conversations/{id}/messages` | 消息记录 |
| POST | `/api/chat/messages` | 发送消息 |

### SignalR Hub `/hubs/chat`
| 事件 | 方向 | 说明 |
|------|------|------|
| `JoinConversation` | Client→Server | 加入会话组 |
| `LeaveConversation` | Client→Server | 离开会话组 |
| `SendMessage` | Client→Server | 发送消息 |
| `Typing` | Client→Server | 输入状态 |
| `MarkAsRead` | Client→Server | 标记已读 |
| `SendLocationNotification` | Client→Server | 位置共享 |
| `ReceiveMessage` | Server→Client | 接收消息 |
| `NewMessageNotification` | Server→Client | 新消息通知 |
| `UserTyping` | Server→Client | 对方正在输入 |
| `MessagesRead` | Server→Client | 消息已读通知 |
| `LocationUpdate` | Server→Client | 位置更新 |
| `UserOnline` / `UserOffline` | Server→Client | 在线状态变更 |

### 附近工匠 `/api/nearby`
| 方法 | 端点 | 说明 |
|------|------|------|
| GET | `/api/nearby` | 附近工匠列表（经纬度+半径+可选分类） |

### 实名认证 `/api/verification`
| 方法 | 端点 | 说明 |
|------|------|------|
| POST | `/api/verification` | 提交实名认证 |
| GET | `/api/verification/status` | 查询认证状态 |

### 资质认证 `/api/certifications`
| 方法 | 端点 | 说明 |
|------|------|------|
| POST | `/api/certifications` | 提交资质证书（需工匠身份） |
| GET | `/api/certifications/my` | 我的资质列表 |

### 保险 `/api/insurance`
| 方法 | 端点 | 说明 |
|------|------|------|
| POST | `/api/insurance` | 购买服务保险 |
| GET | `/api/insurance/order/{orderId}` | 查询订单保险 |

### 争议 `/api/disputes`
| 方法 | 端点 | 说明 |
|------|------|------|
| POST | `/api/disputes` | 提交争议 |
| GET | `/api/disputes/my` | 我的争议列表 |
| GET | `/api/disputes/{id}` | 争议详情 |

### 管理后台 `/api/admin`
| 方法 | 端点 | 说明 |
|------|------|------|
| GET | `/api/admin/dashboard` | 仪表盘统计数据 |
| GET | `/api/admin/stats/daily` | 日统计（近N天趋势） |
| GET | `/api/admin/stats/overview` | 综合概览统计 |
| GET | `/api/admin/stats/top-services` | Top服务排行 |
| GET | `/api/admin/users` | 用户列表（分页） |
| GET | `/api/admin/orders` | 订单列表（分页） |

### 通知 `/api/notifications`
| 方法 | 端点 | 说明 |
|------|------|------|
| GET | `/api/notifications` | 通知列表（分页+筛选） |
| GET | `/api/notifications/unread-count` | 未读数量 |
| PUT | `/api/notifications/{id}/read` | 标记已读 |
| PUT | `/api/notifications/read-all` | 全部已读 |
| DELETE | `/api/notifications/{id}` | 删除通知 |
| GET | `/api/notifications/settings` | 通知设置 |
| PUT | `/api/notifications/settings` | 更新通知设置 |
| POST | `/api/notifications/fcm-token` | 注册 FCM Token |

### 排班 `/api/schedule`
| 方法 | 端点 | 说明 |
|------|------|------|
| GET | `/api/schedule/{handymanId}` | 获取排班 |
| PUT | `/api/schedule` | 更新排班（批量） |
| GET | `/api/schedule/{handymanId}/slots` | 查询可用时段 |
| POST | `/api/schedule/{handymanId}/slots/generate` | 生成预约时段 |

### 排行榜 `/api/rankings`
| 方法 | 端点 | 说明 |
|------|------|------|
| GET | `/api/rankings/handymen` | 工匠排行（按分类+评分） |

### 钱包 `/api/wallet`
| 方法 | 端点 | 说明 |
|------|------|------|
| GET | `/api/wallet/balance` | 查询余额 |
| POST | `/api/wallet/recharge` | 充值 |
| POST | `/api/wallet/withdraw` | 提现 |
| GET | `/api/wallet/transactions` | 交易流水（分页） |

### 优惠券 `/api/coupons`
| 方法 | 端点 | 说明 |
|------|------|------|
| POST | `/api/coupons` | 创建优惠券（管理员） |
| GET | `/api/coupons` | 优惠券列表（管理员） |
| POST | `/api/coupons/validate` | 验证优惠码 |
| GET | `/api/coupons/my` | 我的优惠券 |

### 收藏 `/api/favorites`
| 方法 | 端点 | 说明 |
|------|------|------|
| POST | `/api/favorites` | 添加收藏 |
| DELETE | `/api/favorites` | 取消收藏 |
| GET | `/api/favorites` | 我的收藏列表 |
| GET | `/api/favorites/check` | 检查是否已收藏 |

### 浏览历史 `/api/browsinghistory`
| 方法 | 端点 | 说明 |
|------|------|------|
| POST | `/api/browsinghistory` | 记录浏览 |
| GET | `/api/browsinghistory` | 浏览历史列表 |
| DELETE | `/api/browsinghistory` | 清空历史 |

### 订单模板 `/api/ordertemplates`
| 方法 | 端点 | 说明 |
|------|------|------|
| POST | `/api/ordertemplates` | 创建模板 |
| GET | `/api/ordertemplates` | 我的模板列表 |
| PUT | `/api/ordertemplates/{id}` | 更新模板 |
| DELETE | `/api/ordertemplates/{id}` | 删除模板 |

### 公告 `/api/announcements`
| 方法 | 端点 | 说明 |
|------|------|------|
| GET | `/api/announcements` | 已发布公告（公开） |
| GET | `/api/announcements/admin` | 所有公告（管理员） |
| POST | `/api/announcements` | 创建公告（管理员） |
| PUT | `/api/announcements/{id}` | 更新公告（管理员） |
| POST | `/api/announcements/{id}/publish` | 发布公告（管理员） |
| DELETE | `/api/announcements/{id}` | 删除公告（管理员） |

### 反馈 `/api/feedbacks`
| 方法 | 端点 | 说明 |
|------|------|------|
| POST | `/api/feedbacks` | 提交反馈 |
| GET | `/api/feedbacks/my` | 我的反馈列表 |
| GET | `/api/feedbacks/admin` | 所有反馈（管理员） |
| POST | `/api/feedbacks/{id}/reply` | 回复反馈（管理员） |
| PUT | `/api/feedbacks/{id}/status` | 更新状态（管理员） |

### 帮助中心 `/api/help`
| 方法 | 端点 | 说明 |
|------|------|------|
| GET | `/api/help` | 帮助列表（公开） |
| GET | `/api/help/category/{category}` | 按分类获取 |
| GET | `/api/help/{id}` | 帮助详情（增加浏览量） |
| POST | `/api/help` | 创建帮助（管理员） |
| PUT | `/api/help/{id}` | 更新帮助（管理员） |
| DELETE | `/api/help/{id}` | 删除帮助（管理员） |

## 数据模型

### 核心实体
- **User** - 用户（角色/余额/位置/简介）
- **HandymanProfile** - 工匠档案（经验/评分/等级/完成订单数/是否可接单）
- **Service** - 服务（定价方式/价格/状态/浏览量）
- **ServiceMedia** - 服务媒体（图片/视频/排序）
- **SkillCategory** - 技能分类（树形结构）
- **Order** - 订单（全生命周期状态机）
- **Payment** - 支付记录
- **Review** - 评价
- **Conversation / Message** - 会话与消息
- **Dispute** - 争议
- **InsurancePolicy** - 保险保单
- **IdentityVerification** - 身份认证
- **CraftsmanCertification** - 工匠资质证书
- **Notification** - 通知记录
- **NotificationSetting** - 用户通知偏好
- **UserFcmToken** - FCM 推送令牌
- **Schedule** - 工匠排班（每周时段）
- **AppointmentSlot** - 预约时段（可锁定）
- **Coupon** - 优惠券
- **UserCoupon** - 用户优惠券领取记录
- **WalletTransaction** - 钱包交易流水
- **Favorite** - 收藏（服务/工匠）
- **BrowsingHistory** - 浏览历史
- **ServiceArea** - 工匠服务范围
- **OrderTemplate** - 订单模板
- **Announcement** - 平台公告
- **Feedback** - 用户反馈
- **HelpTopic** - 帮助中心文章
- **OperationLog** - 操作日志审计

### 枚举
- **UserRole** - Customer / Handyman / Admin
- **OrderStatus** - Pending / Accepted / InProgress / Completed / Cancelled / Disputed
- **PricingType** - PerHour / PerUnit / Fixed
- **PaymentStatus** - Pending / Paid / Refunded
- **ServiceStatus** - Active / Inactive / Pending
- **VerificationStatus** - Pending / Approved / Rejected
- **InsuranceType** - PersonalInjury / PropertyDamage / Comprehensive
- **MessageType** - Text / Image / System
- **MediaType** - Image / Video
- **NotificationType** - OrderUpdate / PaymentUpdate / ChatMessage / System / Review / Certification / Insurance / Dispute / Promotion
- **CraftsmanLevel** - Junior / Intermediate / Senior / Expert
- **CouponType** - Percentage / Fixed
- **WalletTransactionType** - Recharge / Withdraw / Income / Payment / Refund
- **AnnouncementType** - System / Promotion / Maintenance / Notice
- **FeedbackType** - Bug / Suggestion / Complaint / Other
- **FeedbackStatus** - Pending / Processing / Resolved
