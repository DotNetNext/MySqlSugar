using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;

namespace MySqlSugar
{
    /// <summary>
    /// ** 描述：底层SQL辅助函数
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public class SqlHelper : IDisposable
    {
        /// <summary>
        /// 连接对象
        /// </summary>
        protected MySqlConnection _MySqlConnection;
        /// <summary>
        /// 事务对象
        /// </summary>
        protected MySqlTransaction _tran = null;
        /// <summary>
        /// 如何解释命令字符串 默认为Text 
        /// </summary>
        public CommandType CommandType = CommandType.Text;
        /// <summary>
        /// 是否启用日志事件(默认为:false)
        /// </summary>
        public bool IsEnableLogEvent = false;
        /// <summary>
        /// 执行访数据库前的回调函数  (sql,pars)=>{}
        /// </summary>
        public Action<string, string> LogEventStarting = null;
        /// <summary>
        /// 执行访数据库后的回调函数  (sql,pars)=>{}
        /// </summary>
        public Action<string, string> LogEventCompleted = null;
        /// <summary>
        /// 是否清空MySqlParameters
        /// </summary>
        public bool IsClearParameters = true;
        /// <summary>
        /// 设置在终止执行命令的尝试并生成错误之前的等待时间。（单位：秒）
        /// </summary>
        public int CommandTimeOut = 30000;
        /// <summary>
        /// 将页面参数自动填充到MySqlParameter []，无需在程序中指定参数
        /// 例如：
        ///     var list = db.Queryable&lt;Student&gt;().Where("id=@id").ToList();
        ///     以前写法
        ///     var list = db.Queryable&lt;Student&gt;().Where("id=@id", new { id=Request["id"] }).ToList();
        /// </summary>
        public bool IsGetPageParas = false;
        /// <summary>
        /// 初始化 SqlHelper 类的新实例
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlHelper(string connectionString)
        {
            _MySqlConnection = new MySqlConnection(connectionString);
        }
        /// <summary>
        /// 主连接
        /// </summary>
        protected MySqlConnection _masterConnection = null;
        /// <summary>
        /// 从连接
        /// </summary>
        protected List<MySqlConnection> _slaveConnections = null;
        /// <summary>
        /// 初始化 SqlHelper 类的新实例
        /// </summary>
        /// <param name="masterConnectionString"></param>
        /// <param name="slaveConnectionStrings"></param>
        public SqlHelper(string masterConnectionString, params string[] slaveConnectionStrings)
        {
            _masterConnection = new MySqlConnection(masterConnectionString);
            if (slaveConnectionStrings == null || slaveConnectionStrings.Length == 0)
            {
                _slaveConnections = new List<MySqlConnection>()
                {
                    _masterConnection
                };
            }
            else
            {
                _slaveConnections = new List<MySqlConnection>();
                foreach (var item in slaveConnectionStrings)
                {
                    _slaveConnections.Add(new MySqlConnection(item));
                }
            }
        }
        /// <summary>
        /// 设置当前主从连接对象
        /// </summary>
        /// <param name="isMaster"></param>
        public virtual void SetCurrentConnection(bool isMaster)
        {
            if (_slaveConnections != null && _slaveConnections.Count > 0)//开启主从模式
            {
                if (isMaster || _tran!=null)
                {
                    _MySqlConnection = _masterConnection;
                }
                else
                {
                    var count=_slaveConnections.Count;
                    _MySqlConnection =_slaveConnections[ new Random().Next(0,count-1)];
                }
            }
        }

        /// <summary>
        /// 获取当前数据库连接对象
        /// </summary>
        /// <returns></returns>

        public virtual MySqlConnection GetConnection()
        {
            return _MySqlConnection;
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        public virtual void BeginTran()
        {
            SetCurrentConnection(true);
            CheckConnect();
            _tran = _MySqlConnection.BeginTransaction();
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="iso">指定事务行为</param>
        public virtual void BeginTran(IsolationLevel iso)
        {
            SetCurrentConnection(true);
            CheckConnect();
            _tran = _MySqlConnection.BeginTransaction(iso);
        }


        /// <summary>
        /// 回滚事务
        /// </summary>
        public virtual void RollbackTran()
        {
            SetCurrentConnection(true);
            CheckConnect();
            if (_tran != null)
            {
                _tran.Rollback();
                _tran = null;
            }
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public virtual void CommitTran()
        {
            SetCurrentConnection(true);
            CheckConnect();
            if (_tran != null)
            {
                _tran.Commit();
                _tran = null;
            }
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual string GetString(string sql, object pars)
        {
            return GetString(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual string GetString(string sql, params MySqlParameter[] pars)
        {
            return Convert.ToString(GetScalar(sql, pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual int GetInt(string sql, object pars)
        {
            return GetInt(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public int GetInt(string sql, params MySqlParameter[] pars)
        {
            return Convert.ToInt32(GetScalar(sql, pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual Double GetDouble(string sql, params MySqlParameter[] pars)
        {
            return Convert.ToDouble(GetScalar(sql, pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual decimal GetDecimal(string sql, params MySqlParameter[] pars)
        {
            return Convert.ToDecimal(GetScalar(sql, pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual DateTime GetDateTime(string sql, params MySqlParameter[] pars)
        {
            return Convert.ToDateTime(GetScalar(sql, pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual object GetScalar(string sql, object pars)
        {
            return GetScalar(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual object GetScalar(string sql, params MySqlParameter[] pars)
        {
            SetCurrentConnection(true);
            ExecLogEvent(sql, pars, true);
            MySqlCommand MySqlCommand = new MySqlCommand(sql, _MySqlConnection);
            MySqlCommand.CommandType = CommandType;
            if (_tran != null)
            {
                MySqlCommand.Transaction = _tran;
            }
            MySqlCommand.CommandTimeout = this.CommandTimeOut;
            if (pars != null)
                MySqlCommand.Parameters.AddRange(pars);
            if (IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(MySqlCommand.Parameters);
            }
            CheckConnect();
            object scalar = MySqlCommand.ExecuteScalar();
            scalar = (scalar == null ? 0 : scalar);
            if (IsClearParameters)
                MySqlCommand.Parameters.Clear();
            ExecLogEvent(sql, pars, false);
            return scalar;
        }

        /// <summary>
        /// 执行SQL返回受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual int ExecuteCommand(string sql, object pars)
        {
            return ExecuteCommand(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 执行SQL返回受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual int ExecuteCommand(string sql, params MySqlParameter[] pars)
        {
            SetCurrentConnection(true);
            ExecLogEvent(sql, pars, true);
            MySqlCommand MySqlCommand = new MySqlCommand(sql, _MySqlConnection);
            MySqlCommand.CommandType = CommandType;
            MySqlCommand.CommandTimeout = this.CommandTimeOut;
            if (_tran != null)
            {
                MySqlCommand.Transaction = _tran;
            }
            if (pars != null)
                MySqlCommand.Parameters.AddRange(pars);
            if (IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(MySqlCommand.Parameters);
            }
            CheckConnect();
            int count = MySqlCommand.ExecuteNonQuery();
            if (IsClearParameters)
                MySqlCommand.Parameters.Clear();
            ExecLogEvent(sql, pars, false);
            return count;
        }

        /// <summary>
        /// 获取DataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual MySqlDataReader GetReader(string sql, object pars)
        {
            return GetReader(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 获取DataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual MySqlDataReader GetReader(string sql, params MySqlParameter[] pars)
        {
            SetCurrentConnection(false);
            ExecLogEvent(sql, pars, true);
            MySqlCommand MySqlCommand = new MySqlCommand(sql, _MySqlConnection);
            MySqlCommand.CommandType = CommandType;
            MySqlCommand.CommandTimeout = this.CommandTimeOut;
            if (_tran != null)
            {
                MySqlCommand.Transaction = _tran;
            }
            if (pars != null)
                MySqlCommand.Parameters.AddRange(pars);
            if (IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(MySqlCommand.Parameters);
            }
            CheckConnect();
            MySqlDataReader sqlDataReader = MySqlCommand.ExecuteReader();
            if (IsClearParameters)
                MySqlCommand.Parameters.Clear();
            ExecLogEvent(sql, pars, false);
            return sqlDataReader;
        }

        /// <summary>
        /// 根据SQL获取T的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(string sql, object pars)
        {
            return GetList<T>(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 根据SQL获取T的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(string sql, params MySqlParameter[] pars)
        {
            var reval = SqlSugarTool.DataReaderToList<T>(typeof(T), GetReader(sql, pars), null);
            return reval;
        }

        /// <summary>
        /// 根据SQL获取T
        /// </summary>
        /// <typeparam name="T">可以是int、string等，也可以是类或者数组、字典</typeparam>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual T GetSingle<T>(string sql, object pars)
        {
            return GetSingle<T>(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 根据SQL获取T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual T GetSingle<T>(string sql, params MySqlParameter[] pars)
        {
            var reval = SqlSugarTool.DataReaderToList<T>(typeof(T), GetReader(sql, pars), null).Single();
            return reval;
        }

        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(string sql, object pars)
        {
            return GetDataTable(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(string sql, params MySqlParameter[] pars)
        {
            SetCurrentConnection(false);
            ExecLogEvent(sql, pars, true);
            MySqlDataAdapter _sqlDataAdapter = new MySqlDataAdapter(sql, _MySqlConnection);
            _sqlDataAdapter.SelectCommand.CommandType = CommandType;
            if (pars != null)
                _sqlDataAdapter.SelectCommand.Parameters.AddRange(pars);
            if (IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(_sqlDataAdapter.SelectCommand.Parameters);
            }
            _sqlDataAdapter.SelectCommand.CommandTimeout = this.CommandTimeOut;
            if (_tran != null)
            {
                _sqlDataAdapter.SelectCommand.Transaction = _tran;
            }
            CheckConnect();
            DataTable dt = new DataTable();
            _sqlDataAdapter.Fill(dt);
            if (IsClearParameters)
                _sqlDataAdapter.SelectCommand.Parameters.Clear();
            ExecLogEvent(sql, pars, false);
            return dt;
        }
        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual DataSet GetDataSetAll(string sql, object pars)
        {
            return GetDataSetAll(sql, SqlSugarTool.GetParameters(pars));
        }
        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual DataSet GetDataSetAll(string sql, params MySqlParameter[] pars)
        {
            SetCurrentConnection(false);
            ExecLogEvent(sql, pars, true);
            MySqlDataAdapter _sqlDataAdapter = new MySqlDataAdapter(sql, _MySqlConnection);
            if (_tran != null)
            {
                _sqlDataAdapter.SelectCommand.Transaction = _tran;
            }
            if (IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(_sqlDataAdapter.SelectCommand.Parameters);
            }
            _sqlDataAdapter.SelectCommand.CommandTimeout = this.CommandTimeOut;
            _sqlDataAdapter.SelectCommand.CommandType = CommandType;
            if (pars != null)
                _sqlDataAdapter.SelectCommand.Parameters.AddRange(pars);
            CheckConnect();
            DataSet ds = new DataSet();
            _sqlDataAdapter.Fill(ds);
            if (IsClearParameters)
                _sqlDataAdapter.SelectCommand.Parameters.Clear();
            ExecLogEvent(sql, pars, false);
            return ds;
        }
        /// <summary>
        /// 执行日志事件
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <param name="isStarting"></param>
        protected virtual void ExecLogEvent(string sql, MySqlParameter[] pars, bool isStarting = true)
        {
            if (IsEnableLogEvent)
            {
                Action<string, string> action = isStarting ? LogEventStarting : LogEventCompleted;
                if (action != null)
                {
                    if (pars == null || pars.Length == 0)
                    {
                        action(sql, null);
                    }
                    else
                    {
                        action(sql, JsonConverter.Serialize(pars.Select(it => new { key = it.ParameterName, value = it.Value })));
                    }
                }
            }
        }
        /// <summary>
        /// 释放数据库连接对象
        /// </summary>
        public virtual void Dispose()
        {
            if (_MySqlConnection != null)
            {
                if (_MySqlConnection.State != ConnectionState.Closed)
                {
                    if (_tran != null)
                        _tran.Commit();
                    _MySqlConnection.Close();
                }
                _MySqlConnection = null;
            }
            if (_masterConnection != null) {
                if (_masterConnection.State != ConnectionState.Closed)
                {
                    if (_tran != null)
                        _tran.Commit();
                    _masterConnection.Close();
                }
                _masterConnection = null;
                foreach (var slave in _slaveConnections)
                {
                    if (slave.State != ConnectionState.Closed)
                    {
                        slave.Close();
                        slave.Dispose();
                    }
                }
            }
        }
        /// <summary>
        /// 检查数据库连接，若未连接，连接数据库
        /// </summary>
        protected virtual void CheckConnect()
        {
            if (_MySqlConnection.State != ConnectionState.Open)
            {
                _MySqlConnection.Open();
            }
        }
    }
}
