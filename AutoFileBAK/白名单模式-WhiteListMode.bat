@echo off
color 0a
echo 相互科技2022(TM)保留所有权利
title 白名单模式
echo 正在启动自动文件备份(AutoFileBAK)白名单服务
echo 插入usb以写入白名单,下次在正常模式插入时将自动识别并将所有备份拷贝进去..
AutoFileBAK.exe --WhiteListMode
pause