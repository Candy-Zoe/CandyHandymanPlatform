# 万能工匠平台 (Universal Handyman Platform)

一个连接技能工匠与顾客的O2O服务平台，支持多端访问。

## 技术栈

| 端 | 技术 |
|---|------|
| 后端API | .NET 9 Web API + Entity Framework Core + SQLite |
| Android | Kotlin + Jetpack Compose + Hilt + Material 3 |
| 桌面管理端 | WPF (.NET 9) + CommunityToolkit.Mvvm |
| 微信小程序 | 原生 WXML/WXSS/JS |

## 项目结构

```
CandyHandymanPlatform-/
├── src/                              # 后端项目
│   ├── CandyHandyman.Core/           # 领域层(实体/枚举)
│   ├── CandyHandyman.Application/    # 应用层(DTO/接口)
│   ├── CandyHandyman.Infrastructure/ # 基础设施层(EF Core/仓储/服务)
│   └── CandyHandyman.Api/            # API层(控制器)
├── android/                          # Android手机端
├── desktop/                          # WPF电脑管理端
└── wechat-miniprogram/               # 微信小程序端
```

## 功能模块

### 用户角色
- **顾客**: 浏览服务、下单、评价
- **工匠**: 发布服务、接单、管理订单
- **管理员**: 用户管理、内容审核、数据统计

### 核心功能
- 技能展示（图片/文字/视频）
- 按时间/数量/固定价格收费
- 实时聊天沟通
- 订单全生命周期管理
- 评价评分系统
- 分类搜索发现

## 快速启动

### 后端API
```bash
cd src/CandyHandyman.Api
dotnet run
# API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

### Android
用 Android Studio 打开 `android/` 目录，Gradle同步后运行。

### 微信小程序
用微信开发者工具导入 `wechat-miniprogram/` 目录。

## API端点

- `POST /api/auth/register` - 注册
- `POST /api/auth/login` - 登录
- `GET /api/services` - 服务列表
- `POST /api/services` - 发布服务
- `POST /api/orders` - 创建订单
- `GET /api/orders` - 订单列表
- `GET /api/chat/conversations` - 会话列表
- `POST /api/chat/messages` - 发送消息
- `POST /api/reviews` - 提交评价
