using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scraper;
using System.Data.SQLite;
using Microsoft.VisualBasic.FileIO;

namespace GooglePlayScraper
{
    class Data
    {
        //public static string CONNECTION_STRING = "Data Source={0};Version=3;";
        //public static string SQL_GET_APP_NAMES = "SELECT " +
        //    "APP_CLONE.ID, APP_CLONE.APPID, APP.NAME, APP.FRIENDLY_NAME " +
        //    "FROM APP_CLONE " +
        //    "INNER JOIN APP ON( APP.ID = APP_CLONE.APPID)";

        public static string INSERT_APP = "INSERT INTO GOOGLE_PLAY (" +
            "APP, TITLE, DESCRIPTION, GENRE, DEVELOPER, RATING, MIN_VERSION, INSTALLS, CURRENT_VERSION, URL, TOTAL_REVIEWS, SCORE, UPDATED_TEXT, UPDATED_TICKS, IS_TOP_DEVELOPER, IS_EDITOR_PICK " +
            ") VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', {10}, {11}, '{12}', {13}, {14}, {15});";

        public static string CREATE_TABLE = "CREATE TABLE GOOGLE_PLAY (" +
    "APP TEXT, " +    
    "TITLE TEXT, " +
    "DESCRIPTION TEXT, " +
    "GENRE TEXT, " +
    "DEVELOPER TEXT, " +
    "RATING TEXT, " +
    "MIN_VERSION TEXT, " +
    "INSTALLS TEXT, " +
    "CURRENT_VERSION TEXT, " +
    "URL TEXT, " +
    "TOTAL_REVIEWS REAL, " +
    "SCORE REAL, " +
    "UPDATED_TEXT TEXT, " +
    "UPDATED_TICKS REAL, " +
    "IS_TOP_DEVELOPER INTEGER DEFAULT 0, " +
    "IS_EDITOR_PICK INTEGER DEFAULT 0 " +
");";




        private string sourceFilePath;
        private string filePath, connectionString;

        public Data(string sourceFilePath)
        {
            this.sourceFilePath = sourceFilePath;

            filePath = $"GooglePlayDB_{DateTime.Now.Ticks}.sqlite";
            connectionString = $"Data Source={filePath};Version=3;";
            CreateDB();
            CreateTable();
        }


        private void CreateDB()
        {
            SQLiteConnection.CreateFile(filePath);
        }

        private void CreateTable()
        {
            using (SQLiteConnection dbConnection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                {
                    dbConnection.Open();
                    using (var transaction = dbConnection.BeginTransaction())
                    {

                        command.CommandText = CREATE_TABLE;
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }

                    dbConnection.Close();
                }
            }
        }

        public List<string> GetApps()
        {
            List<string> apps = new List<string>();

            string[] fields;

            using (TextFieldParser parser = new TextFieldParser(sourceFilePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;
                while (!parser.EndOfData)
                {
                    fields = parser.ReadFields();
                    apps.Add(fields[0].Trim());
                }
            }

            return apps;
        }

        //public Dictionary<string, string> GetApps()
        //{
        //    Dictionary<string, string> apps = new Dictionary<string, string>();

        //    using (SQLiteConnection dbConnection = new SQLiteConnection(string.Format(CONNECTION_STRING, sourceFilePath)))
        //    {
        //        using (SQLiteCommand command = new SQLiteCommand(dbConnection))
        //        {
        //            dbConnection.Open();
        //            using (var transaction = dbConnection.BeginTransaction())
        //            {

        //                command.CommandText = SQL_GET_APP_NAMES;
        //                command.CommandType = System.Data.CommandType.Text;
        //                SQLiteDataReader reader = command.ExecuteReader();
        //                while (reader.Read())
        //                {
        //                    var appID = reader["APPID"].ToString();
        //                    var appName = reader["NAME"].ToString();
        //                    apps.Add(appID, appName);                            
        //                }
        //            }

        //            dbConnection.Close();
        //        }
        //    }


        //    return apps;
        //}

        public void InsertApp(App app)
        {
            if (app == null)
                return;

            using (SQLiteConnection dbConnection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                {
                    dbConnection.Open();
                    using (var transaction = dbConnection.BeginTransaction())
                    {

                        command.CommandText = string.Format(INSERT_APP,
                            app.Name,
                            app.Title.Replace("'", "''"),
                            string.Format("\"{0}\"", app.Description.Replace("'", "''")),
                            app.Genre.Replace("'", "''"),
                            app.Developer.Replace("'", "''"),
                            app.Rating.Replace("'", "''"),
                            app.MinVersion.Replace("'", "''"),
                            app.Installs.Replace("'", "''"),
                            app.CurrentVersion.Replace("'", "''"),
                            app.Url.ToString(),
                            app.TotalReviews,
                            app.Score,
                            app.Updated.ToString(),
                            app.Updated.Ticks,
                            app.IsTopDeveloper ? 1 : 0,
                            app.IsEditorPick ? 1 : 0);

                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }

                    dbConnection.Close();
                }
            }
        }
    }
}
