# leaf.blog

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)


简体中文

## 总览

ServiceFramework是一个面向服务的快速开发框架，采用.netcore3.1+EFCore3.1+AspectCore+EasyCaching实现。本项目中试图对ORM框架进行抽象，不过部分功能仍然依赖于EFCore，支持多DbContext操作，通过AOP方式注入数据上下文、事务管理。其致力于构建了经典三层架构中的服务层、数据层的基础设施。
已实现的功能包括：
#####   RBAC权限管理
#####   基于表达式的数据权限
#####   AOP日志与异常、缓存
##### 	动态插件化(运行时安装、升级、降级)

leaf.blog是基于ServiceFramework框架，采用asp.net core3.1开发的简易博客应用系统。详细信息访问[https://magicyqy.cn](https://qystack.top/article/1.html)

## 相关项目

[sf.vueadmin](https://github.com/magicyqy/sf.vueadmin)

## 演示

##### 后台管理
<https://demo.qystack.top>	账号：admin	密码：123456

##### 前台网站	
<https://www.qystack.top>


