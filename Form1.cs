using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;

namespace WindowsFormsApp_ProgressBar
{
    public partial class Form1 : Form
    {
        
        private StringBuilder resultText;
        OpenFileDialog openFileDialog1 = new OpenFileDialog();
        
        public Form1()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
            openFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            resultText = new StringBuilder();

        }
        
        private void InitializeBackgroundWorker()
        {
            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker1.DoWork += BackgroundWorker_DoWork;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string filename = e.Argument.ToString();
            Encoding encoding = Encoding.GetEncoding("UTF-8");

            try
            {
                using (StreamReader reader = new StreamReader(filename, encoding))
                {
                    long fileSize = new FileInfo(filename).Length;
                    long bytesRead = 0;

                    char[] buffer = new char[8192];
                    int charsRead;
                    

                    while ((charsRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        bytesRead += charsRead;

                        // Додаємо текст до результату
                        resultText.Append(new string(buffer, 0, charsRead));
                        
                        // Оновлюємо прогрес
                        int progressPercentage = (int)((double)bytesRead / fileSize * 100);
                        backgroundWorker1.ReportProgress(progressPercentage);
                    }
                    // Передаємо усі абзаци тексту як результат
                    
                    e.Result = resultText.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка читання файлу: " + ex.Message);
            }
        }
        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            //textBox1.Text = resultText.ToString();
            
        }
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {

                textBox1.Text = resultText.ToString();
                progressBar1.Value = 100;
                MessageBox.Show("Файл завантажено!");
                label1.Visible = false;
                progressBar1.Visible=false;
                progressBar1.Style = ProgressBarStyle.Blocks;
                progressBar1.ForeColor = Color.Red;
            }
            else
            {
                MessageBox.Show("Помилка: " + e.Error.Message);
            }
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            progressBar1.Visible = false;
            //відкрити з файла
            textBox1.Text = "";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            //Encoding encoding = Encoding.GetEncoding("UTF-8");
            // читаем файл в строку
            progressBar1.Visible = true;
            label1.Visible = true;
            // Очищуємо StringBuilder перед кожним відкриттям файлу
            resultText.Clear();
            // Запускаємо BackgroundWorker для читання файлу
            backgroundWorker1.RunWorkerAsync(filename);

        }
    }
}
