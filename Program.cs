using Microsoft.AspNetCore.Http.HttpResults;
using System.Data.SQLite;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseStaticFiles();

app.MapGet("/", () => Results.Redirect("/index.html"));
SQLiteConnection connection = DatabaseConnector.Db();
SQLiteCommand command = connection.CreateCommand();
command.CommandText = "PRAGMA foreign_keys = ON;" +
    "CREATE TABLE IF NOT EXISTS `User` (" +
    "`UserID` INTEGER NOT NULL PRIMARY KEY, " +
    "`Username` TEXT NOT NULL, " +
    "`PasswordHash` TEXT NOT NULL, " +
    "`PasswordSalt` TEXT NOT NULL, " +
    "`Role` TEXT NOT NULL);" +

    "CREATE TABLE IF NOT EXISTS `Session` (" +
    "`SessionCookie` TEXT NOT NULL PRIMARY KEY, " +
    "`UserID` INTEGER NOT NULL, " +
    "`ValidUntil` INTEGER NOT NULL, " +
    "`LoginTime` DATETIME NOT NULL, " +
    "FOREIGN KEY(`UserID`) REFERENCES User(`UserID`));" +

    "CREATE TABLE IF NOT EXISTS `Timetable` (" +
    "`TimetableID` NTEGER NOT NULL PRIMARY KEY, " +
    "`SubjectID` INTEGER NOT NULL, " +
    "`RoomID` INTEGER NOT NULL, " +
    "`TeacherID` INTEGER NOT NULL, " +
    "`ClassID` INTEGER NOT NULL, " +
    "`Day` TEXT NOT NULL, " +
    "`Hour` TEXT NOT NULL, " +
    "FOREIGN KEY(`RoomID`) REFERENCES Room(`RoomID`), " +
    "FOREIGN KEY(`SubjectID`) REFERENCES Subject(`SubjectID`), " +
    "FOREIGN KEY(`ClassID`) REFERENCES Class(`ClassID`));" +

    "CREATE TABLE IF NOT EXISTS `Test` (" +
    "`TestID` INTEGER NOT NULL PRIMARY KEY, " +
    "`CourseID` INTEGER NOT NULL, " +
    "`TestName` TEXT NOT NULL, " +
    "`Description` TEXT NOT NULL, " +
    "`Deadline` TEXT NOT NULL, " +
    "FOREIGN KEY(`CourseID`) REFERENCES Course(`CourseID`));" +

    "CREATE TABLE IF NOT EXISTS `Teacher` (" +
    "`TeacherID` INTEGER NOT NULL PRIMARY KEY, " +
    "`TeacherName` TEXT NOT NULL);" +

    "CREATE TABLE IF NOT EXISTS `SubjectTeacher` (" +
    "`SubjectTeacherID` INTEGER NOT NULL PRIMARY KEY, " +
    "`TeacherID` INTEGER NOT NULL, " +
    "`SubjectID` INTEGER NOT NULL, " +
    "`ClassID` INTEGER NOT NULL, " +
    "FOREIGN KEY(`SubjectID`) REFERENCES Subject(`SubjectID`), " +
    "FOREIGN KEY(`TeacherID`) REFERENCES Teacher(`TeacherID`), " +
    "FOREIGN KEY(`ClassID`) REFERENCES Class(`ClassID`));" +

    "CREATE TABLE IF NOT EXISTS `Subject` (" +
    "`SubjectID` INTEGER NOT NULL PRIMARY KEY, " +
    "`SubjectName` TEXT NOT NULL, " +
    "`IsClosed`	INTEGER NOT NULL);" +

    "CREATE TABLE IF NOT EXISTS `Student` (" +
    "`StudentID` INTEGER NOT NULL PRIMARY KEY, " +
    "`ClassID` INTEGER NOT NULL, " +
    "`StudentName` TEXT NOT NULL, " +
    "FOREIGN KEY(`ClassID`) REFERENCES Class(`ClassID`));" +

    "CREATE TABLE IF NOT EXISTS `Room` (" +
    "`RoomID` INTEGER NOT NULL PRIMARY KEY, " +
    "`RoomName` TEXT NOT NULL);" +

    "CREATE TABLE IF NOT EXISTS `LunchSignup` (" +
    "`SignupID` INTEGER NOT NULL PRIMARY KEY, " +
    "`UserID` INTEGER NOT NULL, " +
    "`Day` TEXT NOT NULL, " +
    "`Meal` TEXT NOT NULL, " +
    "`IsSignedUp` INTEGER NOT NULL, " +
    "FOREIGN KEY(`UserID`) REFERENCES User(`UserID`));" +

    "CREATE TABLE IF NOT EXISTS `Lunch` (" +
    "`LunchID` INTEGER NOT NULL PRIMARY KEY, " +
    "`Day` TEXT NOT NULL, " +
    "`Meal` TEXT NOT NULL);" +

    "CREATE TABLE IF NOT EXISTS `Grade` (" +
    "`GradeID` INTEGER NOT NULL PRIMARY KEY, " +
    "`StudentID` INTEGER NOT NULL, " +
    "`SubjectID` INTEGER NOT NULL, " +
    "`Grade` INTEGER NOT NULL, " +
    "`Date` DATETIME NOT NULL, " +
    "FOREIGN KEY(`SubjectID`) REFERENCES Subject(`SubjectID`));" +

    "CREATE TABLE IF NOT EXISTS `Event` (" +
    "`EventID` INTEGER NOT NULL PRIMARY KEY, " +
    "`Title` TEXT NOT NULL, " +
    "`Description` TEXT NOT NULL, " +
    "`Date` TEXT NOT NULL, " +
    "`Type` TEXT NOT NULL);" +

    "CREATE TABLE IF NOT EXISTS `CourseUser` (" +
    "`CourseUserID` INTEGER NOT NULL PRIMARY KEY, " +
    "`CourseID` INTEGER NOT NULL, " +
    "`UserID` INTEGER NOT NULL, " +
    "FOREIGN KEY(`CourseID`) REFERENCES Course(`CourseID`), " +
    "FOREIGN KEY(`UserID`) REFERENCES User(`UserID`));" +

    "CREATE TABLE IF NOT EXISTS `CourseMaterial` (" +
    "`CourseMaterialID` INTEGER NOT NULL PRIMARY KEY, " +
    "`CourseID` INTEGER NOT NULL, " +
    "`Title` TEXT NOT NULL, " +
    "`Content` TEXT NOT NULL, " +
    "FOREIGN KEY(`CourseID`) REFERENCES Course(`CourseID`));" +

    "CREATE TABLE IF NOT EXISTS `Course` (" +
    "`CourseID` INTEGER NOT NULL PRIMARY KEY, " +
    "`TeacherID` INTEGER NOT NULL, " +
    "`Name` TEXT NOT NULL, " +
    "`IsVisible` INTEGER NOT NULL);" +

    "CREATE TABLE IF NOT EXISTS `Class` (" +
    "`ClassID` INTEGER NOT NULL PRIMARY KEY, " +
    "`ClassName` TEXT NOT NULL);" +

    "CREATE TABLE IF NOT EXISTS `Assignment` (" +
    "`AssignmentID` INTEGER NOT NULL PRIMARY KEY, " +
    "`CourseID` INTEGER NOT NULL, " +
    "`Title` TEXT NOT NULL, " +
    "`Description` TEXT NOT NULL, " +
    "`Deadline` TEXT NOT NULL, " +
    "FOREIGN KEY(`CourseID`) REFERENCES Course(`CourseID`));" +

    "CREATE TABLE IF NOT EXISTS `Absence` (" +
    "`AbsenceID` INTEGER NOT NULL PRIMARY KEY, " +
    "`StudentID` INTEGER NOT NULL, " +
    "`Date` DATETIME NOT NULL, " +
    "`IsJustified` INTEGER NOT NULL);";

command.ExecuteNonQuery();
command.Dispose();

app.Run();