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
            UpdateStatus(string.Format("Creating Google Play Database"));
            Data db = new Data(dbPath);

            List<string> apps = db.GetApps();
            App app;

            foreach (var item in apps)
            {
                UpdateStatus("Processing: " + item);
                app = App.GetAppByID(item);
                if (app != null)
                    {
                        db.InsertApp(app);
                        UpdateStatus(string.Format("{0} is available on Google Play", item));
                    }
                else
                {
                    UpdateStatus(string.Format("{0} is not listed on Google Play", item));
                }
                Thread.Sleep(1500);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            dbPath = textBoxDB.Text;
            if (!File.Exists(dbPath))
            {
                MessageBox.Show("The path for the database file is not valid." + Environment.NewLine + "Please enter a valid file path.");
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

    public static class ListExtensions
    {
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}
