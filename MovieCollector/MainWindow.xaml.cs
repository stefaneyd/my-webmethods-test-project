using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Web;
using System.Reflection;

namespace MovieCollector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables
        enum MovieProperty { Title = 1, Year = 2, Rated = 3, Genre = 4, Director = 5, Writer = 6, Actors = 7, Plot = 8, Poster = 9, Runtime = 10, Rating = 11, Votes = 12, ID = 13, DirectoryPath = 14 };
        string[] movieProperties = new string[]{ "Title", "Year", "Rated", "Genre", "Director", "Writer", "Actors", "Plot", "Poster", "Runtime", "Rating", "Votes", "ID", "DirectoryPath" };
        string selectedPath = "";
        List<Movie> resultArray = new List<Movie>();
        List<Movie> netList = new List<Movie>();
        int index = 0;
        public enum MovieRips : int
        {
            DVDRIP = 1,
            BRRIP = 2,
            CAM = 3,
            TS = 4
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseForFiles_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            DialogResult result = dlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                selectedPath = dlg.SelectedPath;
                textBox1.Text = selectedPath;

                GetFilesAndFolders();
            }
        }

        private void GetFilesAndFolders()
        {
            resultArray.Clear();
            string[] files = System.IO.Directory.GetFiles(selectedPath);
            string[] folders = System.IO.Directory.GetDirectories(selectedPath);

            foreach (string str in folders)
            {
                index++;
                string title = str.Substring(str.LastIndexOf('\\') + 1);
                if (!(title.Contains("-=")))
                    resultArray.Add(new Movie { Nr = index, DirectoryPath = str, Title = title });
            }

            foreach (string str in files)
            {
                index++;
                string title = str.Substring(str.LastIndexOf('\\') + 1);
                resultArray.Add(new Movie { Nr = index, DirectoryPath = selectedPath, Title = title });
            }

            dataGrid1.DataContext = null;
            dataGrid1.DataContext = resultArray;
            index = 0;
        }

        private void WebInfo_Click(object sender, RoutedEventArgs e)
        {
            string searchTitel = "http://www.imdbapi.com/?t=";

            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
            //wc.DownloadFile(test, "C:\\wtf\\temp.txt");
            foreach (Movie m in resultArray)
            {
                
                string title = HttpUtility.UrlEncode(TokanizeFunction(m.Title).Trim());
                
                Uri uri = new Uri(searchTitel + title);
                
                wc.DownloadStringAsync(uri);

                //string text = wc.DownloadString(searchTitel + title);
                //string[] arr = text.Replace("{", string.Empty).Replace("}", string.Empty).Replace("\"", string.Empty).Split(',');

                //Movie movie = new Movie();
                //string currentProperty = "";
                //movie.index = FindRipMethod(m.Title);

                //foreach (string item in arr)
                //{
                //    if (item.Contains(':'))
                //    {
                //        string[] tmp = item.Split(':');
                //        currentProperty = tmp[0].Trim();

                //        switch (currentProperty)
                //        {
                //            case "Title":
                //                movie.Title = tmp[1].ToString();
                //                if (tmp.Count() > 2)
                //                {
                //                    movie.Title += ":";
                //                    for (int i = 2; i < tmp.Count(); i++)
                //                        movie.Title += tmp[i].ToString();
                //                }
                //                break;
                //            case "Year":
                //                movie.Year = tmp[1].ToString();
                //                break;
                //            case "Rated":
                //                movie.Rated = tmp[1].ToString();
                //                break;
                //            case "Released":
                //                break;
                //            case "Genre":
                //                movie.Genre.Add(tmp[1].ToString());
                //                break;
                //            case "Director":
                //                movie.Director = tmp[1].ToString();
                //                break;
                //            case "Writer":
                //                movie.Writer.Add(tmp[1].ToString());
                //                break;
                //            case "Actors":
                //                movie.Actors.Add(tmp[1].ToString());
                //                break;
                //            case "Plot":
                //                movie.Plot = tmp[1].ToString();
                //                if (tmp.Count() > 2)
                //                {
                //                    movie.Plot += ":";
                //                    for (int i = 2; i < tmp.Count(); i++)
                //                        movie.Plot += tmp[i].ToString();
                //                }
                //                break;
                //            case "Poster":
                //                if (!(tmp[1].ToString().Trim() == "N/A"))
                //                    movie.Poster = tmp[1].ToString() + ":" + tmp[2].ToString();
                //                break;
                //            case "Runtime":
                //                movie.Runtime = tmp[1].ToString();
                //                break;
                //            case "Rating":
                //                movie.Rating = tmp[1].ToString();
                //                break;
                //            case "Votes":
                //                movie.Votes = tmp[1].ToString();
                //                break;
                //            case "ID":
                //                movie.ID = tmp[1].ToString();
                //                break;
                //            case "Response":
                //                break;
                //            default:
                //                break;
                //        }
                //    }
                //    else
                //    {
                //        switch (currentProperty)
                //        {
                //            case "Title":
                //                movie.Title += "." + item.ToString();
                //                break;
                //            case "Genre":
                //                movie.Genre.Add(item.Trim());
                //                break;
                //            case "Writer":
                //                movie.Writer.Add(item.Trim());
                //                break;
                //            case "Actors":
                //                movie.Actors.Add(item.Trim());
                //                break;
                //            default:
                //                break;
                //        }
                //    }
                //}
                //index++;
                //movie.Nr = index;
                //if (!string.IsNullOrEmpty(movie.Title))
                //    movie.IsEnabled = true;
                //netList.Add(movie);
                //dataGrid2.ItemsSource = null;
                //dataGrid2.ItemsSource = netList;
            }
            //dataGrid2.ItemsSource = netList;
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
             string str = e.Result;
        }

        void wc_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            byte[] bytedata = e.Result;
            MemoryStream stream = new MemoryStream(bytedata);
            StreamReader reader = new StreamReader(stream);

            string text = reader.ReadToEnd();
        }

        private string TokanizeFunction(string title)
        {
            string result = "";
            #region Forbidden Tokens
            string[] stopArray = new string[] { 
                    "DVDRIP", 
                    "BRRIP",
                    "BDRIP",
                    "DVDSCR",
                    "CAM",
                    "TS",
                    "TC",
                    "R5",
                    "R3",
                    "PPVRIP",
                    "HDRIP",
                    "VODRiP",
                    "DVDSCREENER",
                    "DVDRSCREENER",
                    "SCREENER",
                    "TELESYNC",
                    "SCR",
                    "DVD",
                    "BLURAY",
                    "XVID",
                    "AC3",
                    "DIVX",
                    "480P",
                    "720P",
                    "H264",
                    "AAC",
                    "X264",
                    "HDTV",
                    "NTSC",
                    "HQ",
                    "STV",
                    "HARDCODED",
                    "ENG",
                    "SUB",
                    "SUBS",
                    "NL",
                    "ISLTXT",
                    "ÍSL",
                    "TEXTI",
                    "MVS",
                    "PUKKA",
                    "KVCD", 
                    "HOCKNEY",
                    "TUS",
                    "AXXO",
                    "KLAXXON",
                    "AVI",
                    "INTERNAL"
                };
            /* Risk Text
             ------- UNRATED ------ LIMITED ------- WORKPRINT ------- ENCODED ------- INTERNAL -------
            **/
            #endregion

            if (title.Contains('(') || title.Contains('[') || title.Contains('{'))
            {
                foreach (Char c in title)
                {
                    if (c != '(' && c != '[' && c != '{')
                        result += c;
                    else
                    {
                        title = result;
                        break;
                    }
                }
            }
            
            List<string> resultArray = new List<string>();
            string[] strArray = title.Replace(" ", ".").Replace("_", ".").Replace("-", ".").Replace(",", ".").Replace("+", ".").Replace(";", ".").Split('.');
            StringArrayCheck(stopArray, resultArray, strArray.ToList());
            result = "";

            foreach (string str in resultArray)
            {
                result += str + " ";
            }
        
            return result;
        }

        private int FindRipMethod(string title)
        {
            int result = -1;
            Movie movie = new Movie();
            title = title.Replace(" ", ".").Replace("-", ".").Replace("{", ".").Replace("}", ".").Replace("[", ".").Replace("]", ".").Replace("(", ".").Replace(")", ".").Replace("_", ".");
            string[] array = title.Split('.');

            string[] ripMethod = new string[]{
                    "DVDRIP", 
                    "DVD",
                    "BRRIP",
                    "BDRIP",
                    "BLURAY",
                    "DVDSCR",
                    "DVDSCREENER",
                    "DVDRSCREENER",
                    "CAM",
                    "TELESYNC",
                    "TS",
                    "TC",
                    "R5",
                    "R3",
                    "PPVRIP",
                    "HDRIP",                    
                    "SCREENER",                    
                    "SCR",                    
            };
            bool found = false;
            foreach (string str in array)
            {
                for(int i=0; i < ripMethod.Count(); i++)
                {
                    if (str.ToUpper().Contains(ripMethod[i]))
                    {
                        result = i;
                        found = true;
                        break;
                    }
                }
                if (found)
                    break;
            }

            if (result > -1)
            {
                if (result < 2)
                    result = 1;
                else if (result < 5)
                    result = 2;
                else if (result < 8)
                    result = 3;
                else if (result == 8)
                    result = 4;
                else if (result < 12)
                    result = 5;
                else if (result == 12)
                    result = 6;
                else if (result == 13)
                    result = 7;
                else if (result == 14)
                    result = 8;
                else if (result == 15)
                    result = 9;
                else if (result <= 17)
                    result = 10;
            }
            else
                result = 0;

            return result;
        }

        private static void StringArrayCheck(string[] stopArray, List<string> resultArray, List<string> endResult)
        {
            foreach (string str in endResult)
            {
                foreach (string s in stopArray)
                {
                    if (str.ToUpper().Equals(s) || str.ToUpper().Contains("XVID") || str.ToUpper().Contains("DVDRIP") || str.ToUpper().Contains("DVDSCR"))
                        return;
                }
                resultArray.Add(str);
            }
        }
        
        private void RenameFolders_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < resultArray.Count; i++)
            {
                string newPath = string.Empty;
                string oldPath = string.Empty;
                newPath = selectedPath;
                oldPath = resultArray[i].DirectoryPath;

                if (!string.IsNullOrEmpty(netList[i].Title) && netList[i].IsEnabled)
                {
                    string ripMethod = string.Empty;
                    if (!netList[i].MovieRip[netList[i].index].Equals("None"))
                        ripMethod = " [" + netList[i].MovieRip[netList[i].index] + "]";

                    newPath += "\\" + netList[i].Title.Replace(":", "-") + " (" + netList[i].Year.Trim() + ")" + ripMethod;
                    WebClient wc = new WebClient();

                    if (resultArray[i].DirectoryPath != selectedPath)
                    {
                        if (!Directory.Exists(newPath))
                        {
                            Directory.Move(oldPath, newPath);
                            wc.DownloadFile(netList[i].Poster, newPath + "\\poster.jpg");
                        }
                        else
                        {
                            int copyIndex = 2;
                            while (true)
                            {
                                if (!Directory.Exists(newPath + " (" + copyIndex.ToString() + ")"))
                                {
                                    Directory.Move(oldPath, newPath + " (" + copyIndex.ToString() + ")");
                                    wc.DownloadFile(netList[i].Poster, newPath + " (" + copyIndex.ToString() + ")" + "\\poster.jpg");
                                    break;
                                }
                                else
                                    copyIndex++;
                            }
                        }
                    }
                    else
                    {
                        if (!Directory.Exists(newPath))
                        {
                            Directory.CreateDirectory(newPath);
                            File.Move(oldPath + "\\" + resultArray[i].Title, newPath + "\\" + resultArray[i].Title);
                            wc.DownloadFile(netList[i].Poster, newPath + "\\poster.jpg");
                        }
                        else
                        {
                            int copyIndex = 2;
                            while (true)
                            {
                                if (!Directory.Exists(newPath + " (" + copyIndex.ToString() + ")"))
                                {
                                    Directory.CreateDirectory(newPath + " (" + copyIndex.ToString() + ")");
                                    File.Move(oldPath + "\\" + resultArray[i].Title, newPath + "\\" + resultArray[i].Title);
                                    wc.DownloadFile(netList[i].Poster, newPath + " (" + copyIndex.ToString() + ")" + "\\poster.jpg");
                                    break;
                                }
                                else
                                    copyIndex++;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < resultArray.Count; i++)
            {
                resultArray[i].Title = netList[i].Title;
            }

            dataGrid1.DataContext = null;
            dataGrid1.DataContext = resultArray;
        }
    }

    public class Movie
    {
        private List<string> Gen = new List<string>();
        private List<string> Wri = new List<string>();
        private List<string> Act = new List<string>();
        private bool foundMovie = false;
        private List<string> Rip = new List<string>() { "None", "DVDRIP", "BRRIP", "DVDSCR", "CAM", "TS", "R5", "R3", "PPVRIP", "HDRIP", "SCR" };

        public int Nr { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public string Rated{ get; set; }
        public List<string> Genre { get { return Gen; } set { Gen = value; } }
        public string Director { get; set; }
        public List<string> Writer { get { return Wri; } set { Wri = value; } }
        public List<string> Actors { get { return Act; } set { Act = value; } }
        public string Plot { get; set; }
        public string Poster { get; set; }
        public string Runtime { get; set; }
        public string Rating { get; set; }
        public string Votes { get; set; }
        public string ID { get; set; }
        public string DirectoryPath { get; set; }
        public bool IsEnabled { get { return foundMovie; } set { foundMovie = value; } }
        public List<string> MovieRip { get { return Rip; } }
        public int index { get; set; }

        public Movie()
        {
        }
    }
}

