﻿using System;
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

namespace Chat
{
    /// <summary>
    /// Interaction logic for LoadingModal.xaml
    /// </summary>
    public partial class LoadingModal : Window
    {
        public LoadingModal(Page parent)
        {
            InitializeComponent();

            if (parent.GetType().Equals(typeof(SignUpPage)))
            {
                ((SignUpPage) parent).StopLoading += delegate
                {
                    Close();
                };
            }
            else
            {
                ((LoginPage) parent).StopLoading += delegate
                {
                    Close();
                };
            }
        }
    }
}
