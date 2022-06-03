using System;
using System.Collections.Generic;
using System.Data;
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

namespace Quiztime
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Quiz quiz = new Quiz();
        DataSet dsQuiz;

        public MainWindow()
        {
            InitializeComponent();

            dsQuiz = quiz.getData();
            dsQuiz.Tables[0].Rows.Add();
            dsQuiz.Tables[0].Rows[dsQuiz.Tables[0].Rows.Count - 1]["title"] = "Nieuwe Quiz";
            dgQuiz.DataContext = dsQuiz;
        }

        private void UpdateDatagrid()
        {
            dsQuiz = quiz.getData();
            dsQuiz.Tables[0].Rows.Add();
            dsQuiz.Tables[0].Rows[dsQuiz.Tables[0].Rows.Count - 1]["title"] = "Nieuwe Quiz";
            dgQuiz.DataContext = dsQuiz;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(dsQuiz.Tables[0].Rows[dgQuiz.SelectedIndex]["ID"].ToString()))
            {
                int quizID = Convert.ToInt32(dsQuiz.Tables[0].Rows[dgQuiz.SelectedIndex]["ID"].ToString());
                quiz.DeleteQuiz(quizID);

                UpdateDatagrid();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(dsQuiz.Tables[0].Rows[dgQuiz.SelectedIndex]["ID"].ToString()))
            {
                int quizID = Convert.ToInt32(dsQuiz.Tables[0].Rows[dgQuiz.SelectedIndex]["ID"].ToString());
                EditQuiz window = new EditQuiz(quizID);
                window.Show();
                this.Close();
            } else
            {
                // new quiz
                EditQuiz window = new EditQuiz();
                window.Show();
                this.Close();
            }
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(dsQuiz.Tables[0].Rows[dgQuiz.SelectedIndex]["ID"].ToString()))
            {
                int quizID = Convert.ToInt32(dsQuiz.Tables[0].Rows[dgQuiz.SelectedIndex]["ID"].ToString());
                Play windowPlay = new Play(quizID, true);
                windowPlay.Show();
                Operator windowOperator = new Operator(windowPlay);
                windowOperator.Show();
                this.Close();
            }
        }

        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(dsQuiz.Tables[0].Rows[dgQuiz.SelectedIndex]["ID"].ToString()))
            {
                int quizID = Convert.ToInt32(dsQuiz.Tables[0].Rows[dgQuiz.SelectedIndex]["ID"].ToString());
                Play windowPlay = new Play(quizID, false);
                windowPlay.Show();
                Operator windowOperator = new Operator(windowPlay);
                windowOperator.Show();
                this.Close();
            }
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // fix datagrid intercepting mousescroll
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 3);
        }

    }
}
