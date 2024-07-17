using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Collections.Generic;
using WebApplicationTRY_TWO.Models;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace WebApplicationTRY_TWO.Controllers
{
    public class CommentsController : Controller
    {
        private readonly string _connectionString;

        public CommentsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: Comments
        public IActionResult Index()
        {
            List<CommentViewModel> comments = new List<CommentViewModel>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = @"
                SELECT c.id, c.comment_text, c.comment_date, c.comment_type, c.comment_score, c.product_id, c.user_id, 
                p.products_name AS ProductName, u.user_name AS UserName, c.answer_id
                FROM comments c
                LEFT JOIN products p ON c.product_id = p.id
                LEFT JOIN Users u ON c.user_id = u.id";

                SqlCommand command = new SqlCommand(sql, connection);
        
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        comments.Add(new CommentViewModel
                        {
                            id = reader.GetInt32(reader.GetOrdinal("id")),
                            comment_text = reader.GetString(reader.GetOrdinal("comment_text")),
                            comment_date = reader.IsDBNull(reader.GetOrdinal("comment_date")) ? null : reader.GetDateTime(reader.GetOrdinal("comment_date")),
                            comment_type = reader.GetString(reader.GetOrdinal("comment_type")),
                            comment_score = reader.GetInt32(reader.GetOrdinal("comment_score")),
                            product_id = reader.IsDBNull(reader.GetOrdinal("product_id")) ? null : reader.GetInt32(reader.GetOrdinal("product_id")),
                            user_id = reader.IsDBNull(reader.GetOrdinal("user_id")) ? null : reader.GetInt32(reader.GetOrdinal("user_id")),
                            product_name = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? null : reader.GetString(reader.GetOrdinal("ProductName")),
                            user_name = reader.IsDBNull(reader.GetOrdinal("UserName")) ? null : reader.GetString(reader.GetOrdinal("UserName")),
                            answer_id = reader.IsDBNull(reader.GetOrdinal("answer_id")) ? null : reader.GetInt32(reader.GetOrdinal("answer_id")),
                        });
                    }
                }
            }
            return View(comments);
        }
        
        // GET: Comments/Create
        public IActionResult Create()
        {
            PopulateProductsDropDownList();
            PopulateUsersDropDownList();
            PopulateAnswersDropDownList();
            return View();
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("comment_text,comment_date,comment_type,comment_score,product_id,user_id,answer_id")] CommentViewModel commentViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"
                        INSERT INTO Comments (comment_text, comment_date, comment_type, comment_score, product_id, user_id, answer_id) 
                        VALUES (@CommentText, @CommentDate, @CommentType, @CommentScore, @ProductId, @UserId, @AnswerId)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@CommentText", commentViewModel.comment_text);
                        command.Parameters.AddWithValue("@CommentDate", commentViewModel.comment_date ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CommentType", commentViewModel.comment_type);
                        command.Parameters.AddWithValue("@CommentScore", commentViewModel.comment_score);
                        command.Parameters.AddWithValue("@ProductId", commentViewModel.product_id ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@UserId", commentViewModel.user_id ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@AnswerId", commentViewModel.answer_id ?? (object)DBNull.Value);
                
                        command.ExecuteNonQuery();
                        
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            // Eğer ModelState geçerli değilse veya başka bir hata oluşursa, dropdown listeleri tekrar doldur
            PopulateProductsDropDownList(commentViewModel.product_id);
            PopulateUsersDropDownList(commentViewModel.user_id);
            PopulateAnswersDropDownList(commentViewModel.answer_id);
            return View(commentViewModel);
        }
        
        // GET: Comments/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CommentViewModel commentViewModel = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @"SELECT id, comment_text, comment_score FROM Comments WHERE id = @Id";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id.Value);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            commentViewModel = new CommentViewModel
                            {
                                id = reader.GetInt32(reader.GetOrdinal("id")),
                                comment_text = reader.GetString(reader.GetOrdinal("comment_text")),
                                comment_score = reader.GetInt32(reader.GetOrdinal("comment_score")),
                            };
                        }
                    }
                }
            }

            if (commentViewModel == null)
            {
                return NotFound();
            }

            return View(commentViewModel);
        }

        // POST: Comments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("id,comment_text,comment_score")] CommentViewModel commentViewModel)
        {
            if (id != commentViewModel.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"UPDATE Comments SET comment_text = @CommentText, comment_score = @CommentScore WHERE id = @Id";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", commentViewModel.id);
                        command.Parameters.AddWithValue("@CommentText", commentViewModel.comment_text);
                        command.Parameters.AddWithValue("@CommentScore", commentViewModel.comment_score);

                        command.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(commentViewModel);
        }
        
        // GET: Comments/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CommentViewModel commentViewModel = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @"SELECT c.id, c.comment_text, p.products_name AS ProductName, u.user_name AS UserName 
                            FROM Comments c 
                            LEFT JOIN Products p ON c.product_id = p.id 
                            LEFT JOIN Users u ON c.user_id = u.id 
                            WHERE c.id = @Id";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id.Value);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            commentViewModel = new CommentViewModel
                            {
                                id = reader.GetInt32(reader.GetOrdinal("id")),
                                comment_text = reader.GetString(reader.GetOrdinal("comment_text")),
                                product_name = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? null : reader.GetString(reader.GetOrdinal("ProductName")),
                                user_name = reader.IsDBNull(reader.GetOrdinal("UserName")) ? null : reader.GetString(reader.GetOrdinal("UserName")),
                            };
                        }
                    }
                }
            }

            if (commentViewModel == null)
            {
                return NotFound();
            }

            return View(commentViewModel);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "DELETE FROM Comments WHERE id = @Id";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToAction(nameof(Index));
        }



        private void PopulateProductsDropDownList(object selectedProduct = null)
        {
            var productsList = new List<SelectListItem>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT id, products_name FROM Products", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        productsList.Add(new SelectListItem
                        {
                            Value = reader["id"].ToString(),
                            Text = reader["products_name"].ToString(),
                            Selected = (selectedProduct != null && selectedProduct.Equals(reader["id"]))
                        });
                    }
                }
            }
            ViewData["ProductId"] = new SelectList(productsList, "Value", "Text", selectedProduct);
        }

        private void PopulateUsersDropDownList(object selectedUser = null)
        {
            var usersList = new List<SelectListItem>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT id, user_name FROM Users", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usersList.Add(new SelectListItem
                        {
                            Value = reader["id"].ToString(),
                            Text = reader["user_name"].ToString(),
                            Selected = (selectedUser != null && selectedUser.Equals(reader["id"]))
                        });
                    }
                }
            }
            ViewData["UserId"] = new SelectList(usersList, "Value", "Text", selectedUser);
        }

        private void PopulateAnswersDropDownList(object selectedAnswer = null)
        {
            var answersList = new List<SelectListItem>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(@"
            SELECT c.id, c.comment_text, p.products_name
            FROM Comments c
            LEFT JOIN Products p ON c.product_id = p.id
            WHERE c.answer_id IS NULL", connection);
        
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var text = $"ID: {reader["id"]}, Product: {reader["products_name"] ?? "No Product"}";
                        answersList.Add(new SelectListItem
                        {
                            Value = reader["id"].ToString(),
                            Text = text,
                            Selected = (selectedAnswer != null && selectedAnswer.Equals(reader["id"]))
                        });
                    }
                }
            }
            ViewData["AnswerId"] = new SelectList(answersList, "Value", "Text", selectedAnswer);
        }


      
    }
}
