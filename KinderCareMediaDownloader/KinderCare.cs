using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper;
using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace KinderCareMediaDownloader
{
    public class KCMedia
    {
        public string Url { get; set; }
        public string Date { get; set; }
        public string Child { get; set; }
        public string Caption { get; set; }
    }

    public class KinderCare
    {
        string USERNAME = "";
        string PASSWORD = "";

        string photo_location = "/Users/Jen/Projects/KinderCareMediaDownloader/KinderCareMediaDownloader/media/";
        int SHOW_MORE = 10;
        String test_url = "https://classroom.kindercare.com/headlines";

        IWebDriver driver;

        [SetUp]
        public void start_Browser()
        {
            // Local Selenium WebDriver
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
        }

        [Test]
        public void get_Photos()
        {
            driver.Url = test_url;

            System.Threading.Thread.Sleep(2000);

            Login();

            System.Threading.Thread.Sleep(2000);


            for (int i = 0; i < SHOW_MORE; i++)
           {
                var btn = driver.FindElement(By.XPath("//a[contains(@onclick,'moreImages()')]"));
                var jse = (IJavaScriptExecutor)driver;
                jse.ExecuteScript("arguments[0].click();", btn);
                TestContext.Out.WriteLine(i);
                System.Threading.Thread.Sleep(2000);
            }

            System.Threading.Thread.Sleep(2000);
            var tiles = driver.FindElements(By.CssSelector("div.activity-modal"));
           
          

            var list = new List<KCMedia>();

            foreach (var tile in tiles)
            {
                var id = Regex.Replace(tile.GetAttribute("id"), @"[^\d]", "");
                var dataType = tile.GetAttribute("data-type");

                string _url = "";

                if (dataType.Equals("image"))
                {
                     _url = tile.GetAttribute("data-srcimg");
                }
                else if (dataType.Equals("video"))
                {
                    _url = tile.GetAttribute("data-srcmp4");
                }



                if (_url.Length > 0)
                {
                    var kc = new KCMedia() { Url=_url};

                    var details = driver.FindElement(By.XPath("//a[contains(@data-src,'#activity-"+id+"-modal')]"));
                    var date = details.FindElement(By.ClassName("thumbnail-date")).FindElement(By.CssSelector("p")).Text;
                    var name = details.FindElement(By.ClassName("account-name")).FindElement(By.CssSelector("span")).Text;
                    var title = details.FindElement(By.ClassName("thumbnail-title")).FindElement(By.CssSelector("h4")).Text.Replace(",", " ");
                   // var caption = details.FindElement(By.ClassName("thumbnail-snippet")).FindElement(By.CssSelector("p")).Text.Replace(","," ");

                    //if (title.Length > 0 && caption.Length > 0)
                    //    kc.Caption = title + " - " + caption;
                    //else if (title.Length > 0)
                    //    kc.Caption = title;
                   // else if (caption.Length > 0)
                   //     kc.Caption = caption;

                    kc.Date = date.Replace("st","").Replace("rd","").Replace("nd","").Replace("th","")+" "+DateTime.Now.Year;
                    kc.Child = name;

                    list.Add(kc);
                }
                    
                    
            }

            

            SaveFile(list);



            System.Threading.Thread.Sleep(2000);

            Console.WriteLine("Test Passed");
        }

        [TearDown]
        public void close_Browser()
        {
            driver.Quit();
        }

        private void Login()
        {
            var username=driver.FindElement(By.Id("user_login"));
            var password = driver.FindElement(By.Id("user_password"));
            var login = driver.FindElement(By.Name("commit"));
            username.SendKeys(USERNAME);
            password.SendKeys(PASSWORD);
            login.Click();
        }

        private void SaveFile(List<KCMedia> records)
        {
            using (var writer = new StreamWriter(photo_location+"media.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
            }
        }
    }
}