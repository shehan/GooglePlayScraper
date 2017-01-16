using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper
{
    public class App
    {
        string appId, title, description, genre, developer, rating, minVersion, installs;
        string currentVersion;
        Uri url;
        double totalReviews, score;
        DateTime updated;
        bool isTopDeveloper, isEditorPick;

        public App()
        {
            this.appId = string.Empty;
            this.title = string.Empty;
            this.description = string.Empty;
            this.genre = string.Empty;
            this.developer = string.Empty;
            this.rating = string.Empty;
            this.minVersion = string.Empty;
            this.installs = string.Empty;
            this.currentVersion = string.Empty;
            this.url = null;
            this.totalReviews = 0;
            this.score = 0;
            this.updated = new DateTime();
            this.isTopDeveloper = false;
            this.isEditorPick = false;
        }

        public string AppId { get => appId; set => appId = value; }
        public string Title { get => title; set => title = value; }
        public string Description { get => description; set => description = value; }
        public string Genre { get => genre; set => genre = value; }
        public Uri Url { get => url; set => url = value; }
        public double TotalReviews { get => totalReviews; set => totalReviews = value; }
        public double Score { get => score; set => score = value; }
        public string CurrentVersion { get => currentVersion; set => currentVersion = value; }
        public DateTime Updated { get => updated; set => updated = value; }
        public bool IsTopDeveloper { get => isTopDeveloper; set => isTopDeveloper = value; }
        public bool IsEditorPick { get => isEditorPick; set => isEditorPick = value; }
        public string Developer { get => developer; set => developer = value; }
        public string Rating { get => rating; set => rating = value; }
        public string MinVersion { get => minVersion; set => minVersion = value; }
        public string Installs { get => installs; set => installs = value; }

        public static App GetAppByID(string ID)
        {
            App app = null;// new App();


            app = Scraper.ScrapeSite(ID);

            return app;
        }
    }
}
