﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySqlSugar
{
    /// <summary>
    /// ** 描述：SqlSugar自定义异常
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public class SqlSugarException : Exception
    {
        public SqlSugarException(string message)
            : base(message)
        {

        }
    }
}
