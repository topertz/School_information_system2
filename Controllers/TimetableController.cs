using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;

namespace SchoolAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TimetableController : Controller
    {
        [HttpGet]
        public IActionResult GetAllRooms()
        {
            List<Room> rooms = new List<Room>();
            string sql = "SELECT RoomID, RoomName FROM Room ORDER BY Name";

            using SQLiteConnection conn = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                rooms.Add(new Room { RoomID = reader.GetInt32(0), RoomName = reader.GetString(1) });
            }
            return Json(rooms);
        }

        [HttpGet]
        public IActionResult GetAllSubjects()
        {
            List<Subject> subjects = new List<Subject>();
            string sql = "SELECT SubjectID, SubjectName FROM Subject ORDER BY Name";

            using SQLiteConnection conn = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                subjects.Add(new Subject { SubjectID = reader.GetInt32(0), SubjectName = reader.GetString(1) });
            }
            return Json(subjects);
        }

        [HttpGet]
        public IActionResult GetAllClasses()
        {
            List<Class> classes = new List<Class>();
            string sql = "SELECT ClassID, ClassName FROM Class ORDER BY Name";

            using SQLiteConnection conn = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                classes.Add(new Class { ClassID = reader.GetInt32(0), ClassName = reader.GetString(1) });
            }
            return Json(classes);
        }

        [HttpGet]
        public IActionResult GetTimetable()
        {
            List<TimetableEntry> timetable = new List<TimetableEntry>();
            string sql = "SELECT TimetableID, Day, Hour, SubjectID, RoomID, TeacherID, ClassID FROM Timetable ORDER BY Day, Hour";

            using SQLiteConnection conn = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                timetable.Add(new TimetableEntry
                {
                    TimetableID = reader.GetInt32(0),
                    Day = reader.GetString(1),
                    Hour = reader.GetString(2),
                    SubjectID = reader.GetInt32(3),
                    RoomID = reader.GetInt32(4),
                    TeacherID = reader.GetInt32(5),
                    ClassID = reader.GetInt32(6)
                });
            }
            return Json(timetable);
        }

        [HttpPost]
        public IActionResult CreateTimetableEntry([FromForm] string day, [FromForm] string hour, [FromForm] int subjectID, [FromForm] int roomID, [FromForm] int teacherID, [FromForm] int classID)
        {
            if (CheckForConflicts(day, hour, teacherID, roomID, classID))
                return BadRequest("Ütközés van a megadott idõpontban tanár, terem vagy osztály szinten!");

            string sql = @"INSERT INTO Timetable (Day, Hour, SubjectID, RoomID, TeacherID, ClassID) 
                       VALUES (@Day, @Hour, @SubjectID, @RoomID, @TeacherID, @ClassID)";

            using SQLiteConnection conn = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Day", day);
            cmd.Parameters.AddWithValue("@Hour", hour);
            cmd.Parameters.AddWithValue("@SubjectID", subjectID);
            cmd.Parameters.AddWithValue("@RoomID", roomID);
            cmd.Parameters.AddWithValue("@TeacherID", teacherID);
            cmd.Parameters.AddWithValue("@ClassID", classID);
            cmd.ExecuteNonQuery();

            return Ok("Órarend bejegyzés sikeresen létrehozva.");
        }

        [HttpPost]
        public IActionResult UpdateTimetableEntry([FromForm] int timetableID, [FromForm] string? day, [FromForm] string? hour, [FromForm] int? subjectID, [FromForm] int? roomID, [FromForm] int? teacherID, [FromForm] int? classID)
        {
            string selectSql = "SELECT Day, Hour, TeacherID, RoomID, ClassID FROM Timetable WHERE TimetableID = @TimetableID";

            using SQLiteConnection conn = DatabaseConnector.CreateNewConnection();
            string currentDay = null!;
            string currentHour = null!;
            int currentTeacherID = 0;
            int currentRoomID = 0;
            int currentClassID = 0;

            using (SQLiteCommand selectCmd = new SQLiteCommand(selectSql, conn))
            {
                selectCmd.Parameters.AddWithValue("@TimetableID", timetableID);
                SQLiteDataReader reader = selectCmd.ExecuteReader();
                if (reader.Read())
                {
                    currentDay = reader.GetString(0);
                    currentHour = reader.GetString(1);
                    currentTeacherID = reader.GetInt32(2);
                    currentRoomID = reader.GetInt32(3);
                    currentClassID = reader.GetInt32(4);
                }
                else
                {
                    return NotFound("Órarend bejegyzés nem található");
                }
            }

            string newDay = day ?? currentDay;
            string newHour = hour ?? currentHour;
            int newTeacherID = teacherID ?? currentTeacherID;
            int newRoomID = roomID ?? currentRoomID;
            int newClassID = classID ?? currentClassID;

            if (CheckForConflicts(newDay, newHour, newTeacherID, newRoomID, newClassID, timetableID))
                return BadRequest("Ütközés van a megadott új beállításokkal!");

            string updateSql = @"
            UPDATE Timetable SET
                Day = @Day,
                Hour = @Hour,
                SubjectID = COALESCE(@SubjectID, SubjectID),
                RoomID = @RoomID,
                TeacherID = @TeacherID,
                ClassID = @ClassID
            WHERE TimetableID = @TimetableID";

            using SQLiteCommand updateCmd = new SQLiteCommand(updateSql, conn);
            updateCmd.Parameters.AddWithValue("@Day", newDay);
            updateCmd.Parameters.AddWithValue("@Hour", newHour);
            updateCmd.Parameters.AddWithValue("@SubjectID", subjectID.HasValue ? (object)subjectID.Value : DBNull.Value);
            updateCmd.Parameters.AddWithValue("@RoomID", newRoomID);
            updateCmd.Parameters.AddWithValue("@TeacherID", newTeacherID);
            updateCmd.Parameters.AddWithValue("@ClassID", newClassID);
            updateCmd.Parameters.AddWithValue("@TimetableID", timetableID);

            updateCmd.ExecuteNonQuery();

            return Ok("Órarend bejegyzés sikeresen frissítve.");
        }

        private bool CheckForConflicts(string day, string hour, int teacherID, int roomID, int classID, int? ignoreTimetableID = null)
        {
            string sql = @"
            SELECT COUNT(*)
            FROM Timetable
            WHERE Day = @Day AND Hour = @Hour
            AND (
                TeacherID = @TeacherID OR
                RoomID = @RoomID OR
                ClassID = @ClassID
            )";

            if (ignoreTimetableID.HasValue)
                sql += " AND TimetableID != @IgnoreID";

            using SQLiteConnection conn = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Day", day);
            cmd.Parameters.AddWithValue("@Hour", hour);
            cmd.Parameters.AddWithValue("@TeacherID", teacherID);
            cmd.Parameters.AddWithValue("@RoomID", roomID);
            cmd.Parameters.AddWithValue("@ClassID", classID);
            if (ignoreTimetableID.HasValue)
                cmd.Parameters.AddWithValue("@IgnoreID", ignoreTimetableID.Value);

            int count = Convert.ToInt32(cmd.ExecuteScalar());

            return count > 0;
        }

        [HttpGet]
        public IActionResult GetSubjects()
        {
            List<Subject> subjects = new List<Subject>();
            string sql = "SELECT SubjectID, SubjectName FROM Subject";

            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                subjects.Add(new Subject
                {
                    SubjectID = reader.GetInt32(0),
                    SubjectName = reader.GetString(1)
                });
            }

            return Json(subjects);
        }

        [HttpGet]
        public IActionResult GetRooms()
        {
            List<Room> rooms = new List<Room>();

            string sql = "SELECT RoomID, RoomName FROM Room";

            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                rooms.Add(new Room
                {
                    RoomID = reader.GetInt32(0),
                    RoomName = reader.GetString(1)
                });
            }

            return Json(rooms);
        }

        [HttpGet]
        public IActionResult GetClasses()
        {
            List<Class> classes = new List<Class>();

            string sql = "SELECT ClassID, ClassName FROM Class";

            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                classes.Add(new Class
                {
                    ClassID = reader.GetInt32(0),
                    ClassName = reader.GetString(1)
                });
            }

            return Json(classes);
        }

        [HttpPost]
        public IActionResult SeedSubjects()
        {
            var subjects = new List<string> {
                "Matematika", "Fizika", "Kémia", "Biológia", "Történelem",
                "Földrajz", "Irodalom", "Angol", "Német", "Informatika"
            };

            using var conn = DatabaseConnector.CreateNewConnection();
            foreach (var subjectName in subjects)
            {
                using var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM Subject WHERE SubjectName = @SubjectName", conn);
                checkCmd.Parameters.AddWithValue("@SubjectName", subjectName);
                var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;

                if (!exists)
                {
                    using var insertCmd = new SQLiteCommand("INSERT INTO Subject (SubjectName, IsClosed) VALUES (@SubjectName, @IsClosed)", conn);
                    insertCmd.Parameters.AddWithValue("@SubjectName", subjectName);
                    insertCmd.Parameters.AddWithValue("@IsClosed", false);
                    insertCmd.ExecuteNonQuery();
                }
            }

            return Ok("Subjects feltöltve.");
        }

        [HttpPost]
        public IActionResult SeedRooms()
        {
            var rooms = new List<string> {
                "101", "102", "103", "104", "105",
                "Informatika 1", "Informatika 2", "Labor", "Tornaterem", "Rajzterem"
            };

            using var conn = DatabaseConnector.CreateNewConnection();
            foreach (var roomName in rooms)
            {
                using var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM Room WHERE RoomName = @RoomName", conn);
                checkCmd.Parameters.AddWithValue("@RoomName", roomName);
                var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;

                if (!exists)
                {
                    using var insertCmd = new SQLiteCommand("INSERT INTO Room (RoomName) VALUES (@RoomName)", conn);
                    insertCmd.Parameters.AddWithValue("@RoomName", roomName);
                    insertCmd.ExecuteNonQuery();
                }
            }

            return Ok("Rooms feltöltve.");
        }

        [HttpPost]
        public IActionResult SeedClasses()
        {
            var classes = new List<string> {
                "9.A", "9.B", "9.C",
                "10.A", "10.B", "10.C",
                "11.A", "11.B", "11.C",
                "12.A", "12.B", "12.C"
            };

            using var conn = DatabaseConnector.CreateNewConnection();
            foreach (var className in classes)
            {
                using var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM Class WHERE ClassName = @ClassName", conn);
                checkCmd.Parameters.AddWithValue("@ClassName", className);
                var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;

                if (!exists)
                {
                    using var insertCmd = new SQLiteCommand("INSERT INTO Class (ClassName) VALUES (@ClassName)", conn);
                    insertCmd.Parameters.AddWithValue("@ClassName", className);
                    insertCmd.ExecuteNonQuery();
                }
            }

            return Ok("Osztályok feltöltve.");
        }
    }
}
