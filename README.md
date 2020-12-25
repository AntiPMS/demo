# AntiPMS
业余学习开发管理系统

## 开发环境
### 后端 
[Net 5.0](https://docs.microsoft.com/zh-cn/dotnet/)

[EntityFrameWork](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/)

### 数据库
[Mysql Community-8.0.22](https://dev.mysql.com/downloads/)

## 用法(先打开cmd)
1. 初始化数据库并插入数据
   + cd到sql脚本目录 `cd Document/NetApiModel`
   + 创建表数据 `mysql -u root -p NetApi<CreateDataBase.sql`
   + 插入初始数据 `mysql -u root -p NetApi<InsertData.sql`
2. 启动项目
   + 进入api项目目录 `cd Api/NetApi`
   + 运行net项目`dotnet run`
3. 浏览器访问Api
   + [本地Api地址](http://localhost:5000/swagger/index.html)
