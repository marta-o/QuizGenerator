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
using System.Windows.Navigation;

namespace QuizGenerator.ViewModels
{
    class AddQuizViewModel : BaseViewModel
    {
        private QuizRepository _quizRepository;
        private Frame _mainFrame;

        public AddQuizViewModel(Frame mainFrame)
        {
            _quizRepository = new QuizRepository();
            _mainFrame = mainFrame;

            AddQuestionCommand = new RelayCommand(AddQuestion);
            SaveQuizCommand = new RelayCommand(SaveQuiz);

            for (int i = 0; i < 4; i++)
            {
                Answers.Add(new Answer { Content = $"", IsCorrect = false });
            }
        }

        public string QuizName { get; set; }
        public string QuestionContent { get; set; }
        public ObservableCollection<Question> Questions { get; set; } = new ObservableCollection<Question>();
        public ObservableCollection<Answer> Answers { get; set; } = new ObservableCollection<Answer>();

        public ICommand AddQuestionCommand { get; }
        public ICommand SaveQuizCommand { get; }

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

        private void SaveQuiz()
        {
            var quiz = new Quiz
            {
                Name = QuizName,
                Questions = Questions.ToList()
            };

            using (var connection = new SQLiteConnection("Data Source=quizzes.db"))
            {
                connection.Open();
                _quizRepository.AddQuiz(connection, quiz);
            }
            _mainFrame.Navigate(new MainPage());
        }
    }
}

