using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewTest.Dao;
using Models;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using MySqlSugar;
namespace NewTest.Demos
{
    //读写分离
    public class MasterSlave : IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动Ado.Init");
            using (var db = new SqlSugarClient("server=localhost;Database=SqlSugarTest;Uid=root;Pwd=root", "Server=localhost;database=sqlsugartest;Uid=root;Pwd=root"))
            {
                db.BeginTran();
               var list= db.Queryable<Student>().ToList();

               db.Insert(new Student() { name="写入" });
            }
        }
    }
}
