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

namespace Quiztime
{
    /// <summary>
    /// Interaction logic for Operator.xaml
    /// </summary>
    public partial class Operator : Window
    {
        Play play = new Play();
        bool isPauzed = false;

        public Operator(Play windowPlay)
        {
            InitializeComponent();

            play = windowPlay;

            btnPauze.Click += BtnPauze_Click;
            btnStop.Click += BtnStop_Click;
            btnPrevious.Click += BtnPrevious_Click;
            btnNext.Click += BtnNext_Click;
        }

        private void BtnPauze_Click(object sender, RoutedEventArgs e)
        {
            if (isPauzed)
            {
                play.StartTimer();
                isPauzed = false;
            } else
            {
                play.StopTimer();
                isPauzed = true;
            }
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Weet je zeker dat je de quiz wil stoppen?",
                "Question", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                MainWindow window = new MainWindow();
                window.Show();
                play.Close();
                this.Close();
            }

        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            play.PreviousQuestion();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            play.NextQuestion();
        }
    }
}
