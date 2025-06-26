using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;

namespace SchoolAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CourseController : Controller
    {
        [HttpGet]
        public IActionResult GetCoursesForUser()
        {
            try
            {
                string? sessionID = Request.Cookies["id"];
                Int64 userID = SessionManager.ValidateSession(sessionID);

                List<Course> courses = new List<Course>();

                string sql = @"
            SELECT c.CourseID, c.Name, c.TeacherID, c.IsVisible
            FROM Course c
            JOIN CourseUser cu ON c.CourseID = cu.CourseID
            WHERE cu.UserID = @UserID";

                using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();
                using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@UserID", userID);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    courses.Add(new Course
                    {
                        CourseID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        TeacherID = reader.GetInt32(2),
                        IsVisible = reader.GetBoolean(3)
                    });
                }

                return Json(courses);
            }
            catch (UnauthorizedAccessException)
            {
                Response.Cookies.Delete("id");
                return Unauthorized("Session expired or invalid");
            }
        }

        [HttpGet]
        public IActionResult GetMaterials(int courseID)
        {
            List<CourseMaterial> materials = new List<CourseMaterial>();

            string materialSql = "SELECT CourseMaterialID, Title, Content FROM CourseMaterial WHERE CourseID = @CourseID";

            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(materialSql, connection);
            cmd.Parameters.AddWithValue("@CourseID", courseID);
            using SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                materials.Add(new CourseMaterial
                {
                    CourseMaterialID = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Content = reader.GetString(2)
                });
            }

            return Json(materials);
        }

        [HttpGet]
        public IActionResult GetTests(int courseID)
        {
            List<Test> tests = new List<Test>();

            string sql = "SELECT TestID, TestName, Description, Deadline FROM Test WHERE CourseID = @CourseID";

            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@CourseID", courseID);
            using SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                tests.Add(new Test
                {
                    TestID = reader.GetInt32(0),
                    TestName = reader.GetString(1),
                    Description = reader.GetString(2),
                    Deadline = reader.GetDateTime(3)
                });
            }

            return Json(tests);
        }

        [HttpGet]
        public IActionResult GetAssignments(int courseID)
        {
            List<Assignment> assignments = new List<Assignment>();

            string sql = "SELECT AssignmentID, Title, Description, Deadline FROM Assignment WHERE CourseID = @CourseID";

            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@CourseID", courseID);
            using SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                assignments.Add(new Assignment
                {
                    AssignmentID = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.GetString(2),
                    Deadline = reader.GetDateTime(3)
                });
            }

            return Json(assignments);
        }

        [HttpPost]
        public IActionResult CreateCourse([FromForm] string name, [FromForm] bool isVisible = true)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Kurzus neve nem lehet üres.");

            try
            {
                string? sessionID = Request.Cookies["id"];
                Int64 teacherID = SessionManager.ValidateSession(sessionID);

                using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();

                string sqlInsertCourse = "INSERT INTO Course (Name, TeacherID, IsVisible) VALUES (@Name, @TeacherID, @IsVisible);";
                using (SQLiteCommand cmd = new SQLiteCommand(sqlInsertCourse, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@TeacherID", teacherID);
                    cmd.Parameters.AddWithValue("@IsVisible", isVisible ? 1 : 0);

                    cmd.ExecuteNonQuery();
                }

                long courseID = connection.LastInsertRowId;

                string sqlInsertCourseUser = "INSERT INTO CourseUser (CourseID, UserID) VALUES (@CourseID, @UserID);";
                using (SQLiteCommand cmd2 = new SQLiteCommand(sqlInsertCourseUser, connection))
                {
                    cmd2.Parameters.AddWithValue("@CourseID", courseID);
                    cmd2.Parameters.AddWithValue("@UserID", teacherID);
                    cmd2.ExecuteNonQuery();
                }

                return Ok(new { message = "Kurzus létrehozva.", courseID = courseID });
            }
            catch (UnauthorizedAccessException)
            {
                Response.Cookies.Delete("id");
                return Unauthorized("Session expired or invalid");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Hiba történt a kurzus létrehozásakor: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult AddMaterial([FromForm] int courseID, [FromForm] string title, [FromForm] string content)
        {
            string sql = "INSERT INTO CourseMaterial (CourseID, Title, Content) VALUES (@CourseID, @Title, @Content)";
            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@CourseID", courseID);
            cmd.Parameters.AddWithValue("@Title", title);
            cmd.Parameters.AddWithValue("@Content", content);
            cmd.ExecuteNonQuery();

            return Ok("Tananyag hozzáadva.");
        }

        [HttpPost]
        public IActionResult CreateTest([FromForm] int courseID, [FromForm] string testName, [FromForm] string description, [FromForm] System.DateTime deadline)
        {
            string sql = "INSERT INTO Test (CourseID, TestName, Description, Deadline) VALUES (@CourseID, @TestName, @Description, @Deadline)";
            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@CourseID", courseID);
            cmd.Parameters.AddWithValue("@TestName", testName);
            cmd.Parameters.AddWithValue("@Description", description);
            cmd.Parameters.AddWithValue("@Deadline", deadline);
            cmd.ExecuteNonQuery();

            return Ok("Teszt létrehozva.");
        }

        [HttpPost]
        public IActionResult CreateAssignment([FromForm] int courseID, [FromForm] string title, [FromForm] string description, [FromForm] System.DateTime deadline)
        {
            string sql = "INSERT INTO Assignment (CourseID, Title, Description, Deadline) VALUES (@CourseID, @Title, @Description, @Deadline)";
            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@CourseID", courseID);
            cmd.Parameters.AddWithValue("@Title", title);
            cmd.Parameters.AddWithValue("@Description", description);
            cmd.Parameters.AddWithValue("@Deadline", deadline);
            cmd.ExecuteNonQuery();

            return Ok("Beadandó létrehozva.");
        }

        [HttpPost]
        public IActionResult SetVisibility([FromForm] int courseID, [FromForm] bool isVisible)
        {
            string sql = "UPDATE Course SET IsVisible = @IsVisible WHERE CourseID = @CourseID";
            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@IsVisible", isVisible ? 1 : 0);
            cmd.Parameters.AddWithValue("@CourseID", courseID);
            int affected = cmd.ExecuteNonQuery();

            if (affected == 0)
                return NotFound("Kurzus nem található.");

            return Ok("Kurzus láthatósága frissítve.");
        }

        [HttpGet]
        public IActionResult GetEnrolledStudents(int courseID)
        {
            List<User> students = new List<User>();

            string sql = @"
                SELECT u.UserID, u.Name, u.Email
                FROM User u
                JOIN CourseUser cu ON u.UserID = cu.UserID
                WHERE cu.CourseID = @CourseID AND u.Role = 'Student'";

            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@CourseID", courseID);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                students.Add(new User
                {
                    UserID = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                });
            }

            return Json(students);
        }
    }
}
