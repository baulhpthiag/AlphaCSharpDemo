/*==============================================================*/
/* DBMS name:      MySQL 5.0                                    */
/* Created on:     2020/4/7 22:28:01                            */
/*==============================================================*/


drop table if exists tb_parent;

/*==============================================================*/
/* Table: tb_parent                                             */
/*==============================================================*/
create table tb_parent
(
   id                   varchar(38) not null,
   name                 varchar(100),
   birthday             datetime,
   isWorking            bool,
   age                  int,
   tall                 double,
   primary key (id)
)DEFAULT CHARSET=utf8;

