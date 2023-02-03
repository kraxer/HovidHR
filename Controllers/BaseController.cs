using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HovidHR.Controllers
{
    public class BaseController:Controller
    {
        public static IConfiguration _configuration;
        public BaseController( IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public static IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_configuration.GetConnectionString("HovidHRDatabase"));
            }
        }


    }
}
