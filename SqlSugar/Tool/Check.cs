﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySqlSugar
{
    /// <summary>
    /// ** 描述：逻辑出错抛出异常
    /// ** 创始时间：2015-7-19
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 修改人：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public class Check
    {
        /// <summary>
        /// 使用导致此异常的参数的名称初始化 System.ArgumentNullException 类的新实例。
        /// </summary>
        /// <param name="checkObj"></param>
        /// <param name="message"></param>
        public static void ArgumentNullException(object checkObj, string message)
        {
            if (checkObj == null)
                throw new ArgumentNullException(message);
        }
        /// <summary>
        /// 使用指定的错误消息初始化 System.Exception 类的新实例。
        /// </summary>
        /// <param name="isException"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Exception(bool isException, string message, params string[] args)
        {
            if (isException)
                throw new SqlSugarException(string.Format(message, args));
        }
    }

    
}
