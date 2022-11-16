using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiztime
{
    internal class Quiz
    {
        private int _ID;
        private string? _title;

        public Quiz()
        {
        }

        public int ID { get { return _ID; } }
        public string title
        {
            get
            {
                if (_title != null)
                {
                    return _title;
                } else
                {
                    return string.Empty;
                }
            }

            set { _title = value; }
        }

        SQL sql = new SQL();

        public DataSet getData()
        {
            string SQL = "SELECT ID, title FROM quiz";

            return sql.GetDataSet(SQL);
        }

        public void DeleteQuiz(int quizID)
        {
            if (System.Windows.MessageBox.Show("Weet je zeker dat je deze quiz wil verwijderen?", 
                "Question", System.Windows.MessageBoxButton.YesNo, 
                System.Windows.MessageBoxImage.Warning) == System.Windows.MessageBoxResult.Yes)
            {
                // get questionID
                string SQL = string.Format("SELECT ID FROM question WHERE quizID = {0}", quizID);
                DataTable questionIDs = sql.GetDataTable(SQL);
                for (int i = 0; i < questionIDs.Rows.Count; i++)
                {
                    int questionID = Convert.ToInt32(questionIDs.Rows[i]["ID"]);

                    // delete answers
                    SQL = string.Format("DELETE FROM answer WHERE questionID = {0}", questionID);
                    sql.ExecuteNonQuery(SQL);
                    // delete image
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
                // delete quiz
                SQL = string.Format("DELETE FROM quiz WHERE id = {0}", quizID);
                sql.ExecuteNonQuery(SQL);
            }
        }

        public int Create(string title)
        {
            string SQL = string.Format("INSERT INTO quiz (title) VALUES ('{0}'); SELECT LAST_INSERT_ID()", title);

            DataTable test = sql.GetDataTable(SQL);

            return Convert.ToInt32(test.Rows[0]["LAST_INSERT_ID()"]);
        }

        public void Read(int id)
        {
            string SQL = string.Format("SELECT ID, title FROM quiz WHERE ID = {0}", id);
            DataTable dataTable = sql.GetDataTable(SQL);
            _ID = Convert.ToInt32(dataTable.Rows[0]["ID"].ToString());
            _title = dataTable.Rows[0]["title"].ToString();
        }

        public void Update(int id, string title)
        {
            string SQL = string.Format("UPDATE quiz SET title = '{0}' WHERE ID = {1}", title, id);
            sql.ExecuteNonQuery(SQL);
        }

        public bool Delete(int id)
        {
            bool isDeleted = false;
            if (System.Windows.MessageBox.Show("Moet ik deze gegevens verwijderen?", "Question", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning) == System.Windows.MessageBoxResult.Yes)
            {
                string SQL = string.Format("DELETE FROM quiz WHERE id = {0}", id);
                sql.ExecuteNonQuery(SQL);
                isDeleted = true;
            }
            return isDeleted;
        }
    }
}
