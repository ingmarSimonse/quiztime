using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace Quiztime
{
    /// <summary>
    /// Interaction logic for EditQuiz.xaml
    /// </summary>
    public partial class EditQuiz : Window
    {
        Question question = new Question();
        Quiz quiz = new Quiz();
        int quizID;
        DataTable dtQuiz;

        public EditQuiz(int quizID)
        {
            InitializeComponent();

            this.quizID = quizID;
            dtQuiz = question.getDataQuizID(quizID);
            quiz.Read(quizID);
            txbQuizName.Text = quiz.title;
            if (dtQuiz.Rows.Count == 0)
            {
                dgQuiz.Height = 0;
            }
            dgQuiz.DataContext = dtQuiz;

            btnAdd.Click += BtnAdd_Click;
            btnSave.Click += BtnSave_Click;
        }

        public EditQuiz()
        {
            InitializeComponent();

            dtQuiz = new DataTable();

            btnAdd.Click += BtnAdd_Click;
            btnSave.Click += BtnSave_Click;

            dgQuiz.Height = 0;
            quizID = quiz.Create("Nieuwe Quiz");
            txbQuizName.Text = "Nieuwe Quiz";
        }

        private void UpdateDatagrid(int quizID)
        {
            dtQuiz = question.getDataQuizID(quizID);
            dgQuiz.DataContext = dtQuiz;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            quiz.Update(quizID, txbQuizName.Text);

            MainWindow window = new MainWindow();
            window.Show();
            this.Close();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            EditQuestion window = new EditQuestion(quizID);
            window.Show();
            this.Close();
        }

        public void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            int questionID = Convert.ToInt32(dtQuiz.Rows[dgQuiz.SelectedIndex]["ID"].ToString());
            EditQuestion window = new EditQuestion(questionID, quizID);
            window.Show();
            this.Close();
        }

        public void BtnDelete_Click(object sender, RoutedEventArgs e) {
            try
            {
                int questionID = Convert.ToInt32(dtQuiz.Rows[dgQuiz.SelectedIndex]["ID"].ToString());
                question.DeleteQuestion(questionID);

                UpdateDatagrid(quizID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // fix datagrid intercepting mousescroll
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 3);
        }

    }
}
