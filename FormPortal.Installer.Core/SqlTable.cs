namespace FormPortal.Installer.Core
{
    public class SqlTable
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
