using NotesApp.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace NotesApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        LoginVM viewModel;
        bool hasNotLoggedIn = true;

        public LoginWindow()
        {
            InitializeComponent();

            viewModel = Resources["vm"] as LoginVM;
            viewModel.Authenticated += ViewModel_Authenticated;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if(hasNotLoggedIn)
                Environment.Exit(0);
        }

        private void ViewModel_Authenticated(object? sender, EventArgs e)
        {
            hasNotLoggedIn = false;
            Close();
        }
    }
}
