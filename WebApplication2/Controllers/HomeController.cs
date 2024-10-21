using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using System.Data.SqlClient;
using System.Web.UI;
using System.Data;
using System.IO;
using System.Web.Mvc.Ajax;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection connect = null;
        SqlCommand cmd = null;

        string connectionString = "server=.;database=NimapDb; integrated security = true";

        public ActionResult Index()
        {
            ViewBag.GetCategories = GetCategories();
            return View();
        }

        [HttpPost]
        public ActionResult Index(CategoryMaster c)
        {

            string insertQuery = "insert into tbl_Category_Master(categoryName) values (@cn)";

            using (connect = new SqlConnection(connectionString))
            {
                cmd = new SqlCommand(insertQuery, connect);
                cmd.Parameters.AddWithValue("@cn", c.CategoryName);
                connect.Open();
                cmd.ExecuteNonQuery();
                //  RedirectToAction("Index");
                ModelState.Clear();
                ViewBag.GetCategories = GetCategories();

            }


            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            ViewBag.GetCategories = GetCategories();
            return View();
        }

        [HttpPost]
        public ActionResult About(ProductMaster pm)
        {
            ViewBag.Message = "Your application description page.";




            string insertQuery = "insert into tbl_Product_Master (productName, categoryName, categoryID) values (@pn, @cn, @ci)";

            using (connect = new SqlConnection(connectionString))
            {
                cmd = new SqlCommand(insertQuery, connect);

                cmd.Parameters.AddWithValue("@pn", pm.ProductName);
                cmd.Parameters.AddWithValue("@cn", pm.CategoryName);
                cmd.Parameters.AddWithValue("@ci", pm.CategoryId);


                connect.Open();
                cmd.ExecuteNonQuery();
                ModelState.Clear();
            }
            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            ViewBag.GetCategories = GetCategories();

            return View();
        }

        public ActionResult Products()
        {
            ViewBag.Message = "Your contact page.";
            ViewBag.GetProducts = GetProducts();

            return View();
        }

        public List<CategoryMaster> GetCategories()
        {

            List<CategoryMaster> categories = new List<CategoryMaster>();

            string query = "select * from tbl_Category_Master";

            using (connect = new SqlConnection(connectionString))
            {
                cmd = new SqlCommand(query, connect);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                DataTable CategoryTable = new DataTable();
                sqlDataAdapter.Fill(CategoryTable);

                foreach (DataRow row in CategoryTable.Rows)
                {
                    CategoryMaster cat = new CategoryMaster()
                    {
                        CategoryID = Convert.ToInt32(row["categoryID"]),
                        CategoryName = Convert.ToString(row["categoryName"]),
                    };
                    categories.Add(cat);
                }
                return categories;
            }
        }

        public List<CategoryMaster> GetCategoryName(int categoryID)
        {
            List<CategoryMaster> states = new List<CategoryMaster>();

            using (SqlConnection connect = new SqlConnection(connectionString))
            {
                string str = $"select * from tbl_Category_Master where categoryID ={categoryID};";

                SqlCommand cmd = new SqlCommand(str, connect);

                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(table);

                foreach (DataRow dr in table.Rows)
                {

                    CategoryMaster state = new CategoryMaster()
                    {
                        CategoryID = Convert.ToInt32(dr["categoryID"]),
                        CategoryName = Convert.ToString(dr["categoryName"]),
                    };
                    states.Add(state);
                }
                return states;
            }
        }

        public JsonResult GetStates(int categoryID)
        {
            var states = GetCategoryName(categoryID);
            return Json(states, JsonRequestBehavior.AllowGet);
        }

        public List<Products> GetProducts()
        {
            List<Products> products = new List<Products>();

            string selectQuery = "select * from tbl_Product_Master";

            using (connect = new SqlConnection(connectionString))
            {
                cmd = new SqlCommand(@selectQuery, connect);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                DataTable ProductTable = new DataTable();
                sqlDataAdapter.Fill(ProductTable);

                foreach (DataRow row in ProductTable.Rows)
                {
                    Products prod = new Products()
                    {
                        ProductId = Convert.ToInt32(row["productId"]),
                        ProductName = Convert.ToString(row["productName"]),
                        CategoryId = Convert.ToInt32(row["categoryId"]),
                        CategoryName = Convert.ToString(row["categoryName"])
                    };
                    products.Add(prod);
                }
                return products;
            }
        }

        public ActionResult EditCategory(int ID)
        {
            //int id = Convert.ToInt32(Request.QueryString["id"]);
            string selectQuery = "select * from tbl_Category_Master where categoryID=@id";
            //   string selectQuery = "select * from tbl_Category_Master";
            CategoryMaster categories = null;
            using (connect = new SqlConnection(connectionString))
            {
                cmd = new SqlCommand(selectQuery, connect);
                cmd.Parameters.AddWithValue("@id", ID);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    categories = new CategoryMaster()
                    {
                        CategoryID = Convert.ToInt32(row["categoryID"]),
                        CategoryName = Convert.ToString(row["categoryName"])
                    };

                }
            }
            return View(categories);
        }

        [HttpPost]
        public ActionResult EditCategoryById(CategoryMaster c)
        {
            string updateQuery = "update tbl_Category_Master set categoryName = @cn where categoryID=@id";
            using (connect = new SqlConnection(connectionString))
            {
                cmd = new SqlCommand(updateQuery, connect);
                cmd.Parameters.AddWithValue("@cn", c.CategoryName);
                cmd.Parameters.AddWithValue("@id", c.CategoryID);
                connect.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        public ActionResult DeleteCategory(int ID)
        {


            using (connect = new SqlConnection(connectionString))
            {
                connect.Open();
                string pkr = "Delete from tbl_Category_Master where categoryID=@id";

                cmd = new SqlCommand(pkr, connect);
                cmd.Parameters.AddWithValue("@id", ID);
                cmd.ExecuteNonQuery();
            };
            return RedirectToAction("Index");
        }
        public ActionResult EditProducts(int ID)
        {
            string updateQuery = "select * from tbl_Product_Master where productID = @pi";
            ProductMaster product = null;
            using (connect = new SqlConnection(connectionString))
            {
                cmd = new SqlCommand(updateQuery, connect);
                cmd.Parameters.AddWithValue("@pi", ID);


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    product = new ProductMaster()
                    {
                        ProductId = Convert.ToInt32(row["productID"]),
                        ProductName = Convert.ToString(row["productName"]),
                        CategoryId = Convert.ToInt32(row["categoryID"]),
                        CategoryName = Convert.ToString(row["categoryName"])
                    };
                }
            }

            ViewBag.GetProducts = GetProducts();
            return View(product);
        }

        [HttpPost]
        public ActionResult EditProductsByID(ProductMaster p)
        {
            string updateQuery = "update tbl_Product_Master set productName = @pn, categoryID = @ci, categoryName = @cn where productID=@id";
            using (connect = new SqlConnection(connectionString))
            {
                cmd = new SqlCommand(updateQuery, connect);
                cmd.Parameters.AddWithValue("@pn", p.ProductName);
                cmd.Parameters.AddWithValue("@ci", p.CategoryId);
                cmd.Parameters.AddWithValue("@cn", p.CategoryName);
                cmd.Parameters.AddWithValue("@id", p.ProductId);
                connect.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Products");
        }

        public ActionResult DeleteProduct(int ID)
        {


            using (connect = new SqlConnection(connectionString))
            {
                connect.Open();
                string pkr = "Delete from tbl_Product_Master where productID=@id";

                cmd = new SqlCommand(pkr, connect);
                cmd.Parameters.AddWithValue("@id", ID);
                cmd.ExecuteNonQuery();
            };
            return RedirectToAction("Products");
        }
    }
}