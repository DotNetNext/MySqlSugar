﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using MySql.Data.MySqlClient;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web;


namespace MySqlSugar
{
    /// <summary>
    /// ** 描述：SqlSugar工具类
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public class SqlSugarTool
    {
        internal static Type StringType = typeof(string);
        internal static Type IntType = typeof(int);
        internal static Type DecType = typeof(decimal);
        internal static Type GuidType = typeof(Guid);
        internal static Type DateType = typeof(DateTime);
        internal static Type ByteType = typeof(Byte);
        internal static Type BoolType = typeof(bool);
        internal static Type ObjType = typeof(object);
        internal static Type Dob = typeof(double);
        internal static Type DicSS = typeof(KeyValuePair<string, string>);
        internal static Type DicSi = typeof(KeyValuePair<string, int>);
        internal static Type Dicii = typeof(KeyValuePair<int, int>);
        internal static Type DicOO = typeof(KeyValuePair<object, object>);
        internal static Type DicSo = typeof(KeyValuePair<string, object>);
        internal static Type DicIS = typeof(KeyValuePair<int, string>);

        /// <summary>
        /// Reader转成List《T》
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="dr"></param>
        /// <param name="isClose"></param>
        /// <returns></returns>
        internal static List<T> DataReaderToList<T>(Type type, IDataReader dr, string fields, bool isClose = true, bool isTry = true)
        {
            if (type.Name.Contains("KeyValuePair"))
            {
                List<T> strReval = new List<T>();
                FillValueTypeToDictionary(type, dr, strReval);
                return strReval;
            }
            //值类型
            else if (type.IsValueType || type == SqlSugarTool.StringType)
            {
                List<T> strReval = new List<T>();
                FillValueTypeToDr<T>(type, dr, strReval);
                return strReval;
            }
            //数组类型
            else if (type.IsArray)
            {
                List<T> strReval = new List<T>();
                FillValueTypeToArray(type, dr, strReval);
                return strReval;
            }


            var cacheManager = CacheManager<IDataReaderEntityBuilder<T>>.GetInstance();
            string key = "DataReaderToList." + fields + type.FullName;
            IDataReaderEntityBuilder<T> eblist = null;
            if (cacheManager.ContainsKey(key))
            {
                eblist = cacheManager[key];
            }
            else
            {
                eblist = IDataReaderEntityBuilder<T>.CreateBuilder(type, dr);
                cacheManager.Add(key, eblist, cacheManager.Day);
            }
            List<T> list = new List<T>();
            try
            {
                if (dr == null) return list;
                while (dr.Read())
                {
                    list.Add(eblist.Build(dr));
                }
                if (isClose) { dr.Close(); dr.Dispose(); dr = null; }
            }
            catch (Exception ex)
            {
                if (isTry)//解决实体变更缓存引起的错误
                {

                    try
                    {
                        if (cacheManager.ContainsKey(key))
                        {
                            //清除实体缓存 
                            cacheManager.Remove(key);
                            return DataReaderToList<T>(type, dr, fields, isClose, false);
                        }
                    }
                    catch (Exception innerEx)
                    {
                        throw new Exception("可能实体与数据库类型不匹配，请用 var str = db.ClassGenerating.TableNameToClass(db, \"类名\") 查看正确的实体！！\r\n具体错误信息:" + innerEx.Message);
                    }
                    finally
                    {
                        if (isClose) { dr.Close(); dr.Dispose(); dr = null; }
                    }

                }
                else
                {
                    if (isClose) { dr.Close(); dr.Dispose(); dr = null; }
                    throw ex;
                }
            }
            return list;
        }

        private static void FillValueTypeToDr<T>(Type type, IDataReader dr, List<T> strReval)
        {
            using (IDataReader re = dr)
            {
                while (re.Read())
                {
                    strReval.Add((T)Convert.ChangeType(re.GetValue(0), type));
                }
            }
        }
        private static void FillValueTypeToDictionary<T>(Type type, IDataReader dr, List<T> strReval)
        {
            using (IDataReader re = dr)
            {
                Dictionary<string, string> reval = new Dictionary<string, string>();
                while (re.Read())
                {
                    if (SqlSugarTool.DicOO == type)
                    {
                        var kv = new KeyValuePair<object, object>((object)Convert.ChangeType(re.GetValue(0), typeof(object)), (int)Convert.ChangeType(re.GetValue(1), typeof(object)));
                        strReval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<object, object>)));
                    }
                    else if (SqlSugarTool.Dicii == type)
                    {
                        var kv = new KeyValuePair<int, int>((int)Convert.ChangeType(re.GetValue(0), typeof(int)), (int)Convert.ChangeType(re.GetValue(1), typeof(int)));
                        strReval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<int, int>)));
                    }
                    else if (SqlSugarTool.DicSi == type)
                    {
                        var kv = new KeyValuePair<string, int>((string)Convert.ChangeType(re.GetValue(0), typeof(string)), (int)Convert.ChangeType(re.GetValue(1), typeof(int)));
                        strReval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, int>)));
                    }
                    else if (SqlSugarTool.DicSo == type)
                    {
                        var kv = new KeyValuePair<string, object>((string)Convert.ChangeType(re.GetValue(0), typeof(string)), (object)Convert.ChangeType(re.GetValue(1), typeof(object)));
                        strReval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, object>)));
                    }
                    else if (SqlSugarTool.DicSS == type)
                    {
                        var kv = new KeyValuePair<string, string>((string)Convert.ChangeType(re.GetValue(0), typeof(string)), (string)Convert.ChangeType(re.GetValue(1), typeof(string)));
                        strReval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, string>)));
                    }
                    else
                    {
                        Check.Exception(true, "暂时不支持该类型的Dictionary 你可以试试 Dictionary<string ,string>或者联系作者！！");
                    }
                }
            }
        }
        private static void FillValueTypeToArray<T>(Type type, IDataReader dr, List<T> strReval)
        {
            using (IDataReader re = dr)
            {
                int count = dr.FieldCount;
                var childType = type.GetElementType();
                while (re.Read())
                {
                    object[] array = new object[count];
                    for (int i = 0; i < count; i++)
                    {
                        array[i] = Convert.ChangeType(re.GetValue(i), childType);
                    }
                    if (childType == SqlSugarTool.StringType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => (string)it).ToArray(), type));
                    else if (childType == SqlSugarTool.ObjType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => (object)it).ToArray(), type));
                    else if (childType == SqlSugarTool.BoolType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => (bool)it).ToArray(), type));
                    else if (childType == SqlSugarTool.ByteType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => (byte)it).ToArray(), type));
                    else if (childType == SqlSugarTool.DecType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => (decimal)it).ToArray(), type));
                    else if (childType == SqlSugarTool.GuidType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => (Guid)it).ToArray(), type));
                    else if (childType == SqlSugarTool.DateType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => (DateTime)it).ToArray(), type));
                    else if (childType == SqlSugarTool.IntType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => (int)it).ToArray(), type));
                    else
                        Check.Exception(true, "暂时不支持该类型的Array 你可以试试 object[] 或者联系作者！！");
                }
            }
        }
        /// <summary>
        /// 将实体对象转换成SqlParameter[] 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static MySqlParameter[] GetParameters(object obj)
        {
            List<MySqlParameter> listParams = new List<MySqlParameter>();
            if (obj != null)
            {
                var type = obj.GetType();
                var propertiesObj = type.GetProperties();
                string replaceGuid = Guid.NewGuid().ToString();
                foreach (PropertyInfo r in propertiesObj)
                {
                    var value = r.GetValue(obj, null);
                    if (r.PropertyType.IsEnum)
                    {
                        value = (int)value;
                    }
                    if (value == null) value = DBNull.Value;
                    if (r.Name.ToLower().Contains("hierarchyid"))
                    {

                    }
                    else
                    {
                        listParams.Add(new MySqlParameter("@" + r.Name, value));
                    }
                }
            }
            return listParams.ToArray();
        }

        /// <summary>
        /// 将实体对象转换成 Dictionary《string, string》
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static Dictionary<string, object> GetObjectToDictionary(object obj)
        {

            Dictionary<string, object> reval = new Dictionary<string, object>();
            if (obj == null) return reval;
            var type = obj.GetType();
            var propertiesObj = type.GetProperties();
            string replaceGuid = Guid.NewGuid().ToString();
            foreach (PropertyInfo r in propertiesObj)
            {
                var val = r.GetValue(obj, null);
                if (r.PropertyType.IsEnum)
                {
                    val = (int)val;
                }
                reval.Add(r.Name, val == null ? DBNull.Value : val);
            }

            return reval;
        }

        /// <summary>
        /// 获取type属性cache
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cachePropertiesKey"></param>
        /// <param name="cachePropertiesManager"></param>
        /// <returns></returns>
        internal static PropertyInfo[] GetGetPropertiesByCache(Type type, string cachePropertiesKey, CacheManager<PropertyInfo[]> cachePropertiesManager)
        {
            PropertyInfo[] props = null;
            if (cachePropertiesManager.ContainsKey(cachePropertiesKey))
            {
                props = cachePropertiesManager[cachePropertiesKey];
            }
            else
            {
                props = type.GetProperties();
                cachePropertiesManager.Add(cachePropertiesKey, props, cachePropertiesManager.Day);
            }
            return props;
        }


        /// <summary>
        /// 判段是否包含主键
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        internal static bool IsPrimaryKey(SqlSugarClient db, string tableName)
        {
            return GetPrimaryKeyByTableName(db, tableName) != null;
        }

        /// <summary>
        ///根据表名获取自添列 keyTableName Value columnName
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        internal static List<KeyValue> GetIdentitiesKeyByTableName(SqlSugarClient db, string tableName)
        {
            string key = "GetIdentityKeyByTableName" + tableName;
            var cm = CacheManager<List<KeyValue>>.GetInstance();
            List<KeyValue> identityInfo = null;
            string sql = string.Format(@"
                           select TABLE_NAME as tableName,COLUMN_NAME as keyName  from INFORMATION_SCHEMA.COLUMNS
                       where table_name='"+tableName+@"' AND EXTRA='auto_increment';
         ", tableName);
            if (cm.ContainsKey(key))
            {
                identityInfo = cm[key];
                return identityInfo;
            }
            else
            {
                var dt = db.GetDataTable(sql);
                identityInfo = new List<KeyValue>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        identityInfo.Add(new KeyValue() { Key = dr["tableName"].ToString().ToLower(), Value = dr["keyName"].ToString() });
                    }
                }
                cm.Add(key, identityInfo, cm.Day);
                return identityInfo;
            }
        }


        /// <summary>
        /// 根据表获取主键
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        internal static string GetPrimaryKeyByTableName(SqlSugarClient db, string tableName)
        {
            string key = "GetPrimaryKeyByTableName" + tableName;
            tableName = tableName.ToLower();
            var cm = CacheManager<List<KeyValue>>.GetInstance();
            List<KeyValue> primaryInfo = null;

            //获取主键信息
            if (cm.ContainsKey(key))
                primaryInfo = cm[key];
            else
            {
                string sql = @"select TABLE_NAME as tableName,COLUMN_NAME as keyName from INFORMATION_SCHEMA.COLUMNS where table_name='"+tableName+"' AND COLUMN_KEY='PRI';";
                var dt = db.GetDataTable(sql);
                primaryInfo = new List<KeyValue>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        primaryInfo.Add(new KeyValue() { Key = dr["tableName"].ToString().ToLower(), Value = dr["keyName"].ToString() });
                    }
                }
                cm.Add(key, primaryInfo, cm.Day);
            }

            //反回主键
            if (!primaryInfo.Any(it => it.Key == tableName))
            {
                return null;
            }
            return primaryInfo.First(it => it.Key == tableName).Value;

        }

        /// <summary>
        /// 处理like条件的通配符
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string SqlLikeWordEncode(string word)
        {
            if (word == null) return word;
            return Regex.Replace(word, @"(\[|\%)", "[$1]");
        }

        public static string GetLockString(bool isNoLock)
        {
            return isNoLock ? " WITH(NOLOCK) " : "";
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        internal static Guid GetPropertyValue(object obj, string property)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
            return (Guid)propertyInfo.GetValue(obj, null);
        }
        /// <summary>
        /// 包装SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="shortName"></param>
        /// <returns></returns>
        internal static string PackagingSQL(string sql, string shortName)
        {
            return string.Format(" SELECT * FROM ({0}) {1} ", sql, shortName);
        }

        /// <summary>
        /// 使用页面自动填充sqlParameter时 Request.Form出现特殊字符时可以重写Request.Form方法，使用时注意加锁并且用到将该值设为null
        /// </summary>
        public static Func<string, string> SpecialRequestForm = null;

        /// <summary>
        /// 获取参数到键值集合根据页面Request参数
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetParameterDictionary(bool isNotNullAndEmpty = false)
        {
            if (SpecialRequestForm == null)
            {
                Dictionary<string, string> paraDictionaryByGet = HttpContext.Current.Request.QueryString.Keys.Cast<string>()
                       .ToDictionary(k => k, v => HttpContext.Current.Request.QueryString[v]);

                Dictionary<string, string> paraDictionaryByPost = HttpContext.Current.Request.Form.Keys.Cast<string>()
                    .ToDictionary(k => k, v => HttpContext.Current.Request.Form[v]);

                var paraDictionarAll = paraDictionaryByGet.Union(paraDictionaryByPost);
                if (isNotNullAndEmpty)
                {
                    paraDictionarAll = paraDictionarAll.Where(it => !string.IsNullOrEmpty(it.Value));
                }
                return paraDictionarAll.ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
            }
            else
            {

                var pars = HttpContext.Current.Request.Form.Keys.Cast<string>()
                     .ToDictionary(k => k, v => SpecialRequestForm(v)).Where(it => true);
                if (isNotNullAndEmpty)
                {
                    pars = pars.Where(it => !string.IsNullOrEmpty(it.Value));
                }
                return pars.ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
            }
        }

        internal static void GetSqlableSql(Sqlable sqlable, string fileds, string orderByFiled, int pageIndex, int pageSize, StringBuilder sbSql)
        {

            sbSql.Insert(0, string.Format("SELECT {0}", fileds));
            sbSql.Append(" WHERE 1=1 ").Append(string.Join(" ", sqlable.Where));
            sbSql.Append(sqlable.GroupBy);
            sbSql.AppendFormat(" ORDER BY {0} ", orderByFiled);
            int skip = (pageIndex - 1) * pageSize;
            int take = pageSize;
            if (skip > 0 || take > 0)
            {
                if (skip > 0 && take > 0)
                {
                    sbSql.AppendFormat("limit {0},{1}", skip, take);
                }
                else if (skip > 0)
                {
                    sbSql.AppendFormat("limit {0}", skip);
                }
                else if (take > 0)
                {
                    sbSql.AppendFormat("limit 0,{0}", take);
                }
            }

        }
        /// <summary>
        /// 获取参数到键值集合根据页面Request参数
        /// </summary>
        /// <returns></returns>
        public static MySqlParameter[] GetParameterArray(bool isNotNullAndEmpty = false)
        {
            Dictionary<string, string> paraDictionaryByGet = HttpContext.Current.Request.QueryString.Keys.Cast<string>()
                   .ToDictionary(k => k, v => HttpContext.Current.Request.QueryString[v]);

            Dictionary<string, string> paraDictionaryByPost = HttpContext.Current.Request.Form.Keys.Cast<string>()
                .ToDictionary(k => k, v => HttpContext.Current.Request.Form[v]);

            var paraDictionarAll = paraDictionaryByGet.Union(paraDictionaryByPost);
            if (isNotNullAndEmpty)
            {
                paraDictionarAll = paraDictionarAll.Where(it => !string.IsNullOrEmpty(it.Value));
            }
            return paraDictionarAll.Select(it => new MySqlParameter("@" + it.Key, it.Value)).ToArray();
        }

        internal static StringBuilder GetQueryableSql<T>(Queryable<T> queryable)
        {
            StringBuilder sbSql = new StringBuilder();
            string tableName = queryable.TableName.IsNullOrEmpty() ? queryable.TName : queryable.TableName;
            if (queryable.DB.Language.IsValuable() && queryable.DB.Language.Suffix.IsValuable())
            {
                var viewNameList = LanguageHelper.GetLanguageViewNameList(queryable.DB);
                var isLanView = viewNameList.IsValuable() && viewNameList.Any(it => it == tableName);
                if (!queryable.DB.Language.Suffix.StartsWith(LanguageHelper.PreSuffix))
                {
                    queryable.DB.Language.Suffix = LanguageHelper.PreSuffix + queryable.DB.Language.Suffix;
                }

                //将视图变更为多语言的视图
                if (isLanView)
                    tableName = typeof(T).Name + queryable.DB.Language.Suffix;
            }


            #region offset
            string withNoLock = queryable.DB.IsNoLock ? "WITH(NOLOCK)" : null;
            var order = queryable.OrderBy.IsValuable() ? ("ORDER BY " + queryable.OrderBy + " ") : null;
            sbSql.AppendFormat("SELECT " + queryable.Select.GetSelectFiles() + " {1} FROM {0} {2} WHERE 1=1 {3} {4} ", tableName, "", withNoLock, string.Join("", queryable.Where), queryable.GroupBy.GetGroupBy());
            sbSql.Append(order);
            if (queryable.Skip > 0 || queryable.Take > 0)
            {
                if (queryable.Skip > 0 && queryable.Take > 0)
                {
                    sbSql.AppendFormat("limit {0},{1}", queryable.Skip, queryable.Take);
                }
                else if (queryable.Skip > 0)
                {
                    sbSql.AppendFormat("limit {0}", queryable.Skip);
                }
                else if (queryable.Take > 0)
                {
                    sbSql.AppendFormat("limit 0,{0}",  queryable.Take);
                }
            }
            #endregion
            return sbSql;
        }
    }
}
