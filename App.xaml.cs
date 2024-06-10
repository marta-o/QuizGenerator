using System.Configuration;
using System.Data;
using System.Windows;
using QuizGenerator.Repositories;
using QuizGenerator.ViewModels;
using QuizGenerator.Views;

namespace QuizGenerator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DatabaseInitializer.InitializeDatabase();
        }
    }
}
