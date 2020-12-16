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
using System.Windows.Shapes;

namespace Chat.Friends
{
    /// <summary>
    /// Interaction logic for Add.xaml
    /// </summary>
    public partial class Add : Window
    {
        public Add()
        {
            InitializeComponent();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private async void SendRequest(object sender, RoutedEventArgs e)
        {
            UserIdTextbox.BorderBrush = Brushes.Black;
            MaterialDesignThemes.Wpf.HintAssist.SetHelperText(UserIdTextbox, "");

            string user_id = UserIdTextbox.Text;
            if (!string.IsNullOrEmpty(user_id))
            {
                if (!user_id.Equals(Global.current_user.Id))
                {
                    UserHandler user_handler = new UserHandler();
                    int err_code = await user_handler.SendRequest(user_id);
                    if (err_code == 0)
                    {
                        UserIdTextbox.Clear();
                        MaterialDesignThemes.Wpf.HintAssist.SetHelperText(UserIdTextbox, "Request sent");
                    }
                    else if (err_code == 1)
                    {
                        MessageBox.Show("Failed to send request. Please try again later.");
                    }
                    else
                    {
                        UserIdTextbox.BorderBrush = Brushes.Red;
                        MaterialDesignThemes.Wpf.HintAssist.SetHelperText(UserIdTextbox, "Invalid user ID");
                    }
                }
                else
                {
                    UserIdTextbox.BorderBrush = Brushes.Red;
                    MaterialDesignThemes.Wpf.HintAssist.SetHelperText(UserIdTextbox, "You cannot send request to yourself");
                }
            }
            else
            {
                UserIdTextbox.BorderBrush = Brushes.Red;
                MaterialDesignThemes.Wpf.HintAssist.SetHelperText(UserIdTextbox, "Please enter the user ID");
            }
        }
    }
}
