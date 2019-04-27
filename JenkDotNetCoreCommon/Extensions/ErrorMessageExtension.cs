using System;

namespace JenkDotNetCoreCommon.Extensions
{
    public static class ErrorMessageExtension
    {
        /// <summary>
        /// 异常字符串处理 有InnerException message 抛出内部异常
        /// 没有内部异常 抛出 外部Exception message 统一截取前70个字符
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetErrorMessage(this Exception ex)
        {
            if (ex != null)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                if (!string.IsNullOrEmpty(ex.Message))
                {
                    if (ex.Message.Length > 70)
                    {
                        return ex.Message.Substring(0, 70) + "....";
                    }
                    else
                    {
                        return ex.Message;
                    }
                }
            }
            return "";
        }
    }
}
