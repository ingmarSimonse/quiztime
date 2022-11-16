using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Quiztime
{
    /// <summary>
    /// Interaction logic for Play.xaml
    /// </summary>
    public partial class Play : Window
    {
        Question question = new Question();
        DataTable dt = new DataTable();
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        MediaPlayer mediaPlayer = new MediaPlayer();
        string dirSounds = System.IO.Directory.GetCurrentDirectory() + "../../../../sounds/";
        int quizID;
        int index = -1;
        int time = 20;
        bool gameEnd = false;
        bool playMode;
        bool isQuestion = true;
        bool isPrevious = false;

        public Play(int quizID, bool playMode)
        {
            InitializeComponent();

            dt = question.getDataQuizID(quizID);
            this.quizID = quizID;
            this.playMode = playMode;

            PopulateQuestion();

            dispatcherTimer.Tick += new EventHandler(SetTimer);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        public Play()
        {

        }

        public void StopTimer()
        {
            dispatcherTimer.Stop();
        }

        public void StartTimer()
        {
            dispatcherTimer.Start();
        }

        public void NextQuestion()
        {
            index++;
            if (!isQuestion)
            {
                index--;
            }
            time = 20;
            lblTimer.Content = time.ToString();
            PopulateQuestion();
        }

        public void PreviousQuestion()
        {
            if (gameEnd)
            {
                StartTimer();
                gameEnd = false;
            }
            isPrevious = true;
            index--;
            PopulateQuestion();
            time = 20;
            lblTimer.Content = time.ToString();
        }

        private void SetTimer(object sender, EventArgs e)
        {
            time--;
            lblTimer.Content = time.ToString();
            if (time == 0)
            {
                NextQuestion();
            }
        }

        private void PopulateQuestion()
        {
            if (index < dt.Rows.Count && index >= 0)
            {
                if (playMode || isQuestion || isPrevious)
                {
                    // play sound
                    mediaPlayer.Open(new Uri(dirSounds + "question.mp3"));
                    mediaPlayer.Play();
                    // set image
                    if (!String.IsNullOrEmpty(dt.Rows[index]["image"].ToString()))
                    {
                        imgQuestion.Source = new BitmapImage(new Uri("" + dt.Rows[index]["image"].ToString()));
                    }
                    else
                    {
                        imgQuestion.Source = null;
                    }
                    // set question
                    lblQuestion.Content = dt.Rows[index]["question"].ToString();
                    // set answers
                    if (!String.IsNullOrWhiteSpace(dt.Rows[index]["answer1"].ToString()))
                    {
                        lblAnswer1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFDFA"));
                        lblAnswer1.Content = "A. " + dt.Rows[index]["answer1"].ToString();
                    } else
                    {
                        lblAnswer1.Content = "";
                    }
                    if (!String.IsNullOrWhiteSpace(dt.Rows[index]["answer2"].ToString()))
                    {
                        lblAnswer2.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFDFA"));
                        lblAnswer2.Content = "B. " + dt.Rows[index]["answer2"].ToString();
                    } else
                    {
                        lblAnswer2.Content = "";
                    }
                    if (!String.IsNullOrWhiteSpace(dt.Rows[index]["answer3"].ToString()))
                    {
                        lblAnswer3.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFDFA"));
                        lblAnswer3.Content = "C. " + dt.Rows[index]["answer3"].ToString();
                    } else
                    {
                        lblAnswer3.Content = "";
                    }
                    if (!String.IsNullOrWhiteSpace(dt.Rows[index]["answer4"].ToString()))
                    {
                        lblAnswer4.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFDFA"));
                        lblAnswer4.Content = "D. " + dt.Rows[index]["answer4"].ToString();
                    } else
                    {
                        lblAnswer4.Content = "";
                    }
                    // set number
                    lblNumber.Content = (index + 1) + "/" + dt.Rows.Count;

                    if (!playMode)
                    {
                        isQuestion = false;
                        isPrevious = false;
                    }
                } else
                {
                    // play sound
                    mediaPlayer.Open(new Uri(dirSounds + "answer.mp3"));
                    mediaPlayer.Play();
                    // show answer
                    if (dt.Rows[index]["correct1"] != null)
                    {
                        if (dt.Rows[index]["correct1"].ToString() == "1")
                        {
                            lblAnswer1.Foreground = Brushes.Green;
                        } else
                        {
                            lblAnswer1.Foreground = Brushes.Red;
                        }
                    }
                    if (dt.Rows[index]["correct2"] != null)
                    {
                        if (dt.Rows[index]["correct2"].ToString() == "1")
                        {
                            lblAnswer2.Foreground = Brushes.Green;
                        }
                        else
                        {
                            lblAnswer2.Foreground = Brushes.Red;
                        }
                    }
                    if (dt.Rows[index]["correct3"] != null)
                    {
                        if (dt.Rows[index]["correct3"].ToString() == "1")
                        {
                            lblAnswer3.Foreground = Brushes.Green;
                        }
                        else
                        {
                            lblAnswer3.Foreground = Brushes.Red;
                        }
                    }
                    if (dt.Rows[index]["correct4"] != null)
                    {
                        if (dt.Rows[index]["correct4"].ToString() == "1")
                        {
                            lblAnswer4.Foreground = Brushes.Green;
                        }
                        else
                        {
                            lblAnswer4.Foreground = Brushes.Red;
                        }
                    }


                    isQuestion = true;
                }
            } else
            {
                lblTimer.Content = "";
                imgQuestion.Source = null;
                lblAnswer1.Content = "";
                lblAnswer2.Content = "";
                lblAnswer3.Content = "";
                lblAnswer4.Content = "";
                lblNumber.Content = "";
                if (index <= 0)
                {
                    // start of the game
                    isQuestion = true;
                    Quiz quiz = new Quiz();
                    quiz.Read(quizID);
                    lblQuestion.Content = quiz.title;
                }
                else
                {
                    // end of the game
                    mediaPlayer.Open(new Uri(dirSounds + "end.mp3"));
                    mediaPlayer.Play();
                    StopTimer();
                    gameEnd = true;
                    lblQuestion.Content = "EINDE";
                }
            }
        }
    }
}
