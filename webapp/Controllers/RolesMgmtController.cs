using SmartAdminMvc.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Configuration;
using System.Web.Mvc;

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class RolesMgmtController : Controller
    {
        readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        // GET: RolesMgmt
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        DataTable dt = new DataTable();
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                int custid = (int)Session["Cust_Id"];
                ViewBag.Roles = (from tbl in db.MAS_Roles
                                 where tbl.CustomerID == custid
                                 select tbl).ToList();

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        public ActionResult Delete(int Id)
        {
         
            var Rolename = (from li in db.MAS_Roles
                            where li.Role_ID == Id
                            select li.Role_Name).FirstOrDefault();
            int status = RoleDelete(Id);
            if (status == -1)
            {
                TempData["ErrorMessage"] = "Role  " + Rolename + " deletetion is failure because this role is assigned to some users.";
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Role : " + Rolename+ " deletetion is failure because this role is assigned to some users.", "");
            }
            else if (status == 1)
            {
                TempData["SuccessMessage"] = "Role name deleted successfully.";
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Role : " + Rolename+ " deleted.", "");
            }
                return RedirectToAction("Index");
        }

        public int RoleDelete(int roleId)
        {
            int status;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Delete_Role", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@RoleId", roleId));
                cmd.Parameters.Add("@RetVal", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                con.Close();
                status = Convert.ToInt32(cmd.Parameters["@RetVal"].Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return status;
        }
        public ActionResult Show(int Id)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                int custid = (int)Session["Cust_Id"];
                ViewBag.Roles = (from tbl in db.MAS_Roles
                                 where tbl.CustomerID == custid
                                 select tbl).ToList();
                Session["Role_resource_id"] = Id;

                ViewBag.RoleName = (from tbl in db.MAS_Roles
                                 where tbl.Role_ID == Id
                                 select tbl.Role_Name).SingleOrDefault();
                                
                if (Session["Role_ID"].ToString() == "1")
                {

                    var a = (from tbl1 in db.MAS_Resources
                             join tbl2 in db.MAS_Roles_Resources on tbl1.Resource_ID equals tbl2.FK_Role_Resource_Resource_ID
                             join tbl3 in db.MAS_Roles on tbl2.FK_Role_Resource_Role_ID equals tbl3.Role_ID
                             where tbl3.CustomerID == custid && tbl2.FK_Role_Resource_Role_ID == Id
                             select new
                             {
                                 tbl1.Resource_ID,
                                 tbl1.FK_Parent_Resource_ID,
                                 tbl1.Resource_Name,
                                 tbl2.Role_Resource_IsAssigned
                             }).ToList();

                    dt = LINQToDataTable(a);
                }
                else
                {
                    var a = (from tbl1 in db.MAS_Resources
                             join tbl2 in db.MAS_Roles_Resources on tbl1.Resource_ID equals tbl2.FK_Role_Resource_Resource_ID where tbl1.Resource_ID != 1 && tbl1.Resource_ID != 10 && tbl1.Resource_ID != 11
                             join tbl3 in db.MAS_Roles on tbl2.FK_Role_Resource_Role_ID equals tbl3.Role_ID
                             where tbl3.CustomerID == custid && tbl2.FK_Role_Resource_Role_ID == Id
                             select new
                             {
                                 tbl1.Resource_ID,
                                 tbl1.FK_Parent_Resource_ID,
                                 tbl1.Resource_Name,
                                 tbl2.Role_Resource_IsAssigned
                             }).ToList();

                    dt = LINQToDataTable(a);
                }
                if (dt.Rows.Count >= 1)
                {
                    TempData["Role"] = "ID";
                }
                return View("Index", ConvertToDictionary(dt));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public ActionResult Add(FormCollection frm)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                string rolename;

                var obj = (from tbl in db.MAS_Resources
                           select tbl.Resource_ID);

                int custid = (int)Session["Cust_Id"];

                rolename = Convert.ToString(frm["name"]);

                ObjectParameter op = new ObjectParameter("ret", typeof(int));

                db.ins_Role_Details(rolename, custid, op);

                if (Convert.ToInt32(op.Value) != 0)
                {
                    int roleid = Convert.ToInt32(op.Value);

                    foreach (int n in obj.ToList())
                    {
                        db.ins_Role_Resource_Details(roleid, n, 0);
                    }

                    TempData["SuccessMessage"] = "Role name Created Success exists..!";

                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "New Role Created : " + rolename, "");
                }
                else
                {
                    TempData["SuccessMessage"] = "Role name already exists..!";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpPost]
        public ActionResult Index(int[] ids)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                int roleid = (int)Session["Role_resource_id"];
                if (ids != null)
                {
                    db.update_Role_Resource_Details(roleid, 0);

                    foreach (int id in ids)
                    {
                        db.update_Role_Resource_Details(roleid, id);
                    }
                }
                else
                {
                    db.update_Role_Resource_Details(roleid, 0);
                }
                var Rolename = (from li in db.MAS_Roles
                                where li.Role_ID == roleid
                                select li.Role_Name).FirstOrDefault();
                TempData["SuccessMessage"] = "Resources Updated Successfully..!";
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Role Access updated for : " + Rolename, "");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        private List<IDictionary> ConvertToDictionary(DataTable dtObject)
        {
            var columns = dtObject.Columns.Cast<DataColumn>();

            var dictionaryList = dtObject.AsEnumerable()
                .Select(dataRow => columns
                    .Select(column =>
                        new { Column = column.ColumnName, Value = dataRow[column] })
                             .ToDictionary(data => data.Column, data => data.Value));

            return dictionaryList.ToList<IDictionary>();
        }

        public DataTable LINQToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();

            // column names 
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;

            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others 

                if (oProps == null)
                {
                    oProps = (rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? 0 : pi.GetValue
                    (rec, null);
                }

                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }
    }
}