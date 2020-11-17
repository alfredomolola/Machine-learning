using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace MachineLearningSystem
{
    public partial class Records : Form
    {
        SQLiteConnection con = new SQLiteConnection("Data Source=MachineLearningDb.db; Version=3; New=False; Compress=True;");
        public Records()
        {
            InitializeComponent();
        }

        private void Records_Load(object sender, EventArgs e)
        {
            lblDate.Text = DateTime.Now.ToLongDateString();
            lblTime.Text = DateTime.Now.ToShortDateString();
            fillDatagridView();
        }

        public void OpenConnection()
        {
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
        }

        public void CloseConnection()
        {
            if (con.State != ConnectionState.Closed)
            {
                con.Close();
            }
        }

        public void fillDatagridView()
        {
            try
            {
                OpenConnection();
                SQLiteDataAdapter sda = new SQLiteDataAdapter("SELECT * From records", con);
                DataTable data = new DataTable();
                sda.Fill(data);
                dataGridView1.DataSource = data;
                sda.SelectCommand.ExecuteReader();
                CloseConnection();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}
