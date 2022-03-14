@echo off
color 0a
echo 相互科技2022(TM)保留所有权利
title 正在启动黑名单模式...
echo 正在启动自动文件备份(AutoFileBAK)黑名单服务
echo 插入usb以写入黑名单,下次在正常模式插入时将自动识别并阻止备份..
AutoFileBAK.exe --BlackListMode
pause