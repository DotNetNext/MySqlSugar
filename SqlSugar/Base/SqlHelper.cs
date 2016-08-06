﻿using System;
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
        MySqlConnection _sqlConnection;
        MySqlTransaction _tran = null;
        /// <summary>
        /// 是否清空SqlParameters
        /// </summary>
        public bool isClearParameters = true;
        public int CommandTimeOut = 30000;
        /// <summary>
        /// 将页面参数自动填充到SqlParameter []，无需在程序中指定，这种情况需要注意是否有重复参数
        /// 例如：
        ///     var list = db.Queryable《Student》().Where("id=@id").ToList();
        ///     以前写法
        ///     var list = db.Queryable《Student》().Where("id=@id", new { id=Request["id"] }).ToList();
        /// </summary>
        public bool IsGetPageParas = false;
        public SqlHelper(string connectionString)
        {
            _sqlConnection = new MySqlConnection(connectionString);
            _sqlConnection.Open();
        }
        public MySqlConnection GetConnection()
        {
            return _sqlConnection;
        }
        public void BeginTran()
        {
            _tran = _sqlConnection.BeginTransaction();
        }

        public void BeginTran(IsolationLevel iso)
        {
            _tran = _sqlConnection.BeginTransaction(iso);
        }

   

        public void RollbackTran()
        {
            if (_tran != null)
            {
                _tran.Rollback();
                _tran = null;
            }
        }
        public void CommitTran()
        {
            if (_tran != null)
            {
                _tran.Commit();
                _tran = null;
            }
        }
        public string GetString(string sql, object pars)
        {
            return GetString(sql, SqlSugarTool.GetParameters(pars));
        }
        public string GetString(string sql, params MySqlParameter[] pars)
        {
            return Convert.ToString(GetScalar(sql, pars));
        }
        public int GetInt(string sql, object pars)
        {
            return GetInt(sql, SqlSugarTool.GetParameters(pars));
        }
        public int GetInt(string sql, params MySqlParameter[] pars)
        {
            return Convert.ToInt32(GetScalar(sql, pars));
        }
        public object GetScalar(string sql, object pars)
        {
            return GetScalar(sql, SqlSugarTool.GetParameters(pars));
        }
        public object GetScalar(string sql, params MySqlParameter[] pars)
        {
            MySqlCommand sqlCommand = new MySqlCommand(sql, _sqlConnection);
            if (_tran != null)
            {
                sqlCommand.Transaction = _tran;
            }
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (pars != null)
                sqlCommand.Parameters.AddRange(pars);
            if (IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(sqlCommand.Parameters);
            }
            object scalar = sqlCommand.ExecuteScalar();
            scalar = (scalar == null ? 0 : scalar);
            sqlCommand.Parameters.Clear();
            return scalar;
        }
        public int ExecuteCommand(string sql, object pars)
        {
            return ExecuteCommand(sql, SqlSugarTool.GetParameters(pars));
        }
        public int ExecuteCommand(string sql, params MySqlParameter[] pars)
        {
            MySqlCommand sqlCommand = new MySqlCommand(sql, _sqlConnection);
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (_tran != null)
            {
                sqlCommand.Transaction = _tran;
            }
            if (pars != null)
                sqlCommand.Parameters.AddRange(pars);
            if (IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(sqlCommand.Parameters);
            }
            int count = sqlCommand.ExecuteNonQuery();
            sqlCommand.Parameters.Clear();
            return count;
        }
        public MySqlDataReader GetReader(string sql, object pars)
        {
            return GetReader(sql, SqlSugarTool.GetParameters(pars));
        }
        public MySqlDataReader GetReader(string sql, params MySqlParameter[] pars)
        {
            MySqlCommand sqlCommand = new MySqlCommand(sql, _sqlConnection);
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (_tran != null)
            {
                sqlCommand.Transaction = _tran;
            }
            if (pars != null)
                sqlCommand.Parameters.AddRange(pars);
            if (IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(sqlCommand.Parameters);
            }
            MySqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            if (isClearParameters)
                sqlCommand.Parameters.Clear();
            return sqlDataReader;
        }
        public List<T> GetList<T>(string sql, object pars)
        {
            return GetList<T>(sql, SqlSugarTool.GetParameters(pars));
        }
        public List<T> GetList<T>(string sql, params MySqlParameter[] pars)
        {
            var reval = SqlSugarTool.DataReaderToList<T>(typeof(T), GetReader(sql, pars), null);
            return reval;
        }
        public T GetSingle<T>(string sql, object[] pars)
        {
            return GetSingle<T>(sql, SqlSugarTool.GetParameters(pars));
        }
        public T GetSingle<T>(string sql, params MySqlParameter[] pars)
        {
            var reval = SqlSugarTool.DataReaderToList<T>(typeof(T), GetReader(sql, pars), null).Single();
            return reval;
        }
        public DataTable GetDataTable(string sql, object pars)
        {
            return GetDataTable(sql, SqlSugarTool.GetParameters(pars));
        }
        public DataTable GetDataTable(string sql, params MySqlParameter[] pars)
        {
            MySqlDataAdapter _sqlDataAdapter = new MySqlDataAdapter(sql, _sqlConnection);
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
            DataTable dt = new DataTable();
            _sqlDataAdapter.Fill(dt);
            _sqlDataAdapter.SelectCommand.Parameters.Clear();
            return dt;
        }
        public DataSet GetDataSetAll(string sql, object pars)
        {
            return GetDataSetAll(sql, SqlSugarTool.GetParameters(pars));
        }
        public DataSet GetDataSetAll(string sql, params MySqlParameter[] pars)
        {
            MySqlDataAdapter _sqlDataAdapter = new MySqlDataAdapter(sql, _sqlConnection);
            if (_tran != null)
            {
                _sqlDataAdapter.SelectCommand.Transaction = _tran;
            }
            if (IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(_sqlDataAdapter.SelectCommand.Parameters);
            }
            _sqlDataAdapter.SelectCommand.CommandTimeout = this.CommandTimeOut;
            _sqlDataAdapter.SelectCommand.Parameters.AddRange(pars);
            DataSet ds = new DataSet();
            _sqlDataAdapter.Fill(ds);
            _sqlDataAdapter.SelectCommand.Parameters.Clear();
            return ds;
        }

        public void Dispose()
        {
            if (_sqlConnection != null)
            {
                if (_sqlConnection.State != ConnectionState.Closed)
                {
                    if (_tran != null)
                        _tran.Commit();
                    _sqlConnection.Close();
                }
            }
        }
    }
}
