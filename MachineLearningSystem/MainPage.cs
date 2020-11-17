using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech;
using System.Speech.Synthesis;
using System.Data.SQLite;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;

namespace MachineLearningSystem
{
    public partial class MainPage : Form
    {
        SpeechSynthesizer ss = new SpeechSynthesizer();
        SQLiteConnection con = new SQLiteConnection("Data Source=MachineLearningDb.db; Version=3; New=False; Compress=True;");
        public MainPage()
        {
            InitializeComponent();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            lblDate.Text = DateTime.Now.ToLongDateString();
            lblTime.Text = DateTime.Now.ToShortTimeString();
            trkVolume.Minimum = 0;
            trkVolume.Maximum = 100;
            trkSpeed.Minimum = -10;
            trkSpeed.Maximum = 10;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            rchContent.Clear();
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            if (rchContent.Text == "")
            {
                MessageBox.Show("No Text to read Please Input Text!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    ss = new SpeechSynthesizer();
                    ss.Rate = trkSpeed.Value;
                    ss.Volume = trkVolume.Value;
                    ss.SpeakAsync(rchContent.Text);
                    btnRead.Enabled = false;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void lblTime_Click(object sender, EventArgs e)
        {

        }

        private void lblDate_Click(object sender, EventArgs e)
        {

        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (rchContent.Text == "")
            {
                MessageBox.Show("Text field is Empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                ss.Pause();
                btnPause.Enabled = false;
            }
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            
                ss.Resume();
                btnPause.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (rchContent.Text == "")
            {
                MessageBox.Show("Text field is Empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                btnRead.Enabled = true;
                ss.Dispose();
                
            }
            
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
            if(con.State != ConnectionState.Closed)
            {
                con.Close();
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (rchContent.Text == "")
            {
                MessageBox.Show("No Text to Save, Please input text", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {


                    string fname = string.Empty;
                    string fpath = string.Empty;
                    string saveFileName = string.Empty;

                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "(.Mp3)|*.Mp3";
                    sfd.ShowDialog();
                    fname = sfd.FileName + ".Mp3";
                    fpath = System.IO.Path.GetFullPath(sfd.FileName) + ".Mp3";
                    saveFileName = System.IO.Path.GetFileNameWithoutExtension(sfd.FileName);
                    SpeechSynthesizer ss = new SpeechSynthesizer();
                    ss.SetOutputToWaveFile(fname);
                    ss.Speak(rchContent.Text);
                    ss.SetOutputToDefaultAudioDevice();
                    
                    //Database Connection
                    OpenConnection();
                    SQLiteCommand sda = new SQLiteCommand("SELECT * From records Where File = @file;", con);
                    sda.Parameters.Add(new SQLiteParameter("@file", saveFileName));
                    int count = Convert.ToInt32(sda.ExecuteScalar());
                    if (count != 1)
                    {

                        SQLiteCommand ssd = new SQLiteCommand("INSERT INTO records (file, Path, Content) Values(@files,@path,@content)", con);
                        
                        ssd.Parameters.AddWithValue("@files", saveFileName);
                        ssd.Parameters.AddWithValue("@path", fpath);
                        ssd.Parameters.AddWithValue("@content", rchContent.Text.Trim());
                        ssd.ExecuteNonQuery();
                        MessageBox.Show("File has been saved successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        rchContent.Clear();
                        ss.Dispose();
                        
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    CloseConnection();
                }
            }
        }

        private void picHome_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lnkHome_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }

        private void picRecords_Click(object sender, EventArgs e)
        {
            Records home = new Records();
            home.Show();
        }

        private void lnkRecords_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Records home = new Records();
            home.Show();
        }

        private void picExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to Exit", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)==DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to Exit", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            string pth;
            OpenFileDialog fld = new OpenFileDialog();
            fld.Title = "Select File";
            fld.Filter = "(*.pdf)|*.pdf";
            fld.DefaultExt = "pdf";
            fld.Multiselect = false;
            if (fld.ShowDialog() == DialogResult.OK)
            {
                pth = fld.FileName.ToString();

                string strText = string.Empty;
                PdfReader reader = new PdfReader(pth);
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy sda = new LocationTextExtractionStrategy();
                    String s = PdfTextExtractor.GetTextFromPage(reader, page, sda);
                    s = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(s)));
                    strText += s;
                    rchContent.Text = strText;

                }
                reader.Close();

            }
        }
    }
}
