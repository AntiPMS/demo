/*==============================================================*/
/* DBMS name:      MySQL 5.0                                    */
/* Created on:     2020/12/25 10:29:59                          */
/*==============================================================*/


drop table if exists Users;

/*==============================================================*/
/* Table: Users                                                 */
/*==============================================================*/
create table Users
(
   Id                   int not null auto_increment,
   uname                nvarchar(100) comment '用户名',
   pwd                  nvarchar(100) comment '密码',
   remark               nvarchar(1000) comment '备注',
   primary key (Id)
);

alter table Users comment '用户';

