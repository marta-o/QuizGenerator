using QuizGenerator.Models;
using QuizGenerator.Repositories;
using QuizGenerator.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuizGenerator.ViewModels
{
    public class EditQuizViewModel : BaseViewModel
    {
        private QuizRepository _quizRepository;
        private QuestionRepository _questionRepository;
        private AnswerRepository _answerRepository;
        private SQLiteConnection _connection;
        private Frame _mainFrame;

        public EditQuizViewModel(SQLiteConnection connection, Quiz quiz, Frame mainFrame)
        {

            _quizRepository = new QuizRepository();
            _questionRepository = new QuestionRepository();
            _answerRepository = new AnswerRepository();
            _connection = connection;
            _mainFrame = mainFrame;

            Quiz = quiz;
            QuizName = quiz.Name;
            Questions = new ObservableCollection<Question>();
            Answers = new ObservableCollection<Answer>();

            LoadQuestions(Quiz.Id);

            AddQuestionCommand = new RelayCommand(AddQuestion);
            DeleteQuestionCommand = new RelayCommand(DeleteQuestion);
            SaveQuizCommand = new RelayCommand(SaveQuiz);


            for (int i = 0; i < 4; i++)
            {
                Answers.Add(new Answer { Content = $"", IsCorrect = false });
            }
        }

        public string QuizName { get; set; }
        public string QuestionContent { get; set; }
        public ObservableCollection<Question> Questions { get; set; }
        public ObservableCollection<Answer> Answers { get; set; }
        public Quiz Quiz { get; set; }
        public Question SelectedQuestion { get; set; }

        public ICommand AddQuestionCommand { get; }
        public ICommand DeleteQuestionCommand { get; }
        public ICommand SaveQuizCommand { get; }

        private void LoadQuestions(int id)
        {
            var questions = _questionRepository.GetQuestionsByQuizId(_connection, id);
            Questions.Clear();
            foreach (var question in questions)
            {
                Questions.Add(question);
            }
        }
        private void AddQuestion()
        { 
            var question = new Question
            {
                Content = QuestionContent,
                Answers = new List<Answer>(Answers)
            };

            Questions.Add(question);
            QuestionContent = string.Empty;
            Answers.Clear();

            for (int i = 0; i < 4; i++)
            {
                Answers.Add(new Answer { Content = "", IsCorrect = false });
            }
        }

        private void DeleteQuestion()
        {
            if (SelectedQuestion == null) return;
            _questionRepository.DeleteQuestionFromDatabase(_connection, SelectedQuestion.Id);
            Questions.Remove(SelectedQuestion);
        }

        private void SaveQuiz()
        {
            Quiz.Name = QuizName;
            Quiz.Questions = Questions.ToList();

            _quizRepository.UpdateQuiz(_connection, Quiz);

            foreach (var question in Quiz.Questions)
            {
                if (question.Id == 0)
                {
                    _questionRepository.AddQuestion(_connection, question, Quiz.Id);
                }
                else
                {
                    _questionRepository.UpdateQuestion(_connection, question);
                }
                _mainFrame.Navigate(new MainPage());
            }
        }
    }
}
