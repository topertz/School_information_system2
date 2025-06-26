using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;

[ApiController]
[Route("[controller]/[action]")]
public class UserController : Controller
{

    [HttpPost]
    public IActionResult Create([FromForm] string username, [FromForm] string password, [FromForm] string role)
    {
        string? sessionId = Request.Cookies["id"];
        if (!string.IsNullOrEmpty(sessionId))
        {
            Int64 userID = SessionManager.GetUserID(sessionId);
            if (userID != -1)
            {
                using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
                {
                    string getRoleSql = "SELECT Role FROM User WHERE UserID = @UserID";
                    using (SQLiteCommand cmd = new SQLiteCommand(getRoleSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string? currentUserRole = reader["Role"].ToString();
                            }
                        }
                    }
                }
            }
        }

        using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
        {
            string checkUserSql = "SELECT COUNT(*) FROM User WHERE Username = @Username";
            using (SQLiteCommand checkCmd = new SQLiteCommand(checkUserSql, connection))
            {
                checkCmd.Parameters.AddWithValue("@Username", username);
                long count = (long)checkCmd.ExecuteScalar();
                if (count > 0)
                {
                    return Conflict("Username already exists.");
                }
            }

            string salt = PasswordManager.GenerateSalt();
            string hashedPassword = PasswordManager.GeneratePasswordHash(password, salt);

            string insertSql = "INSERT INTO User (Username, PasswordHash, PasswordSalt, Role) VALUES (@Username, @PasswordHash, @PasswordSalt, @Role)";
            using (SQLiteCommand cmd = new SQLiteCommand(insertSql, connection))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                cmd.Parameters.AddWithValue("@Role", role);
                cmd.ExecuteNonQuery();
            }
        }

        return Ok("User created successfully");
    }

    [HttpPost]
    public IActionResult Login([FromForm] string username, [FromForm] string password)
    {
        Int64 userID = -1;
        string? role = null;

        using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
        {
            string selectSql = "SELECT UserID, PasswordHash, PasswordSalt, Role FROM User WHERE Username = @Username";
            using (SQLiteCommand cmd = new SQLiteCommand(selectSql, connection))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string? storedPasswordHash = reader["PasswordHash"].ToString();
                        string? storedSalt = reader["PasswordSalt"].ToString();
                        role = reader["Role"].ToString();

                        if (!string.IsNullOrEmpty(storedPasswordHash) && !string.IsNullOrEmpty(storedSalt) &&
                            PasswordManager.Verify(password, storedSalt, storedPasswordHash))
                        {
                            userID = Convert.ToInt64(reader["UserID"]);
                        }
                    }
                }
            }
        }

        if (userID == -1)
            return Unauthorized("Invalid username or password");

        SessionManager.InvalidateAllSessions(userID);
        string sessionCookie = SessionManager.CreateSession(userID);
        Response.Cookies.Append("id", sessionCookie, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });

        return Ok(new { message = "Login successful", role = role });
    }

    [HttpPost]
    public IActionResult Logout()
    {
        string? sessionId = Request.Cookies["id"];

        if (!string.IsNullOrEmpty(sessionId))
        {
            SessionManager.InvalidateSession(sessionId);
        }

        Response.Cookies.Delete("id");

        return Ok("Logout successful");
    }

    static public bool IsLoggedIn(string SessionCookie)
    {
        Int64 userID = SessionManager.GetUserID(SessionCookie);
        return userID != -1;
    }

    [HttpGet]
    public IActionResult GetUser()
    {
        try
        {
            string? sessionId = Request.Cookies["id"];
            Int64 userID = SessionManager.ValidateSession(sessionId);
            return Json(userID);
        }
        catch (UnauthorizedAccessException)
        {
            Response.Cookies.Delete("id");
            return Unauthorized("Session expired or invalid");
        }
    }

    [HttpGet]
    public IActionResult CheckSession()
    {
        string? sessionId = Request.Cookies["id"];
        if (string.IsNullOrEmpty(sessionId))
        {
            return Json(new { userID = -1, username = (string?)null, role = (string?)null });
        }
        Int64 userID = SessionManager.GetUserID(sessionId);
        if (userID == -1)
        {
            return Json(new { userID = -1, username = (string?)null, role = (string?)null });
        }

        string? role = null;
        string? username = null;
        using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
        {
            string selectSql = "SELECT Role, Username FROM User WHERE UserID = @UserID";
            using (SQLiteCommand cmd = new SQLiteCommand(selectSql, connection))
            {
                cmd.Parameters.AddWithValue("@UserID", userID);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        role = reader["Role"].ToString();
                        username = reader["Username"].ToString();
                    }
                }
            }
        }

        return Json(new { userID, username, role });
    }

    [HttpGet]
    public IActionResult GetUserList()
    {
        List<User> users = new List<User>();
        try
        {
            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            {
                string selectSql = "SELECT UserID, Username FROM User";
                using (SQLiteCommand cmd = new SQLiteCommand(selectSql, connection))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                Username = reader.GetString(reader.GetOrdinal("Username"))
                            });
                        }
                    }
                }
            }
            return Ok(users);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"An error occurred: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}