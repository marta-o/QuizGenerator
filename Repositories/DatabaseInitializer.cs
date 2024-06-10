using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace QuizGenerator.Repositories
{
    public class DatabaseInitializer
    {
        public static void InitializeDatabase()
        {
            string connectionString = "Data Source=quizzes.db";

            if (!File.Exists("quizzes.db"))
            {
                SQLiteConnection.CreateFile("quizzes.db");
            }

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string createQuizTable = @"
                    CREATE TABLE IF NOT EXISTS Quizzes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL
                )";

                string createQuestionTable = @"
                CREATE TABLE IF NOT EXISTS Questions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Content TEXT NOT NULL,
                    QuizId INTEGER NOT NULL,
                    FOREIGN KEY (QuizId) REFERENCES Quizzes(Id) ON DELETE CASCADE
                )";

                string createAnswerTable = @"
                CREATE TABLE IF NOT EXISTS Answers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Content TEXT NOT NULL,
                    IsCorrect INTEGER NOT NULL,
                    QuestionId INTEGER NOT NULL,
                    FOREIGN KEY (QuestionId) REFERENCES Questions(Id) ON DELETE CASCADE
                )";

                using (var command = new SQLiteCommand(createQuizTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SQLiteCommand(createQuestionTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SQLiteCommand(createAnswerTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
