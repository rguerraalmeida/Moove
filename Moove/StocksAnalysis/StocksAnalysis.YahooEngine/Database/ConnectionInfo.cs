using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace StocksAnalysis.YahooEngine.Database
{
    public class ConnectionInfo
    {
        public const string ConnectionString = "Server=RUIGUERRAP\\SQLSERVER2008R2;Database=StocksDB;User ID=sa;Password=dev2008r2;Trusted_Connection=False;";
        SqlTransaction Transaction;

        public SqlTransaction BeginTransation()
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();

            // Start a local transaction.
            Transaction = connection.BeginTransaction();

            return Transaction;
        }

        public void CommitTransation()
        {
            try
            {
                Transaction.Commit();
            }
            catch (Exception)
            {
                //throw;
                Transaction.Connection.Close();
                Transaction.Connection.Dispose();
            }
            finally
            {
          
            }
        }

        public void RollbackTransation()
        {
            try
            {
                Transaction.Rollback();
            }
            catch (Exception)
            {
                //throw;
                Transaction.Connection.Close();
                Transaction.Connection.Dispose();
            }
            finally
            {

            }
        }
    }
}
