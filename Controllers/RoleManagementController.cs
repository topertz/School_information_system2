using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;

namespace SchoolAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class RoleManagementController : Controller
    {
        [HttpPost]
        public IActionResult InitializeAdmin([FromForm] string adminUsername, [FromForm] string adminPassword)
        {
            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();

            string checkSql = "SELECT COUNT(*) FROM Users WHERE Role = 'Admin'";
            using SQLiteCommand checkCmd = new SQLiteCommand(checkSql, connection);
            long adminCount = (long)checkCmd.ExecuteScalar();

            if (adminCount > 0)
                return BadRequest("Admin felhasználó már létezik.");

            string insertSql = "INSERT INTO Users (Username, PasswordHash, Role) VALUES (@username, @passwordHash, 'Admin')";
            using SQLiteCommand insertCmd = new SQLiteCommand(insertSql, connection);
            insertCmd.Parameters.AddWithValue("@username", adminUsername);
            insertCmd.Parameters.AddWithValue("@passwordHash", adminPassword);
            insertCmd.ExecuteNonQuery();

            return Ok("Admin felhasználó létrehozva.");
        }

        [HttpPost]
        public IActionResult AddUser([FromForm] string username, [FromForm] string password, [FromForm] string role)
        {
            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();

            string checkSql = "SELECT COUNT(*) FROM Users WHERE Username = @username";
            using SQLiteCommand checkCmd = new SQLiteCommand(checkSql, connection);
            checkCmd.Parameters.AddWithValue("@username", username);
            long userExists = (long)checkCmd.ExecuteScalar();

            if (userExists > 0)
                return BadRequest("A felhasználó már létezik.");

            string insertSql = "INSERT INTO Users (Username, PasswordHash, Role) VALUES (@username, @passwordHash, @role)";
            using SQLiteCommand insertCmd = new SQLiteCommand(insertSql, connection);
            insertCmd.Parameters.AddWithValue("@username", username);
            insertCmd.Parameters.AddWithValue("@passwordHash", password);
            insertCmd.Parameters.AddWithValue("@role", role);
            insertCmd.ExecuteNonQuery();

            return Ok("Felhasználó hozzáadva.");
        }

        [HttpPost]
        public IActionResult DeleteUser([FromForm] int userId)
        {
            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();
            string deleteSql = "DELETE FROM Users WHERE UserID = @userId";
            using SQLiteCommand deleteCmd = new SQLiteCommand(deleteSql, connection);
            deleteCmd.Parameters.AddWithValue("@userId", userId);
            int affected = deleteCmd.ExecuteNonQuery();

            if (affected == 0)
                return NotFound("Felhasználó nem található.");

            return Ok("Felhasználó törölve.");
        }

        [HttpPost]
        public IActionResult ChangeUserRole([FromForm] int userId, [FromForm] string newRole)
        {
            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();
            string updateSql = "UPDATE Users SET Role = @newRole WHERE UserID = @userId";
            using SQLiteCommand updateCmd = new SQLiteCommand(updateSql, connection);
            updateCmd.Parameters.AddWithValue("@newRole", newRole);
            updateCmd.Parameters.AddWithValue("@userId", userId);
            int affected = updateCmd.ExecuteNonQuery();

            if (affected == 0)
                return NotFound("Felhasználó nem található.");

            return Ok("Szerepkör módosítva.");
        }

        [HttpGet]
        public IActionResult GetUserRole(int userId)
        {
            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();
            string sql = "SELECT Role FROM Users WHERE UserID = @userId";
            using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@userId", userId);
            string? role = cmd.ExecuteScalar()?.ToString();

            if (role == null)
                return NotFound("Felhasználó nem található.");

            return Ok(role);
        }
    }
}