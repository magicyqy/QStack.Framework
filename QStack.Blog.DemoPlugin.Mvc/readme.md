plugin约定配置：
1.controller 需配置[Area] 特性；
2.不同数据库的do 和 dto，需放在不同的程序集
3.如需使用automapper,需添加profile映射，且与do，dto在同一程序集
4.zip包名称应与插件主入口程序集名称一致
5.