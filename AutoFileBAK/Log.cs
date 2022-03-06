using System;
using System.IO;

namespace AutoFileBAK
{
    /// <summary>
    /// 日志类
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 存储日志
        /// </summary>
        /// <param name="message">日志信息</param>
        public static void SaveLog(string message)
        {
            message = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "] " + message;
            //空格是为了增强日志可读性,DateTime的作用是获取目前时间
            Directory.CreateDirectory("./Log/");
            //如果不存在Log文件夹,则创建(会略微拖慢运行速度,但是用if判断一次代码量和工作量会大很多)
            File.AppendAllText(@"./Log/Console" + DateTime.Now.ToString("yyyy-MM-dd") + ".log", "\r\n" + message);
            //AppendAllTexe是追加到文件末尾.因为文件名不能出现"/",所以这里在ToString里面指定格式为yyyy-MM-dd.
            //为了使文件便于查找,因此一天一个文件
            Console.WriteLine(message);
            //同时将信息反馈到控制台
            //return;
        }
    }
}
