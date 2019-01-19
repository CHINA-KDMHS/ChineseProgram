using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using 中國語真棒.Backend;

namespace 中國語真棒.Frontend
{
    /// <summary>
    /// NewsViewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NewsViewer : Window
    {
        public NewsViewer()
        {
            InitializeComponent();
        }
        
        public ReadOnlyCollection<News> getNews(String keyword)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");

            var chromeDriverService = ChromeDriverService.CreateDefaultService(System.IO.Directory.GetCurrentDirectory() + "\\libs");
            chromeDriverService.HideCommandPromptWindow = true;
            IWebDriver driver = new ChromeDriver(chromeDriverService, options);
            // 네이버 뉴스검색 url 
            driver.Url = "https://search.naver.com/search.naver?where=news&query=중국+" + keyword.Replace(" ", "+") + "&sm=tab_srt&sort=1&photo=0&field=0&reporter_article=&pd=0&ds=&de=&docid=&nso=so%3Add%2Cp%3Aall%2Ca%3Aall&mynews=0&mson=0&refresh_start=0&related=0";

            Newscontentlist.Items.Clear();
            IWebElement newsSection = driver.FindElement(By.ClassName("mynews"));
            ReadOnlyCollection<IWebElement> newsElements = newsSection.FindElement(By.ClassName("type01")).FindElements(By.TagName("li"));

            List<News> newsList = new List<News>();

            for (int i = 0; i < newsElements.Count; i++)
            {
                IWebElement newsElement = newsElements[i];
                IWebElement newsElement_dt = newsElement.FindElement(By.TagName("dt")); // 타이틀
                IWebElement newsElement_a = newsElement_dt.FindElement(By.TagName("a")); // a태그(href 링크)
                String title = newsElement_dt.Text;
                String link = newsElement_a.GetAttribute("href");
                newsList.Add(new News(title, link));
                Newscontentlist.Items.Add(new ListBoxItem() { Content = title, Tag = link });
            }

            return new ReadOnlyCollection<News>(newsList);
        }

    public class News
    {
        private String newsTitle { get; }
        private String link { get; }

        public News(String newsTitle, String link)
        {
            this.newsTitle = newsTitle;
            this.link = link;
        }

    }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            //Newscontentlist.Items.Add(new ListBoxItem() { Content = "뉴스를 불러오고 있습니다!" });
            getNews("IT");
        }


        private void Newscontentlist_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
            {
                System.Diagnostics.Process.Start(item.Tag.ToString());
            }
        }

        private void searchbox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            getNews(searchbox.Text);
        }
    }
}
