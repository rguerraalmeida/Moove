using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace StocksAnalysis.YahooEngine.Database
{
    public class SymbolLookupDal
    {
        
        
        public IEnumerable<string> GetSymbols(string Exchange)
        {
            string selectstring = string.Format(@"select [symbol] from [SymbolLookup] where [state] != 'ER' and [type] ='S' and Symbol is not null and len(Symbol) >0 AND [exchDisp] IN ('{0}') ", Exchange);

            using (SqlConnection connection = new SqlConnection(ConnectionInfo.ConnectionString))
            {
                // Create the command and set its properties.
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = selectstring;
                command.CommandType = System.Data.CommandType.Text;

                // Open the connection and execute the reader.
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                //int index = 0, size = 0;
                //string[] result = new string[0];

                //if (reader.HasRows)
                //{
                //    while (reader.Read())
                //    {
                //        size++;
                //        Array.Resize(ref result, size);
                //        result[index] = reader[0].ToString();
                //        //Console.WriteLine("{0}: {1:C}", reader[0], reader[1]);
                //        index++;
                //    }
                //}

                              
                List<string> result = new List<string>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result.Add(reader[0].ToString());
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                reader.Close();

                return result.AsEnumerable<string>();
            }
        }
    }
}
