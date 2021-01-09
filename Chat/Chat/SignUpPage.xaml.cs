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
    /// Interaction logic for SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        public SignUpPage()
        {
            InitializeComponent();
            EmailTextbox.Focus();
        }

        public event EventHandler StopLoading;

        private async void SignUp(object sender, RoutedEventArgs e)
        {
            string email_address = EmailTextbox.Text;
            string username = UsernameTextbox.Text;
            string password = PasswordTextbox.Password;

            //set default style
            //bottom border color to gray
            EmailTextbox.BorderBrush = Brushes.Gray;
            UsernameTextbox.BorderBrush = Brushes.Gray;
            PasswordTextbox.BorderBrush = Brushes.Gray;

            //helper text to empty
            MaterialDesignThemes.Wpf.HintAssist.SetHelperText(EmailTextbox, "");
            MaterialDesignThemes.Wpf.HintAssist.SetHelperText(PasswordTextbox, "");
            MaterialDesignThemes.Wpf.HintAssist.SetHelperText(UsernameTextbox, "");

            Application.Current.MainWindow.Visibility = Visibility.Collapsed;

            //loading modal
            LoadingModal loading_modal = new LoadingModal(this);
            loading_modal.Show();

            if (!string.IsNullOrEmpty(email_address))
            {
                if (!string.IsNullOrEmpty(username))
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        UserHandler user_handler = new UserHandler();
                        int err_code = await user_handler.Register(email_address, username, password);

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
                        {
                            MessageBox.Show("Failed to add user. Please try again later.");
                        }
                        else
                        {
                            EmailTextbox.BorderBrush = Brushes.Red;
                            MaterialDesignThemes.Wpf.HintAssist.SetHelperText(EmailTextbox, "Email has already been registered.");
                        }
                    }
                    else
                    {
                        Application.Current.MainWindow.Visibility = Visibility.Visible;

                        StopLoading.Invoke(this, EventArgs.Empty);
                        PasswordTextbox.BorderBrush = Brushes.Red;
                        MaterialDesignThemes.Wpf.HintAssist.SetHelperText(PasswordTextbox, "Please enter your password");
                    }
                }
                else
                {
                    Application.Current.MainWindow.Visibility = Visibility.Visible;

                    StopLoading.Invoke(this, EventArgs.Empty);
                    UsernameTextbox.BorderBrush = Brushes.Red;
                    MaterialDesignThemes.Wpf.HintAssist.SetHelperText(UsernameTextbox, "Please enter your username");
                }
            }
            else
            {
                Application.Current.MainWindow.Visibility = Visibility.Visible;

                StopLoading.Invoke(this, EventArgs.Empty);
                EmailTextbox.BorderBrush = Brushes.Red;
                MaterialDesignThemes.Wpf.HintAssist.SetHelperText(EmailTextbox, "Please enter your email address");
            }
        }
    }
}
