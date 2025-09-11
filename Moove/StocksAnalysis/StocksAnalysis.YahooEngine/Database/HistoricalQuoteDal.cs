using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace StocksAnalysis.YahooEngine.Database
{
    public class HistoricalQuoteDal
    {

        public bool AddHistoricalQuotes(string Symbol, List<HistoricalQuote> HistoricalQuotes)
        {

            ConnectionInfo connection = new ConnectionInfo();
            SqlTransaction transaction = connection.BeginTransation();

            try
            {
                DeleteAllHistoricalQuotes(transaction, Symbol);

                foreach (var item in HistoricalQuotes)
                {
                    AddHistoricalQuote(transaction, item);
                }

                connection.CommitTransation();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.Message);
                connection.RollbackTransation();
                return false;
            }

            return true;
        }

        public bool AddHistoricalQuote(HistoricalQuote HistoricalQuote)
        {

            string commandText = string.Format(@"
                INSERT INTO [HistoricalQuote]
                           ([Symbol]
                           ,[Date]
                           ,[OpenPrice]
                           ,[High]
                           ,[Low]
                           ,[ClosePrice]
                           ,[Volume]
                           ,[AdjClose])
                     VALUES
                           ({0}
                           ,{1}
                           ,{2}
                           ,{3}
                           ,{4}
                           ,{5}
                           ,{6}
                           ,{7}", HistoricalQuote.Symbol, HistoricalQuote.Date, HistoricalQuote.Open, HistoricalQuote.High, HistoricalQuote.Low,
                                HistoricalQuote.Close, HistoricalQuote.Volume, HistoricalQuote.AdjClose);



            using (SqlConnection connection = new SqlConnection(ConnectionInfo.ConnectionString))
            {
                // Create the command and set its properties.
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = commandText;
                command.CommandType = System.Data.CommandType.Text;

                //// Add the input parameter and set its properties.
                //OleDbParameter parameter = new OleDbParameter();
                //parameter.ParameterName = "@CategoryName";
                //parameter.OleDbType = OleDbType.VarChar;
                //parameter.Direction = System.Data.ParameterDirection.Input;
                //parameter.Value = symbol;

                // Add the parameter to the Parameters collection. 
                //command.Parameters.Add(parameter);

                // Open the connection and execute the reader.
                connection.Open();
                var ret = command.ExecuteNonQuery();

                return Convert.ToBoolean(ret);
            }

        }

        public bool AddHistoricalQuote(SqlTransaction Transaction, HistoricalQuote HistoricalQuote)
        {

            string commandText = string.Format(@"
                INSERT INTO [HistoricalQuote]
                           ([Symbol]
                           ,[Date]
                           ,[OpenPrice]
                           ,[High]
                           ,[Low]
                           ,[ClosePrice]
                           ,[Volume]
                           ,[AdjClose]
                            ,[IntervalType])
                     VALUES
                           ('{0}'
                           ,'{1}'
                           ,{2}
                           ,{3}
                           ,{4}
                           ,{5}
                           ,{6}
                           ,{7}
                            ,'{8}')", HistoricalQuote.Symbol, HistoricalQuote.Date, HistoricalQuote.Open, HistoricalQuote.High, HistoricalQuote.Low,
                                HistoricalQuote.Close, HistoricalQuote.Volume, HistoricalQuote.AdjClose, HistoricalQuote.IntervalType);

            // Create the command and set its properties.
            SqlCommand command = new SqlCommand(commandText, Transaction.Connection, Transaction);
            command.CommandType = System.Data.CommandType.Text;

            var ret = command.ExecuteNonQuery();

            return Convert.ToBoolean(ret);
        }

        public bool DeleteAllHistoricalQuotes(SqlTransaction Transaction)
        {

            string commandText = string.Format(@"DELETE FROM [HistoricalQuote]");

            // Create the command and set its properties.
            SqlCommand command = new SqlCommand(commandText, Transaction.Connection, Transaction);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandType = System.Data.CommandType.Text;
            var ret = command.ExecuteNonQuery();

            return Convert.ToBoolean(ret);
        }

        public bool DeleteAllHistoricalQuotes(SqlTransaction Transaction, string Symbol)
        {

            string commandText = string.Format(@"
               DELETE FROM [HistoricalQuote] WHERE [Symbol]='{0}'", Symbol);

            // Create the command and set its properties.
            SqlCommand command = new SqlCommand(commandText, Transaction.Connection, Transaction);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandType = System.Data.CommandType.Text;
            var ret = command.ExecuteNonQuery();

            return Convert.ToBoolean(ret);
        }

        public IEnumerable<HistoricalQuote> GetHistoricalQuotes()
        {
            string commandText = string.Format(@"SELECT * FROM [HistoricalQuote]");

            using (SqlConnection connection = new SqlConnection(ConnectionInfo.ConnectionString))
            {
                // Create the command and set its properties.
                SqlCommand command = new SqlCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.Text;

                // Open the connection and execute the reader.
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                int index = 0, size = 0;
                HistoricalQuote[] result = new HistoricalQuote[0];

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        size++;
                        Array.Resize(ref result, size);
                        result[index].Symbol = reader[0].ToString();
                        result[index].Date = Convert.ToDateTime( reader[1]);
                        result[index].Open = Convert.ToDouble(reader[2]);
                        result[index].High = Convert.ToDouble(reader[3]);
                        result[index].Low = Convert.ToDouble(reader[4]);
                        result[index].Close = Convert.ToDouble(reader[5]);
                        result[index].Volume = Convert.ToDouble(reader[6]);
                        result[index].AdjClose = Convert.ToDouble(reader[7]);
                        
                        index++;
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                reader.Close();

                return result.AsEnumerable<HistoricalQuote>();
            }
        }

        public IEnumerable<HistoricalQuote> GetHistoricalQuotes(string Exchange, string Symbol = null)
        {
            string commandText = string.Format(@"SELECT HistoricalQuote.* FROM [HistoricalQuote] inner join [SymbolLookup]  on (HistoricalQuote.Symbol = SymbolLookup.Symbol)
                WHERE SymbolLookup.exchDisp = '{0}' ", Exchange);

            if (Symbol != null)
                if (Symbol.Contains("%"))
                    commandText = string.Format("{0} AND HistoricalQuote.Symbol LIKE '{1}'", commandText, Symbol);
                else
                    commandText = string.Format("{0} AND HistoricalQuote.Symbol = '{1}'", commandText, Symbol);
            

            commandText = string.Format("{0} ORDER BY HistoricalQuote.SYMBOL , HistoricalQuote.[Date]", commandText); 

            using (SqlConnection connection = new SqlConnection(ConnectionInfo.ConnectionString))
            {
                // Create the command and set its properties.
                SqlCommand command = new SqlCommand(commandText, connection);
                command.CommandType = System.Data.CommandType.Text;

                // Open the connection and execute the reader.
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                List<HistoricalQuote> result = new List<HistoricalQuote>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var HistoricalQuote = new HistoricalQuote();
                        HistoricalQuote.Symbol = reader[0].ToString();
                        HistoricalQuote.Date = Convert.ToDateTime(reader[1]);
                        HistoricalQuote.Open = Convert.ToDouble(reader[2]);
                        HistoricalQuote.High = Convert.ToDouble(reader[3]);
                        HistoricalQuote.Low = Convert.ToDouble(reader[4]);
                        HistoricalQuote.Close = Convert.ToDouble(reader[5]);
                        HistoricalQuote.Volume = Convert.ToDouble(reader[6]);
                        HistoricalQuote.AdjClose = Convert.ToDouble(reader[7]);
                        //Console.WriteLine("Processing " + HistoricalQuote.Symbol + "." + HistoricalQuote.Date.ToLongDateString());

                        result.Add(HistoricalQuote);

                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                reader.Close();

                return result.AsEnumerable<HistoricalQuote>();
            }
        }
       
    }
}
