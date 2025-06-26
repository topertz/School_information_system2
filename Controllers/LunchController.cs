using Microsoft.AspNetCore.Mvc;
using SchoolAPI.Models.Lunch;
using System.Data.SQLite;

namespace SchoolAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class LunchController : Controller
    {
        [HttpGet]
        public IActionResult GetMenu()
        {
            List<LunchItem> menu = new List<LunchItem>();
            string sql = "SELECT * FROM Lunch";

            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    menu.Add(new LunchItem
                    {
                        LunchID = reader.GetInt32(0),
                        Day = reader.GetString(1),
                        Meal = reader.GetString(2)
                    });
                }
            }

            return Json(menu);
        }

        [HttpGet]
        public IActionResult GetMenuByDay(string day)
        {
            List<LunchItem> menu = new List<LunchItem>();

            string sql = "SELECT * FROM Lunch WHERE LOWER(Day) = LOWER(@Day)";

            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@Day", day);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    menu.Add(new LunchItem
                    {
                        LunchID = reader.GetInt32(0),
                        Day = reader.GetString(1),
                        Meal = reader.GetString(2)
                    });
                }
            }

            if (!menu.Any())
                return NotFound($"No lunch items found for day '{day}'");

            return Json(menu);
        }

        [HttpGet]
        public IActionResult GetWeeklyMenu()
        {
            List<LunchItem> menu = new List<LunchItem>();
            string sql = "SELECT * FROM Lunch ORDER BY CASE " +
                         "WHEN Day = 'Monday' THEN 1 " +
                         "WHEN Day = 'Tuesday' THEN 2 " +
                         "WHEN Day = 'Wednesday' THEN 3 " +
                         "WHEN Day = 'Thursday' THEN 4 " +
                         "WHEN Day = 'Friday' THEN 5 " +
                         "ELSE 6 END";

            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    menu.Add(new LunchItem
                    {
                        LunchID = reader.GetInt32(0),
                        Day = reader.GetString(1),
                        Meal = reader.GetString(2)
                    });
                }
            }

            return Json(menu);
        }

        [HttpPost]
        public IActionResult CreateLunch([FromForm] string day, [FromForm] string meal)
        {
            string sql = "INSERT INTO Lunch (Day, Meal) VALUES (@Day, @Meal)";

            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@Day", day);
                cmd.Parameters.AddWithValue("@Meal", meal);
                cmd.ExecuteNonQuery();
            }

            return Ok("Lunch entry created successfully");
        }

        [HttpPost]
        public IActionResult InsertDefaultWeeklyMenu()
        {
            var weeklyMenu = new Dictionary<string, string>
            {
                { "Monday", "Chicken with rice" },
                { "Tuesday", "Spaghetti Bolognese" },
                { "Wednesday", "Vegetable stew" },
                { "Thursday", "Fish and chips" },
                { "Friday", "Pizza and salad" }
            };

            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            {
                foreach (var item in weeklyMenu)
                {
                    string checkSql = "SELECT COUNT(*) FROM Lunch WHERE Day = @Day COLLATE NOCASE";
                    using (SQLiteCommand checkCmd = new SQLiteCommand(checkSql, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@Day", item.Key);
                        long count = (long)checkCmd.ExecuteScalar();
                        if (count > 0)
                            continue;
                    }

                    string insertSql = "INSERT INTO Lunch (Day, Meal) VALUES (@Day, @Meal)";
                    using (SQLiteCommand insertCmd = new SQLiteCommand(insertSql, connection))
                    {
                        insertCmd.Parameters.AddWithValue("@Day", item.Key);
                        insertCmd.Parameters.AddWithValue("@Meal", item.Value);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }

            return Ok("Alapértelmezett heti menü beszúrva (ha hiányzott).");
        }

        [HttpPost]
        public IActionResult SignupLunch([FromForm] int userID, [FromForm] string day, [FromForm] string meal)
        {
            if (string.IsNullOrEmpty(meal))
            {
                return BadRequest("A meal paraméter nem lehet üres!");
            }

            try
            {
                string checkSql = "SELECT COUNT(*) FROM LunchSignup WHERE UserID = @UserID AND Day = @Day AND Meal = @Meal";
                int count = 0;

                using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
                using (SQLiteCommand cmd = new SQLiteCommand(checkSql, connection))
                {
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.Parameters.AddWithValue("@Day", day);
                    cmd.Parameters.AddWithValue("@Meal", meal);
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }

                if (count > 0)
                {
                    string updateSql = "UPDATE LunchSignup SET isSignedUp = 1 WHERE UserID = @UserID AND Day = @Day AND Meal = @Meal";
                    using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
                    using (SQLiteCommand cmd = new SQLiteCommand(updateSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@Day", day);
                        cmd.Parameters.AddWithValue("@Meal", meal);
                        cmd.ExecuteNonQuery();
                    }

                    return Ok("Sikeres ebéd jelentkezés!");
                }
                else
                {
                    string insertSql = "INSERT INTO LunchSignup (UserID, Day, Meal, isSignedUp) VALUES (@UserID, @Day, @Meal, 1)";
                    using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
                    using (SQLiteCommand cmd = new SQLiteCommand(insertSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@Day", day);
                        cmd.Parameters.AddWithValue("@Meal", meal);
                        cmd.ExecuteNonQuery();
                    }

                    return Ok("Sikeres ebéd jelentkezés!");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Szerver hiba történt: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult CancelLunchSignup([FromForm] int userID, [FromForm] string day, [FromForm] string meal)
        {
            string sql = "UPDATE LunchSignup SET isSignedUp = 0 WHERE UserID = @UserID AND Day = @Day AND Meal = @Meal AND isSignedUp = 1";

            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@Day", day);
                cmd.Parameters.AddWithValue("@Meal", meal);
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return NotFound("Nem találtunk ilyen jelentkezést.");
                }
            }

            return Ok("Sikeres ebéd lejelentkezés!");
        }

        [HttpGet]
        public IActionResult GetUserLunchSignups(int userID)
        {
            List<LunchSignup> signups = new List<LunchSignup>();
            string sql = "SELECT * FROM LunchSignup WHERE UserID = @UserID AND isSignedUp = 1";

            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@UserID", userID);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    signups.Add(new LunchSignup
                    {
                        ID = reader.GetInt32(0),
                        UserID = reader.GetInt32(1),
                        Day = reader.GetString(2),
                        Meal = reader.GetString(3)
                    });
                }
            }

            return Json(signups);
        }
    }
}