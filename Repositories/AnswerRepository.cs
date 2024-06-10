using QuizGenerator.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGenerator.Repositories
{
    class AnswerRepository
    {
        public void AddAnswer(SQLiteConnection connection, Answer answer, int questionId)
        {
            string insertAnswerQuery = @"
                INSERT INTO Answers(Content, IsCorrect, QuestionId) 
                VALUES (@content, @is_correct, @question_id)";

            using (SQLiteCommand answerCommand = new SQLiteCommand(insertAnswerQuery, connection))
            {
                answerCommand.Parameters.AddWithValue("@content", answer.Content);
                answerCommand.Parameters.AddWithValue("@is_correct", answer.IsCorrect ? 1 : 0);
                answerCommand.Parameters.AddWithValue("@question_id", questionId);
                answerCommand.ExecuteNonQuery();
            }
        }
        public List<Answer> GetAnswersByQuestionId(SQLiteConnection connection, int questionId)
        {
            string selectAnswersQuery = "SELECT * FROM Answers WHERE QuestionId = @question_id";
            List<Answer> answers = new List<Answer>();

            using (SQLiteCommand answerCommand = new SQLiteCommand(selectAnswersQuery, connection))
            {
                answerCommand.Parameters.AddWithValue("@question_id", questionId);

                using (SQLiteDataReader reader = answerCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Answer answer = new Answer
                        {
                            Id = reader.GetInt32(0),
                            Content = reader.GetString(1),
                            IsCorrect = reader.GetInt32(2) == 1,
                            QuestionId = reader.GetInt32(3)
                        };
                        answers.Add(answer);
                    }
                }
            }
            return answers;
        }
        public void DeleteAnswersByQuestionId(SQLiteConnection connection, int questionId)
        {
            string deleteAnswersQuery = "DELETE FROM Answers WHERE QuestionId = @question_id";
            using (SQLiteCommand answerCommand = new SQLiteCommand(deleteAnswersQuery, connection))
            {
                answerCommand.Parameters.AddWithValue("@question_id", questionId);
                answerCommand.ExecuteNonQuery();
            }
        }
        public void DeleteAnswerFromDatabase(SQLiteConnection connection, int answerId)
        {
            string deleteAnswerQuery = "DELETE FROM Answers WHERE Id = @id";
            using (SQLiteCommand answerCommand = new SQLiteCommand(deleteAnswerQuery, connection))
            {
                answerCommand.Parameters.AddWithValue("@id", answerId);
                answerCommand.ExecuteNonQuery();
            }
        }
        public void UpdateAnswer(SQLiteConnection connection, Answer answer)
        {
            string updateAnswerQuery = "UPDATE Answers SET Content = @content, IsCorrect = @is_correct WHERE Id = @id";

            using (SQLiteCommand answerCommand = new SQLiteCommand(updateAnswerQuery, connection))
            {
                answerCommand.Parameters.AddWithValue("@content", answer.Content);
                answerCommand.Parameters.AddWithValue("@is_correct", answer.IsCorrect);
                answerCommand.Parameters.AddWithValue("@id", answer.Id);
                answerCommand.ExecuteNonQuery();
            }
        }
    }
}

