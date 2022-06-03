using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySqlConnector;

namespace Quiztime
{
    class SQL
    {
        MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

        public DataSet GetDataSet(string SQL)
        {
            DataSet dataSet = new DataSet();

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(SQL, conn);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                adp.Fill(dataSet, "LoadDataBinding");
                conn.Close();
            }
            catch (MySqlException ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }

            return dataSet;
        }

        public DataTable GetDataTable(string SQL)
        {
            DataTable dataTable = new DataTable();

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(SQL, conn);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                adp.Fill(dataTable);
                conn.Close();
            }
            catch (MySqlException ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }

            return dataTable;
        }

        public void ExecuteNonQuery(string SQL)
        {
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(SQL, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (MySqlException ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }
    }
}
