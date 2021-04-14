# DotNetGame

#### 介绍
基于.net core的游戏框架，充分利用Task异步编程提升服务器性能

采用SignalR进行客户端与服务端通讯

采用GRpc进行服务端进程间通讯

采用JWT、IdentityServer4等技术实现登陆及授权验证


#### 软件架构
AccountServer 账号登陆验证服务器

AgentServer 网关服务器，负责客户端连接

GameServer 游戏逻辑服务器

DBServer 数据库服务器

HttpServer Http服务器，处理外部http消息

