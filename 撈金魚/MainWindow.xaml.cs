using FastBitmapLib;
using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static 撈金魚.AnalyzeNet;
using Application = System.Windows.Application;
using Color = System.Drawing.Color;
using Cursor = System.Windows.Forms.Cursor;
using Image = System.Drawing.Image;
using MessageBox = System.Windows.MessageBox;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace 撈金魚
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        GetProgramWindow window = new GetProgramWindow("flashplayer_32_sa");
        public const int MOLE_W = 960, MOLE_H = 560, BUCKET_NET_X = 150, BUCKET_NET_Y = -10, BUCKET_WATER_X = 235, BUCKET_WATER_Y = 415, NET_FIX_X = 50, NET_FIX_Y = 45;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            window.updateRect();
            new Thread(playFish).Start();
        }

        public MainWindow()
        {
            InitializeComponent();
            InitializeGlobalHook();
        }

        private void InitializeGlobalHook()
        {
            IKeyboardMouseEvents hook = Hook.GlobalEvents();
            //hook.MouseUpExt += mouseUp;
            hook.KeyUp += exit;
        }

        private void exit(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Escape))
                Environment.Exit(0);
        }

        private void playFish()
        {
            int count = 0;
            Application.Current.Dispatcher.Invoke((ThreadStart)delegate
            {
                count = Convert.ToInt32(input.Text);
            }
            );
            for (int i=0; i<count; i++)
            {
                removeOneTime();
                Thread.Sleep(10);
                startFish();
                getFish();
                Thread.Sleep(50);
                endFish();
            }
            goRestaurant();
        }

        private void startFish()
        {
            DoInput.mouseClickForMole(window.rect, 838, 32);
            Thread.Sleep(1000);
            DoInput.mouseClickForMole(window.rect, 788, 443);
            Thread.Sleep(1000);
            DoInput.mouseClickForMole(window.rect, 474, 485);
            Thread.Sleep(1000);
        }

        private void endFish()
        {
            DoInput.mouseClickForMole(window.rect, 480, 358);
        }

        private void goRestaurant()
        {
            DoInput.mouseClickForMole(window.rect, 880, 538);
            DoInput.mouseClickForMole(window.rect, 880, 449);
        }

        private void getFish()
        {
            for(int i=0; i<25; i++)
            {
                Thread.Sleep(50);
                Point net_center;
                do
                {
                    Point[] fish = findDiferences();
                    if (fish.Length == 7)
                        return;
                    net_center = AnalyzeNet.CalculateBestPoint(fish, window.rect, this);
                } while (net_center.X == -1);
                DoInput.fishClickKit(window.rect, net_center.X + NET_FIX_X*MOLE_W/window.rect.width, net_center.Y + NET_FIX_Y*MOLE_H / window.rect.height);
            }
        }

        private Point[] findDiferences()
        {
            Point[] fish = ScreenAction.GetFish(window);
            if (fish.Length < 3)
            {
                DateTime start = DateTime.Now;
                while (fish.Length < 3)
                {
                    if (DateTime.Now - start >= TimeSpan.FromSeconds(5))
                    {
                        return new Point[7];
                    }
                    fish = ScreenAction.GetFish(window);
                }
            }

            return fish;
        }

        private void removeOneTime()
        {
            Application.Current.Dispatcher.Invoke((ThreadStart)delegate
            {
                input.Text = Convert.ToString(Convert.ToInt32(input.Text)-1);
            }
            );
        }

        public void addText(string s)
        {
            Application.Current.Dispatcher.Invoke((ThreadStart)delegate
            {
                input.Text += s + "\n";
            }
            );
        }

        public void addNum(int num)
        {
            Application.Current.Dispatcher.Invoke((ThreadStart)delegate
            {
                input.Text = Convert.ToString(Convert.ToInt32(input.Text) + num);
            }
            );
        }

        //public void addText(string s)
        //{
        //Application.Current.Dispatcher.Invoke((ThreadStart)delegate
        //    {
        //        text_block.Text += s + "\n";
        //    }
        //    );
        //}

        //private void debugFishDots(Point[] fish)
        //{
        //    Bitmap bitmap = new Bitmap(window.rect.width, window.rect.height);
        //    FastBitmap fast = new FastBitmap(bitmap);
        //    fast.Lock();

        //    //text_block.Text += "\n" + fish.Count();
        //    Application.Current.Dispatcher.Invoke((ThreadStart)delegate
        //        {
        //            text_block.Text += "\n" + fish.Count();
        //        }
        //    );

        //    foreach (Point p in fish)
        //    {
        //        int r = 0;
        //        for(int x=p.X-r; x<=p.X+r; x++)
        //        {
        //            for(int y=p.Y-r; y<=p.Y+r; y++)
        //            {
        //                fast.SetPixel(x, y, Color.Black);
        //            }
        //        }
        //        Console.WriteLine(p);
        //    }

        //    bitmap.Save("dots.png");
        //}
    }
}
