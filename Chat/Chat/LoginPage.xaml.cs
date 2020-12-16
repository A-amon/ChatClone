using Chat.Classes;
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

namespace Chat
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        public event EventHandler StopLoading;

        private async void Login(object sender, RoutedEventArgs e)
        {
            //Main main_window = new Main();
            //main_window.Show();
            string email = EmailTextbox.Text;
            string password = PasswordTextbox.Password;

            //set default style
            //bottom border to gray
            EmailTextbox.BorderBrush = Brushes.Gray;
            PasswordTextbox.BorderBrush = Brushes.Gray;

            //set helper text to empty
            MaterialDesignThemes.Wpf.HintAssist.SetHelperText(EmailTextbox, "");
            MaterialDesignThemes.Wpf.HintAssist.SetHelperText(PasswordTextbox, "");

            Application.Current.MainWindow.Visibility = Visibility.Collapsed;

            //loading modal
            LoadingModal loading_modal = new LoadingModal(this);
            loading_modal.Show();

            if (!string.IsNullOrEmpty(email))
            {
                if (!string.IsNullOrEmpty(password))
                {
                    UserHandler user_handler = new UserHandler();
                    int err_code = await user_handler.LogIn(email, password);

                    Application.Current.MainWindow.Visibility = Visibility.Visible;

                    //stop loading
                    //call StopLoading event
                    //trigger LoadingModal to close
                    StopLoading.Invoke(this, EventArgs.Empty);

                    if (err_code == 0)
                    {
                        Main main_window = new Main();
                        main_window.Show();

                        Application.Current.MainWindow.Close();
                    }
                    else if (err_code == 1)
                        MessageBox.Show("Failed to log in. Please try again later.");
                    else if (err_code == 2)
                    {
                        EmailTextbox.BorderBrush = Brushes.Red;
                        MaterialDesignThemes.Wpf.HintAssist.SetHelperText(EmailTextbox, "Email not registered.");
                    }
                    else
                    {
                        PasswordTextbox.BorderBrush = Brushes.Red;
                        MaterialDesignThemes.Wpf.HintAssist.SetHelperText(PasswordTextbox, "Wrong password.");
                    }
                }
                else
                {
                    Application.Current.MainWindow.Visibility = Visibility.Visible;

                    StopLoading.Invoke(this, EventArgs.Empty);
                    PasswordTextbox.BorderBrush = Brushes.Red;
                    MaterialDesignThemes.Wpf.HintAssist.SetHelperText(PasswordTextbox, "Please enter your password.");
                }
            }
            else
            {
                Application.Current.MainWindow.Visibility = Visibility.Visible;

                StopLoading.Invoke(this, EventArgs.Empty);
                EmailTextbox.BorderBrush = Brushes.Red;
                MaterialDesignThemes.Wpf.HintAssist.SetHelperText(EmailTextbox, "Please enter your email address.");
            }
        }
    }
}
