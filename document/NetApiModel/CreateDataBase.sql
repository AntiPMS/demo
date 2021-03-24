/*==============================================================*/
/* DBMS name:      MySQL 5.0                                    */
/* Created on:     2021/3/22 14:26:40                           */
/*==============================================================*/


drop table if exists CloudHospital_ApplyInfo;

drop table if exists GroupRelationship;

drop table if exists Groups;

drop table if exists UserMsg;

drop table if exists UserRelationship;

drop table if exists doctor;

drop table if exists ms_brda;

/*==============================================================*/
/* Table: CloudHospital_ApplyInfo                               */
/*==============================================================*/
create table CloudHospital_ApplyInfo
(
   ddbm                 int not null,
   brid                 varchar(100),
   ysgh                 varchar(100),
   primary key (ddbm)
);

/*==============================================================*/
/* Table: GroupRelationship                                     */
/*==============================================================*/
create table GroupRelationship
(
   GroupId              int not null comment '群组主键',
   GroupName            nvarchar(500) comment '群组名称',
   UserId               varchar(100) not null comment '用户主键',
   UserType             tinyint not null default 0 comment '用户类型（默认0）：0=患者、1=医生',
   Authority            tinyint default 0 comment '权限级别（默认0）：0=群员 ，1=管理',
   AddDate              datetime default CURRENT_TIMESTAMP comment '添加日期',
   primary key (GroupId, UserId, UserType)
);

alter table GroupRelationship comment '群组关系表';

/*==============================================================*/
/* Table: Groups                                                */
/*==============================================================*/
create table Groups
(
   Id                   int not null auto_increment comment '群组主键',
   GroupName            nvarchar(100) not null comment '群聊名称',
   OwnerId              varchar(100) not null comment '群聊创建人',
   OwnerName            nvarchar(100) comment '创建人姓名',
   Remark               nvarchar(2000) comment '备注信息',
   AddDate              datetime default CURRENT_TIMESTAMP comment '创建日期',
   primary key (Id)
);

alter table Groups comment '用户群组';

/*==============================================================*/
/* Table: UserMsg                                               */
/*==============================================================*/
create table UserMsg
(
   Id                   varchar(36) not null,
   SenderId             varchar(100) comment '发送人Id',
   SenderName           nvarchar(100) comment '发送人Id',
   TargetId             varchar(100) comment '接收者Id',
   MsgType              tinyint comment '消息类型：加入=0  纯文本=1  图片=2  断开连接=9',
   Msg                  mediumtext comment '消息文本',
   SendDate             datetime default CURRENT_TIMESTAMP comment '发送日期',
   IsRead               tinyint default 0 comment '已读：0=未读 1=已读 ',
   ApplyInfoId          nvarchar(100) comment '咨询申请单Id',
   primary key (Id)
);

alter table UserMsg comment '用户历史消息表';

/*==============================================================*/
/* Table: UserRelationship                                      */
/*==============================================================*/
create table UserRelationship
(
   UserId               varchar(100) not null comment '用户主键',
   TargetId             varchar(100) not null comment '好友主键',
   TargetName           nvarchar(100) comment '好友名称',
   TargetType           tinyint default 0 comment '联系人类型：0=患者，1=医生 。默认0',
   AddDate              Datetime default CURRENT_TIMESTAMP comment '添加日期',
   Remark               nvarchar(2000) comment '备注',
   primary key (UserId, TargetId)
);

alter table UserRelationship comment '用户关系表';

/*==============================================================*/
/* Table: doctor                                                */
/*==============================================================*/
create table doctor
(
   jobnumber            varchar(100) not null,
   primary key (jobnumber)
);

/*==============================================================*/
/* Table: ms_brda                                               */
/*==============================================================*/
create table ms_brda
(
   brid                 varchar(100) not null,
   primary key (brid)
);

