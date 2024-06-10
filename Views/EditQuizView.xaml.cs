using QuizGenerator.Models;
using QuizGenerator.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuizGenerator.Views
{
    /// <summary>
    /// Logika interakcji dla klasy EditQuizView.xaml
    /// </summary>
    public partial class EditQuizView : Page
    {
        private readonly Frame _mainFrame;
        private readonly SQLiteConnection _connection;

        public EditQuizView(Frame mainFrame, Quiz quiz)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _connection = new SQLiteConnection("Data Source=quizzes.db");
            DataContext = new EditQuizViewModel(_connection, quiz, _mainFrame);
        }
    }
}
