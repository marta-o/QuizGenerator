using QuizGenerator.Models;
using QuizGenerator.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGenerator.Repositories
{
    class QuestionRepository
    {
        public void AddQuestion(SQLiteConnection connection, Question question, int quizId)
        {
            string insertQuestionQuery = @"
                INSERT INTO Questions(Content, QuizId) 
                VALUES (@content, @quiz_id)";

            using (SQLiteCommand questionCommand = new SQLiteCommand(insertQuestionQuery, connection))
            {
                questionCommand.Parameters.AddWithValue("@content", question.Content);
                questionCommand.Parameters.AddWithValue("@quiz_id", quizId);
                questionCommand.ExecuteNonQuery();
            }

            int questionId = (int)connection.LastInsertRowId;

            AnswerRepository answerRepository = new AnswerRepository();

            foreach (Answer answer in question.Answers)
            {
                answerRepository.AddAnswer(connection, answer, questionId);
            }
        }
        public List<Question> GetQuestionsByQuizId(SQLiteConnection connection, int quizId)
        {
            string selectQuestionsQuery = "SELECT * FROM Questions WHERE QuizId = @quiz_id";
            List<Question> questions = new List<Question>();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using (SQLiteCommand questionCommand = new SQLiteCommand(selectQuestionsQuery, connection))
            {
                questionCommand.Parameters.AddWithValue("@quiz_id", quizId);

                using (SQLiteDataReader reader = questionCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Question question = new Question
                        {
                            Id = reader.GetInt32(0),
                            Content = reader.GetString(1),
                            QuizId = reader.GetInt32(2),
                            Answers = new List<Answer>()
                        };
                        questions.Add(question);
                    }
                }
            }

            AnswerRepository answerRepository = new AnswerRepository();

            foreach (var question in questions)
            {
                question.Answers = answerRepository.GetAnswersByQuestionId(connection, question.Id);
            }
            return questions;
        }
        public Question GetQuestionById(SQLiteConnection connection, int id)
        {
            string selectQuestionQuery = "SELECT * FROM Questions WHERE Id = @id";
            Question question = null;

            using (SQLiteCommand questionCommand = new SQLiteCommand(selectQuestionQuery, connection))
            {
                questionCommand.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = questionCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        question = new Question
                        {
                            Id = reader.GetInt32(0),
                            Content = reader.GetString(1),
                            QuizId = reader.GetInt32(2),
                            Answers = new List<Answer>()
                        };
                    }
                }
            }

            if (question != null)
            {
                AnswerRepository answerRepository = new AnswerRepository();
                question.Answers = answerRepository.GetAnswersByQuestionId(connection, question.Id);
            }
            return question;
        }
        public void DeleteQuestionFromDatabase(SQLiteConnection connection, int questionId)
        {
            AnswerRepository answerRepository = new AnswerRepository();
            answerRepository.DeleteAnswersByQuestionId(connection, questionId);

            string deleteQuestionQuery = "DELETE FROM Questions WHERE Id = @id";
            using (SQLiteCommand questionCommand = new SQLiteCommand(deleteQuestionQuery, connection))
            {
                questionCommand.Parameters.AddWithValue("@id", questionId);
                questionCommand.ExecuteNonQuery();
            }
        }
        public void UpdateQuestion(SQLiteConnection connection, Question question)
        {
            string updateQuestionQuery = "UPDATE Questions SET Content = @content WHERE Id = @id";

            using (SQLiteCommand questionCommand = new SQLiteCommand(updateQuestionQuery, connection))
            {
                questionCommand.Parameters.AddWithValue("@content", question.Content);
                questionCommand.Parameters.AddWithValue("@id", question.Id);
                questionCommand.ExecuteNonQuery();
            }

            AnswerRepository answerRepository = new AnswerRepository();

            foreach (var answer in question.Answers)
            {
                if (answer.Id == 0)
                {
                    answerRepository.AddAnswer(connection, answer, question.Id);
                }
                else
                {
                    answerRepository.UpdateAnswer(connection, answer);
                }
            }

            var existingAnswers = answerRepository.GetAnswersByQuestionId(connection, question.Id);
            foreach (var existingAnswer in existingAnswers)
            {
                if (!question.Answers.Exists(a => a.Id == existingAnswer.Id))
                {
                    answerRepository.DeleteAnswerFromDatabase(connection, existingAnswer.Id);
                }
            }
        }
    }
}
