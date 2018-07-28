using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 中國語真棒.Backend
{
    class Croller
    {
        //    static void Main(string[] args)
        //    {
        //        //Yonhap yonhap = new Yonhap("http://www.yonhapnews.co.kr/international/0603000001.html");
        //        Yonhap yonhap = new Yonhap("http://www.yonhapnews.co.kr/international/0602000001.html");

        //        List<String> titleLinks = yonhap.getTitleLinks();
        //        List<String> titles = yonhap.getTitles(titleLinks);

        //        for (int i = 0; i < titleLinks.Count; i++)
        //        {
        //            Console.WriteLine(titleLinks[i] + ": " + titles[i]);
        //        }

        //        Console.Read();
        //    }
    }

    class Yonhap
    {
        HtmlDocument htmlDoc;
        List<String> values = new List<String>();


        public Yonhap(String address)
        {
            HtmlWeb web = new HtmlWeb();

            htmlDoc = web.Load(address);


        }

        public List<String> getTitleLinks()
        {
            List<String> list = new List<String>();
            if (htmlDoc.DocumentNode != null)
            {
                var node = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'headlines headline-list')]").SelectNodes("//li[contains(@class, 'section02')]/div/strong/a");


                for (int i = 0; i < node.Count; i++)
                {
                    list.Add(node[i].Attributes["href"].Value);
                }
            }

            return list;
        }

        public List<String> getTitles(List<String> links)
        {
            List<String> list = new List<string>();

            for (int i = 0; i < links.Count; i++)
            {
                HtmlWeb web = new HtmlWeb();

                HtmlDocument htmlDoc = web.Load(links[i]);

                list.Add(htmlDoc.DocumentNode.SelectSingleNode("//h1[contains(@class, 'tit-article')]").InnerHtml);
            }

            return list;
        }

        public List<String> getValues()
        {
            return values;
        }
    }
}
