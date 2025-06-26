using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;

namespace SchoolAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class GradebookController : Controller
    {
        [HttpGet]
        public IActionResult GetGrades(int studentID, int subjectID)
        {
            try
            {
                List<GradeEntry> grades = new List<GradeEntry>();
                string sql = "SELECT GradeID, StudentID, SubjectID, Grade, Date FROM Grade WHERE StudentID = @StudentID AND SubjectID = @SubjectID";

                using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", studentID);
                        cmd.Parameters.AddWithValue("@SubjectID", subjectID);
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            grades.Add(new GradeEntry
                            {
                                GradeID = reader.GetInt32(0),
                                StudentID = reader.GetInt32(1),
                                SubjectID = reader.GetInt32(2),
                                Grade = reader.GetInt32(3),
                                Date = reader.GetString(4)
                            });
                        }
                    }

                }
                return Ok(grades);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Hiba történt: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult AddGrade([FromForm] int studentID, [FromForm] int subjectID, [FromForm] int grade, [FromForm] string date)
        {
            try
            {
                DateTime parsedDate = DateTime.Parse(date);

                string sql = "INSERT INTO Grade (StudentID, SubjectID, Grade, Date) VALUES (@StudentID, @SubjectID, @Grade, @Date)";
                using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", studentID);
                        cmd.Parameters.AddWithValue("@SubjectID", subjectID);
                        cmd.Parameters.AddWithValue("@Grade", grade);
                        cmd.Parameters.AddWithValue("@Date", parsedDate.ToString("yyyy-MM-dd"));
                        cmd.ExecuteNonQuery();
                    }
                }
                return Ok("Érdemjegy hozzáadva.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Hiba: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult DeleteGrade([FromForm] int gradeId)
        {
            string sql = "DELETE FROM Grade WHERE GradeID = @gradeId";
            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            {
                using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@gradeId", gradeId);
                int affected = cmd.ExecuteNonQuery();

                if (affected == 0)
                    return NotFound("Érdemjegy nem található.");
            }
            return Ok("Érdemjegy törölve.");
        }

        [HttpPost]
        public IActionResult SetSubjectClosed([FromForm] int subjectId, [FromForm] bool isClosed)
        {
            string sql = "UPDATE Subject SET IsClosed = @IsClosed WHERE SubjectID = @subjectId";
            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            {
                using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@IsClosed", isClosed ? 1 : 0);
                cmd.Parameters.AddWithValue("@subjectId", subjectId);
                int affected = cmd.ExecuteNonQuery();

                if (affected == 0)
                    return NotFound("Tantárgy nem található.");
            }
            return Ok(isClosed ? "Tantárgy lezárva." : "Tantárgy megnyitva.");
        }

        [HttpGet]
        public IActionResult GetAbsences(int studentID)
        {
            List<AbsenceEntry> absences = new List<AbsenceEntry>();
            string sql = "SELECT AbsenceID, StudentID, Date, IsJustified FROM Absence WHERE StudentID = @StudentID";

            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            {
                using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    absences.Add(new AbsenceEntry
                    {
                        AbsenceID = reader.GetInt32(0),
                        StudentID = reader.GetInt32(1),
                        Date = reader.GetDateTime(2),
                        IsJustified = reader.GetBoolean(3)
                    });
                }
            }
            return Json(absences);
        }

        [HttpPost]
        public IActionResult AddAbsence([FromForm] int studentID, [FromForm] DateTime date)
        {
            string sql = "INSERT INTO Absence (StudentID, Date, IsJustified) VALUES (@StudentID, @Date, 0)";
            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            {
                using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                cmd.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
                cmd.ExecuteNonQuery();
            }
            return Ok("Hiányzás hozzáadva.");
        }

        [HttpPost]
        public IActionResult DeleteAbsence([FromForm] int absenceId)
        {
            string sql = "DELETE FROM Absence WHERE AbsenceID = @absenceId";
            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            {
                using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@absenceId", absenceId);
                int affected = cmd.ExecuteNonQuery();

                if (affected == 0)
                    return NotFound("Hiányzás nem található.");
            }
            return Ok("Hiányzás törölve.");
        }

        [HttpPost]
        public IActionResult JustifyAbsence([FromForm] int absenceId)
        {
            string sql = "UPDATE Absence SET IsJustified = 1 WHERE AbsenceID = @absenceId";
            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            {
                using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@absenceId", absenceId);
                int affected = cmd.ExecuteNonQuery();

                if (affected == 0)
                    return NotFound("Hiányzás nem található.");

            }
            return Ok("Hiányzás igazolva.");
        }

        [HttpPost]
        public IActionResult RemoveJustification([FromForm] int absenceId)
        {
            string sql = "UPDATE Absence SET IsJustified = 0 WHERE AbsenceID = @absenceId AND IsJustified = 1";

            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@absenceId", absenceId);
                    int affected = cmd.ExecuteNonQuery();

                    if (affected == 0)
                        return NotFound("Hiányzás nem található, vagy már nincs igazolva.");
                }
            }

            return Ok("Hiányzás igazolása eltávolítva.");
        }
    }
}