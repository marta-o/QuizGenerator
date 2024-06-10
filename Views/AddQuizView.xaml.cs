using System;
using System.Collections.Generic;
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
using QuizGenerator.ViewModels;

namespace QuizGenerator.Views
{
    /// <summary>
    /// Logika interakcji dla klasy AddQuizView.xaml
    /// </summary>
    public partial class AddQuizView : Page
    {
        private readonly Frame _mainFrame;

        public AddQuizView(Frame mainFrame)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            DataContext = new AddQuizViewModel(_mainFrame);
        }
    }
}
