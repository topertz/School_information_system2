using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;

namespace SchoolAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CalendarController : Controller
    {
        [HttpGet]
        public IActionResult GetEvents()
        {
            List<SchoolEvent> events = new List<SchoolEvent>();
            string sql = "SELECT EventID, Title, Description, Date, Type FROM Event ORDER BY Date";

            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                events.Add(new SchoolEvent
                {
                    EventID = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.GetString(2),
                    Date = reader.GetDateTime(3),
                    Type = reader.GetString(4)
                });
            }

            return Json(events);
        }

        [HttpPost]
        public IActionResult AddEvent([FromForm] string title, [FromForm] string description, [FromForm] System.DateTime date, [FromForm] string type)
        {
            string sql = "INSERT INTO Event (Title, Description, Date, Type) VALUES (@title, @description, @date, @type)";

            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@type", type);
            cmd.ExecuteNonQuery();

            return Ok("Esemény hozzáadva.");
        }

        [HttpPost]
        public IActionResult DeleteEvent([FromForm] int eventId)
        {
            string sql = "DELETE FROM Event WHERE EventID = @eventId";

            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@eventId", eventId);
            int affected = cmd.ExecuteNonQuery();

            if (affected == 0)
                return NotFound("Esemény nem található.");

            return Ok("Esemény törölve.");
        }

        [HttpPost]
        public IActionResult AssignEventToTimetable([FromForm] int timetableId, [FromForm] int eventId)
        {
            string sql = "INSERT INTO TimetableEventLink (TimetableEntryID, EventID) VALUES (@timetableId, @eventId)";

            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@timetableId", timetableId);
            cmd.Parameters.AddWithValue("@eventId", eventId);
            cmd.ExecuteNonQuery();

            return Ok("Esemény hozzárendelve az órarendhez.");
        }

        [HttpGet]
        public IActionResult GetEventsForTimetable([FromQuery] int timetableId)
        {
            string sql = @"
        SELECT e.EventID, e.Title, e.Description, e.Date, e.Type
        FROM TimetableEventLink l
        INNER JOIN Event e ON l.EventID = e.EventID
        WHERE l.TimetableEntryID = @timetableId";

            List<SchoolEvent> events = new();

            using SQLiteConnection connection = DatabaseConnector.CreateNewConnection();
            using SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@timetableId", timetableId);
            using SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                events.Add(new SchoolEvent
                {
                    EventID = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.GetString(2),
                    Date = reader.GetDateTime(3),
                    Type = reader.GetString(4)
                });
            }

            return Json(events);
        }
    }
}