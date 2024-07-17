using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using WebApplicationTRY_TWO.Models;

namespace WebApplicationTRY_TWO.Controllers
{
    public class UsersController : Controller
    {
        private readonly string _connectionString;

        public UsersController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            List<Users> users = new List<Users>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM dbo.Users"; 
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Users user = new Users
                        {
                            id = (int)reader["id"],
                            user_name = reader["user_name"].ToString(),
                            user_email = reader.IsDBNull(reader.GetOrdinal("user_email")) ? null : reader["user_email"].ToString(),
                            user_password = reader.IsDBNull(reader.GetOrdinal("user_password")) ? null : reader["user_password"].ToString(),
                            user_type = reader["user_type"].ToString()
                        };
                        users.Add(user);
                    }
                }
            }
            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("user_name,user_email,user_password,user_type")] Users user)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string sql = "INSERT INTO dbo.Users (user_name, user_email, user_password, user_type) VALUES (@UserName, @UserEmail, @UserPassword, @UserType)"; // 'dbo.' eklendi
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@UserName", user.user_name);
                    command.Parameters.AddWithValue("@UserEmail", user.user_email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UserPassword", user.user_password ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UserType", user.user_type);
                    
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Users user = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM dbo.Users WHERE id = @Id"; 
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new Users
                        {
                            id = (int)reader["id"],
                            user_name = reader["user_name"].ToString(),
                            user_email = reader.IsDBNull(reader.GetOrdinal("user_email")) ? null : reader["user_email"].ToString(),
                            user_password = reader.IsDBNull(reader.GetOrdinal("user_password")) ? null : reader["user_password"].ToString(),
                            user_type = reader["user_type"].ToString()
                        };
                    }
                }
            }

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("id,user_name,user_email,user_password,user_type")] Users user)
        {
            if (id != user.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string sql = "UPDATE dbo.Users SET user_name = @UserName, user_email = @UserEmail, user_password = @UserPassword, user_type = @UserType WHERE id = @Id"; 
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@Id", user.id);
                    command.Parameters.AddWithValue("@UserName", user.user_name);
                    command.Parameters.AddWithValue("@UserEmail", user.user_email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UserPassword", user.user_password ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UserType", user.user_type);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Users user = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM dbo.Users WHERE id = @Id"; 
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new Users
                        {
                            id = (int)reader["id"],
                            user_name = reader["user_name"].ToString(),
                            user_email = reader.IsDBNull(reader.GetOrdinal("user_email")) ? null : reader["user_email"].ToString(),
                            user_password = reader.IsDBNull(reader.GetOrdinal("user_password")) ? null : reader["user_password"].ToString(),
                            user_type = reader["user_type"].ToString()
                        };
                    }
                }
            }

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM dbo.Users WHERE id = @Id"; // 'dbo.' eklendi
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                command.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
