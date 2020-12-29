/*==============================================================*/
/* DBMS name:      MySQL 5.0                                    */
/* Created on:     2020/12/29 14:19:20                          */
/*==============================================================*/


drop table if exists authorizations;

drop table if exists company;

drop table if exists departments;

drop table if exists menus;

drop table if exists positions;

drop table if exists projects;

drop table if exists users;

/*==============================================================*/
/* Table: authorizations                                        */
/*==============================================================*/
create table authorizations
(
   Id                   int not null auto_increment,
   MenuId               int,
   AuthType             nvarchar(100),
   TypeId               int,
   primary key (Id)
);

alter table authorizations comment '权限表';

/*==============================================================*/
/* Table: company                                               */
/*==============================================================*/
create table company
(
   Id                   int not null auto_increment,
   Name                 nvarchar(100),
   TexNumber            nvarchar(100),
   primary key (Id)
);

alter table company comment '所属公司';

/*==============================================================*/
/* Table: departments                                           */
/*==============================================================*/
create table departments
(
   Id                   int not null auto_increment comment '自增主键',
   ParentId             int comment '父键',
   Code                 nvarchar(100) comment '部门编号',
   Name                 nvarchar(100) comment '部门名称',
   Remarks              nvarchar(1000) comment '备注',
   primary key (Id)
);

alter table departments comment '部门';

/*==============================================================*/
/* Table: menus                                                 */
/*==============================================================*/
create table menus
(
   Id                   int not null auto_increment comment '菜单自增主键',
   Name                 nvarchar(100) comment '菜单名称',
   primary key (Id)
);

alter table menus comment '菜单';

/*==============================================================*/
/* Table: positions                                             */
/*==============================================================*/
create table positions
(
   Id                   int not null auto_increment comment '岗位自增主键',
   ParentId             int comment '父键',
   Code                 nvarchar(100) comment '岗位编号',
   Name                 nvarchar(100) comment '岗位名称',
   Remarks              nvarchar(1000) comment '备注',
   primary key (Id)
);

alter table positions comment '岗位';

/*==============================================================*/
/* Table: projects                                              */
/*==============================================================*/
create table projects
(
   Id                   int not null auto_increment,
   ParentId             int,
   Code                 nvarchar(200),
   Name                 nvarchar(200),
   StartDate            datetime,
   EndDate              datetime,
   Status               tinyint,
   primary key (Id)
);

alter table projects comment '所属项目';

/*==============================================================*/
/* Table: users                                                 */
/*==============================================================*/
create table users
(
   Id                   int not null auto_increment comment '自增主键',
   Account              nvarchar(100) comment '用户名',
   Pwd                  nvarchar(100) comment '密码',
   Name                 nvarchar(100),
   Sex                  tinyint,
   Birthday             datetime,
   Remarks              nvarchar(1000) comment '备注',
   CompanyId            int,
   ProjectId            int,
   DeptId               int,
   PositionId           int,
   primary key (Id)
);

alter table users comment '用户';

