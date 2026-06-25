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

### 安全与信任
- 身份实名认证（审核流程）
- 工匠资质证书认证
- 订单服务保险（人身伤害/财产损失/综合保障）
- 订单争议处理
- JWT + Refresh Token 认证体系

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
| GET | `/api/admin/users` | 用户列表（分页） |
| GET | `/api/admin/orders` | 订单列表（分页） |

## 数据模型

### 核心实体
- **User** - 用户（角色/余额/位置/简介）
- **HandymanProfile** - 工匠档案（经验/评分/是否可接单）
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
