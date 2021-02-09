/*==============================================================*/
/* DBMS name:      MySQL 5.0                                    */
/* Created on:     2021/2/9 13:32:06                            */
/*==============================================================*/


drop table if exists UserMsg;

/*==============================================================*/
/* Table: UserMsg                                               */
/*==============================================================*/
create table UserMsg
(
   Id                   varchar(36) not null,
   SenderId             varchar(100) comment '发送人Id',
   TargetId             varchar(100) comment '接收者Id',
   MsgType              tinyint comment '消息类型：加入=0  纯文本=1  图片=2  断开连接=9',
   Msg                  mediumtext comment '消息文本',
   SendDate             datetime default CURRENT_TIMESTAMP comment '发送日期',
   IsRead               tinyint default 0 comment '已读：0=未读 1=已读 ',
   primary key (Id)
);

alter table UserMsg comment '用户历史消息表';

