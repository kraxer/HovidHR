using Dapper;
using HovidHR.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq.Dynamic.Core;

namespace HovidHR.Controllers
{
    public class HomeController : BaseController
    {

        public static HovidHrContext HovidHrEntity = new HovidHrContext();

        public HomeController(IConfiguration configuration) : base(configuration)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageUser()
        {
            UserViewModel userVM = new UserViewModel();
            return View(userVM);
        }

        [HttpPost]
        public IActionResult GetUser()
        {
            List<UserViewModel> userVM = new List<UserViewModel>();

            userVM = Connection.Query<UserViewModel>($"SELECT * FROM [dbo].[user]").ToList();

            string json = JsonConvert.SerializeObject(userVM);
            return Content(json, "application/json");
        }

        [HttpPost]
        public JsonResult GetEmployeeList()
        {

            var userVM = Connection.Query<UserViewModel>($"SELECT * FROM [dbo].[user]").AsQueryable();

            int totalRecord = 0;
            int filterRecord = 0;
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var data = userVM;
            //get total count of data in table
            totalRecord = data.Count();
            // search data when search value found
            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(x => x.UserName.ToLower().Contains(searchValue.ToLower()) || x.UserNo.ToLower().Contains(searchValue.ToLower()) || x.CreateDate.ToLower().Contains(searchValue.ToLower()));
            }
            // get total count of records after search
            filterRecord = data.Count();
            //sort data
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection)) data = data.OrderBy(sortColumn + " " + sortColumnDirection);
            //pagination
            var empList = data.Skip(skip).Take(pageSize).ToList();
            var returnObj = new
            {
                draw = draw,
                recordsTotal = totalRecord,
                recordsFiltered = filterRecord,
                empList
            };

            string json = JsonConvert.SerializeObject(returnObj);

            return Json(returnObj);


        }


        public IActionResult CreateUser(string UserName, string UserNo)
        {
            var tempobj = new object();

            string ReturnStr = "";

            using (var con = new SqlConnection(_configuration.GetConnectionString("HovidHRDatabase")))
            {
                con.Open();
                using (var sqlcmd = new SqlCommand("CreateUser", con))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@UserName", UserName);
                    sqlcmd.Parameters.AddWithValue("@UserNo", UserNo);
                    sqlcmd.Parameters.Add(new SqlParameter("@ReturnId", SqlDbType.Int, 20));
                    sqlcmd.Parameters.Add(new SqlParameter("@ReturnStr", SqlDbType.NVarChar, 20));

                    sqlcmd.Parameters["@ReturnId"].Direction = ParameterDirection.Output;
                    sqlcmd.Parameters["@ReturnStr"].Direction = ParameterDirection.Output;


                    sqlcmd.ExecuteNonQuery();

                    ReturnStr = (string)sqlcmd.Parameters["@ReturnStr"].Value;
                }
            }

            if (!string.IsNullOrEmpty(ReturnStr) && ReturnStr == "Success")
            {
                tempobj = new { code = "200", message = "Sucess" };
            }
            else
            {
                tempobj = new { code = "300", message = "Failed" };
            }

            return Content(JsonConvert.SerializeObject(tempobj), "application/json");
        }

        public IActionResult UpdateUser(int UserID,string UserName,string UserNo)
        {

            var tempobj = new object();

            string ReturnStr = "";

            using (var con = new SqlConnection(_configuration.GetConnectionString("HovidHRDatabase")))
            {
                con.Open();
                using (var sqlcmd = new SqlCommand("UpdateUser", con))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@UserID", UserID);
                    sqlcmd.Parameters.AddWithValue("@UserName", UserName);
                    sqlcmd.Parameters.AddWithValue("@UserNo", UserNo);
                    sqlcmd.Parameters.Add(new SqlParameter("@ReturnId", SqlDbType.Int, 20));
                    sqlcmd.Parameters.Add(new SqlParameter("@ReturnStr", SqlDbType.NVarChar, 20));

                    sqlcmd.Parameters["@ReturnId"].Direction = ParameterDirection.Output;
                    sqlcmd.Parameters["@ReturnStr"].Direction = ParameterDirection.Output;


                    sqlcmd.ExecuteNonQuery();

                    ReturnStr = (string)sqlcmd.Parameters["@ReturnStr"].Value;
                }
            }

            if (!string.IsNullOrEmpty(ReturnStr) && ReturnStr == "Success")
            {
                tempobj = new { code = "200", message = "Sucess" };
            }
            else
            {
                tempobj = new { code = "300", message = "Failed" };
            }

            return Content(JsonConvert.SerializeObject(tempobj), "application/json");
        }

        public IActionResult DeleteUser(int UserID)
        {

            var tempobj = new object();

            string ReturnStr = "";

            using (var con = new SqlConnection(_configuration.GetConnectionString("HovidHRDatabase")))
            {
                con.Open();
                using (var sqlcmd = new SqlCommand("DeleteUser", con))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@UserID", UserID);
                    sqlcmd.Parameters.Add(new SqlParameter("@ReturnId", SqlDbType.Int, 20));
                    sqlcmd.Parameters.Add(new SqlParameter("@ReturnStr", SqlDbType.NVarChar, 20));

                    sqlcmd.Parameters["@ReturnId"].Direction = ParameterDirection.Output;
                    sqlcmd.Parameters["@ReturnStr"].Direction = ParameterDirection.Output;


                    sqlcmd.ExecuteNonQuery();

                    ReturnStr = (string)sqlcmd.Parameters["@ReturnStr"].Value;
                }
            }

            if (!string.IsNullOrEmpty(ReturnStr) && ReturnStr == "Success")
            {
                tempobj = new { code = "200", message = "Sucess" };
            }
            else
            {
                tempobj = new { code = "300", message = "Failed" };
            }

            return Content(JsonConvert.SerializeObject(tempobj), "application/json");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}