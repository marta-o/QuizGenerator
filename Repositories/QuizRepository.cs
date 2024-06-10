using QuizGenerator.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGenerator.Repositories
{
    class QuizRepository
    {
        public void AddQuiz(SQLiteConnection connection, Quiz quiz)
        {
            string insertQuizQuery = @"INSERT INTO Quizzes(Name) VALUES (@name)";

            using (SQLiteCommand quizCommand = new SQLiteCommand(insertQuizQuery, connection))
            {
                quizCommand.Parameters.AddWithValue("@name", quiz.Name);
                quizCommand.ExecuteNonQuery();
            }

            int quizId = (int)connection.LastInsertRowId;
            QuestionRepository questionRepository = new QuestionRepository();

            foreach (var question in quiz.Questions)
            {
                questionRepository.AddQuestion(connection, question, quizId);
            }
        }

        public List<Quiz> GetAllQuizzes(SQLiteConnection connection)
        {
            string selectQuizzesQuery = "SELECT * FROM Quizzes";
            List<Quiz> quizzes = new List<Quiz>();

            using (SQLiteCommand quizCommand = new SQLiteCommand(selectQuizzesQuery, connection))
            {
                using (SQLiteDataReader quizReader = quizCommand.ExecuteReader())
                {
                    while (quizReader.Read())
                    {
                        int quizId = quizReader.GetInt32(0);
                        string quizName = quizReader.GetString(1);

                        QuestionRepository questionRepository = new QuestionRepository();
                        List<Question> questions = questionRepository.GetQuestionsByQuizId(connection, quizId);

                        quizzes.Add(new Quiz { Id = quizId, Name = quizName, Questions = questions });
                    }
                }
            }
            return quizzes;
        }

        public Quiz GetQuizById(SQLiteConnection connection, int id)
        {
            string selectQuizQuery = "SELECT * FROM Quizzes WHERE Id = @id";
            Quiz quiz = null;

            using (SQLiteCommand quizCommand = new SQLiteCommand(selectQuizQuery, connection))
            {
                quizCommand.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = quizCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        quiz = new Quiz
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        };

                        QuestionRepository questionRepository = new QuestionRepository();
                        quiz.Questions = questionRepository.GetQuestionsByQuizId(connection, quiz.Id);
                    }
                }
            }
            return quiz;
        }

        public void DeleteQuizFromDatabase(SQLiteConnection connection, int quizId)
        {
            QuestionRepository questionRepository = new QuestionRepository();
            List<Question> questions = questionRepository.GetQuestionsByQuizId(connection, quizId);
            foreach (var question in questions)
            {
                questionRepository.DeleteQuestionFromDatabase(connection, question.Id);
            }

            string deleteQuizQuery = "DELETE FROM Quizzes WHERE Id = @id";
            using (SQLiteCommand quizCommand = new SQLiteCommand(deleteQuizQuery, connection))
            {
                quizCommand.Parameters.AddWithValue("@id", quizId);
                quizCommand.ExecuteNonQuery();
            }
        }
        public void UpdateQuiz(SQLiteConnection connection, Quiz quiz)
        {
            string updateQuizQuery = "UPDATE Quizzes SET Name = @name WHERE Id = @id";

            using (SQLiteCommand quizCommand = new SQLiteCommand(updateQuizQuery, connection))
            {
                quizCommand.Parameters.AddWithValue("@name", quiz.Name);
                quizCommand.Parameters.AddWithValue("@id", quiz.Id);
                quizCommand.ExecuteNonQuery();
            }

            QuestionRepository questionRepository = new QuestionRepository();

            foreach (var question in quiz.Questions)
            {
                if (question.Id == 0)
                {
                    questionRepository.AddQuestion(connection, question, quiz.Id);
                }
                else
                {
                    questionRepository.UpdateQuestion(connection, question);
                }
            }

            var existingQuestions = questionRepository.GetQuestionsByQuizId(connection, quiz.Id);
            foreach (var existingQuestion in existingQuestions)
            {
                if (!quiz.Questions.Exists(q => q.Id == existingQuestion.Id))
                {
                    questionRepository.DeleteQuestionFromDatabase(connection, existingQuestion.Id);
                }
            }
        }
    }
}
