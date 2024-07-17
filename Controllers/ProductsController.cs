using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Collections.Generic;
using WebApplicationTRY_TWO.Models;
using Microsoft.Extensions.Configuration;

namespace WebApplicationTRY_TWO.Controllers
{
    //ADO.NET ile kullanılır
    public class ProductsController : Controller
    {
        private readonly string _connectionString;

        public ProductsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: Products
        public IActionResult Index()
        {
            var products = new List<Products>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Products", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Products
                        {
                            id = (int)reader["id"],
                            products_name = reader["products_name"].ToString(),
                            products_description = reader["products_description"].ToString(),
                            products_price = (decimal)reader["products_price"]
                        });
                    }
                }
            }
            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("products_name,products_description,products_price")] Products product)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var sql = "INSERT INTO Products (products_name, products_description, products_price) VALUES (@products_name, @products_description, @products_price)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@products_name", product.products_name);
                        command.Parameters.AddWithValue("@products_description", product.products_description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@products_price", product.products_price);
                        command.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
        
        // GET: Products/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Products product = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "SELECT * FROM Products WHERE id = @id";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product = new Products
                            {
                                id = (int)reader["id"],
                                products_name = reader["products_name"].ToString(),
                                products_description = reader["products_description"].ToString(),
                                products_price = (decimal)reader["products_price"]
                            };
                        }
                    }
                }
            }

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("id,products_name,products_description,products_price")] Products product)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var sql = "UPDATE Products SET products_name = @products_name, products_description = @products_description, products_price = @products_price WHERE id = @id";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", product.id);
                        command.Parameters.AddWithValue("@products_name", product.products_name);
                        command.Parameters.AddWithValue("@products_description", product.products_description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@products_price", product.products_price);
                        command.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
        
        // GET: Products/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Products product = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "SELECT * FROM Products WHERE id = @id";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product = new Products
                            {
                                id = (int)reader["id"],
                                products_name = reader["products_name"].ToString(),
                                products_description = reader["products_description"].ToString(),
                                products_price = (decimal)reader["products_price"]
                            };
                        }
                    }
                }
            }

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "DELETE FROM Products WHERE id = @id";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToAction(nameof(Index));
        }



    }
}
