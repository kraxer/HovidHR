using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HovidHR
{
    public class DapperWrapper<T>
    {
        private static List<Log> Logs = new List<Log>();
        private static IConfiguration _configuration;
        private static string connectionString;
        public DapperWrapper(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("HovidHRDatabase");
        }
        public static IDbConnection Connection
        {
            get
            {
                return new SqlConnection(connectionString);
            }
        }

        public static List<T> GetList(string query, DynamicParameters args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                return Connection.Query<T>(query, args).ToList();
            }
            catch (Exception ex)
            {
                SqlException(query, ex.Message, watch.ElapsedMilliseconds);
            }
            finally
            {
                watch.Stop();
            }

            return null;
        }

        public static List<T> GetList(string query)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                return Connection.Query<T>(query).ToList();
            }
            catch (Exception ex)
            {
                SqlException(query, ex.Message, watch.ElapsedMilliseconds);
            }
            finally
            {
                watch.Stop();
            }

            return null;
        }

        public static DataTable ReturnDatatableFromSql(string Query)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (DataTable dt = new DataTable())
                {
                    using (SqlDataAdapter sqladapter1 = new SqlDataAdapter(Query, connection))
                    {
                        sqladapter1.Fill(dt);
                    }

                    connection.Close();

                    return dt;
                }
            }
        }

        public static T GetSingle(string query)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                return Connection.Query<T>(query).FirstOrDefault();
            }
            catch (Exception ex)
            {
                SqlException(query, ex.Message, watch.ElapsedMilliseconds);
            }
            finally
            {
                watch.Stop();
            }

            return default;
        }

        public static T GetSingle(string query, DynamicParameters args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                return Connection.Query<T>(query, args).FirstOrDefault();
            }
            catch (Exception ex)
            {
                SqlException(query, ex.Message, watch.ElapsedMilliseconds);
            }
            finally
            {
                watch.Stop();
            }

            return default;
        }

        public static List<Tuple<T, T2, T3, T4>> QueryMultiple<T2, T3, T4>(string sql)
       where T2 : class
       where T3 : class
        {
            return Connection.Query<T, T2, T3, T4, Tuple<T, T2, T3, T4>>(sql, (t, t2, t3, t4) => Tuple.Create(t, t2, t3, t4)).ToList();
        }

        public static bool Execute(string query, DynamicParameters args = null)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                return Connection.Execute(query, args) > 0;
            }
            catch (Exception ex)
            {
                SqlException(query, ex.Message, watch.ElapsedMilliseconds);
            }
            finally
            {
                watch.Stop();
            }

            return false;
        }

        private static void SqlException(string query, string exception, long elapsedMilliseconds)
        {
            var error = $"query : {query} → exception : {exception} → time : {elapsedMilliseconds}";

            Logs.Add(
                new Log()
                {
                    Title = "Error in dapper",
                    Description = error
                }
            );
        }


    }

    public class Log
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}