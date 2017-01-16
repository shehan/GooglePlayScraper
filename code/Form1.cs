using Scraper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GooglePlayScraper
{
    public partial class Form1 : Form
    {
        string dbPath;
        public Form1()
        {
            InitializeComponent();
        }

        private void StartProcessing()
        {
            Data db = new Data(dbPath);
            Dictionary<string, string> dict = db.GetApps();
            dict.Count();
            
            decimal numberOfGroups = 3;
            int counter = 0;
            int groupSize = Convert.ToInt32(Math.Ceiling(dict.Count / numberOfGroups));

            var result = dict.GroupBy(x => counter++ / groupSize);

            int threadCount = result.Count() + 1;

            List<App> appList = new List<App>();
            App app;

            Parallel.For(0, threadCount, i =>
            {
                var dataSet = result.ElementAt(i);
                Console.WriteLine("Processing dataset: "+i +"; Count: "+dataSet.Count());
                foreach (var item in dataSet)
                {
                    UpdateStatus("Processing: " + item.Value);
                    app = App.GetAppByID(item.Value);
                    if (app != null)
                        lock (appList)
                        {
                            db.InsertApp(app, item.Key);
                            appList.Add(app);
                            UpdateStatus(string.Format("{0} is available on Google Play", item.Value));
                        }
                    else
                    {
                        UpdateStatus(string.Format("{0} is not listed on Google Play", item.Value));
                    }
                    Thread.Sleep(1500);
                }
                UpdateStatus(string.Format("Thread {0} is done", i));
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dbPath = textBoxDB.Text;
            if (!File.Exists(dbPath))
            {
                MessageBox.Show("The path for the database file is not valid."+Environment.NewLine+"Please enter a valid file path.");
                return;
            }

            button1.Enabled = false;
            textBoxDB.Enabled = false;

            Thread startProcess = new Thread(StartProcessing);
            startProcess.Start();
        }

        private void UpdateStatus(string text)
        {
            if (this.button1.InvokeRequired)
            {
                UpdateStatusCallback callback = new UpdateStatusCallback(UpdateStatus);
                this.Invoke(callback, new object[] { text });
            }
            else
            {
                textBoxLog.Text = textBoxLog.Text + Environment.NewLine + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " " + text;
            }
        }

        delegate void UpdateStatusCallback(string text);
    }
}
