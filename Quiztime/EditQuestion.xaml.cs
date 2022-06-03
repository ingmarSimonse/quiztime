using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for EditQuestion.xaml
    /// </summary>
    public partial class EditQuestion : Window
    {
        Question question = new Question();
        Answer answer = new Answer();
        DataTable dtQuestion = new DataTable();
        int quizID;
        int questionID = -1;
        string uploadFileName = "";
        string imageName = "";
        string dirImages = System.IO.Directory.GetCurrentDirectory() + "../../../../images/";

        public EditQuestion(int quizID)
        {
            InitializeComponent();

            this.quizID = quizID;
            btnSave.Click += BtnSave_Click;
            btnUploadImage.Click += BtnUploadImage_Click;
            btnDeleteImage.Click += BtnDeleteImage_Click;
        }

        public EditQuestion(int questionID, int quizID)
        {
            InitializeComponent();

            this.quizID = quizID;
            this.questionID = questionID;
            btnSave.Click += BtnSave_Click;
            btnUploadImage.Click += BtnUploadImage_Click;
            btnDeleteImage.Click += BtnDeleteImage_Click;

            dtQuestion = question.getFullQuestion(questionID);
            question.Read(questionID);
            txbQuestion.Text = dtQuestion.Rows[0]["question"].ToString();
            txbAnswerA.Text = dtQuestion.Rows[0]["answer1"].ToString();
            txbAnswerB.Text = dtQuestion.Rows[0]["answer2"].ToString();
            txbAnswerC.Text = dtQuestion.Rows[0]["answer3"].ToString();
            txbAnswerD.Text = dtQuestion.Rows[0]["answer4"].ToString();
            if (dtQuestion.Rows[0]["correct1"] != DBNull.Value)
            {
                cbCorrectA.IsChecked = Convert.ToInt32(dtQuestion.Rows[0]["correct1"]) != 0;
            }
            if (dtQuestion.Rows[0]["correct2"] != DBNull.Value)
            {
                cbCorrectB.IsChecked = Convert.ToInt32(dtQuestion.Rows[0]["correct2"]) != 0;
            }
            if (dtQuestion.Rows[0]["correct3"] != DBNull.Value)
            {
                cbCorrectC.IsChecked = Convert.ToInt32(dtQuestion.Rows[0]["correct3"]) != 0;
            }
            if (dtQuestion.Rows[0]["correct4"] != DBNull.Value)
            {
                cbCorrectD.IsChecked = Convert.ToInt32(dtQuestion.Rows[0]["correct4"]) != 0;
            }
            if (!String.IsNullOrEmpty(dtQuestion.Rows[0]["image"].ToString()))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(dirImages + dtQuestion.Rows[0]["image"].ToString());
                image.EndInit();
                imgUpload.Source = image;
            }
        }

        private void BtnDeleteImage_Click(object sender, RoutedEventArgs e)
        {
            uploadFileName = "";
            imgUpload.Source = null;
            if (!String.IsNullOrEmpty(question.image))
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                System.IO.File.Delete(dirImages + question.image);
                question.Update(questionID, question.question, "");
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            bool blCorrectA = txbAnswerA.Text != "" && cbCorrectA.IsChecked.GetValueOrDefault();
            bool blCorrectB = txbAnswerB.Text != "" && cbCorrectB.IsChecked.GetValueOrDefault();
            bool blCorrectC = txbAnswerC.Text != "" && cbCorrectC.IsChecked.GetValueOrDefault();
            bool blCorrectD = txbAnswerD.Text != "" && cbCorrectD.IsChecked.GetValueOrDefault();

            // check if at least one answer is correct
            if (blCorrectA || blCorrectB || blCorrectC || blCorrectD)
            {
                // save image
                if (!String.IsNullOrWhiteSpace(uploadFileName))
                {
                    // generate filename
                    Guid guid = Guid.NewGuid();
                    string[] fileNameArray = uploadFileName.Split('.');
                    Array.Reverse(fileNameArray);
                    imageName = guid.ToString() + "." + fileNameArray[0];
                    // upload file
                    System.IO.File.Copy(uploadFileName, dirImages + imageName);
                }

                if (questionID != -1)
                {
                    // delete old answers
                    question.DeleteAnswers(questionID);

                    // delete old image if image changed
                    question.Read(questionID);
                    if (!String.IsNullOrWhiteSpace(question.image))
                    {
                        if (!String.IsNullOrWhiteSpace(imageName))
                        {
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            System.IO.File.Delete(dirImages + question.image);
                        } else
                        {
                            imageName = question.image;
                        }
                    }

                    question.Update(questionID, txbQuestion.Text, imageName);
                }
                else
                {
                    // create question
                    questionID = question.Create(txbQuestion.Text, imageName, quizID);
                }

                // save answers
                if (txbAnswerA.Text != "")
                {
                    answer.Create(txbAnswerA.Text, 1, blCorrectA, questionID);
                }
                if (txbAnswerB.Text != "")
                {
                    answer.Create(txbAnswerB.Text, 2, blCorrectB, questionID);
                }
                if (txbAnswerC.Text != "")
                {
                    answer.Create(txbAnswerC.Text, 3, blCorrectC, questionID);
                }
                if (txbAnswerD.Text != "")
                {
                    answer.Create(txbAnswerD.Text, 4, blCorrectD, questionID);
                }

                EditQuiz window = new EditQuiz(quizID);
                window.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Minimaal 1 antwoord moet correct zijn!");
            }
        }

        public void BtnUploadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Selecteer een afbeelding.";
            openFileDialog.Filter = "Image Only(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";
            openFileDialog.FilterIndex = 1;
            try
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    if (openFileDialog.CheckFileExists)
                    {
                        uploadFileName = openFileDialog.FileName;
                        imgUpload.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                    }
                }
                else
                {
                    MessageBox.Show("Upload een Afbeelding AUB.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
