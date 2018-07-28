using System;
using System.Collections.Generic;
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
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        internal void EnableBlur()
        {
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void GetNews()
        {


            Yonhap yonhap = new Yonhap("http://www.yonhapnews.co.kr/international/0603000001.html");
            List<String> titleLinks = yonhap.getTitleLinks();
            List<String> titles = yonhap.getTitles(titleLinks);
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate {
                Newscontentlist.Items.Clear();

                for (int i = 0; i < 20; i++)
                {
                    Newscontentlist.Items.Add(new ListBoxItem() { Content = titles[i], Tag = titleLinks[i] });
                }
            }));

        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            EnableBlur();
            Newscontentlist.Items.Add(new ListBoxItem() { Content = "뉴스를 불러오고 있습니다!" });
            System.Threading.Thread newsthread = new System.Threading.Thread(GetNews);
            newsthread.Start();
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void Title_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Newscontentlist_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
            {
                System.Diagnostics.Process.Start(item.Tag.ToString());
            }
        }
    }
}
