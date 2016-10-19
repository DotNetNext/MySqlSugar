﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySqlSugar
{
    //局部类：解析属性
    internal partial class ResolveExpress
    {
        private string GetProMethod(string methodName, string value, bool isField)
        {
            switch (methodName)
            {
                case "Length":
                    return ProLength(value, isField);
                default: throw new SqlSugarException("不支持属性扩展方法" + methodName + "。");
            }
        }
        private string ProLength(string value, bool isField)
        {
            if (isField)
            {
                return string.Format("length({0})", value.GetTranslationSqlName());
            }
            else
            {
                return string.Format("{0}", value.ObjToString().Length);
            }
        }
    }
}
