using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Linq;

namespace SchoolAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class StatisticsController : Controller
    {
        [HttpGet]
        public IActionResult GetStudentStatistics(int studentID)
        {
            try
            {
                var grades = GetGrades(studentID);
                var absences = GetAbsences(studentID);
                var average = GetGradesAverage(studentID);
                return Ok(new { grades = grades, absences = absences });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Hiba történt: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult GetTeacherStatistics(int teacherID)
        {
            try
            {
                var students = GetStudentsByTeacher(teacherID);
                var studentStatistics = new List<object>();

                foreach (var student in students)
                {
                    var grades = GetGrades(student.StudentID);
                    var absences = GetAbsences(student.StudentID);
                    studentStatistics.Add(new { Student = student, Grades = grades, Absences = absences });
                }

                return Ok(studentStatistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Hiba történt: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult GetAdminStatistics()
        {
            try
            {
                var allStudents = GetAllStudents();
                var allTeachers = GetAllTeachers();
                var allClasses = GetAllClasses();
                var statistics = new
                {
                    Students = allStudents,
                    Teachers = allTeachers,
                    Classes = allClasses
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Hiba történt: {ex.Message}");
            }
        }
        private List<GradeEntry> GetGrades(int studentID)
        {
            var grades = new List<GradeEntry>();
            string sql = "SELECT GradeID, StudentID, SubjectID, Grade, Date FROM Grade WHERE StudentID = @StudentID";

            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentID);
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
            return grades;
        }

        private List<AbsenceEntry> GetAbsences(int studentID)
        {
            var absences = new List<AbsenceEntry>();
            string sql = "SELECT AbsenceID, StudentID, Date, IsJustified FROM Absence WHERE StudentID = @StudentID";

            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
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
            }
            return absences;
        }

        private List<Student> GetStudentsByTeacher(int teacherID)
        {
            var students = new List<Student>();
            string sql = "SELECT s.StudentID, s.StudentName FROM Student s INNER JOIN SubjectTeacher st ON s.ClassID = st.ClassID WHERE st.TeacherID = @TeacherID";

            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@TeacherID", teacherID);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        students.Add(new Student
                        {
                            StudentID = reader.GetInt32(0),
                            StudentName = reader.GetString(1)
                        });
                    }
                }
            }
            return students;
        }

        private List<Student> GetAllStudents()
        {
            var students = new List<Student>();
            string sql = "SELECT StudentID, StudentName FROM Student";

            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        students.Add(new Student
                        {
                            StudentID = reader.GetInt32(0),
                            StudentName = reader.GetString(1)
                        });
                    }
                }
            }
            return students;
        }

        private List<Teacher> GetAllTeachers()
        {
            var teachers = new List<Teacher>();
            string sql = "SELECT TeacherID, TeacherName FROM Teacher";

            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        teachers.Add(new Teacher
                        {
                            TeacherID = reader.GetInt32(0),
                            TeacherName = reader.GetString(1)
                        });
                    }
                }
            }
            return teachers;
        }

        private List<Class> GetAllClasses()
        {
            var classes = new List<Class>();
            string sql = "SELECT ClassID, ClassName FROM Class";

            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        classes.Add(new Class
                        {
                            ClassID = reader.GetInt32(0),
                            ClassName = reader.GetString(1)
                        });
                    }
                }
            }
            return classes;
        }

        [HttpGet]
        public IActionResult GetGradesAverage(int studentID)
        {
            var grades = new List<GradeEntry>();
            string sql = "SELECT Grade FROM Grade WHERE StudentID = @StudentID";

            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentID);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        grades.Add(new GradeEntry
                        {
                            Grade = reader.GetInt32(0)
                        });
                    }
                }
            }

            if (grades.Count == 0)
            {
                return Ok(new { average = 0 });
            }

            double average = grades.Average(g => g.Grade);
            return Ok(new { average = average });
        }

        [HttpPost]
        public IActionResult SeedStudents()
        {
            var students = new List<(string Name, string Class)>
            {
                ("John Doe", "9.A"),
                ("Jane Smith", "9.B"),
                ("Alex Johnson", "10.A"),
                ("Emily Davis", "10.B"),
                ("Michael Brown", "11.A"),
                ("Sarah Wilson", "11.B")
            };

            using var conn = DatabaseConnector.CreateNewConnection();
            foreach (var student in students)
            {
                using var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM Student WHERE StudentName = @StudentName", conn);
                checkCmd.Parameters.AddWithValue("@StudentName", student.Name);
                var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;

                if (!exists)
                {
                    using var insertCmd = new SQLiteCommand("INSERT INTO Student (StudentName, ClassID) VALUES (@StudentName, (SELECT ClassID FROM Class WHERE ClassName = @ClassName LIMIT 1))", conn);
                    insertCmd.Parameters.AddWithValue("@StudentName", student.Name);
                    insertCmd.Parameters.AddWithValue("@ClassName", student.Class);
                    insertCmd.ExecuteNonQuery();
                }
            }

            return Ok("Students feltöltve.");
        }

        [HttpPost]
        public IActionResult SeedTeachers()
        {
            var teachers = new List<string>
            {
                "Dr. Kovács Gábor", "Szabó Anna", "Tóth Béla", "Nagy Éva", "Farkas Péter"
            };

            using var conn = DatabaseConnector.CreateNewConnection();
            foreach (var teacherName in teachers)
            {
                using var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM Teacher WHERE TeacherName = @TeacherName", conn);
                checkCmd.Parameters.AddWithValue("@TeacherName", teacherName);
                var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;

                if (!exists)
                {
                    using var insertCmd = new SQLiteCommand("INSERT INTO Teacher (TeacherName) VALUES (@TeacherName)", conn);
                    insertCmd.Parameters.AddWithValue("@TeacherName", teacherName);
                    insertCmd.ExecuteNonQuery();
                }
            }

            return Ok("Teachers feltöltve.");
        }

        [HttpPost]
        public IActionResult SeedSubjectTeachers()
        {
            var subjectTeacherAssignments = new List<(string TeacherName, string SubjectName, string ClassName)>
            {
                ("Dr. Kovács Gábor", "Matematika", "9.A"),
                ("Szabó Anna", "Fizika", "9.B"),
                ("Tóth Béla", "Kémia", "10.A"),
                ("Nagy Éva", "Történelem", "10.B"),
                ("Farkas Péter", "Irodalom", "11.A")
            };

            using var conn = DatabaseConnector.CreateNewConnection();
            foreach (var assignment in subjectTeacherAssignments)
            {
                using var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM SubjectTeacher WHERE TeacherID = (SELECT TeacherID FROM Teacher WHERE TeacherName = @TeacherName LIMIT 1) AND SubjectID = (SELECT SubjectID FROM Subject WHERE SubjectName = @SubjectName LIMIT 1)", conn);
                checkCmd.Parameters.AddWithValue("@TeacherName", assignment.TeacherName);
                checkCmd.Parameters.AddWithValue("@SubjectName", assignment.SubjectName);
                var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;

                if (!exists)
                {
                    using var getClassCmd = new SQLiteCommand("SELECT ClassID FROM Class WHERE ClassName = @ClassName LIMIT 1", conn);
                    getClassCmd.Parameters.AddWithValue("@ClassName", assignment.ClassName);
                    var classId = getClassCmd.ExecuteScalar();

                    if (classId != null)
                    {
                        using var insertCmd = new SQLiteCommand("INSERT INTO SubjectTeacher (TeacherID, SubjectID, ClassID) VALUES ((SELECT TeacherID FROM Teacher WHERE TeacherName = @TeacherName LIMIT 1), (SELECT SubjectID FROM Subject WHERE SubjectName = @SubjectName LIMIT 1), @ClassID)", conn);
                        insertCmd.Parameters.AddWithValue("@TeacherName", assignment.TeacherName);
                        insertCmd.Parameters.AddWithValue("@SubjectName", assignment.SubjectName);
                        insertCmd.Parameters.AddWithValue("@ClassID", classId);
                        insertCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        Console.Error.WriteLine($"Error: Class '{assignment.ClassName}' not found for subject '{assignment.SubjectName}'");
                    }
                }
            }

            return Ok("Subject-Teacher kapcsolatok feltöltve.");
        }
    }
}