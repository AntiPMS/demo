# AntiPMS
项目管理系统

## 开发环境
### 数据库
[MySql Community-8.0.22](https://dev.mysql.com/downloads/)
   1. 切换路径 `cd Document/NetApiModel`
   2. 建表 `mysql -uroot -p密码 NetApi<CreateDataBase.sql`
   3. 插入数据 `mysql -uroot -p密码 NetApi<InsertData.sql`

### 前端
   1. npm换源  `npm config set registry https://registry.npm.taobao.org`
   2. 新建vue项目 `vue init webpack AntiPMS`
   3. 运行 `npm run dev`
   4. 访问 `http://localhost:10501/`

### 后端
[Net 5.0](https://docs.microsoft.com/zh-cn/dotnet/)

[EntityFrameWork](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/)

#### 运行api(先打开cmd)
   1. 进入api项目目录 `cd Api/NetApi`
   2. 运行net项目 `dotnet run`
   3. 浏览器访问Api
      + [本地](http://localhost:5000/swagger/index.html)
      + [线上(dev)](http://api.qinko.club/swagger/index.html)

