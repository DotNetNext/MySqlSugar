﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySqlSugar
{

    /// <summary>
    /// join类型
    /// </summary>
    public enum JoinType
    {
        INNER = 0,
        LEFT = 1,
        RIGHT = 2
    }
    /// <summary>
    /// Apply类型
    /// </summary>
    public enum ApplyType
    {
        CORSS = 1,
        OUTRE = 2
    }
    /// <summary>
    /// 排序类型
    /// </summary>
    public enum OrderByType
    {
        asc = 0,
        desc = 1
    }
    /// <summary>
    /// 分页类型
    /// </summary>
    public enum PageModel
    {
        Default = 0,
    }
}
