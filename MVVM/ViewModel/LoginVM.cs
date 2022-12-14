using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NotesApp.MVVM.Model;
using NotesApp.MVVM.ViewModel.Commands;
using NotesApp.MVVM.ViewModel.Helpers;

namespace NotesApp.MVVM.ViewModel
{
    public class LoginVM : INotifyPropertyChanged
    {
        //Fields
        private bool _isShowingRegister = false;
        private User _user;
        private string _userName;
        private string _password;
        private string _name;
        private string _lastName;
        private string _confirmPassword;
        private Visibility _loginVisible;
        private Visibility _registerVisible;

        //Events
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler Authenticated;

        //Constructor
        public LoginVM()
        {
            LoginVisible = Visibility.Visible;
            RegisterVisible = Visibility.Collapsed;

            RegisterCommand = new RegisterCommand(this);
            LoginCommand = new LoginCommand(this);
            ShowRegisterCommand = new ShowRegisterCommand(this);

            User = new User();
        }

        //Properties
        public User User
        {
            get { return _user; }
            set 
            { 
                _user = value;
                OnPropertyChanged(nameof(User));
            }
        }

        public string Username
        {
            get { return _userName; }
            set
            {
                _userName = value;
                User = new User
                {
                    Username = _userName,
                    Password = this.Password,
                    Name = this.Name,
                    Lastname = this.LastName,
                    ConfirmPassword = this.ConfirmPassword
                };
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                User = new User
                {
                    Username = this.Username,
                    Password = _password,
                    Name = this.Name,
                    Lastname = this.LastName,
                    ConfirmPassword = this.ConfirmPassword,
                };
                OnPropertyChanged(nameof(Password));
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                User = new User
                {
                    Username = this.Username,
                    Password = this.Password,
                    Name = _name,
                    Lastname = this.LastName,
                    ConfirmPassword = this.ConfirmPassword
                };
                OnPropertyChanged(nameof(Name));
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                User = new User
                {
                    Username = this.Username,
                    Password = this.Password,
                    Name = this.Name,
                    Lastname = _lastName,
                    ConfirmPassword = this.ConfirmPassword
                };
                OnPropertyChanged(nameof(LastName));
            }
        }

        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                _confirmPassword = value;
                User = new User
                {
                    Username = this.Username,
                    Password = this.Password,
                    Name = this.Name,
                    Lastname = this.LastName,
                    ConfirmPassword = _confirmPassword
                };
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }

        public Visibility LoginVisible
        {
            get { return _loginVisible; }
            set 
            { 
                _loginVisible = value; 
                OnPropertyChanged(nameof(LoginVisible));
            }
        }

        public Visibility RegisterVisible
        {
            get { return _registerVisible; }
            set
            {
                _registerVisible = value;
                OnPropertyChanged(nameof(RegisterVisible));
            }
        }

        public RegisterCommand RegisterCommand { get; set; }
        public LoginCommand LoginCommand { get; set; }
        public ShowRegisterCommand ShowRegisterCommand { get; set; }


        //Methods
        public void SwitchViews()
        {
            _isShowingRegister = !_isShowingRegister;

            if(_isShowingRegister)
            {
                RegisterVisible = Visibility.Visible;
                LoginVisible = Visibility.Collapsed;
            }
            else
            {
                LoginVisible = Visibility.Visible;
                RegisterVisible = Visibility.Collapsed;
            }
        }

        public async void Login()
        {
            bool result = await FirebaseAuthHelper.Login(User);

            if(result)
            {
                Authenticated?.Invoke(this, new EventArgs());
            }
        }

        public async void Register()
        {
            bool result = await FirebaseAuthHelper.Register(User);

            if (result)
            {
                Authenticated?.Invoke(this, new EventArgs());
            }

        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
