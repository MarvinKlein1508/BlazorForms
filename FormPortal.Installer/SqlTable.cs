using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormPortal.Installer
{
    internal class SqlTable
    {
        public string TableName { get; set; }
        public string SQL { get; set; }

        public SqlTable(string tableName, string sql)
        {
            TableName = tableName;
            SQL = sql;
        }



    }
}
