using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using CsvHelper;

namespace WriteMedia
{
    public class KCMedia
    {
        public string Url { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello!");
            string dl_path = "/Users/Jen/Projects/KinderCareMediaDownloader/KinderCareMediaDownloader/media/";

            string path = dl_path+"media.csv";
            using (TextReader fileReader = File.OpenText(path))
            {
                using (var reader = new StreamReader(path))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<KCMedia>();
                    var counter = 0;
                    Console.WriteLine("Files");
                    using (var wc = new WebClient())
                    {
                        Console.WriteLine(counter);
                        foreach (var record in records)
                        {
                            var ext = record.Url.Split(".");
                            wc.DownloadFile(record.Url, dl_path + counter++.ToString() + "."+ext[ext.Length-1]);
                        }
                    }
                }
            }
        }
       
    }
}
