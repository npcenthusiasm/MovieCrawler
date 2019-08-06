using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace home0505_Webcrawler_Movie
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string weburl = "";
        string movieId = "";
        string movieImg = "";
        string movieTheater = "";
        string theaterId = "";
        int TheaterID;
        int MovieID;

        // information
        //https://www.vscinemas.com.tw/vsweb/film/detail.aspx?id=3833

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";

            //HtmlAgilityPack.HtmlDocument doc = web.Load(weburl)
            string[,] SQLData = {
                { "https://www.vscinemas.com.tw/vsTicketing/ticketing/ticket.aspx?cinema=15|TZ&movie=HO00008003","film_20190610001.jpg" },//(IMAX 3D)蜘蛛人：離家日
                {"https://www.vscinemas.com.tw/vsTicketing/ticketing/ticket.aspx?cinema=15|TZ&movie=HO00007891","film_20190418020.jpg" }, //(數位 英)阿拉丁
                //{"https://www.vscinemas.com.tw/vsTicketing/ticketing/ticket.aspx?cinema=15|TZ&movie=HO00007905","film_20190514001.jpg" }, //(數位)哥吉拉 II 怪獸之王
                {"https://www.vscinemas.com.tw/vsTicketing/ticketing/ticket.aspx?cinema=15|TZ&movie=HO00007970","film_20190517001.jpg" }, //(數位 國)玩具總動員 4
                //{"https://www.vscinemas.com.tw/vsTicketing/ticketing/ticket.aspx?cinema=15|TZ&movie=HO00007884","film_20190411020.jpg" }, //(數位)捍衛任務 3 : 全面開戰
                {"https://www.vscinemas.com.tw/vsTicketing/ticketing/ticket.aspx?cinema=15|TZ&movie=HO00007933","film_20190530001.jpg" }, //(數位)X戰警：黑鳳凰
                {"https://www.vscinemas.com.tw/vsTicketing/ticketing/ticket.aspx?cinema=15|TZ&movie=HO00007988","film_20190429031.jpg" }, //(數位)MIB星際戰警：跨國行動
                {"https://www.vscinemas.com.tw/vsTicketing/ticketing/ticket.aspx?cinema=15|TZ&movie=HO00007997","film_20190613002.jpg" }, //(數位)極惡對決
                {"https://www.vscinemas.com.tw/vsTicketing/ticketing/ticket.aspx?cinema=15|TZ&movie=HO00008027","film_20190521015.jpg" } //(數位)安娜貝爾回家囉
            };
            int[] SQLDMovieID = { 1, 2, 4, 6, 7, 8, 9 };

            //SQL                movieTheater  ,                theaterId
            movieTheater = "台中大遠百威秀影城";
            TheaterID = 1; 
            SqlConnection cn = new SqlConnection("server=.\\SQLExpress ; Database=MovieCrawler;Integrated Security=true");
            //SqlConnection cn = new SqlConnection("server=movietimelist.database.windows.net ; Database=newsDB;Integrated Security=false ; user id=Project0508;password=Npcok72571");
            cn.Open();

            HtmlWeb web = new HtmlWeb();
            int insertcount = 0; /*MessageBox.Show(SQLData.Length.ToString());*/
            for (int k = 0; k < SQLData.Length/2; k++)
            {
              
                MovieID = SQLDMovieID[k];
                weburl = SQLData[k,0];
                //movieId = SQLData[k, 0];
                movieImg = SQLData[k, 1];


                //先印出來 然後 作格式上的轉換 字串轉時間  現在的時間跟網頁的時間比 以現在時間為標準 往後續抓5筆 在判斷是某跟資料庫是否重複 --->這樣就不用判斷資料庫有沒有資料了 因為沒重負直接INSERT
                //MessageBox.Show(nowTime.ToString());

                //MessageBox.Show(weburl);
                //MessageBox.Show(movieId);
                //MessageBox.Show(movieImg);
                //return;
                int i = 0;  //抓的比數
                int count = 0;
                HtmlAgilityPack.HtmlDocument doc = web.Load(weburl);//--------------------------------------------------------------網址
                //MessageBox.Show(movieId);
                HtmlNode root = doc.DocumentNode;


                var dateNodeList = root.SelectNodes("/html/body/article[2]/section[2]/div");               //day節點 
                                                                                                           //time結點
                string movieName = root.SelectSingleNode("//*[@class=\"movieDescribe\"]/h1").InnerText; //movie                (數位)復仇者聯盟：終局之戰 //(數位)名偵探皮卡丘

                //------------------insertMovieList------------------
                SqlCommand cmdGetID = new SqlCommand("Select Count(*)  FROM movieList WHERE MovieID='" + MovieID + "' ", cn); //抓SQL MovieID 是否有相同的時間
                string resultID = cmdGetID.ExecuteScalar().ToString();
                if (resultID != "1")                  //==1 在表在SQL資料庫找到相同的資料，就不要Insert
                {
                    string insertMovieList = string.Format(
                        "INSERT MovieList (MovieID, MovieName, MovieImg) VALUES ('{0}', '{1}', '{2}' )",
                         MovieID, movieName, movieImg);
                    SqlCommand cmdInsertMovieList = new SqlCommand(insertMovieList, cn);
                    cmdInsertMovieList.ExecuteNonQuery();
                    string inset = "MovieID: " + MovieID + "MovieName: " + movieName + "MovieImg: " + movieImg ;
                    textBox1.Text += inset + "\r\n";

                    //------------------insertMovieList------------------
                }


                foreach (HtmlNode dateNode in dateNodeList)
                {
                    string date = dateNode.SelectSingleNode("./h4").InnerText;//  2019 年 05 月 02 日 Thursday
                    DateTime mDateTime = DateTime.Parse(date); //2019/5/5 上午 12:00:00
                    //textBox1.Text = mDateTime.ToString();
                    string day = mDateTime.ToString("yyyy-MM-dd");//2019-05-05

                    count++;
                    var timeNodeList = root.SelectNodes("//html/body/article[2]/section[2]/div[" + count + "]/ul/li"); //幹
                    foreach (HtmlNode timeNode in timeNodeList)
                    {
                        string hour = timeNode.SelectSingleNode("./a").InnerText;//  time  10:40   00:00(隔日)---> 負4  
                                                                                 //textBox1.Text +=day + hour + "\r\n";
                        if (hour.Length > 5)
                        {
                            hour = hour.Substring(0, 5);
                            DateTime AddDateTime = mDateTime.AddDays(1);
                            day = AddDateTime.ToString("yyyy-MM-dd");//2019-05-05+1
                        }
                        string movieDaytime = day + " " + hour;   //2019-05-05 10:40
                                                                  //textBox1.Text += movieDaytime + "\r\n";

                        DateTime nowTime = DateTime.Now;                                     //2019/5/5 下午 11:00:00
                        DateTime movieTime = Convert.ToDateTime(movieDaytime);               //把字串轉時間 跟now 做比較
                                                                                             //textBox1.Text +=movieDaytime +"...."+ real.ToString() + "\r\n";
                        if (DateTime.Compare(movieTime, nowTime) > 0)                           //電影時間比現在還 晚的話 ex.  電影12:00 現在10:00  那就往後抓5筆 抓的時候先判斷有沒有重複
                        {
                            if (i < 50)       //-------------------------- Insert的比數
                            {

                                SqlCommand cmdGetTime = new SqlCommand("Select Count(*)  FROM movieInfo WHERE TheaterId='" + TheaterID + "'AND MovieDaytime='" + movieDaytime + "' ", cn); //抓SQL movieDaytime 是否有相同的時間

                                //SqlCommand cmdGetTime = new SqlCommand(
                                //    "Select Count(*)  FROM 資料表名稱 WHERE 條件=''AND 條件=''");

                                string resultTime = cmdGetTime.ExecuteScalar().ToString();


                                //textBox1.Text += resultTime + "\r\n";
                                if (resultTime != "1")                  //==1 在表在SQL資料庫找到相同的資料，就不要Insert
                                {


                                    //MessageBox.Show("movieId=" + movieId + "i=" + i.ToString());
                                    //------------------------------------Insert SQL--------------------------------------------------------
                                    string insertText = string.Format(
                                        "INSERT movieInfo (TheaterID,MovieTheater,MovieID,MovieName,MovieDaytime,MovieImg) VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}' )",
                                         TheaterID, movieTheater, MovieID, movieName, movieDaytime, movieImg);
                                    SqlCommand cmdInsert = new SqlCommand(insertText, cn);
                                    cmdInsert.ExecuteNonQuery();
                                    //------------------------------------Insert SQL--------------------------------------------------------
                                    string inset = "TheaterID: " + TheaterID + "movieTheater: " + movieTheater + "movieId: " + MovieID + "movieName: " + movieName + "movieDaytime: " + movieDaytime + "movieImg: " + movieImg;
                                    textBox1.Text += inset + "\r\n";
                                    i = i + 1;
                                    insertcount++;
                                }
                                else
                                {
                                    i = i + 1;
                                }
                            }
                            else
                            {
                                textBox1.Text += "不需更新" + "\r\n";
                                label1.Text = "insert " + insertcount + " 筆";
                            }
                        }

                    }
                }
            }
        }


        //-----------------------------------------------台中新時代凱擘影城------------------------------------------------------
        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";

            //HtmlAgilityPack.HtmlDocument doc = web.Load(weburl)
            string[,] SQLData = {
                {"https://center.kbrocinemas.com/Browsing/Movies/Details/h-HO00000054","film_20190610001.jpg" },//(IMAX 3D)蜘蛛人：離家日
                {"https://center.kbrocinemas.com/Browsing/Movies/Details/h-HO00000023","film_20190418020.jpg" }, //(數位 英)阿拉丁
                {"https://center.kbrocinemas.com/Browsing/Movies/Details/h-HO00000020","film_20190514001.jpg" }, //(數位)哥吉拉 II 怪獸之王
                {"https://center.kbrocinemas.com/Browsing/Movies/Details/h-HO00000045","film_20190517001.jpg" }, //(數位 國)玩具總動員 4
                //{"","film_20190411020.jpg" }, //(數位)捍衛任務 3 : 全面開戰
                {"https://center.kbrocinemas.com/Browsing/Movies/Details/h-HO00000033","film_20190530001.jpg" }, //(數位)X戰警：黑鳳凰
                {"https://center.kbrocinemas.com/Browsing/Movies/Details/h-HO00000040","film_20190429031.jpg" }, //(數位)MIB星際戰警：跨國行動
                {"https://center.kbrocinemas.com/Browsing/Movies/Details/h-HO00000051","film_20190613002.jpg" }, //(數位)極惡對決
                {"https://center.kbrocinemas.com/Browsing/Movies/Details/h-HO00000052","film_20190521015.jpg" } //(數位)安娜貝爾回家囉
            };
            int[] SQLDMovieID = {1,2,3,4,6,7,8,9 };

            //SQL                movieTheater  ,   theaterId
            movieTheater = "台中新時代凱擘影城";
            int TheaterID=2;
            SqlConnection cn = new SqlConnection("server=.\\SQLExpress ; Database=MovieCrawler;Integrated Security=true");
            //SqlConnection cn = new SqlConnection("server=movietimelist.database.windows.net ; Database=newsDB;Integrated Security=false ; user id=Project0508;password=Npcok72571");

            cn.Open();

            HtmlWeb web = new HtmlWeb();

            int insertcount = 0;

            for (int k = 0; k < SQLData.Length / 2; k++)
            {

                MovieID = SQLDMovieID[k];
                weburl = SQLData[k, 0];
                //movieId = SQLData[k, 0];
                movieImg = SQLData[k, 1];
                

                HtmlAgilityPack.HtmlDocument doc = web.Load(weburl);//--------------------------------------------------------記得改
                HtmlNode root = doc.DocumentNode;
                int i = 0;
                //---------------------------------------------------今天----------------------------------------------------------
                string todadyDateNode = root.SelectSingleNode("//*[@class=\" session\"]/h4[1]").InnerText;//今日場次的日期   星期日, 05 五月 2019 
                DateTime today = DateTime.Parse(todadyDateNode); //2019/5/5 上午 12:00:00
                string day = today.ToString("yyyy-MM-dd");//2019-05-05
                var todayTimeNodeList = root.SelectNodes("//*[@id=\"show-times\"]/div[2]/div/div/div[2]/div/a"); //今日場次的時間 00:00 ; 10:40 ; 11:15 ; 11:50  

                //---------------------------------------------------今天----------------------------------------------------------



                //---------------------------------------------------明天----------------------------------------------------------
                //string futureTimeNode = root.SelectSingleNode("//*[@class=\"future session\"]/h4[1]").InnerText; //明天場次的日期  星期一, 06 五月 2019 
                //DateTime b = DateTime.Parse(futureTimeNode); //2019/5/5 上午 12:00:00

                //string tomorrow = b.ToString("yyyy-MM-dd");//2019-05-05    
                //var futureTimeNodeList = root.SelectNodes("//*[@id=\"show-times\"]/div[2]/div/div/div[3]/div/a");  //明天場次的時間 00:00 ; 10:40 ; 11:15 ; 11:50
                //---------------------------------------------------明天----------------------------------------------------------

                string title = root.SelectSingleNode("//*[@id=\"show-times\"]/h2[1]").InnerText;//title
                string movieNameB = title.Trim().Replace(" ", "").Replace(":", "：");   //(數位)復仇者聯盟：終局之戰
                //MessageBox.Show(movieNameB);
                string movieName = movieNameB.Substring(0, movieNameB.Length - 4);   //(數位)復仇者聯盟：終局之戰
                //MessageBox.Show(movieName);

                //MessageBox.Show("1");

                //-------------------------- Insert的比數

                foreach (var Node in todayTimeNodeList)
                {

                    string hour = Node.SelectSingleNode("./time").InnerText;//10:40
                    string movieDaytime = day + " " + hour;   //2019-05-05 10:40


                    //textBox1.Text += movieDaytime + "\r\n";
                    DateTime movieTime = Convert.ToDateTime(movieDaytime);
                    DateTime now = DateTime.Now;

                    //if (i > 5)
                    //{


                    //    return;
                    //}

                    if (i < 50)       //-------------------------- Insert的比數
                    {

                        if (DateTime.Compare(movieTime, now) > 0) //電影時間比現在還 晚的話 ex.  電影12:00 現在10:00  那就往後抓5筆 抓的時候先判斷有沒有重複
                        {
                            SqlCommand cmdGetTime = new SqlCommand("Select Count(*)  FROM movieInfo WHERE TheaterId='" + theaterId + "'AND MovieDaytime='" + movieDaytime + "' ", cn); //抓SQL movieDaytime 是否有相同的時間
                            string resultTime = cmdGetTime.ExecuteScalar().ToString();
                            //MessageBox.Show(movieTime.ToString() + "...." + resultTime);
                            if (resultTime != "1") //如果資料庫沒資料
                            {
                                //  ------------------------------------Insert SQL--------------------------------------------------------
                                string insertText = string.Format(
                                    "INSERT MovieInfo (TheaterID,MovieTheater,MovieID,MovieName,MovieDaytime,MovieImg) VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}' )",
                                     TheaterID, movieTheater, MovieID, movieName, movieDaytime, movieImg);
                                SqlCommand cmdInsert = new SqlCommand(insertText, cn);
                                cmdInsert.ExecuteNonQuery();
                                //  ------------------------------------Insert SQL--------------------------------------------------------
                                string inset = "TheaterID: " + TheaterID + "MovieTheater: " + movieTheater + "MovieID: " + MovieID + "movieName: " + movieName + "movieDaytime: " + movieDaytime + "movieImg: " + movieImg;
                                textBox1.Text += inset + "\r\n";

                                i = i + 1;
                                insertcount++;
                                label1.Text = "insert " + insertcount + " 筆";
                            }
                            else
                            {
                                i = i + 1;
                                textBox1.Text += "不需更新" + "\r\n";

                            }
                        }
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";

            //HtmlAgilityPack.HtmlDocument doc = web.Load(weburl)
            string[,] SQLData = {
                {"https://www.vscinemas.com.tw/vsTicketing/ticketing/ticket.aspx?cinema=3|TT01&movie=HO00008000","film_20190610001.jpg" },//(IMAX 3D)蜘蛛人：離家日
                {"https://www.vscinemas.com.tw/vsTicketing/ticketing/ticket.aspx?cinema=3|TT01&movie=HO00007891","film_20190418020.jpg" }, //(數位 英)阿拉丁
                {"https://www.vscinemas.com.tw/vsTicketing/ticketing/ticket.aspx?cinema=3|TT01&movie=HO00007905","film_20190514001.jpg" }, //(數位)哥吉拉 II 怪獸之王
                {"https://www.vscinemas.com.tw/vsTicketing/ticketing/ticket.aspx?cinema=3|TT01&movie=HO00007970","film_20190517001.jpg" }, //(數位 國)玩具總動員 4
                //{"","film_20190411020.jpg" }, //(數位)捍衛任務 3 : 全面開戰
                //{"https://www.vscinemas.com.tw/vsTicketing/ticketing/ticket.aspx?cinema=3|TT01&movie=HO00007933","film_20190530001.jpg" }, //(數位)X戰警：黑鳳凰
                {"https://www.vscinemas.com.tw/vsTicketing/ticketing/ticket.aspx?cinema=3|TT01&movie=HO00007988","film_20190429031.jpg" }, //(數位)MIB星際戰警：跨國行動
                {"https://www.vscinemas.com.tw/vsTicketing/ticketing/ticket.aspx?cinema=3|TT01&movie=HO00007997","film_20190613002.jpg" }, //(數位)極惡對決
                {"https://www.vscinemas.com.tw/vsTicketing/ticketing/ticket.aspx?cinema=3|TT01&movie=HO00008027","film_20190521015.jpg" } //(數位)安娜貝爾回家囉
            };
            int[] SQLDMovieID = { 1, 2, 3, 4, 7, 8, 9 };

            //SQL                movieTheater  ,                theaterId
            movieTheater = "台中Tiger City威秀影城";
            TheaterID = 3;
            SqlConnection cn = new SqlConnection("server=.\\SQLExpress ; Database=MovieCrawler;Integrated Security=true");
            //SqlConnection cn = new SqlConnection("server=movietimelist.database.windows.net ; Database=newsDB;Integrated Security=false ; user id=Project0508;password=Npcok72571");
            cn.Open();

            HtmlWeb web = new HtmlWeb();
            int insertcount = 0; /*MessageBox.Show(SQLData.Length.ToString());*/
            for (int k = 0; k < SQLData.Length / 2; k++)
            {

                MovieID = SQLDMovieID[k];
                weburl = SQLData[k, 0];
                movieImg = SQLData[k, 1];


                //先印出來 然後 作格式上的轉換 字串轉時間  現在的時間跟網頁的時間比 以現在時間為標準 往後續抓5筆 在判斷是某跟資料庫是否重複 --->這樣就不用判斷資料庫有沒有資料了 因為沒重負直接INSERT
                //MessageBox.Show(nowTime.ToString());
                //return;
                int i = 0;  //抓的比數
                int count = 0;
                HtmlAgilityPack.HtmlDocument doc = web.Load(weburl);//--------------------------------------------------------------網址
                //MessageBox.Show(movieId);
                HtmlNode root = doc.DocumentNode;


                var dateNodeList = root.SelectNodes("/html/body/article[2]/section[2]/div");               //day節點 
                                                                                                           //time結點
                string movieName = root.SelectSingleNode("//*[@class=\"movieDescribe\"]/h1").InnerText; //movie                (數位)復仇者聯盟：終局之戰 //(數位)名偵探皮卡丘

                //------------------insertMovieList------------------
                SqlCommand cmdGetID = new SqlCommand("Select Count(*)  FROM movieList WHERE MovieID='" + MovieID + "' ", cn); //抓SQL MovieID 是否有相同的時間
                string resultID = cmdGetID.ExecuteScalar().ToString();
                if (resultID != "1")                  //==1 在表在SQL資料庫找到相同的資料，就不要Insert
                {
                    string insertMovieList = string.Format(
                        "INSERT MovieList (MovieID, MovieName, MovieImg) VALUES ('{0}', '{1}', '{2}' )",
                         MovieID, movieName, movieImg);
                    SqlCommand cmdInsertMovieList = new SqlCommand(insertMovieList, cn);
                    cmdInsertMovieList.ExecuteNonQuery();
                    string inset = "MovieID: " + MovieID + "MovieName: " + movieName + "MovieImg: " + movieImg;
                    textBox1.Text += inset + "\r\n";

                    //------------------insertMovieList------------------
                }


                foreach (HtmlNode dateNode in dateNodeList)
                {
                    string date = dateNode.SelectSingleNode("./h4").InnerText;//  2019 年 05 月 02 日 Thursday
                    DateTime mDateTime = DateTime.Parse(date); //2019/5/5 上午 12:00:00
                    //textBox1.Text = mDateTime.ToString();
                    string day = mDateTime.ToString("yyyy-MM-dd");//2019-05-05

                    count++;
                    var timeNodeList = root.SelectNodes("//html/body/article[2]/section[2]/div[" + count + "]/ul/li"); //幹
                    foreach (HtmlNode timeNode in timeNodeList)
                    {
                        string hour = timeNode.SelectSingleNode("./a").InnerText;//  time  10:40   00:00(隔日)---> 負4  
                                                                                 //textBox1.Text +=day + hour + "\r\n";
                        if (hour.Length > 5)
                        {
                            hour = hour.Substring(0, 5);
                            DateTime AddDateTime = mDateTime.AddDays(1);
                            day = AddDateTime.ToString("yyyy-MM-dd");//2019-05-05+1
                        }
                        string movieDaytime = day + " " + hour;   //2019-05-05 10:40
                                                                  //textBox1.Text += movieDaytime + "\r\n";

                        DateTime nowTime = DateTime.Now;                                     //2019/5/5 下午 11:00:00
                        DateTime movieTime = Convert.ToDateTime(movieDaytime);               //把字串轉時間 跟now 做比較
                                                                                             //textBox1.Text +=movieDaytime +"...."+ real.ToString() + "\r\n";
                        if (DateTime.Compare(movieTime, nowTime) > 0)                           //電影時間比現在還 晚的話 ex.  電影12:00 現在10:00  那就往後抓5筆 抓的時候先判斷有沒有重複
                        {
                            if (i < 50)       //-------------------------- Insert的比數
                            {

                                SqlCommand cmdGetTime = new SqlCommand("Select Count(*)  FROM movieInfo WHERE TheaterId='" + TheaterID + "'AND MovieDaytime='" + movieDaytime + "' ", cn); //抓SQL movieDaytime 是否有相同的時間

                                //SqlCommand cmdGetTime = new SqlCommand(
                                //    "Select Count(*)  FROM 資料表名稱 WHERE 條件=''AND 條件=''");

                                string resultTime = cmdGetTime.ExecuteScalar().ToString();


                                //textBox1.Text += resultTime + "\r\n";
                                if (resultTime != "1")                  //==1 在表在SQL資料庫找到相同的資料，就不要Insert
                                {


                                    //MessageBox.Show("movieId=" + movieId + "i=" + i.ToString());
                                    //------------------------------------Insert SQL--------------------------------------------------------
                                    string insertText = string.Format(
                                        "INSERT movieInfo (TheaterID,MovieTheater,MovieID,MovieName,MovieDaytime,MovieImg) VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}' )",
                                         TheaterID, movieTheater, MovieID, movieName, movieDaytime, movieImg);
                                    SqlCommand cmdInsert = new SqlCommand(insertText, cn);
                                    cmdInsert.ExecuteNonQuery();
                                    //------------------------------------Insert SQL--------------------------------------------------------
                                    string inset = "TheaterID: " + TheaterID + "movieTheater: " + movieTheater + "movieId: " + MovieID + "movieName: " + movieName + "movieDaytime: " + movieDaytime + "movieImg: " + movieImg;
                                    textBox1.Text += inset + "\r\n";
                                    i = i + 1;
                                    insertcount++;
                                }
                                else
                                {
                                    i = i + 1;
                                }
                            }
                            else
                            {
                                textBox1.Text += "不需更新" + "\r\n";
                                label1.Text = "insert " + insertcount + " 筆";
                            }
                        }

                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

    }
}