using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseControllerProvider
{
    public sealed class DbProviderService
    {

        public IDbController GetDbController(string provider, string connectionString)
        {
            if (provider is "mssql")
            {
                return (IDbController)new SqlController(connectionString);
            }
            else if (provider is "mysql")
            {
                return (IDbController)new MySqlController(connectionString);
            }
            else
            {
                throw new ArgumentException();
            }

        }


    }
}
