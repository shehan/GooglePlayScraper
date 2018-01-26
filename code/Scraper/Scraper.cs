using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;

namespace Scraper
{
    internal class Scraper
    {
        private static object locker = new object();

        public static App ScrapeSite(string appName)
        {
            try
            {
                App scrapedApp = new App();
                var html = new HtmlDocument();

                html.LoadHtml(new System.Net.WebClient().DownloadString(BuildURL(appName)));

                scrapedApp.Name = appName;            
                scrapedApp.Url = new Uri(BuildURL(appName));

                var appScore = html.DocumentNode.SelectNodes("//div[@class='score']");
                if (appScore != null)
                    scrapedApp.Score = Convert.ToDouble(appScore.FirstOrDefault().InnerText.Trim());

                var appVersion = html.DocumentNode.SelectNodes("//div[@itemprop='softwareVersion']");
                if (appVersion != null)
                    scrapedApp.CurrentVersion = (appVersion.FirstOrDefault().InnerText.Trim());

                var appGenre = html.DocumentNode.SelectNodes("//span[@itemprop='genre']");
                if (appGenre != null)
                    scrapedApp.Genre = appGenre.FirstOrDefault().InnerText.Trim();

                var appDescription = html.DocumentNode.SelectNodes("//div[@itemprop='description']");
                if(appDescription !=null)
                    scrapedApp.Description = appDescription.FirstOrDefault().InnerText.Trim();

                var appTitle = html.DocumentNode.SelectNodes("//div[@class='id-app-title']");
                if (appTitle != null)
                    scrapedApp.Title = appTitle.FirstOrDefault().InnerText.Trim();

                var appTotalReviews = html.DocumentNode.SelectNodes("//span[@class='reviews-num']");
                if (appTotalReviews != null)
                    scrapedApp.TotalReviews = Convert.ToDouble(appTotalReviews.FirstOrDefault().InnerText.Trim());

                var appDeveloper = html.DocumentNode.SelectNodes("//span[@itemprop='name']");
                if (appDeveloper != null)
                    scrapedApp.Developer = appDeveloper.FirstOrDefault().InnerText.Trim();

                var appRating = html.DocumentNode.SelectNodes("//div[@itemprop='contentRating']");
                if (appRating != null)
                    scrapedApp.Rating = appRating.FirstOrDefault().InnerText.Trim();

                var appMinVersion = html.DocumentNode.SelectNodes("//div[@itemprop='operatingSystems']");
                if (appMinVersion != null)
                    scrapedApp.MinVersion = appMinVersion.FirstOrDefault().InnerText.Trim();

                var appInstalls = html.DocumentNode.SelectNodes("//div[@itemprop='numDownloads']");
                if (appInstalls != null)
                    scrapedApp.Installs = appInstalls.FirstOrDefault().InnerText.Trim();

                var appUpdated = html.DocumentNode.SelectNodes("//div[@itemprop='datePublished']");
                if (appUpdated != null)
                    scrapedApp.Updated = DateTime.Parse(appUpdated.FirstOrDefault().InnerText.Trim());

                var appIsTopDeveloper = html.DocumentNode.SelectNodes("//meta[@itemprop='topDeveloperBadgeUrl']");
                if (appIsTopDeveloper != null)
                    scrapedApp.IsTopDeveloper = true;

                var appIsEditorPick= html.DocumentNode.SelectNodes("//meta[@itemprop='editorsChoiceBadgeUrl']");
                if (appIsEditorPick != null)
                    scrapedApp.IsEditorPick = true;

                return scrapedApp;

            }
            catch (Exception error)
            {
                // string c = error.Message;
                // throw error;
                lock (locker)
                {
                    if (error.Message.Contains("404") || error.Message.Contains("403"))
                    {
                        using (StreamWriter w = File.AppendText("error.txt"))
                        {
                            w.WriteLine(string.Format("{0};{1}", appName, error.Message));
                        }
                        return null;
                    }
                    else
                    {
                        using (StreamWriter w = File.AppendText("error.txt"))
                        {
                            w.WriteLine(string.Format("UNKNOWN FAULT! - {0};{1}", appName, error.Message));
                        }
                        return null;
                    }
                }
            }
        } 

        private static string BuildURL(string appID)
        {
            string templateURL = "https://play.google.com/store/apps/details?id={0}";
            
            return string.Format(templateURL,appID);
        }

    }
}
