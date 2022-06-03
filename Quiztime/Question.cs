using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiztime
{
    internal class Question
    {
        private int _ID;
        private string? _question;
        private string? _image;
        private int _quizID;

        public Question()
        {
        }

        public int ID { 
            get { return _ID; } 
        }
        public string question
        {
            get { 
                if (_question != null)
                {
                    return _question;
                }
                else
                {
                    return string.Empty;
                }
            }
            set { _question = value; }
        }
        public string image
        {
            get { 
                if (_image != null)
                {
                    return _image;
                }
                else
                {
                    return string.Empty;
                }
            }
            set { _image = value; }
        }
        public int quizID
        {
            get { return _quizID; }
            set { _quizID = value; }
        }

        SQL sql = new SQL();

        public DataTable getDataQuizID(int quizID)
        {
            string SQL = string.Format("SELECT ID, question, image FROM question WHERE quizID = {0}", quizID);
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("question");
            dt.Columns.Add("image");
            dt = sql.GetDataTable(SQL);
            for (int i = 1; i < 5; i++)
            {
                dt.Columns.Add("ID" + i);
                dt.Columns.Add("answer" + i);
                dt.Columns.Add("position" + i);
                dt.Columns.Add("correct" + i);
            }

            // image full path
            string dirImages = System.IO.Directory.GetCurrentDirectory() + "../../../../images/";
            for (int x = 0; x < dt.Rows.Count; x++)
            {
                if (!String.IsNullOrEmpty(dt.Rows[x]["image"].ToString()))
                {
                    dt.Rows[x]["image"] = dirImages + dt.Rows[x]["image"].ToString();
                }
            }

            // get answer
            string SQL_ = string.Format("SELECT ID, answer, position, correct, questionID FROM answer WHERE " +
                "questionID IN (SELECT ID FROM question WHERE quizID = {0}) ORDER BY position ASC", quizID);
            DataTable dtAnswer = sql.GetDataTable(SQL_);

            // add answer to datatable
            for (int i = 0; i < dtAnswer.Rows.Count; i++)
            {
                for (int x = 0; x < dt.Rows.Count; x++)
                {
                    if (dt.Rows[x]["ID"].ToString() == dtAnswer.Rows[i]["questionID"].ToString())
                    {
                        dt.Rows[x]["ID" + dtAnswer.Rows[i]["position"]] = dtAnswer.Rows[i]["ID"];
                        dt.Rows[x]["answer" + dtAnswer.Rows[i]["position"]] = dtAnswer.Rows[i]["answer"];
                        dt.Rows[x]["position" + dtAnswer.Rows[i]["position"]] = dtAnswer.Rows[i]["position"];
                        dt.Rows[x]["correct" + dtAnswer.Rows[i]["position"]] = dtAnswer.Rows[i]["correct"];
                        break;
                    }
                }
            }

            return dt;
        }

        public DataTable getFullQuestion(int questionID)
        {
            string SQL = string.Format("SELECT ID, question, image FROM question WHERE ID = {0}", questionID);
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("question");
            dt.Columns.Add("image");
            dt = sql.GetDataTable(SQL);
            for (int i = 1; i < 5; i++)
            {
                dt.Columns.Add("ID" + i);
                dt.Columns.Add("answer" + i);
                dt.Columns.Add("position" + i);
                dt.Columns.Add("correct" + i);
            }

            // get answer
            SQL = string.Format("SELECT ID, answer, position, correct, questionID FROM answer WHERE " +
                "questionID = {0} ORDER BY position ASC", questionID);
            DataTable dtAnswer = sql.GetDataTable(SQL);

            // add answer to datatable
            for (int i = 0; i < dtAnswer.Rows.Count; i++)
            {
                for (int x = 0; x < dt.Rows.Count; x++)
                {
                    if (dt.Rows[x]["ID"].ToString() == dtAnswer.Rows[i]["questionID"].ToString())
                    {
                        dt.Rows[x]["ID" + dtAnswer.Rows[i]["position"]] = dtAnswer.Rows[i]["ID"];
                        dt.Rows[x]["answer" + dtAnswer.Rows[i]["position"]] = dtAnswer.Rows[i]["answer"];
                        dt.Rows[x]["position" + dtAnswer.Rows[i]["position"]] = dtAnswer.Rows[i]["position"];
                        dt.Rows[x]["correct" + dtAnswer.Rows[i]["position"]] = dtAnswer.Rows[i]["correct"];
                        break;
                    }
                }
            }

            return dt;
        }

        public void DeleteQuestion(int questionID)
        {
            if (System.Windows.MessageBox.Show("Weet je zeker dat je deze vraag wil verwijderen?",
                    "Question", System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning) == System.Windows.MessageBoxResult.Yes)
            {
                // delete answers
                string SQL = string.Format("DELETE FROM answer WHERE questionID = {0}", questionID);
                sql.ExecuteNonQuery(SQL);
                // delete image file
                string dirImages = System.IO.Directory.GetCurrentDirectory() + "../../../../images/";
                Question question_ = new Question();
                question_.Read(questionID);
                if (!String.IsNullOrWhiteSpace(question_.image))
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    System.IO.File.Delete(dirImages + question_.image);
                }
                // delete questions
                SQL = string.Format("DELETE FROM question WHERE id = {0}", questionID);
                sql.ExecuteNonQuery(SQL);
            }
        }

        public void DeleteAnswers(int questionID)
        {
            string SQL = string.Format("DELETE FROM answer WHERE questionID = {0}", questionID);
            sql.ExecuteNonQuery(SQL);
        }

        public int Create(string question, string image, int quizID)
        {
            string SQL = string.Format("INSERT INTO question (question, image, quizID) VALUES ('{0}', '{1}', {2}); SELECT LAST_INSERT_ID()", 
                question, image, quizID);

            DataTable test = sql.GetDataTable(SQL);

            return Convert.ToInt32(test.Rows[0]["LAST_INSERT_ID()"]);
        }

        public void Read(int id)
        {
            string SQL = string.Format("SELECT ID, question, image, quizID FROM question WHERE ID = {0}", id);
            DataTable dataTable = sql.GetDataTable(SQL);
            _ID = Convert.ToInt32(dataTable.Rows[0]["ID"].ToString());
            _question = dataTable.Rows[0]["question"].ToString();
            _image = dataTable.Rows[0]["image"].ToString();
            _quizID = Convert.ToInt32(dataTable.Rows[0]["quizID"].ToString());
        }

        public void Update(int id, string question, string image)
        {
            string SQL = string.Format("UPDATE question SET question = '{0}', image = '{1}' WHERE ID = {2}", question, image, id);
            sql.ExecuteNonQuery(SQL);
        }

        public bool Delete(int id)
        {
            bool isDeleted = false;
            if (System.Windows.MessageBox.Show("Moet ik deze gegevens verwijderen?", "Question", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning) == System.Windows.MessageBoxResult.Yes)
            {
                string SQL = string.Format("DELETE FROM question WHERE id = {0}", id);
                sql.ExecuteNonQuery(SQL);
                isDeleted = true;
            }
            return isDeleted;
        }
    }
}
