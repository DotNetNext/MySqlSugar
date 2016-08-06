﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

namespace MySqlSugar
{

    /// <summary>
    /// 云计算连接配置
    /// </summary>
    public class CloudConnectionConfig
    {
        /// <summary>
        /// 处理机率,值越大机率越高
        /// </summary>
        public int Rate { get; set; }
        /// <summary>
        /// 链接字符串名称
        /// </summary>
        public string ConnectionString { get; set; }
    }

    /// <summary>
    /// 云搜索Task反回类
    /// </summary>
    public class CloudSearchResult<T>
    {
        /// <summary>
        /// 集合
        /// </summary>
        public List<T> Entities { get; set; }
        /// <summary>
        /// 单个对象
        /// </summary>
        public T Value { get; set; }
        /// <summary>
        /// DataTable
        /// </summary>
        public DataTable DataTable { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
    }



    /// <summary>
    /// 云计扩展类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Taskable<T>
    {
        /// <summary>
        /// 任务
        /// </summary>
        public Task<CloudSearchResult<T>>[] Tasks { get; set; }
        /// <summary>
        /// sql
        /// </summary>
        public string Sql { get; set; }
        /// <summary>
        /// 数据库参数(例如:new{id=1,name="张三"})
        /// </summary>
        public object WhereObj { get; set; }

    }

    /// <summary>
    /// 云计扩展类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskableWithCount<T>
    {
        /// <summary>
        /// 任务
        /// </summary>
        public Task<CloudSearchResult<T>>[] Tasks { get; set; }
        /// <summary>
        /// sql
        /// </summary>
        public string Sql { get; set; }
        /// <summary>
        /// 数据库参数(例如:new{id=1,name="张三"})
        /// </summary>
        public object WhereObj { get; set; }

    }

    internal class PageRowInnerParamsResult
    {
        public DataRow Row { get; set; }
        public int Count { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Begin { get; set; }
        public int End { get; set; }
        public string Sql { get; set; }
        public bool IsEqs { get; set; }
        public string OrderByField { get; set; }
        public string UnqueField { get; set; }
        public int RowIndex { get; set; }
        public bool isGreater { get; set; }
        public string Symbol { get; set; }
        public object OrderByValue { get; set; }
        public object UnqueValue { get; set; }
        public OrderByType OrderByType { get; set; }
        public string FullOrderByString { get; set; }
        public string FullOrderByStringReverse { get; set; }
        public object WhereObj { get; set; }
        public string SymbolReverse { get; set; }
        public int ConfigCount { get; set; }
        public OrderByType OrderByTypeReverse { get; set; }
    }

    internal class PageRowInnerParamsResultMultipleOrderBy
    {
        public DataRow Row { get; set; }
        public int Count { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Begin { get; set; }
        public int End { get; set; }
        public string Sql { get; set; }
        public string UnqueField { get; set; }
        public int RowIndex { get; set; }
        public bool isGreater { get; set; }
        public object UnqueValue { get; set; }
        public string FullOrderByString { get; set; }
        public string FullOrderByStringReverse { get; set; }
        public object WhereObj { get; set; }
        public int ConfigCount { get; set; }
 
        public string orderByFieldsString { get; set; }

        public List<OrderByDictionary> OrderByTypes { get; set; }

        public int SampleEachIndex { get; set; }
    }

    public class OrderByDictionary
    {
        public string OrderByField { get; set; }
        public OrderByType OrderByType { get; set; }
        public string OrderByString
        {
            get
            {
                return string.Format(" {0} {1} ", OrderByField, OrderByType.ToString());
            }
        }
        public string OrderByStringReverse
        {
            get
            {
                return string.Format(" {0} {1} ", OrderByField, OrderByTypeReverse.ToString());
            }
        }
        public OrderByType OrderByTypeReverse
        {
            get
            {
                return IsAsc ? OrderByType.desc : OrderByType.asc;
            }
        }
        public bool IsAsc
        {
            get
            {
                return OrderByType == OrderByType.asc;
            }
        }
        public string Symbol
        {
            get
            {
                return IsAsc ? "<" : ">";
            }
        }
        public string SymbolReverse
        {
            get
            {
                return IsAsc ? ">" : "<";
            }
        }
    }
}
