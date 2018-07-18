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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 中國語真棒
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    internal enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_INVALID_STATE = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        // ...
        WCA_ACCENT_POLICY = 19
        // ...
    }
    public partial class MainWindow : Window
    {

        Storyboard MenuOn;
        Storyboard MenuOff;


        private bool Dragging;

        public MainWindow()
        {
            InitializeComponent();
            MenuOn = (Storyboard)FindResource("OpenDisplay");
            MenuOff = (Storyboard)FindResource("CloseDisplay");
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EnableBlur();

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }


        private void Image_DragEnter(object sender, DragEventArgs e)
        {
            Console.WriteLine("Drag Enter");
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

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CaptureMouse();
            Dragging = true;
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging)
            {
                Point canvPosToWindow = ((Image)sender).TransformToAncestor(this).Transform(new Point(0, 0));

                Image r = sender as Image;
                var upperlimit = canvPosToWindow.Y + (r.Height / 2);
                var lowerlimit = canvPosToWindow.Y + ((Image)sender).ActualHeight - (r.Height / 2);

                var leftlimit = canvPosToWindow.X + (r.Width / 2);
                var rightlimit = canvPosToWindow.X + ((Image)sender).ActualWidth - (r.Width / 2);


                var absmouseXpos = e.GetPosition(this).X;
                var absmouseYpos = e.GetPosition(this).Y;

                Console.WriteLine(absmouseXpos + " " + absmouseYpos);
                if ((absmouseXpos > leftlimit && absmouseXpos < rightlimit)
                    && (absmouseYpos > upperlimit && absmouseYpos < lowerlimit))
                {
                    r.SetValue(Canvas.LeftProperty, e.GetPosition(((Image)sender)).X - (r.Width / 2));
                    r.SetValue(Canvas.TopProperty, e.GetPosition(((Image)sender)).Y - (r.Height / 2));
                }
            }
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            Dragging = false;
        }

        private void Title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Count.Tag = "down";
            display.IsHitTestVisible = true;
            MenuOn.Begin();
            MenuOff.Stop();
        }

        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            Count.Tag = "up";
            display.IsHitTestVisible = true;
            MenuOn.Begin();
            MenuOff.Stop();
        }

        private void display_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            display.IsHitTestVisible = false;
            MenuOff.Begin();
            MenuOn.Stop();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String counttype = Count.Tag.ToString();
            TextBlock SelectTeam = null;

            switch(((ComboBoxItem)selectteam.SelectedItem).Tag.ToString())
            {

                case "one":
                    SelectTeam = oneteam;
                    break;
                case "two":
                    SelectTeam = twoteam;
                    break;
                case "three":
                    SelectTeam = threeteam;
                    break;
                case "four":
                    SelectTeam = fourteam;
                    break;
                case "five":
                    SelectTeam = fiveteam;
                    break;
                case "six":
                    SelectTeam = sixteam;
                    break;
                default:
                    break;
            }

            if(counttype == "up")
            {
                SelectTeam.Text = "X" + (Int32.Parse(SelectTeam.Text.Replace("X", "")) + Int32.Parse(givecount.Text)).ToString();
            } else if (counttype == "down") {
                SelectTeam.Text = "X" + (Int32.Parse(SelectTeam.Text.Replace("X", "")) - Int32.Parse(givecount.Text)).ToString();
            }
            display.IsHitTestVisible = false;
            MenuOff.Begin();
            MenuOn.Stop();
        }
    }
}
