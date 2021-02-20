using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using CsvHelper;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace KinderCareMediaDownloader
{
    public class KCMedia
    {
        public string Url { get; set; }
    }

    public class KinderCare
    {
    
        string photo_location = "/Users/Jen/Projects/KinderCareMediaDownloader/KinderCareMediaDownloader/media/";
        int SHOW_MORE = 0;
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
            var images = driver.FindElements(By.CssSelector("div.activity-modal"));

            var urls = new List<KCMedia>();

            foreach (var image in images)
            {
                var path = image.GetAttribute("data-srcimg");
                TestContext.Out.WriteLine(path);
                if (path.Length > 0)
                urls.Add(new KCMedia() { Url = path });
            }

            SaveFile(urls);



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