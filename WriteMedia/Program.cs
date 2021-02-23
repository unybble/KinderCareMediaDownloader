using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using CsvHelper;

namespace WriteMedia
{
    public class KCMedia
    {
        public string Url { get; set; }
        public string Date { get; set; }
        public string Child { get; set; }
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

                    var counter = 1;
                    using (var wc = new WebClient())
                    {
                        
                        foreach (var record in records)
                        {
                            var fname = new StringBuilder();

                            var ext = record.Url.Split(".");
                            var extWithExtra = ext[ext.Length - 1];
                            var extParts = extWithExtra.Split("?");


                            fname.Append(record.Child+"_"+record.Date.Replace(" ","-")+"_"+counter++);
                            fname.Append("." + extParts[0]);
                           
                            Console.WriteLine(fname);
                  
                            wc.DownloadFile(record.Url, dl_path + fname );
                        }
                    }
                }
            }
        }
       
    }
}
