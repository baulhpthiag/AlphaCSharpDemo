using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MysqlDemo
{
    class ParentDao
    {
        public void AddParent(Parent parent)
        {

            string sql = "insert into tb_parent (id, name,age,birthday,isWorking,tall) values "+
               " (@id, @name,@age,@birthday,@isWorking,@tall)";
            MySqlParameter[] parameters = {
                new MySqlParameter("@id", MySqlDbType.Guid),
                new MySqlParameter("@name", MySqlDbType.String),
                new MySqlParameter("@age", MySqlDbType.Int16),
                new MySqlParameter("@birthday", MySqlDbType.DateTime),
                new MySqlParameter("@isWorking", MySqlDbType.Bit),
                new MySqlParameter("@tall", MySqlDbType.Double)
            };


            parameters[0].Value = parent.id;
            parameters[1].Value = parent.name;
            parameters[2].Value = parent.age;
            parameters[3].Value = parent.birthday;
            parameters[4].Value = parent.isWorking;
            parameters[5].Value = parent.tall;

            MyHelper.MySqlHelper.ExecuteSql(sql, parameters);
        }
    }
}
