# Wealthome 家庭账本

一个简单的支持单人和多个家庭成员录入收支信息并汇总分析的账本工具，可以部署在轻量级云服务器、NAS或树莓派上。

* **前端**：React
* **后端**：ASP.NET Core 8
* **数据库**：SQLite 3

## 功能

1. 单人/多家庭成员支持：支持多个家庭成员使用不同账户录入和管理收支信息。
2. 复式记账：类似BeanCount的双向收支信息记录，能够跟踪多个账户的变化。
3. 报表统计：通过图表展示家庭收支情况。

## 截图

<div style="display: flex; justify-content: space-between; gap: 10px;">
<img src="doc/1.webp" style="flex: 1; max-width: 50%; height: auto;">
<img src="doc/2.webp" style="flex: 1; max-width: 50%; height: auto;">
</div>

## 使用Docker部署

使用Docker部署可以参考如下命令。

```bash
docker run --name wealthome -p 5000:5000 -v wealthome-data:/data -d ghcr.io/gacfox/wealthome:latest
```

## 构建和运行项目

**环境要求**：构建项目需要安装.NET SDK 8、NodeJS 18。

数据库和头像默认分别保存在应用内容目录的`Data/wealthome.db`和`Data/Avatar`，相对路径不受进程启动目录影响。需要自定义存储位置时，无需修改配置文件，可以通过`ConnectionStrings__DefaultConnection`和`App__AvatarSaveDir`环境变量覆盖；开发环境也可以使用.NET User Secrets。执行以下命令分别编译前端和服务端程序。

```bash
cd Frontend && npm install && npm run build:production
```

```bash
dotnet publish -c Release --self-contained true --runtime linux-x64 -o bin/release/net8.0/publish
```

首次启动时，服务端会通过EF Core自动创建SQLite3数据库和数据表，无需手动执行建表脚本。如果需要从头初始化一个空账本，可以停止服务后删除`Data`目录再重新启动；该操作会永久删除现有账本数据。

构建并发布完成后并创建数据库后，我们可以在服务器上执行以下命令以生产环境模式启动服务。

```bash
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://0.0.0.0:5000"
nohup ./Gacfox.Wealthome > wealthome.log 2>&1 &
```
