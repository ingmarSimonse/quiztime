using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiztime
{
    internal class Answer
    {
        private int _ID;
        private string? _answer;
        private int _position;
        private int _correct;
        private int _questionID;

        public Answer()
        {
        }

        public int ID
        {
            get { return _ID; }
        }
        public string? answer
        {
            get { 
                if (_answer != null)
                {
                    return _answer;
                }
                else
                {
                    return string.Empty;
                }
            }
            set { _answer = value; }
        }
        public int position
        {
            get { return _position; }
            set { _position = value; }
        }
        public bool correct
        {
            get { return (_correct == 0); }
            set { _correct = value ? 1 : 0; }
        }
        public int questionID
        {
            get { return _questionID; }
            set { _questionID = value; }
        }

        SQL sql = new SQL();

        public void Create(string answer, int position, bool correct, int questionID)
        {
            string SQL = string.Format("INSERT INTO answer (answer, position, correct, questionID) VALUES ('{0}', {1}, {2}, {3})",
                answer, position, correct, questionID);

            sql.ExecuteNonQuery(SQL);
        }

        public void Read(int id)
        {
            string SQL = string.Format("SELECT ID, answer, position, correct, questionID FROM answer WHERE ID = {0}", id);
            DataTable dataTable = sql.GetDataTable(SQL);
            _ID = Convert.ToInt32(dataTable.Rows[0]["ID"].ToString());
            _answer = dataTable.Rows[0]["answer"].ToString();
            _position = Convert.ToInt32(dataTable.Rows[0]["position"].ToString());
            _correct = Convert.ToInt32(dataTable.Rows[0]["correct"].ToString());
            _questionID = Convert.ToInt32(dataTable.Rows[0]["questionID"].ToString());
        }

        public void Update(int id, string answer, int position, bool correct)
        {
            string SQL = string.Format("UPDATE answer SET answer = '{0}', position = {1}, correct = {2} WHERE ID = {3}", answer, position, correct, id);
            sql.ExecuteNonQuery(SQL);
        }

        public bool Delete(int id)
        {
            bool isDeleted = false;
            if (System.Windows.MessageBox.Show("Moet ik deze gegevens verwijderen?", "Question", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning) == System.Windows.MessageBoxResult.Yes)
            {
                string SQL = string.Format("DELETE FROM answer WHERE id = {0}", id);
                sql.ExecuteNonQuery(SQL);
                isDeleted = true;
            }
            return isDeleted;
        }
    }
}
