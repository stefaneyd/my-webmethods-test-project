using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Web;
using System.IO;
using System.Threading;

namespace MovieCollector_WinForm
{
    public partial class Form1 : Form
    {
        #region Variables
        enum MovieProperty { Title = 1, Year = 2, Rated = 3, Genre = 4, Director = 5, Writer = 6, Actors = 7, Plot = 8, Poster = 9, Runtime = 10, Rating = 11, Votes = 12, ID = 13, DirectoryPath = 14 };
        string[] movieProperties = new string[] { "Title", "Year", "Rated", "Genre", "Director", "Writer", "Actors", "Plot", "Poster", "Runtime", "Rating", "Votes", "ID", "DirectoryPath" };
        string selectedPath = "";
        public List<Movie> resultArray = new List<Movie>();
        public static List<Movie> netList = new List<Movie>();
        public List<string> MovieRip = new List<string>() { "None", "DVDRIP", "BRRIP", "DVDSCR", "CAM", "TS", "R5", "R3", "PPVRIP", "HDRIP", "SCR" };
        int index = 0;
        Movie currentMovie = null;
        string runningPath = string.Empty;
        public enum MovieRips : int
        {
            DVDRIP = 1,
            BRRIP = 2,
            CAM = 3,
            TS = 4
        }
        private bool bScrollbarsLocked = true;   // =true: scrollbar positions synchronized.
        private int iScrollPos_L;         // Left DataGridView scrollbar position.
        private int iScrollPos_R;         // Right DataGridView scrollbar position.
        #endregion

        public Form1()
        {
            InitializeComponent();

            runningPath = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\")) + "\\TmpImg";

        }

        private void GetFilesAndFolders()
        {
            resultArray.Clear();
            dataGridView1.DataSource = null;
            dataGridView1.Columns.Clear();
            netList.Clear();
            dataGridView2.DataSource = null;
            dataGridView2.Columns.Clear();

            index = 0;
            InitializeGridView1();
            InitializeGridView2();

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

            foreach (Movie mov in resultArray)
                netList.Add(null);

            dataGridView1.DataSource = resultArray;
            dataGridView2.DataSource = netList;

            //index = 0;
        }

        private void InitializeGridView2()
        {
            dataGridView2.AutoGenerateColumns = false;
            DataGridViewTextBoxColumn txtColm = new DataGridViewTextBoxColumn();
            txtColm.Width = 30;
            txtColm.HeaderText = "Nr";
            txtColm.DataPropertyName = "Nr";
            dataGridView2.Columns.Add(txtColm);

            DataGridViewTextBoxColumn txtColm2 = new DataGridViewTextBoxColumn();
            txtColm2.Width = 300;
            txtColm2.HeaderText = "Title";
            txtColm2.DataPropertyName = "Title";
            dataGridView2.Columns.Add(txtColm2);

            DataGridViewTextBoxColumn txtColm3 = new DataGridViewTextBoxColumn();
            txtColm3.Width = 100;
            txtColm3.HeaderText = "Year";
            txtColm3.DataPropertyName = "Year";
            dataGridView2.Columns.Add(txtColm3);

            DataGridViewImageColumn imgColm = new DataGridViewImageColumn();
            imgColm.HeaderText = "Poster";
            imgColm.DataPropertyName = "PosterImage";
            imgColm.ImageLayout = DataGridViewImageCellLayout.Zoom;
            //imgColm.Image = Image.FromFile("C:\\Users\\Stefan\\Pictures\\movie.png", true);

            dataGridView2.Columns.Add(imgColm);

            DataGridViewComboBoxColumn comCol = new DataGridViewComboBoxColumn();
            comCol.HeaderText = "RIP Style";
            comCol.DataSource = MovieRip;
            //comCol.DataPropertyName = "MovieRip";
            //comCol.DisplayMember = "MovieRip";
            //comCol.ValueMember = "MovieRip";
            dataGridView2.Columns.Add(comCol);
        }

        private void InitializeGridView1()
        {
            dataGridView1.AutoGenerateColumns = false;
            DataGridViewTextBoxColumn txtColumn = new DataGridViewTextBoxColumn();
            txtColumn.Width = 30;
            txtColumn.HeaderText = "Nr";
            txtColumn.DataPropertyName = "Nr";
            dataGridView1.Columns.Add(txtColumn);
            DataGridViewTextBoxColumn txtColumn2 = new DataGridViewTextBoxColumn();
            txtColumn2.Width = 300;
            txtColumn2.HeaderText = "Title";
            txtColumn2.DataPropertyName = "Title";
            dataGridView1.Columns.Add(txtColumn2);
        }

        private void PopulateDataGrid(string text, Movie current)
        {
            string[] arr = text.Replace("{", string.Empty).Replace("}", string.Empty).Replace("\"", string.Empty).Split(',');

            Movie movie = new Movie();
            string currentProperty = "";
            movie.index = FindRipMethod(current.Title);

            foreach (string item in arr)
            {
                if (item.Contains(':'))
                {
                    string[] tmp = item.Split(':');
                    currentProperty = tmp[0].Trim();

                    switch (currentProperty)
                    {
                        case "Title":
                            movie.Title = tmp[1].ToString();
                            if (tmp.Count() > 2)
                            {
                                movie.Title += ":";
                                for (int i = 2; i < tmp.Count(); i++)
                                    movie.Title += tmp[i].ToString();
                            }
                            break;
                        case "Year":
                            movie.Year = tmp[1].ToString();
                            break;
                        case "Rated":
                            movie.Rated = tmp[1].ToString();
                            break;
                        case "Released":
                            break;
                        case "Genre":
                            movie.Genre.Add(tmp[1].ToString());
                            break;
                        case "Director":
                            movie.Director = tmp[1].ToString();
                            break;
                        case "Writer":
                            movie.Writer.Add(tmp[1].ToString());
                            break;
                        case "Actors":
                            movie.Actors.Add(tmp[1].ToString());
                            break;
                        case "Plot":
                            movie.Plot = tmp[1].ToString();
                            if (tmp.Count() > 2)
                            {
                                movie.Plot += ":";
                                for (int i = 2; i < tmp.Count(); i++)
                                    movie.Plot += tmp[i].ToString();
                            }
                            break;
                        case "Poster":
                            if (!(tmp[1].ToString().Trim() == "N/A"))
                            {
                                movie.Poster = tmp[1].ToString() + ":" + tmp[2].ToString();
                            }
                            break;
                        case "Runtime":
                            movie.Runtime = tmp[1].ToString();
                            break;
                        case "Rating":
                            movie.Rating = tmp[1].ToString();
                            break;
                        case "Votes":
                            movie.Votes = tmp[1].ToString();
                            break;
                        case "ID":
                            movie.ID = tmp[1].ToString();
                            break;
                        case "Response":
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (currentProperty)
                    {
                        case "Title":
                            movie.Title += "." + item.ToString();
                            break;
                        case "Genre":
                            movie.Genre.Add(item.Trim());
                            break;
                        case "Writer":
                            movie.Writer.Add(item.Trim());
                            break;
                        case "Actors":
                            movie.Actors.Add(item.Trim());
                            break;
                        default:
                            break;
                    }
                }
            }
            index++;
            movie.Nr = current.Nr;
            if (!string.IsNullOrEmpty(movie.Title))
                movie.IsEnabled = true;
            netList[movie.Nr-1] =  movie;

            if (!Directory.Exists(runningPath))
                Directory.CreateDirectory(runningPath);

            if (movie.Title != null || movie.Poster != null)
            {
                string img = runningPath + "\\" + (movie.Title + movie.Poster.Substring(movie.Poster.LastIndexOf("."))).Replace(':', ' ');
                if (!File.Exists(img))
                {
                    WebClient wc = new WebClient();
                    Uri uri = new Uri(movie.Poster);
                    wc.DownloadFile(uri, img);
                }
                movie.PosterImage = Image.FromFile(img);
            }
            //DataGridViewImageColumn imgColm = new DataGridViewImageColumn();
            //DataGridViewTextBoxColumn txtColm = new DataGridViewTextBoxColumn();
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
                for (int i = 0; i < ripMethod.Count(); i++)
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

        #region Event Functions
        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            PopulateDataGrid(e.Result, (Movie)e.UserState);
            dataGridView2.Refresh();
        }

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            if (bScrollbarsLocked == false) { return; }

            // Send position from DataGridView_L.
            if ((sender == dataGridView1) &&
                (e.ScrollOrientation == ScrollOrientation.VerticalScroll))
            {
                iScrollPos_L = dataGridView1.FirstDisplayedScrollingRowIndex;
                // Call Scroll event handler for DataGridView_R.
                dataGridView2_Scroll(dataGridView1,
                        new ScrollEventArgs(ScrollEventType.ThumbPosition,
                        e.NewValue, ScrollOrientation.VerticalScroll));
            }

            // Get position from DataGridView_R.
            if ((sender == dataGridView2) &&
                (e.ScrollOrientation == ScrollOrientation.VerticalScroll))
            {
                // Scroll same distance as DataGridView_R.
                dataGridView1.FirstDisplayedScrollingRowIndex = iScrollPos_R;
            }
        }

        private void dataGridView2_Scroll(object sender, ScrollEventArgs e)
        {
            if (bScrollbarsLocked == false) { return; }

            // Send position from DataGridView_R.
            if ((sender == dataGridView2) &&
                (e.ScrollOrientation == ScrollOrientation.VerticalScroll))
            {
                iScrollPos_R = dataGridView2.FirstDisplayedScrollingRowIndex;
                // Call Scroll event handler for DataGridView_L.
                dataGridView1_Scroll(dataGridView2,
                        new ScrollEventArgs(ScrollEventType.ThumbPosition,
                        e.NewValue, ScrollOrientation.VerticalScroll));
            }

            // Get position from DataGridView_L.
            if ((sender == dataGridView1) &&
                (e.ScrollOrientation == ScrollOrientation.VerticalScroll))
            {
                // Scroll same distance as DataGridView_L.
                dataGridView2.FirstDisplayedScrollingRowIndex = iScrollPos_L;
            }
        }

        private void GetFolderPath(object sender, EventArgs e)
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

        private void FetchFromWeb(object sender, EventArgs e)
        {
            string searchTitel = "http://www.imdbapi.com/?t=";
            WebClient wc = null;

            foreach (Movie m in resultArray)
            {
                wc = new WebClient();
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);

                string title = HttpUtility.UrlEncode(TokanizeFunction(m.Title).Trim());

                Uri uri = new Uri(searchTitel + title);
                currentMovie = m;
                wc.DownloadStringAsync(uri, m);
                wc.Dispose();
            }
        }
        #endregion
    }

    public class Movie
    {
        private List<string> Gen = new List<string>();
        private List<string> Wri = new List<string>();
        private List<string> Act = new List<string>();
        private bool foundMovie = false;
        private List<string> Rip = new List<string>() { "None", "DVDRIP", "BRRIP", "DVDSCR", "CAM", "TS", "R5", "R3", "PPVRIP", "HDRIP", "SCR" };
        public Image PosterImage { get; set; }
        public int Nr { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public string Rated { get; set; }
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
