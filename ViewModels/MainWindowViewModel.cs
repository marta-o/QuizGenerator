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
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace QuizGenerator.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly QuizRepository _quizRepository;
        private readonly SQLiteConnection _connection;
        private Frame _mainFrame;

        private ObservableCollection<Quiz> _quizzes;
        private Quiz _selectedQuiz;
        public ObservableCollection<Quiz> Quizzes
        {
            get => _quizzes;
            set
            {
                _quizzes = value;
                OnPropertyChanged();
            }
        }
        public Quiz SelectedQuiz
        {
            get => _selectedQuiz;
            set
            {
                _selectedQuiz = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddQuizCommand { get; }
        public ICommand EditQuizCommand { get; }
        public ICommand DeleteQuizCommand { get; }

        public MainWindowViewModel(Frame mainFrame)
        {
            _mainFrame = mainFrame;
            _connection = new SQLiteConnection("Data Source=quizzes.db");
            _connection.Open();
            _quizRepository = new QuizRepository();

            Quizzes = new ObservableCollection<Quiz>();
            LoadQuizzes();

            AddQuizCommand = new RelayCommand(AddQuiz);
            EditQuizCommand = new RelayCommand(EditQuiz, () => SelectedQuiz != null);
            DeleteQuizCommand = new RelayCommand(DeleteQuiz, () => SelectedQuiz != null);
        }

        private void LoadQuizzes()
        {
            var quizzes = _quizRepository.GetAllQuizzes(_connection);
            foreach (var quiz in quizzes)
            {
                Quizzes.Add(quiz);
            }
        }

        private void AddQuiz()
        {
            var addQuizView = new AddQuizView(_mainFrame);
            _mainFrame.Navigate(addQuizView);
        }

        private void EditQuiz()
        {
            if (SelectedQuiz == null) return;

            var editQuizView = new EditQuizView(_mainFrame, SelectedQuiz);
            _mainFrame.Navigate(editQuizView);
        }

        private void DeleteQuiz()
        {
            if (SelectedQuiz == null) return;
            _quizRepository.DeleteQuizFromDatabase(_connection, SelectedQuiz.Id);
            Quizzes.Remove(SelectedQuiz);
        }
    }
}
