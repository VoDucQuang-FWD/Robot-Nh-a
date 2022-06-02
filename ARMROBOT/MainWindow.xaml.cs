using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ModBus_RTU;
using System.IO.Ports;
using System.ComponentModel;
using Microsoft.Win32;
using System.IO;
using System;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Diagnostics;
using System.Collections.ObjectModel;
using ARMROBOT.ViewModel;
using System.Threading;
using ARMROBOT.View;

namespace ARMROBOT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ModBus_RS485 _RS485 = new ModBus_RS485();
        BackgroundWorker BackgroundWorker1 = new BackgroundWorker();
        int bn1 = 0, bn2 = 0, bn3 = 0;
        byte ID1 = 1, ID2 = 5, ID3 = 3, ID4 = 4, ID5 = 5;
        Int16[] enable = new Int16[1];
        Int16[] AlHome_X = new Int16[2];
        Int16[] AlHome_Y1 = new Int16[2];
        Int16[] AlHome_Y2 = new Int16[2];
        Int16[] AlHome_Z1 = new Int16[2];
        Int16[] AlHome_Z2 = new Int16[2];
        Int16[] R_Point_X = new Int16[2];
        Int16[] R_Point_Y1 = new Int16[2];
        Int16[] R_Point_Y2 = new Int16[2];
        Int16[] R_Point_Z1 = new Int16[2];
        Int16[] R_Point_Z2 = new Int16[2];
        Int16[] Speed = new Int16[2];
        Int16[] Accel = new Int16[1];
        Int16[] Decel = new Int16[1];
        Int16[] SpeedRun = new Int16[2];
        Int16[] AccelRun = new Int16[1];
        Int16[] DecelRun = new Int16[1];
        Int16[] PointX = new Int16[2];
        Int16[] PointY1 = new Int16[2];
        Int16[] PointY2 = new Int16[2];
        Int16[] PointZ1 = new Int16[2];
        Int16[] PointZ2 = new Int16[2];
        Int16[] WRITE_IO = new Int16[2];
        short[] READ_IO = new Int16[2];
        short[] READ_DO = new Int16[2];
        string Command;
        string Command1;
        string Command2;
        Int32 X;
        Int32 Y1;
        Int32 Y2;
        Int32 Z1;
        Int32 Z2;
        Int16 Delay;
        Int16 SDOON;
        Int16 SDOOF;
        Int16 WDION;
        Int16 WDIOF;
        Int16 PositioHomeX = 0;
        Int16 PositioHomeY1 = 0;
        Int16 PositioHomeY2 = 0;
        Int16 PositioHomeZ1 = 0;
        Int16 PositioHomeZ2 = 0;
        Int16 SpeedHomeX = 0;
        Int16 SpeedHomeY1 = 0;
        Int16 SpeedHomeY2 = 0;
        Int16 SpeedHomeZ1 = 0;
        Int16 SpeedHomeZ2 = 0;
        Int16 AccelHomeX = 0;
        Int16 AccelHomeY1 = 0;
        Int16 AccelHomeY2 = 0;
        Int16 AccelHomeZ1 = 0;
        Int16 AccelHomeZ2 = 0;
        Int16 DecelHomeX = 0;
        Int16 DecelHomeY1 = 0;
        Int16 DecelHomeY2 = 0;
        Int16 DecelHomeZ1 = 0;
        Int16 DecelHomeZ2 = 0;
        int a, b;
        char[] out_put1 = new char[8];
        char[] out_put2 = new char[8];
        char[] out_put3 = new char[8];
        char[] out_put4 = new char[8];
        public static DispatcherTimer TimerDelay = new DispatcherTimer();
        DispatcherTimer TimerSpeed = new DispatcherTimer();
        DispatcherTimer TimerHome = new DispatcherTimer();
        public static DispatcherTimer TimerRefrest = new DispatcherTimer();
        public static DispatcherTimer TimerRichtextBox = new DispatcherTimer();
       
        public MainWindow()
        {

            InitializeComponent();

            BackgroundWorker1.DoWork += BackgroundWorker1_DoWork;
            BackgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            BackgroundWorker1.WorkerSupportsCancellation = true;
            TimerDelay.Tick += TimerDelay_Tick;
            TimerDelay.Interval = TimeSpan.FromSeconds(1);
            TimerSpeed.Tick += TimerSpeed_Tick;
            TimerSpeed.Interval = TimeSpan.FromMilliseconds(10);
            TimerHome.Tick += TimerHome_Tick;
            TimerHome.Interval = TimeSpan.FromMilliseconds(500);
            TimerRefrest.Tick += TimerRefrest_Tick;
            TimerRefrest.Interval = TimeSpan.FromMilliseconds(0.2);
            TimerRichtextBox.Tick += TimerRichtextBox_Tick;
            TimerRichtextBox.Interval += TimeSpan.FromMilliseconds(0.2);
            
        }
        #region TIMER
        private void TimerRichtextBox_Tick(object sender, EventArgs e)
        {
            RichTextBoxRun.IsEnabled = true;
            openrun1();
            TimerRichtextBox.IsEnabled = false;
        }
        private void TimerRefrest_Tick(object sender, EventArgs e)
        {
            testl.UpdateLayout();
            testl.Items.Refresh();
            testl.UnselectAll();
            TimerRefrest.IsEnabled = false;
        }

        int t = 0;
        bool T = false;
        private void TimerDelay_Tick(object sender, EventArgs e)
        {
            t++;
            if (T == true)
            {
                Timerdelay.Content = t.ToString();
            }
        }
        int speed = 100;
        private void TimerSpeed_Tick(object sender, EventArgs e)
        {
            if (addspeed)
            {
                speed += 7;
                Velocity.Value = speed;

            }
            else if (minusspeed)
            {
                speed -= 7;
                if (speed <= 0)
                { speed = 0; }
                Velocity.Value = speed;
            }
            Speed[0] = Convert.ToInt16(Velocity.Value);
            if (Speed[0] == 100)
            {
                Speed[0] = 102;
            }
            else if (Speed[0] == 90)
            {
                Speed[0] = 92;
            }
            else if (Speed[0] == 200)
            {
                Speed[0] = 202;
            }
            Accel[0] = Convert.ToInt16(Speed[0] * 10 / 100);
            Decel[0] = Convert.ToInt16(Speed[0] * 10 / 100);

        }
        int th = 0;
        private void TimerHome_Tick(object sender, EventArgs e)
        {
            th++;
        }
        #endregion
        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (BackgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                try
                {
                    if (bn1 == 1 && Wrun == false)
                    {
                        Thread.Sleep(new TimeSpan(10000));
                        _RS485.SendFc3(ID1, 11055, 2, ref R_Point_X);
                        Thread.Sleep(new TimeSpan(10000));
                        _RS485.SendFc3(ID2, 11055, 2, ref R_Point_Y1);
                        Thread.Sleep(new TimeSpan(10000));
                        //_RS485.SendFc3(3, 11055, 2, ref R_Point_Y2);
                        //_RS485.SendFc3(4, 11055, 2, ref R_Point_Z1);
                        //_RS485.SendFc3(5, 11055, 2, ref R_Point_Z2);
                        _RS485.SendFc02(6, 0, 16, ref READ_IO);
                        Thread.Sleep(new TimeSpan(10000));
                        _RS485.SendFc01(6, 0, 16, ref READ_DO);
                        Dispatcher.BeginInvoke(new Action(delegate
                        {
                            Point_X.Content = GetIntFromBits_Read(R_Point_X[0], R_Point_X[1]);
                            Point_XRun.Content = GetIntFromBits_Read(R_Point_X[0], R_Point_X[1]);
                            Point_Y1.Content = GetIntFromBits_Read(R_Point_Y1[0], R_Point_Y1[1]);
                            Point_Y1Run.Content = GetIntFromBits_Read(R_Point_Y1[0], R_Point_Y1[1]);
                            //Point_Y2.Content = R_Point_Y2[0].ToString();
                            //Point_Y2Run.Content = R_Point_Y2[0].ToString();
                            //Point_Z1.Content = R_Point_Z1[0].ToString();
                            //Point_Z1Run.Content = R_Point_Z1[0].ToString();
                            //Point_Z2.Content = R_Point_Z2[0].ToString();
                            //Point_Z2Run.Content = R_Point_Z1[0].ToString();
                            var output1 = Convert.ToString(READ_IO[0], 2);
                            var output2 = Convert.ToString(READ_IO[1], 2);

                            string out1 = output1.PadLeft(8, '0');
                            string out2 = output2.PadLeft(8, '0');
                            for (int i = 0; i < 8; i++)
                            {
                                out_put1[i] = out1[i];
                                out_put2[i] = out2[i];
                            }
                            var output3 = Convert.ToString(READ_DO[0], 2);
                            var output4 = Convert.ToString(READ_DO[1], 2);

                            string out3 = output3.PadLeft(8, '0');
                            string out4 = output4.PadLeft(8, '0');
                            for (int i = 0; i < 8; i++)
                            {
                                out_put3[i] = out3[i];
                                out_put4[i] = out4[i];
                            }
                            Check_IO();
                            Check_DO();
                        }));
                    }
                    else if (bn1 == 0)
                    {
                        if (reset)
                        {
                            Reset();
                            reset = false;
                        }
                    }
                    if (Wenable == true)
                    {
                        Enable();
                        Wenable = false;
                    }
                    if (Whome == true)
                    {
                        HOME();
                        Whome = false;
                    }
                    if (WWhome)
                    {
                        home();
                        Thread.Sleep(new TimeSpan(10000));
                        _RS485.SendFc16(ID1, 11616, 1, new short[1] { 1 });
                        Thread.Sleep(new TimeSpan(10000));
                        _RS485.SendFc16(ID2, 11616, 1, new short[1] { 1 });
                        WWhome = false;
                    }
                    #region Manual

                    if (Wjog31_X == true)
                    {
                        Jog31_X();
                        Wjog31_X = false;
                    }
                    else if (Wjog31_Y1 == true)
                    {
                        Jog31_Y1();
                        Wjog31_Y1 = false;
                    }
                    else if (Wjog31_Y2 == true)
                    {
                        Jog31_Y2();
                        Wjog31_Y2 = false;
                    }
                    else if (Wjog31_Z1 == true)
                    {
                        Jog31_Z1();
                        Wjog31_Z1 = false;
                    }
                    else if (Wjog31_Z2 == true)
                    {
                        Jog31_Z2();
                        Wjog31_Z2 = false;
                    }
                    else if (Wjog63_X == true)
                    {
                        Jog63_X();
                        Wjog63_X = false;
                    }
                    else if (Wjog63_Y1 == true)
                    {
                        Jog63_Y1();
                        Wjog63_Y1 = false;
                    }
                    else if (Wjog63_Y2 == true)
                    {
                        Jog63_Y2();
                        Wjog63_Y2 = false;
                    }
                    else if (Wjog63_Z1 == true)
                    {
                        Jog63_Z1();
                        Wjog63_Z1 = false;
                    }
                    else if (Wjog63_Z2 == true)
                    {
                        Jog63_Z2();
                        Wjog63_Z2 = false;
                    }
                    if (Wjogoff == true)
                    {
                        JogOFF();
                        Wjogoff = false;
                    }
                    else if (WmoveX31 == true)
                    {
                        MoveX31();
                        WmoveX31 = false;
                    }
                    else if (WmoveY131 == true)
                    {
                        MoveY131();
                        WmoveY131 = false;
                    }
                    else if (WmoveY231 == true)
                    {
                        MoveY231();
                        WmoveY231 = false;
                    }
                    else if (WmoveZ131 == true)
                    {
                        MoveZ131();
                        WmoveZ131 = false;
                    }
                    else if (WmoveZ231 == true)
                    {
                        MoveZ231();
                        WmoveZ231 = false;
                    }


                    #endregion
                    #region IO

                    if (Wio == true)
                    {
                        _RS485.SendFc15(6, 0, 16, WRITE_IO);
                        Wio = false;
                    }
                    #endregion
                    if (Wrun == true)
                    {
                        ReadTXT();
                        Wrun = false;
                    }
                    if (WRunline == true)
                    {
                        Runline();
                        WRunline = false;
                    }
                    if (Wshutdown == true)
                    {
                        ShutDown();
                        Wshutdown = false;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!BackgroundWorker1.IsBusy && !chay)
            {
                BackgroundWorker1.RunWorkerAsync();
            }
        }
        private void Reset()
        {
            Thread.Sleep(new TimeSpan(10000));
            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 128 });
            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 0 });
            Thread.Sleep(new TimeSpan(10000));
            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 128 });
            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 0 });
            //_RS485.SendFc16(3, 24640, 1, new short[1] { 128 });
            //_RS485.SendFc16(4, 24640, 1, new short[1] { 128 });
            //_RS485.SendFc16(5, 24640, 1, new short[1] { 128 });
            //_RS485.SendFc16(3, 24640, 1, new short[1] { 0 });
            //_RS485.SendFc16(4, 24640, 1, new short[1] { 0 });
            //_RS485.SendFc16(5, 24640, 1, new short[1] { 0 });
            Dispatcher.BeginInvoke(new Action(delegate
            {
                bool closed = _RS485.Closed();
                Estop.Content = "ESTOP OFF";
                Robot_On.Content = "ROBOT ON";
                bn2 = 0;
                if (closed)
                {
                    if (BackgroundWorker1.IsBusy)
                    {
                        chay = true;
                        BackgroundWorker1.CancelAsync();
                    }
                    Estop.Background = new SolidColorBrush(Color.FromArgb(255, 151, 229, 35));
                    Robot_On.IsEnabled = false;
                    Home.IsEnabled = false;
                }
            }));
        }
        bool chay = false;
        bool reset = false;
        private void Estop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bn1 = 1 - bn1;
                if (bn1 == 1)
                {
                    bool open = _RS485.Opened("COM7", 115200, 8, Parity.None, StopBits.One);

                    if (open)
                    {
                        Estop.Content = "ESTOP ON";
                        Estop.Background = new SolidColorBrush(Color.FromArgb(255, 229, 35, 35));
                        Robot_On.IsEnabled = true;
                        // testread.IsEnabled = true;
                        if (!BackgroundWorker1.IsBusy)
                        {
                            chay = false;
                            BackgroundWorker1.RunWorkerAsync();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy cổng Com");
                        bn1 = 0;
                    }
                }
                if (bn1 == 0)
                {
                    reset = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Enable()
        {
            Thread.Sleep(new TimeSpan(10000));
            _RS485.SendFc16(ID1, 24640, 1, enable);
            Thread.Sleep(new TimeSpan(10000));
            _RS485.SendFc16(ID2, 24640, 1, enable);
            //_RS485.SendFc16(3, 24640, 1, enable);
            //_RS485.SendFc16(4, 24640, 1, enable);
            //_RS485.SendFc16(5, 24640, 1, enable);
        }
        bool Wenable = false;
        private void Robot_On_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bn2 = 1 - bn2;
                if (bn2 == 1)
                {
                    enable[0] = 15;
                    Wenable = true;
                    Robot_On.Content = "ROBOT OFF";
                    Home.IsEnabled = true;
                    OpenFileRun.IsEnabled = true;
                    StopRun.IsEnabled = false;
                    PauseRun.IsEnabled = false;
                }
                if (bn2 == 0)
                {
                    enable[0] = 0;
                    Wenable = true;
                    Robot_On.Content = "ROBOT ON";
                    Home.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        bool Wshutdown = false;
        private void ShutDown()
        {
            Thread.Sleep(new TimeSpan(10000));
            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 0 });
            Thread.Sleep(new TimeSpan(10000));
            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 0 });
            Thread.Sleep(new TimeSpan(10000));
            //_RS485.SendFc16(3, 24640, 1, new short[1] { 0 });
            //_RS485.SendFc16(4, 24640, 1, new short[1] { 0 });
            //_RS485.SendFc16(5, 24640, 1, new short[1] { 0 });
            _RS485.SendFc15(6, 0, 16, new short[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        }
        private void Shutdown_Click(object sender, RoutedEventArgs e)
        {
            bn3 = 1 - bn3;
            if (bn3 == 1)
            {
                Wshutdown = true;
                Shutdown.Background = new SolidColorBrush(Color.FromRgb(229, 35, 35));
            }
            else if (bn3 == 0)
            {
                _RS485.Closed();
                Close();
            }
        }
        #region HOME
        private void JhomeX()
        {
            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID1, 24672, 1, new short[1] { -100 });
            _RS485.SendFc16(ID1, 24705, 2, new short[2] { SpeedHomeX, 0 });
            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 63 });
        }
        private void JhomeY1()
        {
            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID2, 24672, 1, new short[1] { -100 });
            _RS485.SendFc16(ID2, 24705, 2, new short[2] { SpeedHomeY1, 0 });
            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 63 });
        }
        private void JhomeY2()
        {
            _RS485.SendFc16(ID3, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID3, 24672, 1, new short[1] { -100 });
            _RS485.SendFc16(ID3, 24705, 2, new short[2] { SpeedHomeY2, 0 });
            _RS485.SendFc16(ID3, 24640, 1, new short[1] { 63 });
        }
        private void JhomeZ1()
        {
            _RS485.SendFc16(ID4, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID4, 24672, 1, new short[1] { -100 });
            _RS485.SendFc16(ID4, 24705, 2, new short[2] { SpeedHomeZ1, 0 });
            _RS485.SendFc16(ID4, 24640, 1, new short[1] { 63 });
        }
        private void JhomeZ2()
        {
            _RS485.SendFc16(ID5, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID5, 24672, 1, new short[1] { -100 });
            _RS485.SendFc16(ID5, 24705, 2, new short[2] { SpeedHomeZ2, 0 });
            _RS485.SendFc16(ID5, 24640, 1, new short[1] { 63 });
        }
        private void home()
        {
            Thread.Sleep(new TimeSpan(10000));
            _RS485.SendFc16(ID1, 24728, 1, new short[1] { 35 });
            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID1, 24672, 1, new short[1] { 6 });
            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 63 });
            Thread.Sleep(new TimeSpan(10000));
            _RS485.SendFc16(ID2, 24728, 1, new short[1] { 35 });
            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID2, 24672, 1, new short[1] { 6 });
            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 63 });
            //_RS485.SendFc16(3, 24728, 1, new short[1] { 35 });
            //_RS485.SendFc16(4, 24728, 1, new short[1] { 35 });
            //_RS485.SendFc16(5, 24728, 1, new short[1] { 35 });
            //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
            //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
            //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });
            //_RS485.SendFc16(3, 24672, 1, new short[1] { 6 });
            //_RS485.SendFc16(4, 24672, 1, new short[1] { 6 });
            //_RS485.SendFc16(5, 24672, 1, new short[1] { 6 });
            //_RS485.SendFc16(3, 24640, 1, new short[1] { 63 });
            //_RS485.SendFc16(4, 24640, 1, new short[1] { 63 });
            //_RS485.SendFc16(5, 24640, 1, new short[1] { 63 });

        }
        private void homex()
        {
            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID1, 24672, 1, new short[1] { -101 });
            _RS485.SendFc16(ID1, 10241, 9, new short[9] { 1, PositioHomeX, 0, SpeedHomeX, AccelHomeX, DecelHomeX, 0, 0, 0 });
            _RS485.SendFc16(ID1, 11616, 1, new short[1] { 1 });
            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 31 });
        }
        private void homey1()
        {
            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID2, 24672, 1, new short[1] { -101 });
            _RS485.SendFc16(ID2, 10241, 9, new short[9] { 1, PositioHomeY1, 0, SpeedHomeY1, AccelHomeY1, DecelHomeY1, 0, 0, 0 });
            _RS485.SendFc16(ID2, 11616, 1, new short[1] { 1 });
            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 31 });
        }
        private void homey2()
        {
            _RS485.SendFc16(ID3, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID3, 24672, 1, new short[1] { -101 });
            _RS485.SendFc16(ID3, 10241, 9, new short[9] { 1, PositioHomeY2, 0, SpeedHomeY2, AccelHomeY2, DecelHomeY2, 0, 0, 0 });
            _RS485.SendFc16(ID3, 11616, 1, new short[1] { 1 });
            _RS485.SendFc16(ID3, 24640, 1, new short[1] { 31 });
        }
        private void homez1()
        {
            _RS485.SendFc16(ID4, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID4, 24672, 1, new short[1] { -101 });
            _RS485.SendFc16(ID4, 10241, 9, new short[9] { 1, PositioHomeZ1, 0, SpeedHomeZ1, AccelHomeZ1, DecelHomeZ1, 0, 0, 0 });
            _RS485.SendFc16(ID4, 11616, 1, new short[1] { 1 });
            _RS485.SendFc16(ID4, 24640, 1, new short[1] { 31 });
        }
        private void homez2()
        {
            _RS485.SendFc16(ID5, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID5, 24672, 1, new short[1] { -101 });
            _RS485.SendFc16(ID5, 10241, 9, new short[9] { 1, PositioHomeZ2, 0, SpeedHomeZ2, AccelHomeZ2, DecelHomeZ2, 0, 0, 0 });
            _RS485.SendFc16(ID5, 11616, 1, new short[1] { 1 });
            _RS485.SendFc16(ID5, 24640, 1, new short[1] { 31 });
        }
        private void HOME()
        {
            JhomeX();
            while (!(AlHome_X[1] == 81))
            {
                _RS485.SendFc3(1, 11055, 2, ref R_Point_X);
                _RS485.SendFc3(1, 10817, 2, ref AlHome_X);
                Dispatcher.BeginInvoke(new Action(delegate
                {
                    Point_X.Content = R_Point_X[0].ToString();
                    Point_XRun.Content = R_Point_X[0].ToString();
                }));
            }
            th = 0;
            while (!(th > 1)) { }
            _RS485.SendFc16(1, 24640, 1, new short[1] { 128 });
            _RS485.SendFc16(1, 24640, 1, new short[1] { 15 });
            th = 0;
            while (!(th > 1)) { }
            home();
            homex();
            while (!(R_Point_X[0] == PositioHomeX))
            {
                _RS485.SendFc3(1, 11055, 2, ref R_Point_X);
                Dispatcher.BeginInvoke(new Action(delegate
                {
                    Point_X.Content = R_Point_X[0].ToString();
                    Point_XRun.Content = R_Point_X[0].ToString();
                }));
            }
            home();
            TimerHome.IsEnabled = false;
            Dispatcher.BeginInvoke(new Action(delegate
            {
                PlayRun.IsEnabled = true;
            }));
        }
        bool Whome = false;
        bool WWhome = false;
        bool WPlayRun = false;
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Whome = true;
            WPlayRun = true;
            TimerHome.IsEnabled = true;
            AlHome_X[1] = 0;
            OpenFileRun.IsEnabled = true;
            StopRun.IsEnabled = false;
            PauseRun.IsEnabled = false;
        }
        #endregion
        #region MANUAL
        bool Wjogoff = false;
        private void JogOFF()
        {
            _RS485.SendFc16(ID1, 24705, 2, new short[2] { 0, 0 });
            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 15 });
            Thread.Sleep(new TimeSpan(10000));
            _RS485.SendFc16(ID2, 24705, 2, new short[2] { 0, 0 });
            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 15 });
            //_RS485.SendFc16(3, 24705, 2, new short[2] { 0, 0 });
            //_RS485.SendFc16(4, 24705, 2, new short[2] { 0, 0 });
            //_RS485.SendFc16(5, 24705, 2, new short[2] { 0, 0 });
            //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
            //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
            //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });
        }
        private void Jog31_X()
        {
            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID1, 24672, 1, new short[1] { -100 });
            _RS485.SendFc16(ID1, 24705, 2, Speed);
            _RS485.SendFc16(ID1, 10241, 9, new short[9] { 0, 0, 0, 0, Accel[0], Decel[0], 0, 0, 0 });
            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 31 });
        }
        private void Jog31_Y1()
        {
            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID2, 24672, 1, new short[1] { -100 });
            _RS485.SendFc16(ID2, 24705, 2, Speed);
            _RS485.SendFc16(ID2, 10241, 9, new short[9] { 0, 0, 0, Speed[0], Accel[0], Decel[0], 0, 0, 0 });
            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 31 });


        }
        private void Jog31_Y2()
        {
            _RS485.SendFc16(ID3, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID3, 24672, 1, new short[1] { -100 });
            _RS485.SendFc16(ID3, 11616, 1, new short[1] { 1 });
            _RS485.SendFc16(ID3, 24705, 2, Speed);
            _RS485.SendFc16(ID3, 10241, 9, new short[9] { 0, 0, 0, 0, Accel[0], Decel[0], 0, 0, 0 });
            _RS485.SendFc16(ID3, 24640, 1, new short[1] { 31 });
        }
        private void Jog31_Z1()
        {
            _RS485.SendFc16(ID4, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID4, 24672, 1, new short[1] { -100 });
            _RS485.SendFc16(ID4, 24705, 2, Speed);
            _RS485.SendFc16(ID4, 10241, 9, new short[9] { 0, 0, 0, 0, Accel[0], Decel[0], 0, 0, 0 });
            _RS485.SendFc16(4, 24640, 1, new short[1] { 31 });
        }
        private void Jog31_Z2()
        {
            _RS485.SendFc16(ID5, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID5, 24672, 1, new short[1] { -100 });
            _RS485.SendFc16(ID5, 24705, 2, Speed);
            _RS485.SendFc16(ID5, 10241, 9, new short[9] { 0, 0, 0, 0, Accel[0], Decel[0], 0, 0, 0 });
            _RS485.SendFc16(ID5, 24640, 1, new short[1] { 31 });
        }
        private void Jog63_X()
        {
            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID1, 24672, 1, new short[1] { -100 });
            _RS485.SendFc16(ID1, 24705, 2, Speed);
            _RS485.SendFc16(ID1, 10241, 9, new short[9] { 0, 0, 0, 0, Accel[0], Decel[0], 0, 0, 0 });
            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 63 });
        }
        private void Jog63_Y1()
        {
            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID2, 24672, 1, new short[1] { -100 });
            _RS485.SendFc16(ID2, 24705, 2, Speed);
            _RS485.SendFc16(ID2, 10241, 9, new short[9] { 0, 0, 0, 0, Accel[0], Decel[0], 0, 0, 0 });
            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 63 });
        }
        private void Jog63_Y2()
        {
            _RS485.SendFc16(ID3, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID3, 24672, 1, new short[1] { -100 });
            _RS485.SendFc16(ID3, 24705, 2, Speed);
            _RS485.SendFc16(ID3, 10241, 9, new short[9] { 0, 0, 0, 0, Accel[0], Decel[0], 0, 0, 0 });
            _RS485.SendFc16(ID3, 24640, 1, new short[1] { 63 });
        }
        private void Jog63_Z1()
        {
            _RS485.SendFc16(ID4, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID4, 24672, 1, new short[1] { -100 });
            _RS485.SendFc16(ID4, 24705, 2, Speed);
            _RS485.SendFc16(ID4, 10241, 9, new short[9] { 0, 0, 0, 0, Accel[0], Decel[0], 0, 0, 0 });
            _RS485.SendFc16(ID4, 24640, 1, new short[1] { 63 });
        }
        private void Jog63_Z2()
        {
            _RS485.SendFc16(ID5, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID5, 24672, 1, new short[1] { -100 });
            _RS485.SendFc16(ID5, 24705, 2, Speed);
            _RS485.SendFc16(ID5, 10241, 9, new short[9] { 0, 0, 0, 0, Accel[0], Decel[0], 0, 0, 0 });
            _RS485.SendFc16(ID5, 24640, 1, new short[1] { 63 });
        }
        bool Wjog31_X = false;
        private void JogX31_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Wjog31_X = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro" + ex);
            }
        }
        private void JogX31_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Wjogoff = true;
        }
        bool Wjog31_Y1 = false;
        private void JogY131_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Wjog31_Y1 = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro" + ex);
            }
        }
        private void JogY131_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Wjogoff = true;
        }
        bool Wjog31_Y2 = false;
        private void JogY231_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Wjog31_Y2 = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro" + ex);
            }
        }
        private void JogY231_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Wjogoff = true;
        }
        bool Wjog31_Z1 = false;
        private void JogZ131_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Wjog31_Z1 = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro" + ex);
            }
        }
        private void JogZ131_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Wjogoff = true;
        }
        bool Wjog31_Z2 = false;
        private void JogZ231_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {

                Wjog31_Z2 = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro" + ex);
            }
        }
        private void JogZ231_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Wjogoff = true;
        }
        bool Wjog63_X = false;
        private void JogX63_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Wjog63_X = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro" + ex);
            }
        }
        private void JogX63_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Wjogoff = true;
        }
        bool Wjog63_Y1 = false;
        private void JogY163_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {

                Wjog63_Y1 = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro" + ex);
            }
        }
        private void JogY163_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Wjogoff = true;
        }
        bool Wjog63_Y2 = false;
        private void JogY263_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {

                Wjog63_Y2 = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro" + ex);
            }
        }
        private void JogY263_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Wjogoff = true;
        }
        bool Wjog63_Z1 = false;
        private void JogZ163_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {

                Wjog63_Z1 = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro" + ex);
            }
        }
        private void JogZ163_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Wjogoff = true;
        }
        bool Wjog63_Z2 = false;
        private void JogZ263_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {

                Wjog63_Z2 = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro" + ex);
            }
        }
        private void JogZ263_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Wjogoff = true;
        }
        private static short[] Get_PointData(int a)
        {
            short[] pointdata = new short[2];
            pointdata[0] = (short)(a & 0xffff);
            pointdata[1] = (short)(a >> 16);
            return pointdata;
        }

        public static int GetIntFromBits_Read(short Lsb, short Msb)
        {
            int varInt = 0;
            int b;

            varInt = 0;
            varInt = Msb << 16;
            b = (Lsb & 0xffff);
            varInt = varInt | (Lsb & 0xffff);

            byte[] byteArray = BitConverter.GetBytes(varInt);
            int intNum = BitConverter.ToInt32(byteArray, 0);
            return intNum;

        }
        private void MoveX31()
        {
            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID1, 24672, 1, new short[1] { -101 });
            _RS485.SendFc16(ID1, 10241, 9, new short[9] { 1, PointX[0], PointX[1], Speed[0], Accel[0], Decel[0], 0, 0, 0 });
            _RS485.SendFc16(ID1, 11616, 1, new short[1] { 1 });
            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 31 });
        }
        private void MoveY131()
        {
            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID2, 24672, 1, new short[1] { -101 });
            _RS485.SendFc16(ID2, 10241, 9, new short[9] { 1, PointY1[0], PointY1[1], Speed[0], Accel[0], Decel[0], 0, 0, 0 });
            _RS485.SendFc16(ID2, 11616, 1, new short[1] { 1 });
            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 31 });
        }
        private void MoveY231()
        {
            _RS485.SendFc16(ID3, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID3, 24672, 1, new short[1] { -101 });
            _RS485.SendFc16(ID3, 10241, 9, new short[9] { 1, PointY2[0], PointY2[1], Speed[0], Accel[0], Decel[0], 0, 0, 0 });
            _RS485.SendFc16(ID3, 11616, 1, new short[1] { 1 });
            _RS485.SendFc16(ID3, 24640, 1, new short[1] { 31 });
        }
        private void MoveZ131()
        {
            _RS485.SendFc16(ID4, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID4, 24672, 1, new short[1] { -101 });
            _RS485.SendFc16(ID4, 10241, 9, new short[9] { 1, PointZ1[0], PointZ1[1], Speed[0], Accel[0], Decel[0], 0, 0, 0 });
            _RS485.SendFc16(ID4, 11616, 1, new short[1] { 1 });
            _RS485.SendFc16(ID4, 24640, 1, new short[1] { 31 });
        }
        private void MoveZ231()
        {
            _RS485.SendFc16(ID5, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(ID5, 24672, 1, new short[1] { -101 });
            _RS485.SendFc16(ID5, 10241, 9, new short[9] { 1, PointZ2[0], PointZ2[1], Speed[0], Accel[0], Decel[0], 0, 0, 0 });
            _RS485.SendFc16(ID5, 11616, 1, new short[1] { 1 });
            _RS485.SendFc16(ID5, 24640, 1, new short[1] { 31 });
        }
        private void MoveX63()
        {
            _RS485.SendFc16(1, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(1, 24672, 1, new short[1] { -101 });
            _RS485.SendFc16(1, 10241, 9, new short[9] { 1, PointX[0], PointX[1], Speed[0], Accel[0], Decel[0], 0, 0, 0 });
            _RS485.SendFc16(1, 11616, 1, new short[1] { 1 });
            _RS485.SendFc16(1, 24640, 1, new short[1] { 31 });
        }
        private void MoveY163()
        {
            _RS485.SendFc16(2, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(2, 24672, 1, new short[1] { -101 });
            _RS485.SendFc16(2, 10241, 9, new short[9] { 1, PointY1[0], PointY1[1], Speed[0], Accel[0], Decel[0], 0, 0, 0 });
            _RS485.SendFc16(2, 11616, 1, new short[1] { 1 });
            _RS485.SendFc16(2, 24640, 1, new short[1] { 31 });
        }
        private void MoveY263()
        {
            _RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(3, 24672, 1, new short[1] { -101 });
            _RS485.SendFc16(3, 10241, 9, new short[9] { 1, PointY2[0], PointY2[1], Speed[0], Accel[0], Decel[0], 0, 0, 0 });
            _RS485.SendFc16(3, 11616, 1, new short[1] { 1 });
            _RS485.SendFc16(3, 24640, 1, new short[1] { 31 });
        }
        private void MoveZ163()
        {
            _RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(4, 24672, 1, new short[1] { -101 });
            _RS485.SendFc16(4, 10241, 9, new short[9] { 1, PointZ1[0], PointZ1[1], Speed[0], Accel[0], Decel[0], 0, 0, 0 });
            _RS485.SendFc16(4, 11616, 1, new short[1] { 1 });
            _RS485.SendFc16(4, 24640, 1, new short[1] { 31 });
        }
        private void MoveZ263()
        {
            _RS485.SendFc16(5, 24640, 1, new short[1] { 15 });
            _RS485.SendFc16(5, 24672, 1, new short[1] { -101 });
            _RS485.SendFc16(5, 10241, 9, new short[9] { 1, PointZ2[0], PointZ2[1], Speed[0], Accel[0], Decel[0], 0, 0, 0 });
            _RS485.SendFc16(5, 11616, 1, new short[1] { 1 });
            _RS485.SendFc16(5, 24640, 1, new short[1] { 31 });
        }
        bool WmoveX31 = false;
        private void PointX31_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                short[] Pointdata = Get_PointData(Convert.ToInt32(Text_PointX.Text));
                PointX[0] = Pointdata[0];
                PointX[1] = Pointdata[1];

                WmoveX31 = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro: " + ex.Message);
            }
        }
        bool WmoveY131 = false;
        private void PointY131_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                short[] Pointdata = Get_PointData(Convert.ToInt32(Text_PointY1.Text));
                PointY1[0] = Pointdata[0];
                PointY1[1] = Pointdata[1];

                WmoveY131 = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro: " + ex.Message);
            }
        }
        bool WmoveY231 = false;
        private void PointY231_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                short[] Pointdata = Get_PointData(Convert.ToInt32(Text_PointY2.Text));
                PointY2[0] = Pointdata[0];
                PointY2[1] = Pointdata[1];

                WmoveY231 = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro: " + ex.Message);
            }
        }
        bool WmoveZ131 = false;
        private void PointZ131_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                short[] Pointdata = Get_PointData(Convert.ToInt32(Text_PointZ1.Text));
                PointZ1[0] = Pointdata[0];
                PointZ1[1] = Pointdata[1];

                WmoveZ131 = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro: " + ex.Message);
            }
        }
        bool WmoveZ231 = false;
        private void PointZ231_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                short[] Pointdata = Get_PointData(Convert.ToInt32(Text_PointZ2.Text));
                PointZ2[0] = Pointdata[0];
                PointZ2[1] = Pointdata[1];

                WmoveZ231 = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro: " + ex.Message);
            }
        }
        private void PointX63_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                short[] Pointdata = Get_PointData((Convert.ToInt32(Text_PointX.Text)) * (-1));
                PointX[0] = Pointdata[0];
                PointX[1] = Pointdata[1];

                WmoveX31 = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro: " + ex.Message);
            }
        }
        private void PointY163_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                short[] Pointdata = Get_PointData((Convert.ToInt32(Text_PointY1.Text)) * (-1));
                PointY1[0] = Pointdata[0];
                PointY1[1] = Pointdata[1];

                WmoveY131 = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro: " + ex.Message);
            }
        }
        private void PointY263_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                short[] Pointdata = Get_PointData((Convert.ToInt32(Text_PointY2.Text)) * (-1));
                PointY2[0] = Pointdata[0];
                PointY2[1] = Pointdata[1];

                WmoveY231 = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro: " + ex.Message);
            }
        }
        private void PointZ163_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                short[] Pointdata = Get_PointData((Convert.ToInt32(Text_PointZ1.Text)) * (-1));
                PointZ1[0] = Pointdata[0];
                PointZ1[1] = Pointdata[1];

                WmoveZ131 = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro: " + ex.Message);
            }
        }
        private void PointZ263_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                short[] Pointdata = Get_PointData((Convert.ToInt32(Text_PointZ2.Text)) * (-1));
                PointZ2[0] = Pointdata[0];
                PointZ2[1] = Pointdata[1];

                WmoveZ231 = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro: " + ex.Message);
            }
        }
        #endregion
        #region IO
        public static short BinaryToShort(string data)
        {
            List<byte> byteList = new List<byte>();
            string strHex = Convert.ToInt32(data, 2).ToString();
            short hex = Convert.ToInt16(strHex);
            return hex;
        }
        string data_write1 = string.Empty;
        string data_Write2 = string.Empty;
        char[] temp1 = new char[8] { '0', '0', '0', '0', '0', '0', '0', '0' };
        char[] temp2 = new char[8] { '0', '0', '0', '0', '0', '0', '0', '0' };
        private void build_data()
        {
            data_write1 = temp1[0].ToString()
                + temp1[1].ToString()
                + temp1[2].ToString()
                + temp1[3].ToString()
                + temp1[4].ToString()
                + temp1[5].ToString()
                + temp1[6].ToString()
                + temp1[7].ToString();
            data_Write2 = temp2[0].ToString()
                + temp2[1].ToString()
                + temp2[2].ToString()
                + temp2[3].ToString()
                + temp2[4].ToString()
                + temp2[5].ToString()
                + temp2[6].ToString()
                + temp2[7].ToString();
        }
        bool Wio = false;


        private void Out0_Click(object sender, RoutedEventArgs e)
        {
            if (Out0.IsChecked == true)
            {
                temp1[7] = '1'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
            else
            {
                temp1[7] = '0'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
        }
        private void Out1_Click(object sender, RoutedEventArgs e)
        {
            if (Out1.IsChecked == true)
            {
                temp1[6] = '1'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
            else
            {
                temp1[6] = '0'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
        }
        private void Out2_Click(object sender, RoutedEventArgs e)
        {
            if (Out2.IsChecked == true)
            {
                temp1[5] = '1'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
            else
            {
                temp1[5] = '0'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
        }
        private void Out3_Click(object sender, RoutedEventArgs e)
        {
            if (Out3.IsChecked == true)
            {
                temp1[4] = '1'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
            else
            {
                temp1[4] = '0'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
        }

        private void Out4_Click(object sender, RoutedEventArgs e)
        {
            if (Out4.IsChecked == true)
            {
                temp1[3] = '1';
                build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
            else
            {
                temp1[3] = '0';
                build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
        }
        private void Out5_Click(object sender, RoutedEventArgs e)
        {
            if (Out5.IsChecked == true)
            {
                temp1[2] = '1'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
            else
            {
                temp1[2] = '0'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
        }
        private void Out6_Click(object sender, RoutedEventArgs e)
        {
            if (Out6.IsChecked == true)
            {
                temp1[1] = '1'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
            else
            {
                temp1[1] = '0'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
        }
        private void Out7_Click(object sender, RoutedEventArgs e)
        {
            if (Out7.IsChecked == true)
            {
                temp1[0] = '1'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
            else
            {
                temp1[0] = '0'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
        }
        private void Out8_Click(object sender, RoutedEventArgs e)
        {
            if (Out8.IsChecked == true)
            {
                temp2[7] = '1'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
            else
            {
                temp2[7] = '0'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
        }
        private void Out9_Click(object sender, RoutedEventArgs e)
        {
            if (Out9.IsChecked == true)
            {
                temp2[6] = '1'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
            else
            {
                temp2[6] = '0'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
        }
        private void Out11_Click(object sender, RoutedEventArgs e)
        {
            if (Out11.IsChecked == true)
            {
                temp2[4] = '1'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
            else
            {
                temp2[4] = '0'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
        }
        private void Out10_Click(object sender, RoutedEventArgs e)
        {
            if (Out10.IsChecked == true)
            {
                temp2[5] = '1'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
            else
            {
                temp2[5] = '0'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
        }
        private void Out12_Click(object sender, RoutedEventArgs e)
        {
            if (Out12.IsChecked == true)
            {
                temp2[3] = '1'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
            else
            {
                temp2[3] = '0'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
        }
        private void Out13_Click(object sender, RoutedEventArgs e)
        {
            if (Out13.IsChecked == true)
            {
                temp2[2] = '1'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
            else
            {
                temp2[2] = '0'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
        }
        private void Out14_Click(object sender, RoutedEventArgs e)
        {
            if (Out14.IsChecked == true)
            {
                temp2[1] = '1'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
            else
            {
                temp2[1] = '0'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
        }
        private void Out15_Click(object sender, RoutedEventArgs e)
        {
            if (Out15.IsChecked == true)
            {
                temp2[0] = '1'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
            else
            {
                temp2[0] = '0'; build_data();
                short[] value = new short[2];
                value[0] = BinaryToShort(data_write1);
                value[1] = BinaryToShort(data_Write2);
                WRITE_IO[0] = value[0];
                WRITE_IO[1] = value[1]; Wio = true;
            }
        }
        #endregion
        #region TEXTPROGRAM
        OpenFileDialog OpenRun = new OpenFileDialog();
        public static bool pressopen = false;
        public static bool newsave = false;

        private void openrun()
        {
            try
            {
                TextRange range;
                FileStream fStream;
                if (OpenRun.ShowDialog() == true)
                {
                    range = new TextRange(RichTextBoxRun.Document.ContentStart, RichTextBoxRun.Document.ContentEnd);
                    fStream = new FileStream(OpenRun.FileName, FileMode.OpenOrCreate);
                    range.Load(fStream, DataFormats.Text);
                    fStream.Close();
                    filenamerun.Content = OpenRun.FileName;
                    int count = 0;
                    using (StreamReader sr = new StreamReader(OpenRun.FileName))
                    {
                        while (sr.ReadLine() != null)
                        {
                            count++;
                        }
                    }
                    using (StreamReader sr = new StreamReader(OpenRun.FileName))
                    {
                        for (int i = 0; i < count; i++)
                        {
                            string point = sr.ReadLine();
                            testrun.Add(point);
                            string[] coordinateAxis = point.Split('.', ':', ' ');
                            Command = coordinateAxis[3];
                            if (Command == "LOOP") SelectLineL(i);
                            if (Command == "ENDLOOP") SelectLineL(i);
                            if (Command == "REPEAT") SelectLineL(i);
                            if (Command == "ENDREPEAT") SelectLineL(i);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void openrun1()
        {
            try
            {
                TextRange range;
                FileStream fStream;

                range = new TextRange(RichTextBoxRun.Document.ContentStart, RichTextBoxRun.Document.ContentEnd);
                fStream = new FileStream(MainViewModel.duongdanrun, FileMode.OpenOrCreate);
                range.Load(fStream, DataFormats.Text);
                fStream.Close();
                filenamerun.Content = MainViewModel.duongdanrun;
                int count = 0;
                using (StreamReader sr = new StreamReader(MainViewModel.duongdanrun))
                {
                    while (sr.ReadLine() != null)
                    {
                        count++;
                    }
                }
                using (StreamReader sr = new StreamReader(MainViewModel.duongdanrun))
                {
                    for (int i = 0; i < count; i++)
                    {
                        string point = sr.ReadLine();
                        testrun.Add(point);
                        string[] coordinateAxis = point.Split('.', ':', ' ');
                        Command = coordinateAxis[3];
                        if (Command == "LOOP") SelectLineL(i);
                        if (Command == "ENDLOOP") SelectLineL(i);
                        if (Command == "REPEAT") SelectLineL(i);
                        if (Command == "ENDREPEAT") SelectLineL(i);
                    }

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //private void OpenFileRun_Click(object sender, RoutedEventArgs e)
        //{
        //    //RichTextBoxRun.IsEnabled = true;
        //    //openrun();
        //}
        private void OpenFileProgram_Click(object sender, RoutedEventArgs e)
        {
            newsave = false;
            pressopen = true;
            Moving.IsEnabled = true;
            Setting.IsEnabled = true;
            Block.IsEnabled = true;
            MoveRobot.IsEnabled = true;
            SetPoint.IsEnabled = true;
            SDelay.IsEnabled = true;
            SAO.IsEnabled = true;
            SDO.IsEnabled = true;
            WDI.IsEnabled = true;
            Bloop.IsEnabled = true;
            Bendloop.IsEnabled = true;
            EndRepeat.IsEnabled = true;
            Reapeat.IsEnabled = true;
        }
        private void NewProgram_Click(object sender, RoutedEventArgs e)
        {
            newsave = true;
            pressopen = false;
            filenameprogram.Content = "NewProgram";
            Moving.IsEnabled = true;
            Setting.IsEnabled = true;
            Block.IsEnabled = true;
            MoveRobot.IsEnabled = true;
            SetPoint.IsEnabled = true;
            SDelay.IsEnabled = true;
            SAO.IsEnabled = true;
            SDO.IsEnabled = true;
            WDI.IsEnabled = true;
            Bloop.IsEnabled = true;
            Bendloop.IsEnabled = true;
            EndRepeat.IsEnabled = true;
            Reapeat.IsEnabled = true;
        }
        private void DeleteTextProgram_Click(object sender, RoutedEventArgs e)
        {
            testl.UpdateLayout();
            testl.Items.Refresh();
        }

        #endregion
        #region READ
        private void ReadTXT()
        {
            do
            {
                if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                {
                    break;
                }
                int count = 0;
                using (StreamReader sr = new StreamReader(MainViewModel.duongdanrun))
                {
                    while (sr.ReadLine() != null)
                    {
                        count++;
                    }
                }
                using (StreamReader sr = new StreamReader(MainViewModel.duongdanrun))
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (Stop == 1)
                        {
                            break;
                        }
                        if (Whome || Wshutdown || Wenable || reset) { Whome = false; Wshutdown = false; Wenable = false; reset = false; break; }
                        string point = sr.ReadLine();
                        string[] coordinateAxis = point.Split('.', ':', ' ');
                        Command = coordinateAxis[3];
                        if (Command == "MOVE")
                        {
                            X = Convert.ToInt16(coordinateAxis[5]);
                            Y1 = Convert.ToInt16(coordinateAxis[7]);
                            Y2 = Convert.ToInt16(coordinateAxis[9]);
                            Z1 = Convert.ToInt16(coordinateAxis[11]);
                            Z2 = Convert.ToInt16(coordinateAxis[13]);
                            SpeedRun[0] = Convert.ToInt16(coordinateAxis[15]);
                            AccelRun[0] = Convert.ToInt16(((SpeedRun[0]) * 40) / 100);
                            DecelRun[0] = Convert.ToInt16(((SpeedRun[0]) * 40) / 100);
                            short[] PointdataRunX = Get_PointData(X);
                            short[] PointdataRunY1 = Get_PointData(Y1);
                            short[] PointdataRunY2 = Get_PointData(Y2);
                            short[] PointdataRunZ1 = Get_PointData(Z1);
                            short[] PointdataRunZ2 = Get_PointData(Z2);
                            PointX[0] = PointdataRunX[0];
                            PointX[1] = PointdataRunX[1];
                            PointY1[0] = PointdataRunY1[0];
                            PointY1[1] = PointdataRunY1[1];
                            PointY2[0] = PointdataRunY2[0];
                            PointY2[1] = PointdataRunY2[1];
                            PointZ1[0] = PointdataRunZ1[0];
                            PointZ1[1] = PointdataRunZ1[1];
                            PointZ2[0] = PointdataRunZ2[0];
                            PointZ2[1] = PointdataRunZ2[1];
                            Dispatcher.Invoke(new Action(delegate
                            {
                                SelectLine(i);
                            }));
                            Thread.Sleep(new TimeSpan(10000));
                            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 15 });
                            _RS485.SendFc16(ID1, 24672, 1, new short[1] { -101 });
                            _RS485.SendFc16(ID1, 10241, 9, new short[9] { 7, PointX[0], PointX[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                            Thread.Sleep(new TimeSpan(10000));
                            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 15 });
                            _RS485.SendFc16(ID2, 24672, 1, new short[1] { -101 });
                            _RS485.SendFc16(ID2, 10241, 9, new short[9] { 7, PointY1[0], PointY1[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 31 });
                            Thread.Sleep(new TimeSpan(10000));
                            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 31 });
                            //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
                            //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
                            //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });

                            //_RS485.SendFc16(3, 24672, 1, new short[1] { -101 });
                            //_RS485.SendFc16(4, 24672, 1, new short[1] { -101 });
                            //_RS485.SendFc16(5, 24672, 1, new short[1] { -101 });


                            //_RS485.SendFc16(3, 10241, 9, new short[9] { 1, PointY2[0], PointY2[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                            //_RS485.SendFc16(4, 10241, 9, new short[9] { 1, PointZ1[0], PointZ1[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                            //_RS485.SendFc16(5, 10241, 9, new short[9] { 1, PointZ2[0], PointZ2[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });




                            //_RS485.SendFc16(3, 11616, 1, new short[1] { 1 });
                            //_RS485.SendFc16(4, 11616, 1, new short[1] { 1 });
                            //_RS485.SendFc16(5, 11616, 1, new short[1] { 1 });

                            //_RS485.SendFc16(3, 24640, 1, new short[1] { 31 });
                            //_RS485.SendFc16(4, 24640, 1, new short[1] { 31 });
                            //_RS485.SendFc16(5, 24640, 1, new short[1] { 31 });
                            while (!(R_Point_X[0] == PointX[0] && R_Point_X[1] == PointX[1]) || !(R_Point_Y1[0] == PointY1[0] && R_Point_Y1[1] == PointY1[1]))// || !(R_Point_Y2[0] == Y2) || !(R_Point_Z1[0] == Z1) || !(R_Point_Z2[0] == Z2))
                            {
                                Thread.Sleep(new TimeSpan(10000));
                                _RS485.SendFc3(ID1, 11055, 2, ref R_Point_X);
                                Thread.Sleep(new TimeSpan(10000));
                                _RS485.SendFc3(ID2, 11055, 2, ref R_Point_Y1);
                                //_RS485.SendFc3(3, 11055, 2, ref R_Point_Y2);
                                //_RS485.SendFc3(4, 11055, 2, ref R_Point_Z1);
                                //_RS485.SendFc3(5, 11055, 2, ref R_Point_Z2);
                                Dispatcher.Invoke(new Action(delegate
                                {
                                    Point_XRun.Content = GetIntFromBits_Read(R_Point_X[0], R_Point_X[1]);
                                    Point_Y1Run.Content = GetIntFromBits_Read(R_Point_Y1[0], R_Point_Y1[1]);
                                    //Point_Y2Run.Content = R_Point_Y2[0].ToString();
                                    //Point_Z1Run.Content = R_Point_Z1[0].ToString();
                                    //Point_Z2Run.Content = R_Point_Z2[0].ToString();
                                    VelocityRun.Content = SpeedRun[0].ToString();
                                }));
                                if (Wshutdown)
                                {
                                    ShutDown();
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        SelectLine1(i);
                                    }));
                                    break;
                                }
                                if (Wenable)
                                {
                                    Enable();
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        SelectLine1(i);
                                    }));
                                    break;
                                }
                                if (reset)
                                {
                                    Reset();
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        SelectLine1(i);
                                    }));
                                    break;
                                }
                                if (Pause == 1 || Stop == 1)
                                {
                                    if (Pause == 1)
                                    {
                                        Thread.Sleep(new TimeSpan(10000));
                                        _RS485.SendFc16(ID1, 24640, 1, new short[1] { 271 });
                                        Thread.Sleep(new TimeSpan(10000));
                                        _RS485.SendFc16(ID2, 24640, 1, new short[1] { 271 });
                                        //_RS485.SendFc16(3, 24640, 1, new short[1] { 271 });
                                        //_RS485.SendFc16(4, 24640, 1, new short[1] { 271 });
                                        //_RS485.SendFc16(5, 24640, 1, new short[1] { 271 });
                                        while (Pause == 1)
                                        {
                                            Thread.Sleep(new TimeSpan(10000));
                                            _RS485.SendFc3(ID1, 11055, 2, ref R_Point_X);
                                            Thread.Sleep(new TimeSpan(10000));
                                            _RS485.SendFc3(ID2, 11055, 2, ref R_Point_Y1);
                                            //_RS485.SendFc3(3, 11055, 2, ref R_Point_Y2);
                                            //_RS485.SendFc3(4, 11055, 2, ref R_Point_Z1);
                                            //_RS485.SendFc3(5, 11055, 2, ref R_Point_Z2);
                                            Dispatcher.Invoke(new Action(delegate
                                            {
                                                Point_XRun.Content = GetIntFromBits_Read(R_Point_X[0], R_Point_X[1]);
                                                Point_Y1Run.Content = GetIntFromBits_Read(R_Point_Y1[0], R_Point_Y1[1]);
                                                //Point_Y2Run.Content = R_Point_Y2[0].ToString();
                                                //Point_Z1Run.Content = R_Point_Z1[0].ToString();
                                                //Point_Z2Run.Content = R_Point_Z2[0].ToString();
                                            }));
                                            if (Pause == 0)
                                            {
                                                Thread.Sleep(new TimeSpan(10000));
                                                _RS485.SendFc16(ID1, 24640, 1, new short[1] { 31 });
                                                Thread.Sleep(new TimeSpan(10000));
                                                _RS485.SendFc16(ID2, 24640, 1, new short[1] { 31 });
                                                //_RS485.SendFc16(3, 24640, 1, new short[1] { 31 });
                                                //_RS485.SendFc16(4, 24640, 1, new short[1] { 31 });
                                                //_RS485.SendFc16(5, 24640, 1, new short[1] { 31 });
                                            }
                                            if (Stop == 1)
                                            {
                                                Thread.Sleep(new TimeSpan(10000));
                                                _RS485.SendFc16(ID1, 24640, 1, new short[1] { 0 });
                                                _RS485.SendFc16(ID1, 24640, 1, new short[1] { 15 });
                                                Thread.Sleep(new TimeSpan(10000));
                                                _RS485.SendFc16(ID2, 24640, 1, new short[1] { 0 });
                                                _RS485.SendFc16(ID2, 24640, 1, new short[1] { 15 });
                                                //_RS485.SendFc16(3, 24640, 1, new short[1] { 0 });
                                                //_RS485.SendFc16(4, 24640, 1, new short[1] { 0 });
                                                //_RS485.SendFc16(5, 24640, 1, new short[1] { 0 });
                                                //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
                                                //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
                                                //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(i);
                                                }));
                                                break;
                                            }
                                            if (Whome == true)
                                            {
                                                HOME();
                                                break;
                                            }
                                            if (Wshutdown)
                                            {
                                                ShutDown();
                                                break;
                                            }
                                            if (Wenable)
                                            {
                                                Enable();
                                                break;
                                            }
                                            if (reset)
                                            {
                                                Reset();
                                                break;
                                            }
                                        }
                                    }
                                    if (Stop == 1)
                                    {
                                        Thread.Sleep(new TimeSpan(10000));
                                        _RS485.SendFc16(ID1, 24640, 1, new short[1] { 0 });
                                        _RS485.SendFc16(ID1, 24640, 1, new short[1] { 15 });
                                        Thread.Sleep(new TimeSpan(10000));
                                        _RS485.SendFc16(ID2, 24640, 1, new short[1] { 0 });
                                        _RS485.SendFc16(ID2, 24640, 1, new short[1] { 15 });
                                        //_RS485.SendFc16(3, 24640, 1, new short[1] { 0 });
                                        //_RS485.SendFc16(4, 24640, 1, new short[1] { 0 });
                                        //_RS485.SendFc16(5, 24640, 1, new short[1] { 0 });


                                        //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
                                        //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
                                        //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });
                                        Dispatcher.Invoke(new Action(delegate
                                        {
                                            SelectLine1(i);
                                        }));
                                        break;
                                    }
                                    if (Whome == true || Wshutdown || Wenable || reset)
                                    {
                                        Dispatcher.Invoke(new Action(delegate
                                        {
                                            SelectLine1(i);
                                        }));
                                        break;
                                    }
                                }
                            }
                            if (R_Point_X[0] == PointX[0] && R_Point_X[1] == PointX[1] && R_Point_Y1[0] == PointY1[0] && R_Point_Y1[1] == PointY1[1])// && R_Point_Y2[0] == Y2 && R_Point_Z1[0] == Z1 && R_Point_Z2[0] == Z2)
                            {
                                Dispatcher.Invoke(new Action(delegate
                                {
                                    SelectLine1(i);
                                }));
                            }
                        }
                        else if (Command == "LOOP")
                        {

                            do
                            {
                                if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                {
                                    break;
                                }

                                using (StreamReader sr1 = new StreamReader(MainViewModel.duongdanrun))
                                {
                                    for (int j = 0; j < count; j++)
                                    {
                                        string point1 = sr1.ReadLine();
                                        if (j > i)
                                        {
                                            if (Whome || Stop == 1 || Wshutdown || Wenable || reset)
                                            {
                                                break;
                                            }

                                            string[] coordinateAxis1 = point1.Split('.', ':', ' ');
                                            Command1 = coordinateAxis1[3];
                                            if (Command1 == "MOVE")
                                            {
                                                X = Convert.ToInt16(coordinateAxis1[5]);
                                                Y1 = Convert.ToInt16(coordinateAxis1[7]);
                                                Y2 = Convert.ToInt16(coordinateAxis1[9]);
                                                Z1 = Convert.ToInt16(coordinateAxis1[11]);
                                                Z2 = Convert.ToInt16(coordinateAxis1[13]);
                                                SpeedRun[0] = Convert.ToInt16(coordinateAxis1[15]);
                                                AccelRun[0] = Convert.ToInt16(((SpeedRun[0]) * 40) / 100);
                                                DecelRun[0] = Convert.ToInt16(((SpeedRun[0]) * 40) / 100);
                                                short[] PointdataRunX = Get_PointData(X);
                                                short[] PointdataRunY1 = Get_PointData(Y1);
                                                short[] PointdataRunY2 = Get_PointData(Y2);
                                                short[] PointdataRunZ1 = Get_PointData(Z1);
                                                short[] PointdataRunZ2 = Get_PointData(Z2);
                                                PointX[0] = PointdataRunX[0];
                                                PointX[1] = PointdataRunX[1];
                                                PointY1[0] = PointdataRunY1[0];
                                                PointY1[1] = PointdataRunY1[1];
                                                PointY2[0] = PointdataRunY2[0];
                                                PointY2[1] = PointdataRunY2[1];
                                                PointZ1[0] = PointdataRunZ1[0];
                                                PointZ1[1] = PointdataRunZ1[1];
                                                PointZ2[0] = PointdataRunZ2[0];
                                                PointZ2[1] = PointdataRunZ2[1];
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine(j);
                                                }));
                                                Thread.Sleep(new TimeSpan(10000));
                                                _RS485.SendFc16(ID1, 24640, 1, new short[1] { 15 });
                                                _RS485.SendFc16(ID1, 24672, 1, new short[1] { -101 });
                                                _RS485.SendFc16(ID1, 10241, 9, new short[9] { 7, PointX[0], PointX[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                                                Thread.Sleep(new TimeSpan(10000));
                                                _RS485.SendFc16(ID2, 24640, 1, new short[1] { 15 });
                                                _RS485.SendFc16(ID2, 24672, 1, new short[1] { -101 });
                                                _RS485.SendFc16(ID2, 10241, 9, new short[9] { 7, PointY1[0], PointY1[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                                                _RS485.SendFc16(ID2, 24640, 1, new short[1] { 31 });
                                                Thread.Sleep(new TimeSpan(10000));
                                                _RS485.SendFc16(ID1, 24640, 1, new short[1] { 31 });
                                                //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
                                                //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
                                                //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });


                                                // _RS485.SendFc16(1, 24672, 1, new short[1] { -101 });
                                                // _RS485.SendFc16(2, 24672, 1, new short[1] { -101 });
                                                //_RS485.SendFc16(3, 24672, 1, new short[1] { -101 });
                                                //_RS485.SendFc16(4, 24672, 1, new short[1] { -101 });
                                                //_RS485.SendFc16(5, 24672, 1, new short[1] { -101 });




                                                //_RS485.SendFc16(3, 10241, 9, new short[9] { 1, PointY2[0], PointY2[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                                                //_RS485.SendFc16(4, 10241, 9, new short[9] { 1, PointZ1[0], PointZ1[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                                                //_RS485.SendFc16(5, 10241, 9, new short[9] { 1, PointZ2[0], PointZ2[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });

                                                //  _RS485.SendFc16(1, 11616, 1, new short[1] { 1 });
                                                //  _RS485.SendFc16(2, 11616, 1, new short[1] { 1 });



                                                //_RS485.SendFc16(3, 11616, 1, new short[1] { 1 });
                                                //_RS485.SendFc16(4, 11616, 1, new short[1] { 1 });
                                                //_RS485.SendFc16(5, 11616, 1, new short[1] { 1 });

                                                //_RS485.SendFc16(3, 24640, 1, new short[1] { 31 });
                                                //_RS485.SendFc16(4, 24640, 1, new short[1] { 31 });
                                                //_RS485.SendFc16(5, 24640, 1, new short[1] { 31 });
                                                while (!(R_Point_X[0] == PointX[0] && R_Point_X[1] == PointX[1]) || !(R_Point_Y1[0] == PointY1[0] && R_Point_Y1[1] == PointY1[1]))// || !(R_Point_Y2[0] == Y2) || !(R_Point_Z1[0] == Z1) || !(R_Point_Z2[0] == Z2))
                                                {
                                                    Thread.Sleep(new TimeSpan(10000));
                                                    _RS485.SendFc3(ID1, 11055, 2, ref R_Point_X);
                                                    Thread.Sleep(new TimeSpan(10000));
                                                    _RS485.SendFc3(ID2, 11055, 2, ref R_Point_Y1);

                                                    //_RS485.SendFc3(3, 11055, 2, ref R_Point_Y2);
                                                    //_RS485.SendFc3(4, 11055, 2, ref R_Point_Z1);
                                                    //_RS485.SendFc3(5, 11055, 2, ref R_Point_Z2);
                                                    Dispatcher.Invoke(new Action(delegate
                                                    {
                                                        Point_XRun.Content = GetIntFromBits_Read(R_Point_X[0], R_Point_X[1]);
                                                        Point_Y1Run.Content = GetIntFromBits_Read(R_Point_Y1[0], R_Point_Y1[1]);
                                                        //Point_Y2Run.Content = R_Point_Y2[0].ToString();
                                                        //Point_Z1Run.Content = R_Point_Z1[0].ToString();
                                                        //Point_Z2Run.Content = R_Point_Z2[0].ToString();
                                                        VelocityRun.Content = SpeedRun[0].ToString();
                                                    }));
                                                    if (Wshutdown)
                                                    {
                                                        ShutDown();
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            SelectLine1(j);
                                                        }));
                                                        break;
                                                    }
                                                    if (Wenable)
                                                    {
                                                        Enable();
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            SelectLine1(j);
                                                        }));
                                                        break;
                                                    }
                                                    if (reset)
                                                    {
                                                        Reset();
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            SelectLine1(j);
                                                        }));
                                                        break;
                                                    }
                                                    if (Pause == 1 || Stop == 1)
                                                    {
                                                        if (Pause == 1)
                                                        {
                                                            Thread.Sleep(new TimeSpan(10000));
                                                            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 271 });
                                                            Thread.Sleep(new TimeSpan(10000));
                                                            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 271 });
                                                            //_RS485.SendFc16(3, 24640, 1, new short[1] { 271 });
                                                            //_RS485.SendFc16(4, 24640, 1, new short[1] { 271 });
                                                            //_RS485.SendFc16(5, 24640, 1, new short[1] { 271 });
                                                            while (Pause == 1)
                                                            {
                                                                Thread.Sleep(new TimeSpan(10000));
                                                                _RS485.SendFc3(ID1, 11055, 2, ref R_Point_X);
                                                                Thread.Sleep(new TimeSpan(10000));
                                                                _RS485.SendFc3(ID2, 11055, 2, ref R_Point_Y1);
                                                                //_RS485.SendFc3(3, 11055, 2, ref R_Point_Y2);
                                                                //_RS485.SendFc3(4, 11055, 2, ref R_Point_Z1);
                                                                //_RS485.SendFc3(5, 11055, 2, ref R_Point_Z2);
                                                                Dispatcher.Invoke(new Action(delegate
                                                                {
                                                                    Point_XRun.Content = GetIntFromBits_Read(R_Point_X[0], R_Point_X[1]);
                                                                    Point_Y1Run.Content = GetIntFromBits_Read(R_Point_Y1[0], R_Point_Y1[1]);
                                                                    //Point_Y2Run.Content = R_Point_Y2[0].ToString();
                                                                    //Point_Z1Run.Content = R_Point_Z1[0].ToString();
                                                                    //Point_Z2Run.Content = R_Point_Z2[0].ToString();
                                                                }));
                                                                if (Pause == 0)
                                                                {
                                                                    Thread.Sleep(new TimeSpan(10000));
                                                                    _RS485.SendFc16(ID1, 24640, 1, new short[1] { 31 });
                                                                    Thread.Sleep(new TimeSpan(10000));
                                                                    _RS485.SendFc16(ID2, 24640, 1, new short[1] { 31 });
                                                                    //_RS485.SendFc16(3, 24640, 1, new short[1] { 31 });
                                                                    //_RS485.SendFc16(4, 24640, 1, new short[1] { 31 });
                                                                    //_RS485.SendFc16(5, 24640, 1, new short[1] { 31 });
                                                                }
                                                                else if (Stop == 1)
                                                                {
                                                                    Thread.Sleep(new TimeSpan(10000));
                                                                    _RS485.SendFc16(ID1, 24640, 1, new short[1] { 0 });
                                                                    _RS485.SendFc16(ID1, 24640, 1, new short[1] { 15 });
                                                                    Thread.Sleep(new TimeSpan(10000));
                                                                    _RS485.SendFc16(ID2, 24640, 1, new short[1] { 0 });
                                                                    _RS485.SendFc16(ID2, 24640, 1, new short[1] { 15 });
                                                                    //_RS485.SendFc16(3, 24640, 1, new short[1] { 0 });
                                                                    //_RS485.SendFc16(4, 24640, 1, new short[1] { 0 });
                                                                    //_RS485.SendFc16(5, 24640, 1, new short[1] { 0 });


                                                                    //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
                                                                    //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
                                                                    //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });
                                                                    Dispatcher.Invoke(new Action(delegate
                                                                    {
                                                                        SelectLine1(j);
                                                                    }));
                                                                    break;
                                                                }
                                                                if (Whome)
                                                                {
                                                                    HOME();
                                                                    break;
                                                                }
                                                                else if (Wshutdown)
                                                                {
                                                                    ShutDown();
                                                                    break;
                                                                }
                                                                else if (Wenable)
                                                                {
                                                                    Enable();
                                                                    break;
                                                                }
                                                                else if (reset)
                                                                {
                                                                    Reset();
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        if (Stop == 1)
                                                        {
                                                            Thread.Sleep(new TimeSpan(10000));
                                                            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 0 });
                                                            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 15 });
                                                            Thread.Sleep(new TimeSpan(10000));
                                                            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 0 });
                                                            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 15 });
                                                            //_RS485.SendFc16(3, 24640, 1, new short[1] { 0 });
                                                            //_RS485.SendFc16(4, 24640, 1, new short[1] { 0 });
                                                            //_RS485.SendFc16(5, 24640, 1, new short[1] { 0 });


                                                            //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
                                                            //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
                                                            //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (Whome || Wshutdown || Wenable || reset)
                                                        {
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                    }
                                                }
                                                if (R_Point_X[0] == PointX[0] && R_Point_X[1] == PointX[1] && R_Point_Y1[0] == PointY1[0] && R_Point_Y1[1] == PointY1[1])// && R_Point_Y2[0] == Y2 && R_Point_Z1[0] == Z1 && R_Point_Z2[0] == Z2)
                                                {
                                                    Dispatcher.Invoke(new Action(delegate
                                                    {
                                                        SelectLine1(j);
                                                    }));
                                                }
                                            }
                                            else if (Command1 == "DELAY")
                                            {
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine(j);
                                                }));
                                                Delay = Convert.ToInt16(coordinateAxis1[4]);
                                                t = 0;
                                                T = true;
                                                while (!(t >= Delay))
                                                {
                                                    if (Wshutdown)
                                                    {
                                                        ShutDown();
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            SelectLine1(j);
                                                        }));
                                                        break;
                                                    }
                                                    if (Wenable)
                                                    {
                                                        Enable();
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            SelectLine1(j);
                                                        }));
                                                        break;
                                                    }
                                                    if (reset)
                                                    {
                                                        Reset();
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            SelectLine1(j);
                                                        }));
                                                        break;
                                                    }

                                                    if (Pause == 1 || Stop == 1)
                                                    {
                                                        if (Pause == 1)
                                                        {
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                Timerdelay.Content = t.ToString();
                                                            }));
                                                            while (Pause == 1)
                                                            {
                                                                if (Stop == 1)
                                                                {
                                                                    break;
                                                                }
                                                                if (Whome)
                                                                {
                                                                    HOME();
                                                                    break;
                                                                }
                                                                if (Wshutdown)
                                                                {
                                                                    ShutDown();
                                                                    break;
                                                                }
                                                                if (Wenable)
                                                                {
                                                                    Enable();
                                                                    break;
                                                                }
                                                                if (reset)
                                                                {
                                                                    Reset();
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                        {
                                                            break;
                                                        }
                                                    }
                                                }
                                                t = 0;
                                                T = false;
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                            }
                                            else if (Command1 == "SDOON")
                                            {
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine(j);
                                                }));
                                                SDOON = Convert.ToInt16(coordinateAxis1[4]);
                                                #region ON
                                                if (SDOON == 0)
                                                {
                                                    temp1[7] = '1';
                                                }
                                                if (SDOON == 1)
                                                {
                                                    temp1[6] = '1';
                                                }
                                                if (SDOON == 2)
                                                {
                                                    temp1[5] = '1';
                                                }
                                                if (SDOON == 3)
                                                {
                                                    temp1[4] = '1';
                                                }
                                                if (SDOON == 4)
                                                {
                                                    temp1[3] = '1';
                                                }
                                                if (SDOON == 5)
                                                {
                                                    temp1[2] = '1';
                                                }
                                                if (SDOON == 6)
                                                {
                                                    temp1[1] = '1';
                                                }
                                                if (SDOON == 7)
                                                {
                                                    temp1[0] = '1';
                                                }
                                                if (SDOON == 8)
                                                {
                                                    temp2[7] = '1';
                                                }
                                                if (SDOON == 9)
                                                {
                                                    temp2[6] = '1';
                                                }
                                                if (SDOON == 10)
                                                {
                                                    temp2[5] = '1';
                                                }
                                                if (SDOON == 11)
                                                {
                                                    temp2[4] = '1';
                                                }
                                                if (SDOON == 12)
                                                {
                                                    temp2[3] = '1';
                                                }
                                                if (SDOON == 13)
                                                {
                                                    temp2[2] = '1';
                                                }
                                                if (SDOON == 14)
                                                {
                                                    temp2[1] = '1';
                                                }
                                                if (SDOON == 15)
                                                {
                                                    temp2[0] = '1';
                                                }
                                                #endregion
                                                build_data();
                                                short[] value = new short[2];
                                                value[0] = BinaryToShort(data_write1);
                                                value[1] = BinaryToShort(data_Write2);
                                                WRITE_IO[0] = value[0];
                                                WRITE_IO[1] = value[1];
                                                Thread.Sleep(new TimeSpan(10000));
                                                _RS485.SendFc15(6, 0, 16, WRITE_IO);
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                            }
                                            else if (Command1 == "SDOOF")
                                            {
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine(j);
                                                }));
                                                SDOOF = Convert.ToInt16(coordinateAxis1[4]);
                                                #region OF
                                                if (SDOOF == 0)
                                                {
                                                    temp1[7] = '0';
                                                }
                                                if (SDOOF == 1)
                                                {
                                                    temp1[6] = '0';
                                                }
                                                if (SDOOF == 2)
                                                {
                                                    temp1[5] = '0';
                                                }
                                                if (SDOOF == 3)
                                                {
                                                    temp1[4] = '0';
                                                }
                                                if (SDOOF == 4)
                                                {
                                                    temp1[3] = '0';
                                                }
                                                if (SDOOF == 5)
                                                {
                                                    temp1[2] = '0';
                                                }
                                                if (SDOOF == 6)
                                                {
                                                    temp1[1] = '0';
                                                }
                                                if (SDOOF == 7)
                                                {
                                                    temp1[0] = '0';
                                                }
                                                if (SDOOF == 8)
                                                {
                                                    temp2[7] = '0';
                                                }
                                                if (SDOOF == 9)
                                                {
                                                    temp2[6] = '0';
                                                }
                                                if (SDOOF == 10)
                                                {
                                                    temp2[5] = '0';
                                                }
                                                if (SDOOF == 11)
                                                {
                                                    temp2[4] = '0';
                                                }
                                                if (SDOOF == 12)
                                                {
                                                    temp2[3] = '0';
                                                }
                                                if (SDOOF == 13)
                                                {
                                                    temp2[2] = '0';
                                                }
                                                if (SDOOF == 14)
                                                {
                                                    temp2[1] = '0';
                                                }
                                                if (SDOOF == 15)
                                                {
                                                    temp2[0] = '0';
                                                }
                                                #endregion
                                                build_data();
                                                short[] value = new short[2];
                                                value[0] = BinaryToShort(data_write1);
                                                value[1] = BinaryToShort(data_Write2);
                                                WRITE_IO[0] = value[0];
                                                WRITE_IO[1] = value[1];
                                                Thread.Sleep(new TimeSpan(10000));
                                                _RS485.SendFc15(6, 0, 16, WRITE_IO);
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                            }
                                            else if (Command1 == "WDION")
                                            {
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine(j);
                                                }));
                                                WDION = Convert.ToInt16(coordinateAxis1[4]);
                                                Thread.Sleep(new TimeSpan(10000));
                                                _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    var output1 = Convert.ToString(READ_IO[0], 2);
                                                    var output2 = Convert.ToString(READ_IO[1], 2);

                                                    string out1 = output1.PadLeft(8, '0');
                                                    string out2 = output2.PadLeft(8, '0');
                                                    for (int O = 0; O < 8; O++)
                                                    {
                                                        out_put1[O] = out1[O];
                                                        out_put2[O] = out2[O];
                                                    }
                                                }));
                                                if (Stop == 1 || Pause == 1)
                                                {
                                                    while (Pause == 1)
                                                    {
                                                        if (Stop == 1)
                                                        {
                                                            break;
                                                        }
                                                        if (Whome)
                                                        {
                                                            HOME();
                                                            break;
                                                        }
                                                        if (Wshutdown)
                                                        {
                                                            ShutDown();
                                                            break;
                                                        }
                                                        if (Wenable)
                                                        {
                                                            Enable();
                                                            break;
                                                        }
                                                        if (reset)
                                                        {
                                                            Reset();
                                                            break;
                                                        }
                                                    }
                                                    if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                    {
                                                        break;
                                                    }
                                                }
                                                if (WDION <= 7)
                                                {
                                                    if (WDION == 7) WDION = 0;
                                                    else if (WDION == 6) WDION = 1;
                                                    else if (WDION == 5) WDION = 2;
                                                    else if (WDION == 4) WDION = 3;
                                                    else if (WDION == 3) WDION = 4;
                                                    else if (WDION == 2) WDION = 5;
                                                    else if (WDION == 1) WDION = 6;
                                                    else if (WDION == 0) WDION = 7;
                                                    while (!(out_put1[WDION] == '1'))
                                                    {
                                                        Thread.Sleep(new TimeSpan(10000));
                                                        _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            var output1 = Convert.ToString(READ_IO[0], 2);
                                                            string out1 = output1.PadLeft(8, '0');
                                                            for (int O = 0; O < 8; O++)
                                                            {
                                                                out_put1[O] = out1[O];
                                                            }
                                                        }));
                                                        if (Wshutdown)
                                                        {
                                                            ShutDown();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (Wenable)
                                                        {
                                                            Enable();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (reset)
                                                        {
                                                            Reset();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (Stop == 1 || Pause == 1)
                                                        {
                                                            while (Pause == 1)
                                                            {
                                                                if (Stop == 1)
                                                                {
                                                                    break;
                                                                }
                                                                if (Whome)
                                                                {
                                                                    HOME();
                                                                    break;
                                                                }
                                                                if (Wshutdown)
                                                                {
                                                                    ShutDown();
                                                                    break;
                                                                }
                                                                if (Wenable)
                                                                {
                                                                    Enable();
                                                                    break;
                                                                }
                                                                if (reset)
                                                                {
                                                                    Reset();
                                                                    break;
                                                                }
                                                            }
                                                            if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                            {
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                if (WDION > 7)
                                                {
                                                    if (WDION == 15) WDION = 0;
                                                    else if (WDION == 14) WDION = 1;
                                                    else if (WDION == 13) WDION = 2;
                                                    else if (WDION == 12) WDION = 3;
                                                    else if (WDION == 11) WDION = 4;
                                                    else if (WDION == 10) WDION = 5;
                                                    else if (WDION == 9) WDION = 6;
                                                    else if (WDION == 8) WDION = 7;
                                                    while (!(out_put2[WDION] == '1'))
                                                    {
                                                        Thread.Sleep(new TimeSpan(10000));
                                                        _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {

                                                            var output2 = Convert.ToString(READ_IO[1], 2);


                                                            string out2 = output2.PadLeft(8, '0');
                                                            for (int O = 0; O < 8; O++)
                                                            {

                                                                out_put2[O] = out2[O];
                                                            }
                                                        }));
                                                        if (Wshutdown)
                                                        {
                                                            ShutDown();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (Wenable)
                                                        {
                                                            Enable();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (reset)
                                                        {
                                                            Reset();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (Stop == 1 || Pause == 1)
                                                        {
                                                            while (Pause == 1)
                                                            {
                                                                if (Stop == 1)
                                                                {
                                                                    break;
                                                                }
                                                                if (Whome)
                                                                {
                                                                    HOME();
                                                                    break;
                                                                }
                                                                if (Wshutdown)
                                                                {
                                                                    ShutDown();
                                                                    break;
                                                                }
                                                                if (Wenable)
                                                                {
                                                                    Enable();
                                                                    break;
                                                                }
                                                                if (reset)
                                                                {
                                                                    Reset();
                                                                    break;
                                                                }
                                                            }
                                                            if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                            {
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                            }
                                            else if (Command1 == "WDIOF")
                                            {
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine(j);
                                                }));
                                                WDIOF = Convert.ToInt16(coordinateAxis1[4]);
                                                Thread.Sleep(new TimeSpan(10000));
                                                _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    var output1 = Convert.ToString(READ_IO[0], 2);
                                                    var output2 = Convert.ToString(READ_IO[1], 2);

                                                    string out1 = output1.PadLeft(8, '0');
                                                    string out2 = output2.PadLeft(8, '0');
                                                    for (int O = 0; O < 8; O++)
                                                    {
                                                        out_put1[O] = out1[O];
                                                        out_put2[O] = out2[O];
                                                    }
                                                }));
                                                if (Stop == 1 || Pause == 1)
                                                {
                                                    while (Pause == 1)
                                                    {
                                                        if (Stop == 1)
                                                        {
                                                            break;
                                                        }
                                                        if (Whome)
                                                        {
                                                            HOME();
                                                            break;
                                                        }
                                                        if (Wshutdown)
                                                        {
                                                            ShutDown();
                                                            break;
                                                        }
                                                        if (Wenable)
                                                        {
                                                            Enable();
                                                            break;
                                                        }
                                                        if (reset)
                                                        {
                                                            Reset();
                                                            break;
                                                        }
                                                    }
                                                    if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                    {
                                                        break;
                                                    }
                                                }
                                                if (WDIOF <= 7)
                                                {
                                                    if (WDIOF == 7) WDIOF = 0;
                                                    else if (WDIOF == 6) WDIOF = 1;
                                                    else if (WDIOF == 5) WDIOF = 2;
                                                    else if (WDIOF == 4) WDIOF = 3;
                                                    else if (WDIOF == 3) WDIOF = 4;
                                                    else if (WDIOF == 2) WDIOF = 5;
                                                    else if (WDIOF == 1) WDIOF = 6;
                                                    else if (WDIOF == 0) WDIOF = 7;
                                                    while (!(out_put1[WDIOF] == '0'))
                                                    {
                                                        Thread.Sleep(new TimeSpan(10000));
                                                        _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            var output1 = Convert.ToString(READ_IO[0], 2);
                                                            string out1 = output1.PadLeft(8, '0');
                                                            for (int O = 0; O < 8; O++)
                                                            {
                                                                out_put1[O] = out1[O];
                                                            }
                                                        }));
                                                        if (Wshutdown)
                                                        {
                                                            ShutDown();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (Wenable)
                                                        {
                                                            Enable();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (reset)
                                                        {
                                                            Reset();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (Stop == 1 || Pause == 1)
                                                        {
                                                            while (Pause == 1)
                                                            {
                                                                if (Stop == 1)
                                                                {
                                                                    break;
                                                                }
                                                                if (Whome)
                                                                {
                                                                    HOME();
                                                                    break;
                                                                }
                                                                if (Wshutdown)
                                                                {
                                                                    ShutDown();
                                                                    break;
                                                                }
                                                                if (Wenable)
                                                                {
                                                                    Enable();
                                                                    break;
                                                                }
                                                                if (reset)
                                                                {
                                                                    Reset();
                                                                    break;
                                                                }
                                                            }
                                                            if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                            {
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                if (WDIOF > 7)
                                                {
                                                    if (WDIOF == 15) WDIOF = 0;
                                                    else if (WDIOF == 14) WDIOF = 1;
                                                    else if (WDIOF == 13) WDIOF = 2;
                                                    else if (WDIOF == 12) WDIOF = 3;
                                                    else if (WDIOF == 11) WDIOF = 4;
                                                    else if (WDIOF == 10) WDIOF = 5;
                                                    else if (WDIOF == 9) WDIOF = 6;
                                                    else if (WDIOF == 8) WDIOF = 7;
                                                    while (!(out_put2[WDIOF] == '0'))
                                                    {
                                                        Thread.Sleep(new TimeSpan(10000));
                                                        _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            var output2 = Convert.ToString(READ_IO[1], 2);
                                                            string out2 = output2.PadLeft(8, '0');
                                                            for (int O = 0; O < 8; O++)
                                                            {
                                                                out_put2[O] = out2[O];
                                                            }
                                                        }));
                                                        if (Wshutdown)
                                                        {
                                                            ShutDown();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (Wenable)
                                                        {
                                                            Enable();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (reset)
                                                        {
                                                            Reset();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (Stop == 1 || Pause == 1)
                                                        {
                                                            while (Pause == 1)
                                                            {
                                                                if (Stop == 1)
                                                                {
                                                                    break;
                                                                }
                                                                if (Whome)
                                                                {
                                                                    HOME();
                                                                    break;
                                                                }
                                                                if (Wshutdown)
                                                                {
                                                                    ShutDown();
                                                                    break;
                                                                }
                                                                if (Wenable)
                                                                {
                                                                    Enable();
                                                                    break;
                                                                }
                                                                if (reset)
                                                                {
                                                                    Reset();
                                                                    break;
                                                                }
                                                            }
                                                            if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                            {
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                            }
                                            else if (Command1 == "REPEAT")
                                            {

                                                Int16 Times = Convert.ToInt16(coordinateAxis1[4]);
                                                Int16 times = 0;
                                                do
                                                {
                                                    if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                    {
                                                        break;
                                                    }

                                                    using (StreamReader sr2 = new StreamReader(MainViewModel.duongdanrun))
                                                    {
                                                        for (int k = 0; k < count; k++)
                                                        {
                                                            string point2 = sr2.ReadLine();
                                                            if (k > j)
                                                            {
                                                                if (Whome || Stop == 1 || Wshutdown || Wenable || reset)
                                                                {
                                                                    break;
                                                                }
                                                                string[] coordinateAxis2 = point2.Split('.', ':', ' ');
                                                                Command2 = coordinateAxis2[3];

                                                                if (Command2 == "MOVE")
                                                                {

                                                                    X = Convert.ToInt16(coordinateAxis2[5]);
                                                                    Y1 = Convert.ToInt16(coordinateAxis2[7]);
                                                                    Y2 = Convert.ToInt16(coordinateAxis2[9]);
                                                                    Z1 = Convert.ToInt16(coordinateAxis2[11]);
                                                                    Z2 = Convert.ToInt16(coordinateAxis2[13]);
                                                                    SpeedRun[0] = Convert.ToInt16(coordinateAxis2[15]);
                                                                    AccelRun[0] = Convert.ToInt16(((SpeedRun[0]) * 40) / 100);
                                                                    DecelRun[0] = Convert.ToInt16(((SpeedRun[0]) * 40) / 100);
                                                                    short[] PointdataRunX = Get_PointData(X);
                                                                    short[] PointdataRunY1 = Get_PointData(Y1);
                                                                    short[] PointdataRunY2 = Get_PointData(Y2);
                                                                    short[] PointdataRunZ1 = Get_PointData(Z1);
                                                                    short[] PointdataRunZ2 = Get_PointData(Z2);
                                                                    PointX[0] = PointdataRunX[0];
                                                                    PointX[1] = PointdataRunX[1];
                                                                    PointY1[0] = PointdataRunY1[0];
                                                                    PointY1[1] = PointdataRunY1[1];
                                                                    PointY2[0] = PointdataRunY2[0];
                                                                    PointY2[1] = PointdataRunY2[1];
                                                                    PointZ1[0] = PointdataRunZ1[0];
                                                                    PointZ1[1] = PointdataRunZ1[1];
                                                                    PointZ2[0] = PointdataRunZ2[0];
                                                                    PointZ2[1] = PointdataRunZ2[1];
                                                                    Dispatcher.Invoke(new Action(delegate
                                                                    {
                                                                        SelectLine(k);
                                                                    }));
                                                                    Thread.Sleep(new TimeSpan(10000));
                                                                    _RS485.SendFc16(ID1, 24640, 1, new short[1] { 15 });
                                                                    _RS485.SendFc16(ID1, 24672, 1, new short[1] { -101 });
                                                                    _RS485.SendFc16(ID1, 10241, 9, new short[9] { 7, PointX[0], PointX[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                                                                    Thread.Sleep(new TimeSpan(10000));
                                                                    _RS485.SendFc16(ID2, 24640, 1, new short[1] { 15 });
                                                                    _RS485.SendFc16(ID2, 24672, 1, new short[1] { -101 });
                                                                    _RS485.SendFc16(ID2, 10241, 9, new short[9] { 7, PointY1[0], PointY1[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                                                                    _RS485.SendFc16(ID2, 24640, 1, new short[1] { 31 });
                                                                    Thread.Sleep(new TimeSpan(10000));
                                                                    _RS485.SendFc16(ID1, 24640, 1, new short[1] { 31 });
                                                                    //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
                                                                    //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
                                                                    //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });


                                                                    //_RS485.SendFc16(3, 24672, 1, new short[1] { -101 });
                                                                    //_RS485.SendFc16(4, 24672, 1, new short[1] { -101 });
                                                                    //_RS485.SendFc16(5, 24672, 1, new short[1] { -101 });


                                                                    //_RS485.SendFc16(3, 10241, 9, new short[9] { 1, PointY2[0], PointY2[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                                                                    //_RS485.SendFc16(4, 10241, 9, new short[9] { 1, PointZ1[0], PointZ1[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                                                                    //_RS485.SendFc16(5, 10241, 9, new short[9] { 1, PointZ2[0], PointZ2[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });


                                                                    //_RS485.SendFc16(3, 11616, 1, new short[1] { 1 });
                                                                    //_RS485.SendFc16(4, 11616, 1, new short[1] { 1 });
                                                                    //_RS485.SendFc16(5, 11616, 1, new short[1] { 1 });

                                                                    //_RS485.SendFc16(3, 24640, 1, new short[1] { 31 });
                                                                    //_RS485.SendFc16(4, 24640, 1, new short[1] { 31 });
                                                                    //_RS485.SendFc16(5, 24640, 1, new short[1] { 31 });
                                                                    while (!(R_Point_X[0] == PointX[0] && R_Point_X[1] == PointX[1]) || !(R_Point_Y1[0] == PointY1[0] && R_Point_Y1[1] == PointY1[1]))// || !(R_Point_Y2[0] == Y2) || !(R_Point_Z1[0] == Z1) || !(R_Point_Z2[0] == Z2))
                                                                    {
                                                                        Thread.Sleep(new TimeSpan(10000));
                                                                        _RS485.SendFc3(ID1, 11055, 2, ref R_Point_X);
                                                                        Thread.Sleep(new TimeSpan(10000));
                                                                        _RS485.SendFc3(ID2, 11055, 2, ref R_Point_Y1);

                                                                        //_RS485.SendFc3(3, 11055, 2, ref R_Point_Y2);
                                                                        //_RS485.SendFc3(4, 11055, 2, ref R_Point_Z1);
                                                                        //_RS485.SendFc3(5, 11055, 2, ref R_Point_Z2);
                                                                        Dispatcher.Invoke(new Action(delegate
                                                                        {
                                                                            Point_XRun.Content = GetIntFromBits_Read(R_Point_X[0], R_Point_X[1]);
                                                                            Point_Y1Run.Content = GetIntFromBits_Read(R_Point_Y1[0], R_Point_Y1[1]);
                                                                            //Point_Y2Run.Content = R_Point_Y2[0].ToString();
                                                                            //Point_Z1Run.Content = R_Point_Z1[0].ToString();
                                                                            //Point_Z2Run.Content = R_Point_Z2[0].ToString();
                                                                            VelocityRun.Content = SpeedRun[0].ToString();
                                                                        }));
                                                                        if (Wshutdown)
                                                                        {

                                                                            ShutDown();
                                                                            Dispatcher.Invoke(new Action(delegate
                                                                            {
                                                                                SelectLine1(k);
                                                                            }));
                                                                            break;
                                                                        }
                                                                        if (Wenable)
                                                                        {
                                                                            Enable();
                                                                            Dispatcher.Invoke(new Action(delegate
                                                                            {
                                                                                SelectLine1(k);
                                                                            }));
                                                                            break;
                                                                        }
                                                                        if (reset)
                                                                        {
                                                                            Reset();
                                                                            Dispatcher.Invoke(new Action(delegate
                                                                            {
                                                                                SelectLine1(k);
                                                                            }));
                                                                            break;
                                                                        }
                                                                        if (Pause == 1 || Stop == 1)
                                                                        {
                                                                            if (Pause == 1)
                                                                            {
                                                                                Thread.Sleep(new TimeSpan(10000));
                                                                                _RS485.SendFc16(ID1, 24640, 1, new short[1] { 271 });
                                                                                Thread.Sleep(new TimeSpan(10000));
                                                                                _RS485.SendFc16(ID2, 24640, 1, new short[1] { 271 });
                                                                                //_RS485.SendFc16(3, 24640, 1, new short[1] { 271 });
                                                                                //_RS485.SendFc16(4, 24640, 1, new short[1] { 271 });
                                                                                //_RS485.SendFc16(5, 24640, 1, new short[1] { 271 });
                                                                                while (Pause == 1)
                                                                                {
                                                                                    Thread.Sleep(new TimeSpan(10000));
                                                                                    _RS485.SendFc3(ID1, 11055, 2, ref R_Point_X);
                                                                                    Thread.Sleep(new TimeSpan(10000));
                                                                                    _RS485.SendFc3(ID2, 11055, 2, ref R_Point_Y1);
                                                                                    //_RS485.SendFc3(3, 11055, 2, ref R_Point_Y2);
                                                                                    //_RS485.SendFc3(4, 11055, 2, ref R_Point_Z1);
                                                                                    //_RS485.SendFc3(5, 11055, 2, ref R_Point_Z2);
                                                                                    Dispatcher.Invoke(new Action(delegate
                                                                                    {
                                                                                        Point_XRun.Content = GetIntFromBits_Read(R_Point_X[0], R_Point_X[1]);
                                                                                        Point_Y1Run.Content = GetIntFromBits_Read(R_Point_Y1[0], R_Point_Y1[1]);
                                                                                        //Point_Y2Run.Content = R_Point_Y2[0].ToString();
                                                                                        //Point_Z1Run.Content = R_Point_Z1[0].ToString();
                                                                                        //Point_Z2Run.Content = R_Point_Z2[0].ToString();
                                                                                    }));
                                                                                    if (Pause == 0)
                                                                                    {
                                                                                        Thread.Sleep(new TimeSpan(10000));
                                                                                        _RS485.SendFc16(ID1, 24640, 1, new short[1] { 31 });
                                                                                        Thread.Sleep(new TimeSpan(10000));
                                                                                        _RS485.SendFc16(ID2, 24640, 1, new short[1] { 31 });
                                                                                        //_RS485.SendFc16(3, 24640, 1, new short[1] { 31 });
                                                                                        //_RS485.SendFc16(4, 24640, 1, new short[1] { 31 });
                                                                                        //_RS485.SendFc16(5, 24640, 1, new short[1] { 31 });
                                                                                    }
                                                                                    if (Stop == 1)
                                                                                    {
                                                                                        Thread.Sleep(new TimeSpan(10000));
                                                                                        _RS485.SendFc16(ID1, 24640, 1, new short[1] { 0 });
                                                                                        _RS485.SendFc16(ID1, 24640, 1, new short[1] { 15 });
                                                                                        Thread.Sleep(new TimeSpan(10000));
                                                                                        _RS485.SendFc16(ID2, 24640, 1, new short[1] { 0 });
                                                                                        _RS485.SendFc16(ID2, 24640, 1, new short[1] { 15 });
                                                                                        //_RS485.SendFc16(3, 24640, 1, new short[1] { 0 });
                                                                                        //_RS485.SendFc16(4, 24640, 1, new short[1] { 0 });
                                                                                        //_RS485.SendFc16(5, 24640, 1, new short[1] { 0 });


                                                                                        //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
                                                                                        //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
                                                                                        //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });
                                                                                        Dispatcher.Invoke(new Action(delegate
                                                                                        {
                                                                                            SelectLine1(k);
                                                                                        }));
                                                                                        break;
                                                                                    }
                                                                                    if (Whome)
                                                                                    {
                                                                                        HOME();
                                                                                        break;
                                                                                    }
                                                                                    if (Wshutdown)
                                                                                    {
                                                                                        ShutDown();
                                                                                        break;
                                                                                    }
                                                                                    if (Wenable)
                                                                                    {
                                                                                        Enable();
                                                                                        break;
                                                                                    }
                                                                                    if (reset)
                                                                                    {
                                                                                        Reset();
                                                                                        break;
                                                                                    }
                                                                                }

                                                                            }
                                                                            if (Stop == 1)
                                                                            {
                                                                                Thread.Sleep(new TimeSpan(10000));
                                                                                _RS485.SendFc16(ID1, 24640, 1, new short[1] { 0 });
                                                                                _RS485.SendFc16(ID1, 24640, 1, new short[1] { 15 });
                                                                                Thread.Sleep(new TimeSpan(10000));
                                                                                _RS485.SendFc16(ID2, 24640, 1, new short[1] { 0 });
                                                                                _RS485.SendFc16(ID2, 24640, 1, new short[1] { 15 });
                                                                                //_RS485.SendFc16(3, 24640, 1, new short[1] { 0 });
                                                                                //_RS485.SendFc16(4, 24640, 1, new short[1] { 0 });
                                                                                //_RS485.SendFc16(5, 24640, 1, new short[1] { 0 });


                                                                                //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
                                                                                //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
                                                                                //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });
                                                                                Dispatcher.Invoke(new Action(delegate
                                                                                {
                                                                                    SelectLine1(k);
                                                                                }));
                                                                                break;
                                                                            }
                                                                            if (Whome || Wshutdown || Wenable || reset)
                                                                            {
                                                                                Dispatcher.Invoke(new Action(delegate
                                                                                {
                                                                                    SelectLine1(k);
                                                                                }));
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                    if (R_Point_X[0] == PointX[0] && R_Point_X[1] == PointX[1] && R_Point_Y1[0] == PointY1[0] && R_Point_Y1[1] == PointY1[1])// && R_Point_Y2[0] == Y2 && R_Point_Z1[0] == Z1 && R_Point_Z2[0] == Z2)
                                                                    {
                                                                        Dispatcher.Invoke(new Action(delegate
                                                                        {
                                                                            SelectLine1(k);
                                                                        }));
                                                                    }
                                                                }
                                                                else if (Command2 == "DELAY")
                                                                {
                                                                    Dispatcher.Invoke(new Action(delegate
                                                                    {
                                                                        SelectLine(k);
                                                                    }));
                                                                    Delay = Convert.ToInt16(coordinateAxis2[4]);
                                                                    t = 0;
                                                                    T = true;
                                                                    while (!(t >= Delay))
                                                                    {
                                                                        if (Wshutdown)
                                                                        {
                                                                            ShutDown();
                                                                            Dispatcher.Invoke(new Action(delegate
                                                                            {
                                                                                SelectLine1(k);
                                                                            }));
                                                                            break;
                                                                        }
                                                                        if (Wenable)
                                                                        {
                                                                            Enable();
                                                                            Dispatcher.Invoke(new Action(delegate
                                                                            {
                                                                                SelectLine1(k);
                                                                            }));
                                                                            break;
                                                                        }
                                                                        if (reset)
                                                                        {
                                                                            Reset();
                                                                            Dispatcher.Invoke(new Action(delegate
                                                                            {
                                                                                SelectLine1(k);
                                                                            }));
                                                                            break;
                                                                        }
                                                                        if (Pause == 1 || Stop == 1)
                                                                        {
                                                                            if (Pause == 1)
                                                                            {
                                                                                Dispatcher.Invoke(new Action(delegate
                                                                                {
                                                                                    Timerdelay.Content = t.ToString();
                                                                                }));
                                                                                while (Pause == 1)
                                                                                {
                                                                                    if (Stop == 1)
                                                                                    {
                                                                                        break;
                                                                                    }
                                                                                    if (Whome)
                                                                                    {
                                                                                        HOME();
                                                                                        break;
                                                                                    }
                                                                                    if (Wshutdown)
                                                                                    {
                                                                                        ShutDown();
                                                                                        break;
                                                                                    }
                                                                                    if (Wenable)
                                                                                    {
                                                                                        Enable();
                                                                                        break;
                                                                                    }
                                                                                    if (reset)
                                                                                    {
                                                                                        Reset();
                                                                                        break;
                                                                                    }
                                                                                }
                                                                            }
                                                                            if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                                            {
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                    t = 0;
                                                                    T = false;
                                                                    Dispatcher.Invoke(new Action(delegate
                                                                    {
                                                                        SelectLine1(k);
                                                                    }));
                                                                }
                                                                else if (Command2 == "SDOON")
                                                                {
                                                                    Dispatcher.Invoke(new Action(delegate
                                                                    {
                                                                        SelectLine(k);
                                                                    }));
                                                                    SDOON = Convert.ToInt16(coordinateAxis2[4]);
                                                                    #region ON
                                                                    switch (SDOON)
                                                                    {
                                                                        case 0:
                                                                            temp1[7] = '1';
                                                                            break;
                                                                        case 1:
                                                                            temp1[6] = '1';
                                                                            break;
                                                                        case 2:
                                                                            temp1[5] = '1';
                                                                            break;
                                                                        case 3:
                                                                            temp1[4] = '1';
                                                                            break;
                                                                        case 4:
                                                                            temp1[3] = '1';
                                                                            break;
                                                                        case 5:
                                                                            temp1[2] = '1';
                                                                            break;
                                                                        case 6:
                                                                            temp1[1] = '1';
                                                                            break;
                                                                        case 7:
                                                                            temp1[0] = '1';
                                                                            break;
                                                                        case 8:
                                                                            temp2[7] = '1';
                                                                            break;
                                                                        case 9:
                                                                            temp2[6] = '1';
                                                                            break;
                                                                        case 10:
                                                                            temp2[5] = '1';
                                                                            break;
                                                                        case 11:
                                                                            temp2[4] = '1';
                                                                            break;
                                                                        case 12:
                                                                            temp2[3] = '1';
                                                                            break;
                                                                        case 13:
                                                                            temp2[3] = '1';
                                                                            break;
                                                                        case 14:
                                                                            temp2[1] = '1';
                                                                            break;
                                                                        case 15:
                                                                            temp2[0] = '1';
                                                                            break;

                                                                    }

                                                                    #endregion
                                                                    build_data();
                                                                    short[] value = new short[2];
                                                                    value[0] = BinaryToShort(data_write1);
                                                                    value[1] = BinaryToShort(data_Write2);
                                                                    WRITE_IO[0] = value[0];
                                                                    WRITE_IO[1] = value[1];
                                                                    Thread.Sleep(new TimeSpan(10000));
                                                                    _RS485.SendFc15(6, 0, 16, WRITE_IO);
                                                                    Dispatcher.Invoke(new Action(delegate
                                                                    {
                                                                        SelectLine1(k);
                                                                    }));
                                                                }
                                                                else if (Command2 == "SDOOF")
                                                                {
                                                                    Dispatcher.Invoke(new Action(delegate
                                                                    {
                                                                        SelectLine(k);
                                                                    }));
                                                                    SDOOF = Convert.ToInt16(coordinateAxis2[4]);
                                                                    #region OF
                                                                    switch (SDOOF)
                                                                    {
                                                                        case 0:
                                                                            temp1[7] = '0';
                                                                            break;
                                                                        case 1:
                                                                            temp1[6] = '0';
                                                                            break;
                                                                        case 2:
                                                                            temp1[5] = '0';
                                                                            break;
                                                                        case 3:
                                                                            temp1[4] = '0';
                                                                            break;
                                                                        case 4:
                                                                            temp1[3] = '0';
                                                                            break;
                                                                        case 5:
                                                                            temp1[2] = '0';
                                                                            break;
                                                                        case 6:
                                                                            temp1[1] = '0';
                                                                            break;
                                                                        case 7:
                                                                            temp1[0] = '0';
                                                                            break;
                                                                        case 8:
                                                                            temp2[7] = '0';
                                                                            break;
                                                                        case 9:
                                                                            temp2[6] = '0';
                                                                            break;
                                                                        case 10:
                                                                            temp2[5] = '0';
                                                                            break;
                                                                        case 11:
                                                                            temp2[4] = '0';
                                                                            break;
                                                                        case 12:
                                                                            temp2[3] = '0';
                                                                            break;
                                                                        case 13:
                                                                            temp2[3] = '0';
                                                                            break;
                                                                        case 14:
                                                                            temp2[1] = '0';
                                                                            break;
                                                                        case 15:
                                                                            temp2[0] = '0';
                                                                            break;

                                                                    }
                                                                    #endregion
                                                                    build_data();
                                                                    short[] value = new short[2];
                                                                    value[0] = BinaryToShort(data_write1);
                                                                    value[1] = BinaryToShort(data_Write2);
                                                                    WRITE_IO[0] = value[0];
                                                                    WRITE_IO[1] = value[1];
                                                                    Thread.Sleep(new TimeSpan(10000));
                                                                    _RS485.SendFc15(6, 0, 16, WRITE_IO);
                                                                    Dispatcher.Invoke(new Action(delegate
                                                                    {
                                                                        SelectLine1(k);
                                                                    }));
                                                                }
                                                                else if (Command2 == "WDION")
                                                                {
                                                                    Dispatcher.Invoke(new Action(delegate
                                                                    {
                                                                        SelectLine(k);
                                                                    }));
                                                                    WDION = Convert.ToInt16(coordinateAxis2[4]);


                                                                    _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                                                    Dispatcher.Invoke(new Action(delegate
                                                                    {
                                                                        var output1 = Convert.ToString(READ_IO[0], 2);
                                                                        var output2 = Convert.ToString(READ_IO[1], 2);

                                                                        string out1 = output1.PadLeft(8, '0');
                                                                        string out2 = output2.PadLeft(8, '0');
                                                                        for (int O = 0; O < 8; O++)
                                                                        {
                                                                            out_put1[O] = out1[O];
                                                                            out_put2[O] = out2[O];
                                                                        }
                                                                    }));
                                                                    if (Stop == 1 || Pause == 1)
                                                                    {
                                                                        while (Pause == 1)
                                                                        {
                                                                            if (Stop == 1)
                                                                            {
                                                                                break;
                                                                            }
                                                                            if (Whome)
                                                                            {
                                                                                HOME();
                                                                                break;
                                                                            }
                                                                            if (Wshutdown)
                                                                            {
                                                                                ShutDown();
                                                                                break;
                                                                            }
                                                                            if (Wenable)
                                                                            {
                                                                                Enable();
                                                                                break;
                                                                            }
                                                                            if (reset)
                                                                            {
                                                                                Reset();
                                                                                break;
                                                                            }
                                                                        }
                                                                        if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                                        {
                                                                            break;
                                                                        }
                                                                    }
                                                                    if (WDION <= 7)
                                                                    {
                                                                        if (WDION == 7) WDION = 0;
                                                                        else if (WDION == 6) WDION = 1;
                                                                        else if (WDION == 5) WDION = 2;
                                                                        else if (WDION == 4) WDION = 3;
                                                                        else if (WDION == 3) WDION = 4;
                                                                        else if (WDION == 2) WDION = 5;
                                                                        else if (WDION == 1) WDION = 6;
                                                                        else if (WDION == 0) WDION = 7;
                                                                        while (!(out_put1[WDION] == '1'))
                                                                        {
                                                                            Thread.Sleep(new TimeSpan(10000));
                                                                            _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                                                            Dispatcher.Invoke(new Action(delegate
                                                                            {
                                                                                var output1 = Convert.ToString(READ_IO[0], 2);
                                                                                string out1 = output1.PadLeft(8, '0');
                                                                                for (int O = 0; O < 8; O++)
                                                                                {
                                                                                    out_put1[O] = out1[O];
                                                                                }
                                                                            }));
                                                                            if (Wshutdown)
                                                                            {
                                                                                ShutDown();
                                                                                Dispatcher.Invoke(new Action(delegate
                                                                                {
                                                                                    SelectLine1(k);
                                                                                }));
                                                                                break;
                                                                            }
                                                                            if (Wenable)
                                                                            {
                                                                                Enable();
                                                                                Dispatcher.Invoke(new Action(delegate
                                                                                {
                                                                                    SelectLine1(k);
                                                                                }));
                                                                                break;
                                                                            }
                                                                            if (reset)
                                                                            {
                                                                                Reset();
                                                                                Dispatcher.Invoke(new Action(delegate
                                                                                {
                                                                                    SelectLine1(k);
                                                                                }));
                                                                                break;
                                                                            }
                                                                            if (Stop == 1 || Pause == 1)
                                                                            {
                                                                                while (Pause == 1)
                                                                                {
                                                                                    if (Stop == 1)
                                                                                    {
                                                                                        break;
                                                                                    }
                                                                                    if (Whome)
                                                                                    {
                                                                                        HOME();
                                                                                        break;
                                                                                    }
                                                                                    if (Wshutdown)
                                                                                    {
                                                                                        ShutDown();
                                                                                        break;
                                                                                    }
                                                                                    if (Wenable)
                                                                                    {
                                                                                        Enable();
                                                                                        break;
                                                                                    }
                                                                                    if (reset)
                                                                                    {
                                                                                        Reset();
                                                                                        break;
                                                                                    }
                                                                                }
                                                                                if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                                                {
                                                                                    break;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else if (WDION > 7)
                                                                    {
                                                                        if (WDION == 15) WDION = 0;
                                                                        else if (WDION == 14) WDION = 1;
                                                                        else if (WDION == 13) WDION = 2;
                                                                        else if (WDION == 12) WDION = 3;
                                                                        else if (WDION == 11) WDION = 4;
                                                                        else if (WDION == 10) WDION = 5;
                                                                        else if (WDION == 9) WDION = 6;
                                                                        else if (WDION == 8) WDION = 7;
                                                                        while (!(out_put2[WDION] == '1'))
                                                                        {
                                                                            Thread.Sleep(new TimeSpan(10000));
                                                                            _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                                                            Dispatcher.Invoke(new Action(delegate
                                                                            {
                                                                                var output2 = Convert.ToString(READ_IO[1], 2);

                                                                                string out2 = output2.PadLeft(8, '0');
                                                                                for (int O = 0; O < 8; O++)
                                                                                {
                                                                                    out_put2[O] = out2[O];
                                                                                }
                                                                            }));
                                                                            if (Wshutdown)
                                                                            {
                                                                                ShutDown();
                                                                                Dispatcher.Invoke(new Action(delegate
                                                                                {
                                                                                    SelectLine1(k);
                                                                                }));
                                                                                break;
                                                                            }
                                                                            if (Wenable)
                                                                            {
                                                                                Enable();
                                                                                Dispatcher.Invoke(new Action(delegate
                                                                                {
                                                                                    SelectLine1(k);
                                                                                }));
                                                                                break;
                                                                            }
                                                                            if (reset)
                                                                            {
                                                                                Reset();
                                                                                Dispatcher.Invoke(new Action(delegate
                                                                                {
                                                                                    SelectLine1(k);
                                                                                }));
                                                                                break;
                                                                            }
                                                                            if (Stop == 1 || Pause == 1)
                                                                            {
                                                                                while (Pause == 1)
                                                                                {
                                                                                    if (Stop == 1)
                                                                                    {
                                                                                        break;
                                                                                    }
                                                                                    if (Whome)
                                                                                    {
                                                                                        HOME();
                                                                                        break;
                                                                                    }
                                                                                    if (Wshutdown)
                                                                                    {
                                                                                        ShutDown();
                                                                                        break;
                                                                                    }
                                                                                    if (Wenable)
                                                                                    {
                                                                                        Enable();
                                                                                        break;
                                                                                    }
                                                                                    if (reset)
                                                                                    {
                                                                                        Reset();
                                                                                        break;
                                                                                    }
                                                                                }
                                                                                if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                                                {
                                                                                    break;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    Dispatcher.Invoke(new Action(delegate
                                                                    {
                                                                        SelectLine1(k);
                                                                    }));
                                                                }
                                                                else if (Command2 == "WDIOF")
                                                                {
                                                                    Dispatcher.Invoke(new Action(delegate
                                                                    {
                                                                        SelectLine(k);
                                                                    }));
                                                                    WDIOF = Convert.ToInt16(coordinateAxis2[4]);
                                                                    Thread.Sleep(new TimeSpan(10000));
                                                                    _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                                                    Dispatcher.Invoke(new Action(delegate
                                                                    {
                                                                        var output1 = Convert.ToString(READ_IO[0], 2);
                                                                        var output2 = Convert.ToString(READ_IO[1], 2);

                                                                        string out1 = output1.PadLeft(8, '0');
                                                                        string out2 = output2.PadLeft(8, '0');
                                                                        for (int O = 0; O < 8; O++)
                                                                        {
                                                                            out_put1[O] = out1[O];
                                                                            out_put2[O] = out2[O];
                                                                        }
                                                                    }));
                                                                    if (Stop == 1 || Pause == 1)
                                                                    {
                                                                        while (Pause == 1)
                                                                        {
                                                                            if (Stop == 1)
                                                                            {
                                                                                break;
                                                                            }
                                                                            if (Whome)
                                                                            {
                                                                                HOME();
                                                                                break;
                                                                            }
                                                                            if (Wshutdown)
                                                                            {
                                                                                ShutDown();
                                                                                break;
                                                                            }
                                                                            if (Wenable)
                                                                            {
                                                                                Enable();
                                                                                break;
                                                                            }
                                                                            if (reset)
                                                                            {
                                                                                Reset();
                                                                                break;
                                                                            }
                                                                        }
                                                                        if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                                        {
                                                                            break;
                                                                        }
                                                                    }
                                                                    if (WDIOF <= 7)
                                                                    {
                                                                        if (WDIOF == 7) WDIOF = 0;
                                                                        else if (WDIOF == 6) WDIOF = 1;
                                                                        else if (WDIOF == 5) WDIOF = 2;
                                                                        else if (WDIOF == 4) WDIOF = 3;
                                                                        else if (WDIOF == 3) WDIOF = 4;
                                                                        else if (WDIOF == 2) WDIOF = 5;
                                                                        else if (WDIOF == 1) WDIOF = 6;
                                                                        else if (WDIOF == 0) WDIOF = 7;
                                                                        while (!(out_put1[WDIOF] == '0'))
                                                                        {
                                                                            Thread.Sleep(new TimeSpan(10000));
                                                                            _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                                                            Dispatcher.Invoke(new Action(delegate
                                                                            {
                                                                                var output1 = Convert.ToString(READ_IO[0], 2);

                                                                                string out1 = output1.PadLeft(8, '0');
                                                                                for (int O = 0; O < 8; O++)
                                                                                {
                                                                                    out_put1[O] = out1[O];
                                                                                }
                                                                            }));
                                                                            if (Wshutdown)
                                                                            {
                                                                                ShutDown();
                                                                                Dispatcher.Invoke(new Action(delegate
                                                                                {
                                                                                    SelectLine1(k);
                                                                                }));
                                                                                break;
                                                                            }
                                                                            if (Wenable)
                                                                            {
                                                                                Enable();
                                                                                Dispatcher.Invoke(new Action(delegate
                                                                                {
                                                                                    SelectLine1(k);
                                                                                }));
                                                                                break;
                                                                            }
                                                                            if (reset)
                                                                            {
                                                                                Reset();
                                                                                Dispatcher.Invoke(new Action(delegate
                                                                                {
                                                                                    SelectLine1(k);
                                                                                }));
                                                                                break;
                                                                            }
                                                                            if (Stop == 1 || Pause == 1)
                                                                            {
                                                                                while (Pause == 1)
                                                                                {
                                                                                    if (Stop == 1)
                                                                                    {
                                                                                        break;
                                                                                    }
                                                                                    if (Whome)
                                                                                    {
                                                                                        HOME();
                                                                                        break;
                                                                                    }
                                                                                    if (Wshutdown)
                                                                                    {
                                                                                        ShutDown();
                                                                                        break;
                                                                                    }
                                                                                    if (Wenable)
                                                                                    {
                                                                                        Enable();
                                                                                        break;
                                                                                    }
                                                                                    if (reset)
                                                                                    {
                                                                                        Reset();
                                                                                        break;
                                                                                    }
                                                                                }
                                                                                if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                                                {
                                                                                    break;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    if (WDIOF > 7)
                                                                    {
                                                                        if (WDIOF == 15) WDIOF = 0;
                                                                        else if (WDIOF == 14) WDIOF = 1;
                                                                        else if (WDIOF == 13) WDIOF = 2;
                                                                        else if (WDIOF == 12) WDIOF = 3;
                                                                        else if (WDIOF == 10) WDIOF = 5;
                                                                        else if (WDIOF == 9) WDIOF = 6;
                                                                        else if (WDIOF == 8) WDIOF = 7;
                                                                        while (!(out_put2[WDIOF] == '0'))
                                                                        {
                                                                            Thread.Sleep(new TimeSpan(10000));
                                                                            _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                                                            Dispatcher.Invoke(new Action(delegate
                                                                            {
                                                                                var output2 = Convert.ToString(READ_IO[1], 2);

                                                                                string out2 = output2.PadLeft(8, '0');
                                                                                for (int O = 0; O < 8; O++)
                                                                                {
                                                                                    out_put2[O] = out2[O];
                                                                                }
                                                                            }));
                                                                            if (Wshutdown)
                                                                            {
                                                                                ShutDown();
                                                                                Dispatcher.Invoke(new Action(delegate
                                                                                {
                                                                                    SelectLine1(k);
                                                                                }));
                                                                                break;
                                                                            }
                                                                            if (Wenable)
                                                                            {
                                                                                Enable();
                                                                                Dispatcher.Invoke(new Action(delegate
                                                                                {
                                                                                    SelectLine1(k);
                                                                                }));
                                                                                break;
                                                                            }
                                                                            if (reset)
                                                                            {
                                                                                Reset();
                                                                                Dispatcher.Invoke(new Action(delegate
                                                                                {
                                                                                    SelectLine1(k);
                                                                                }));
                                                                                break;
                                                                            }
                                                                            if (Stop == 1 || Pause == 1)
                                                                            {
                                                                                while (Pause == 1)
                                                                                {
                                                                                    if (Stop == 1)
                                                                                    {
                                                                                        break;
                                                                                    }
                                                                                    if (Whome)
                                                                                    {
                                                                                        HOME();
                                                                                        break;
                                                                                    }
                                                                                    if (Wshutdown)
                                                                                    {
                                                                                        ShutDown();
                                                                                        break;
                                                                                    }
                                                                                    if (Wenable)
                                                                                    {
                                                                                        Enable();
                                                                                        break;
                                                                                    }
                                                                                    if (reset)
                                                                                    {
                                                                                        Reset();
                                                                                        break;
                                                                                    }
                                                                                }
                                                                                if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                                                {
                                                                                    break;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    Dispatcher.Invoke(new Action(delegate
                                                                    {
                                                                        SelectLine1(k);
                                                                    }));
                                                                }
                                                                else if (Command2 == "ENDREPEAT")
                                                                {
                                                                    times++;
                                                                    b = k;
                                                                    break;
                                                                }
                                                            }

                                                        }
                                                    }
                                                    if (times == 1)
                                                    {
                                                        for (int n = j; n < b; n++)
                                                        {
                                                            sr1.ReadLine();
                                                        }
                                                    }
                                                    if (times == Times)
                                                    {
                                                        j = b;
                                                    }

                                                } while (times != Times);
                                            }
                                            else if (Command1 == "ENDLOOP")
                                            {
                                                break;
                                            }
                                        }

                                    }
                                }
                            } while (true);
                        }
                        else if (Command == "REPEAT")
                        {

                            Int16 Times = Convert.ToInt16(coordinateAxis[4]);
                            Int16 times = 0;
                            do
                            {
                                if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                {
                                    break;
                                }

                                using (StreamReader sr1 = new StreamReader(MainViewModel.duongdanrun))
                                {
                                    for (int j = 0; j < count; j++)
                                    {
                                        string point1 = sr1.ReadLine();
                                        if (j > i)
                                        {
                                            if (Whome || Stop == 1 || Wshutdown || Wenable || reset)
                                            {
                                                break;
                                            }
                                            string[] coordinateAxis1 = point1.Split('.', ':', ' ');
                                            Command1 = coordinateAxis1[3];

                                            if (Command1 == "MOVE")
                                            {
                                                X = Convert.ToInt16(coordinateAxis1[5]);
                                                Y1 = Convert.ToInt16(coordinateAxis1[7]);
                                                Y2 = Convert.ToInt16(coordinateAxis1[9]);
                                                Z1 = Convert.ToInt16(coordinateAxis1[11]);
                                                Z2 = Convert.ToInt16(coordinateAxis1[13]);
                                                SpeedRun[0] = Convert.ToInt16(coordinateAxis1[15]);
                                                AccelRun[0] = Convert.ToInt16(((SpeedRun[0]) * 40) / 100);
                                                DecelRun[0] = Convert.ToInt16(((SpeedRun[0]) * 40) / 100);
                                                short[] PointdataRunX = Get_PointData(X);
                                                short[] PointdataRunY1 = Get_PointData(Y1);
                                                short[] PointdataRunY2 = Get_PointData(Y2);
                                                short[] PointdataRunZ1 = Get_PointData(Z1);
                                                short[] PointdataRunZ2 = Get_PointData(Z2);
                                                PointX[0] = PointdataRunX[0];
                                                PointX[1] = PointdataRunX[1];
                                                PointY1[0] = PointdataRunY1[0];
                                                PointY1[1] = PointdataRunY1[1];
                                                PointY2[0] = PointdataRunY2[0];
                                                PointY2[1] = PointdataRunY2[1];
                                                PointZ1[0] = PointdataRunZ1[0];
                                                PointZ1[1] = PointdataRunZ1[1];
                                                PointZ2[0] = PointdataRunZ2[0];
                                                PointZ2[1] = PointdataRunZ2[1];
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine(j);
                                                }));
                                                Thread.Sleep(new TimeSpan(10000));
                                                _RS485.SendFc16(ID1, 24640, 1, new short[1] { 15 });
                                                _RS485.SendFc16(ID1, 24672, 1, new short[1] { -101 });
                                                _RS485.SendFc16(ID1, 10241, 9, new short[9] { 7, PointX[0], PointX[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                                                Thread.Sleep(new TimeSpan(10000));
                                                _RS485.SendFc16(ID2, 24640, 1, new short[1] { 15 });
                                                _RS485.SendFc16(ID2, 24672, 1, new short[1] { -101 });
                                                _RS485.SendFc16(ID2, 10241, 9, new short[9] { 7, PointY1[0], PointY1[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                                                _RS485.SendFc16(ID2, 24640, 1, new short[1] { 31 });
                                                Thread.Sleep(new TimeSpan(10000));
                                                _RS485.SendFc16(ID1, 24640, 1, new short[1] { 31 });
                                                //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
                                                //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
                                                //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });


                                                //_RS485.SendFc16(3, 24672, 1, new short[1] { -101 });
                                                //_RS485.SendFc16(4, 24672, 1, new short[1] { -101 });
                                                //_RS485.SendFc16(5, 24672, 1, new short[1] { -101 });


                                                //_RS485.SendFc16(3, 10241, 9, new short[9] { 1, PointY2[0], PointY2[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                                                //_RS485.SendFc16(4, 10241, 9, new short[9] { 1, PointZ1[0], PointZ1[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                                                //_RS485.SendFc16(5, 10241, 9, new short[9] { 1, PointZ2[0], PointZ2[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                                                //_RS485.SendFc16(1, 11616, 1, new short[1] { 1 });
                                                //_RS485.SendFc16(2, 11616, 1, new short[1] { 1 });
                                                //_RS485.SendFc16(3, 11616, 1, new short[1] { 1 });
                                                //_RS485.SendFc16(4, 11616, 1, new short[1] { 1 });
                                                //_RS485.SendFc16(5, 11616, 1, new short[1] { 1 });

                                                //_RS485.SendFc16(3, 24640, 1, new short[1] { 31 });
                                                //_RS485.SendFc16(4, 24640, 1, new short[1] { 31 });
                                                //_RS485.SendFc16(5, 24640, 1, new short[1] { 31 });
                                                while (!(R_Point_X[0] == PointX[0] && R_Point_X[1] == PointX[1]) || !(R_Point_Y1[0] == PointY1[0] && R_Point_Y1[1] == PointY1[1]))// || !(R_Point_Y2[0] == Y2) || !(R_Point_Z1[0] == Z1) || !(R_Point_Z2[0] == Z2))
                                                {
                                                    Thread.Sleep(new TimeSpan(10000));
                                                    _RS485.SendFc3(ID1, 11055, 2, ref R_Point_X);
                                                    Thread.Sleep(new TimeSpan(10000));
                                                    _RS485.SendFc3(ID2, 11055, 2, ref R_Point_Y1);
                                                    //_RS485.SendFc3(3, 11055, 2, ref R_Point_Y2);
                                                    //_RS485.SendFc3(4, 11055, 2, ref R_Point_Z1);
                                                    //_RS485.SendFc3(5, 11055, 2, ref R_Point_Z2);
                                                    Dispatcher.Invoke(new Action(delegate
                                                    {
                                                        Point_XRun.Content = GetIntFromBits_Read(R_Point_X[0], R_Point_X[1]);
                                                        Point_Y1Run.Content = GetIntFromBits_Read(R_Point_Y1[0], R_Point_Y1[1]);
                                                        //Point_Y2Run.Content = R_Point_Y2[0].ToString();
                                                        //Point_Z1Run.Content = R_Point_Z1[0].ToString();
                                                        //Point_Z2Run.Content = R_Point_Z2[0].ToString();
                                                        VelocityRun.Content = SpeedRun[0].ToString();
                                                    }));
                                                    if (Wshutdown)
                                                    {
                                                        ShutDown();
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            SelectLine1(j);
                                                        }));
                                                        break;
                                                    }
                                                    if (Wenable)
                                                    {
                                                        Enable();
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            SelectLine1(j);
                                                        }));
                                                        break;
                                                    }
                                                    if (reset)
                                                    {
                                                        Reset();
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            SelectLine1(j);
                                                        }));
                                                        break;
                                                    }
                                                    if (Pause == 1 || Stop == 1)
                                                    {
                                                        if (Pause == 1)
                                                        {
                                                            Thread.Sleep(new TimeSpan(10000));
                                                            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 271 });
                                                            Thread.Sleep(new TimeSpan(10000));
                                                            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 271 });
                                                            //_RS485.SendFc16(3, 24640, 1, new short[1] { 271 });
                                                            //_RS485.SendFc16(4, 24640, 1, new short[1] { 271 });
                                                            //_RS485.SendFc16(5, 24640, 1, new short[1] { 271 });
                                                            while (Pause == 1)
                                                            {
                                                                Thread.Sleep(new TimeSpan(10000));
                                                                _RS485.SendFc3(ID1, 11055, 2, ref R_Point_X);
                                                                Thread.Sleep(new TimeSpan(10000));
                                                                _RS485.SendFc3(ID2, 11055, 2, ref R_Point_Y1);
                                                                //_RS485.SendFc3(3, 11055, 2, ref R_Point_Y2);
                                                                //_RS485.SendFc3(4, 11055, 2, ref R_Point_Z1);
                                                                //_RS485.SendFc3(5, 11055, 2, ref R_Point_Z2);
                                                                Dispatcher.Invoke(new Action(delegate
                                                                {
                                                                    Point_XRun.Content = GetIntFromBits_Read(R_Point_X[0], R_Point_X[1]);
                                                                    Point_Y1Run.Content = GetIntFromBits_Read(R_Point_Y1[0], R_Point_Y1[1]);
                                                                    //Point_Y2Run.Content = R_Point_Y2[0].ToString();
                                                                    //Point_Z1Run.Content = R_Point_Z1[0].ToString();
                                                                    //Point_Z2Run.Content = R_Point_Z2[0].ToString();
                                                                }));
                                                                if (Pause == 0)
                                                                {
                                                                    Thread.Sleep(new TimeSpan(10000));
                                                                    _RS485.SendFc16(ID1, 24640, 1, new short[1] { 31 });
                                                                    Thread.Sleep(new TimeSpan(10000));
                                                                    _RS485.SendFc16(ID2, 24640, 1, new short[1] { 31 });
                                                                    //_RS485.SendFc16(3, 24640, 1, new short[1] { 31 });
                                                                    //_RS485.SendFc16(4, 24640, 1, new short[1] { 31 });
                                                                    //_RS485.SendFc16(5, 24640, 1, new short[1] { 31 });
                                                                }
                                                                if (Stop == 1)
                                                                {
                                                                    Thread.Sleep(new TimeSpan(10000));
                                                                    _RS485.SendFc16(ID1, 24640, 1, new short[1] { 0 });
                                                                    _RS485.SendFc16(ID1, 24640, 1, new short[1] { 15 });
                                                                    Thread.Sleep(new TimeSpan(10000));
                                                                    _RS485.SendFc16(ID2, 24640, 1, new short[1] { 0 });
                                                                    _RS485.SendFc16(ID2, 24640, 1, new short[1] { 15 });

                                                                    //_RS485.SendFc16(3, 24640, 1, new short[1] { 0 });
                                                                    //_RS485.SendFc16(4, 24640, 1, new short[1] { 0 });
                                                                    //_RS485.SendFc16(5, 24640, 1, new short[1] { 0 });


                                                                    //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
                                                                    //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
                                                                    //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });
                                                                    Dispatcher.Invoke(new Action(delegate
                                                                    {
                                                                        SelectLine1(j);
                                                                    }));
                                                                    break;
                                                                }
                                                                if (Whome)
                                                                {
                                                                    HOME();
                                                                    break;
                                                                }
                                                                if (Wshutdown)
                                                                {
                                                                    ShutDown();
                                                                    break;
                                                                }
                                                                if (Wenable)
                                                                {
                                                                    Enable();
                                                                    break;
                                                                }
                                                                if (reset)
                                                                {
                                                                    Reset();
                                                                    break;
                                                                }
                                                            }

                                                        }
                                                        if (Stop == 1)
                                                        {
                                                            Thread.Sleep(new TimeSpan(10000));
                                                            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 0 });
                                                            _RS485.SendFc16(ID1, 24640, 1, new short[1] { 15 });
                                                            Thread.Sleep(new TimeSpan(10000));
                                                            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 0 });
                                                            _RS485.SendFc16(ID2, 24640, 1, new short[1] { 15 });
                                                            //_RS485.SendFc16(3, 24640, 1, new short[1] { 0 });
                                                            //_RS485.SendFc16(4, 24640, 1, new short[1] { 0 });
                                                            //_RS485.SendFc16(5, 24640, 1, new short[1] { 0 });


                                                            //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
                                                            //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
                                                            //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (Whome || Wshutdown || Wenable || reset)
                                                        {
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                    }
                                                }
                                                if (R_Point_X[0] == PointX[0] && R_Point_X[1] == PointX[1] && R_Point_Y1[0] == PointY1[0] && R_Point_Y1[1] == PointY1[1])// && R_Point_Y2[0] == Y2 && R_Point_Z1[0] == Z1 && R_Point_Z2[0] == Z2)
                                                {
                                                    Dispatcher.Invoke(new Action(delegate
                                                    {
                                                        SelectLine1(j);
                                                    }));
                                                }
                                            }
                                            else if (Command1 == "DELAY")
                                            {
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine(j);
                                                }));
                                                Delay = Convert.ToInt16(coordinateAxis1[4]);
                                                t = 0;
                                                T = true;
                                                while (!(t >= Delay))
                                                {
                                                    if (Wshutdown)
                                                    {
                                                        ShutDown();
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            SelectLine1(j);
                                                        }));
                                                        break;
                                                    }
                                                    if (Wenable)
                                                    {
                                                        Enable();
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            SelectLine1(j);
                                                        }));
                                                        break;
                                                    }
                                                    if (reset)
                                                    {
                                                        Reset();
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            SelectLine1(j);
                                                        }));
                                                        break;
                                                    }
                                                    if (Pause == 1 || Stop == 1)
                                                    {
                                                        if (Pause == 1)
                                                        {
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                Timerdelay.Content = t.ToString();
                                                            }));
                                                            while (Pause == 1)
                                                            {
                                                                if (Stop == 1)
                                                                {
                                                                    break;
                                                                }
                                                                if (Whome)
                                                                {
                                                                    HOME();
                                                                    break;
                                                                }
                                                                if (Wshutdown)
                                                                {
                                                                    ShutDown();
                                                                    break;
                                                                }
                                                                if (Wenable)
                                                                {
                                                                    Enable();
                                                                    break;
                                                                }
                                                                if (reset)
                                                                {
                                                                    Reset();
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                        {
                                                            break;
                                                        }
                                                    }
                                                }
                                                t = 0;
                                                T = false;
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                            }
                                            else if (Command1 == "SDOON")
                                            {
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine(j);
                                                }));
                                                SDOON = Convert.ToInt16(coordinateAxis1[4]);
                                                #region ON
                                                switch (SDOON)
                                                {
                                                    case 0:
                                                        temp1[7] = '1';
                                                        break;
                                                    case 1:
                                                        temp1[6] = '1';
                                                        break;
                                                    case 2:
                                                        temp1[5] = '1';
                                                        break;
                                                    case 3:
                                                        temp1[4] = '1';
                                                        break;
                                                    case 4:
                                                        temp1[3] = '1';
                                                        break;
                                                    case 5:
                                                        temp1[2] = '1';
                                                        break;
                                                    case 6:
                                                        temp1[1] = '1';
                                                        break;
                                                    case 7:
                                                        temp1[0] = '1';
                                                        break;
                                                    case 8:
                                                        temp2[7] = '1';
                                                        break;
                                                    case 9:
                                                        temp2[6] = '1';
                                                        break;
                                                    case 10:
                                                        temp2[5] = '1';
                                                        break;
                                                    case 11:
                                                        temp2[4] = '1';
                                                        break;
                                                    case 12:
                                                        temp2[3] = '1';
                                                        break;
                                                    case 13:
                                                        temp2[3] = '1';
                                                        break;
                                                    case 14:
                                                        temp2[1] = '1';
                                                        break;
                                                    case 15:
                                                        temp2[0] = '1';
                                                        break;

                                                }

                                                #endregion
                                                build_data();
                                                short[] value = new short[2];
                                                value[0] = BinaryToShort(data_write1);
                                                value[1] = BinaryToShort(data_Write2);
                                                WRITE_IO[0] = value[0];
                                                WRITE_IO[1] = value[1];
                                                Thread.Sleep(new TimeSpan(10000));
                                                _RS485.SendFc15(6, 0, 16, WRITE_IO);
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                            }
                                            else if (Command1 == "SDOOF")
                                            {
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine(j);
                                                }));
                                                SDOOF = Convert.ToInt16(coordinateAxis1[4]);
                                                #region OF
                                                switch (SDOOF)
                                                {
                                                    case 0:
                                                        temp1[7] = '0';
                                                        break;
                                                    case 1:
                                                        temp1[6] = '0';
                                                        break;
                                                    case 2:
                                                        temp1[5] = '0';
                                                        break;
                                                    case 3:
                                                        temp1[4] = '0';
                                                        break;
                                                    case 4:
                                                        temp1[3] = '0';
                                                        break;
                                                    case 5:
                                                        temp1[2] = '0';
                                                        break;
                                                    case 6:
                                                        temp1[1] = '0';
                                                        break;
                                                    case 7:
                                                        temp1[0] = '0';
                                                        break;
                                                    case 8:
                                                        temp2[7] = '0';
                                                        break;
                                                    case 9:
                                                        temp2[6] = '0';
                                                        break;
                                                    case 10:
                                                        temp2[5] = '0';
                                                        break;
                                                    case 11:
                                                        temp2[4] = '0';
                                                        break;
                                                    case 12:
                                                        temp2[3] = '0';
                                                        break;
                                                    case 13:
                                                        temp2[3] = '0';
                                                        break;
                                                    case 14:
                                                        temp2[1] = '0';
                                                        break;
                                                    case 15:
                                                        temp2[0] = '0';
                                                        break;

                                                }
                                                #endregion
                                                build_data();
                                                short[] value = new short[2];
                                                value[0] = BinaryToShort(data_write1);
                                                value[1] = BinaryToShort(data_Write2);
                                                WRITE_IO[0] = value[0];
                                                WRITE_IO[1] = value[1];
                                                Thread.Sleep(new TimeSpan(10000));
                                                _RS485.SendFc15(6, 0, 16, WRITE_IO);
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                            }
                                            else if (Command1 == "WDION")
                                            {
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine(j);
                                                }));
                                                WDION = Convert.ToInt16(coordinateAxis1[4]);

                                                Thread.Sleep(new TimeSpan(10000));
                                                _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    var output1 = Convert.ToString(READ_IO[0], 2);
                                                    var output2 = Convert.ToString(READ_IO[1], 2);

                                                    string out1 = output1.PadLeft(8, '0');
                                                    string out2 = output2.PadLeft(8, '0');
                                                    for (int O = 0; O < 8; O++)
                                                    {
                                                        out_put1[O] = out1[O];
                                                        out_put2[O] = out2[O];
                                                    }
                                                }));
                                                if (Stop == 1 || Pause == 1)
                                                {
                                                    while (Pause == 1)
                                                    {
                                                        if (Stop == 1)
                                                        {
                                                            break;
                                                        }
                                                        if (Whome)
                                                        {
                                                            HOME();
                                                            break;
                                                        }
                                                        if (Wshutdown)
                                                        {
                                                            ShutDown();
                                                            break;
                                                        }
                                                        if (Wenable)
                                                        {
                                                            Enable();
                                                            break;
                                                        }
                                                        if (reset)
                                                        {
                                                            Reset();
                                                            break;
                                                        }
                                                    }
                                                    if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                    {
                                                        break;
                                                    }
                                                }
                                                if (WDION <= 7)
                                                {
                                                    if (WDION == 7) WDION = 0;
                                                    else if (WDION == 6) WDION = 1;
                                                    else if (WDION == 5) WDION = 2;
                                                    else if (WDION == 4) WDION = 3;
                                                    else if (WDION == 3) WDION = 4;
                                                    else if (WDION == 2) WDION = 5;
                                                    else if (WDION == 1) WDION = 6;
                                                    else if (WDION == 0) WDION = 7;
                                                    while (!(out_put1[WDION] == '1'))
                                                    {
                                                        Thread.Sleep(new TimeSpan(10000));
                                                        _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            var output1 = Convert.ToString(READ_IO[0], 2);
                                                            string out1 = output1.PadLeft(8, '0');
                                                            for (int O = 0; O < 8; O++)
                                                            {
                                                                out_put1[O] = out1[O];
                                                            }
                                                        }));
                                                        if (Wshutdown)
                                                        {
                                                            ShutDown();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (Wenable)
                                                        {
                                                            Enable();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (reset)
                                                        {
                                                            Reset();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (Stop == 1 || Pause == 1)
                                                        {
                                                            while (Pause == 1)
                                                            {
                                                                if (Stop == 1)
                                                                {
                                                                    break;
                                                                }
                                                                if (Whome)
                                                                {
                                                                    HOME();
                                                                    break;
                                                                }
                                                                if (Wshutdown)
                                                                {
                                                                    ShutDown();
                                                                    break;
                                                                }
                                                                if (Wenable)
                                                                {
                                                                    Enable();
                                                                    break;
                                                                }
                                                                if (reset)
                                                                {
                                                                    Reset();
                                                                    break;
                                                                }
                                                            }
                                                            if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                            {
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (WDION > 7)
                                                {
                                                    if (WDION == 15) WDION = 0;
                                                    else if (WDION == 14) WDION = 1;
                                                    else if (WDION == 13) WDION = 2;
                                                    else if (WDION == 12) WDION = 3;
                                                    else if (WDION == 11) WDION = 4;
                                                    else if (WDION == 10) WDION = 5;
                                                    else if (WDION == 9) WDION = 6;
                                                    else if (WDION == 8) WDION = 7;
                                                    while (!(out_put2[WDION] == '1'))
                                                    {
                                                        Thread.Sleep(new TimeSpan(10000));
                                                        _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            var output2 = Convert.ToString(READ_IO[1], 2);

                                                            string out2 = output2.PadLeft(8, '0');
                                                            for (int O = 0; O < 8; O++)
                                                            {
                                                                out_put2[O] = out2[O];
                                                            }
                                                        }));
                                                        if (Wshutdown)
                                                        {
                                                            ShutDown();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (Wenable)
                                                        {
                                                            Enable();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (reset)
                                                        {
                                                            Reset();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (Stop == 1 || Pause == 1)
                                                        {
                                                            while (Pause == 1)
                                                            {
                                                                if (Stop == 1)
                                                                {
                                                                    break;
                                                                }
                                                                if (Whome)
                                                                {
                                                                    HOME();
                                                                    break;
                                                                }
                                                                if (Wshutdown)
                                                                {
                                                                    ShutDown();
                                                                    break;
                                                                }
                                                                if (Wenable)
                                                                {
                                                                    Enable();
                                                                    break;
                                                                }
                                                                if (reset)
                                                                {
                                                                    Reset();
                                                                    break;
                                                                }
                                                            }
                                                            if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                            {
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                            }
                                            else if (Command1 == "WDIOF")
                                            {
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine(j);
                                                }));
                                                WDIOF = Convert.ToInt16(coordinateAxis1[4]);
                                                Thread.Sleep(new TimeSpan(10000));
                                                _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    var output1 = Convert.ToString(READ_IO[0], 2);
                                                    var output2 = Convert.ToString(READ_IO[1], 2);

                                                    string out1 = output1.PadLeft(8, '0');
                                                    string out2 = output2.PadLeft(8, '0');
                                                    for (int O = 0; O < 8; O++)
                                                    {
                                                        out_put1[O] = out1[O];
                                                        out_put2[O] = out2[O];
                                                    }
                                                }));
                                                if (Stop == 1 || Pause == 1)
                                                {
                                                    while (Pause == 1)
                                                    {
                                                        if (Stop == 1)
                                                        {
                                                            break;
                                                        }
                                                        if (Whome)
                                                        {
                                                            HOME();
                                                            break;
                                                        }
                                                        if (Wshutdown)
                                                        {
                                                            ShutDown();
                                                            break;
                                                        }
                                                        if (Wenable)
                                                        {
                                                            Enable();
                                                            break;
                                                        }
                                                        if (reset)
                                                        {
                                                            Reset();
                                                            break;
                                                        }
                                                    }
                                                    if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                    {
                                                        break;
                                                    }
                                                }
                                                if (WDIOF <= 7)
                                                {
                                                    if (WDIOF == 7) WDIOF = 0;
                                                    else if (WDIOF == 6) WDIOF = 1;
                                                    else if (WDIOF == 5) WDIOF = 2;
                                                    else if (WDIOF == 4) WDIOF = 3;
                                                    else if (WDIOF == 3) WDIOF = 4;
                                                    else if (WDIOF == 2) WDIOF = 5;
                                                    else if (WDIOF == 1) WDIOF = 6;
                                                    else if (WDIOF == 0) WDIOF = 7;
                                                    while (!(out_put1[WDIOF] == '0'))
                                                    {
                                                        Thread.Sleep(new TimeSpan(10000));
                                                        _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            var output1 = Convert.ToString(READ_IO[0], 2);

                                                            string out1 = output1.PadLeft(8, '0');
                                                            for (int O = 0; O < 8; O++)
                                                            {
                                                                out_put1[O] = out1[O];
                                                            }
                                                        }));
                                                        if (Wshutdown)
                                                        {
                                                            ShutDown();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (Wenable)
                                                        {
                                                            Enable();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (reset)
                                                        {
                                                            Reset();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (Stop == 1 || Pause == 1)
                                                        {
                                                            while (Pause == 1)
                                                            {
                                                                if (Stop == 1)
                                                                {
                                                                    break;
                                                                }
                                                                if (Whome)
                                                                {
                                                                    HOME();
                                                                    break;
                                                                }
                                                                if (Wshutdown)
                                                                {
                                                                    ShutDown();
                                                                    break;
                                                                }
                                                                if (Wenable)
                                                                {
                                                                    Enable();
                                                                    break;
                                                                }
                                                                if (reset)
                                                                {
                                                                    Reset();
                                                                    break;
                                                                }
                                                            }
                                                            if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                            {
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                if (WDIOF > 7)
                                                {
                                                    if (WDIOF == 15) WDIOF = 0;
                                                    else if (WDIOF == 14) WDIOF = 1;
                                                    else if (WDIOF == 13) WDIOF = 2;
                                                    else if (WDIOF == 12) WDIOF = 3;
                                                    else if (WDIOF == 10) WDIOF = 5;
                                                    else if (WDIOF == 9) WDIOF = 6;
                                                    else if (WDIOF == 8) WDIOF = 7;
                                                    while (!(out_put2[WDIOF] == '0'))
                                                    {
                                                        Thread.Sleep(new TimeSpan(10000));
                                                        _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            var output2 = Convert.ToString(READ_IO[1], 2);

                                                            string out2 = output2.PadLeft(8, '0');
                                                            for (int O = 0; O < 8; O++)
                                                            {
                                                                out_put2[O] = out2[O];
                                                            }
                                                        }));
                                                        if (Wshutdown)
                                                        {
                                                            ShutDown();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (Wenable)
                                                        {
                                                            Enable();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (reset)
                                                        {
                                                            Reset();
                                                            Dispatcher.Invoke(new Action(delegate
                                                            {
                                                                SelectLine1(j);
                                                            }));
                                                            break;
                                                        }
                                                        if (Stop == 1 || Pause == 1)
                                                        {
                                                            while (Pause == 1)
                                                            {
                                                                if (Stop == 1)
                                                                {
                                                                    break;
                                                                }
                                                                if (Whome)
                                                                {
                                                                    HOME();
                                                                    break;
                                                                }
                                                                if (Wshutdown)
                                                                {
                                                                    ShutDown();
                                                                    break;
                                                                }
                                                                if (Wenable)
                                                                {
                                                                    Enable();
                                                                    break;
                                                                }
                                                                if (reset)
                                                                {
                                                                    Reset();
                                                                    break;
                                                                }
                                                            }
                                                            if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                            {
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                            }
                                            else if (Command1 == "ENDREPEAT")
                                            {
                                                times++;
                                                a = j;
                                                break;
                                            }
                                        }

                                    }
                                }
                                if (times == 1)
                                {
                                    for (int n = i; n < a; n++)
                                    {
                                        sr.ReadLine();
                                    }
                                }
                                if (times == Times)
                                {
                                    i = a;
                                }

                            } while (times != Times);
                        }
                        else if (Command == "DELAY")
                        {
                            Dispatcher.Invoke(new Action(delegate
                            {
                                SelectLine(i);
                            }));
                            Delay = Convert.ToInt16(coordinateAxis[4]);
                            t = 0;
                            T = true;
                            while (!(t >= Delay))
                            {
                                if (Wshutdown)
                                {
                                    ShutDown();
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        SelectLine1(i);
                                    }));
                                    break;
                                }
                                if (Wenable)
                                {
                                    Enable();
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        SelectLine1(i);
                                    }));
                                    break;
                                }
                                if (reset)
                                {
                                    Reset();
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        SelectLine1(i);
                                    }));
                                    break;
                                }
                                if (Pause == 1 || Stop == 1)
                                {
                                    if (Pause == 1)
                                    {
                                        Dispatcher.Invoke(new Action(delegate
                                        {
                                            Timerdelay.Content = t.ToString();
                                        }));
                                        while (Pause == 1)
                                        {
                                            if (Stop == 1)
                                            {
                                                break;
                                            }
                                            if (Whome)
                                            {
                                                HOME();
                                                break;
                                            }
                                            if (Wshutdown)
                                            {
                                                ShutDown();
                                                break;
                                            }
                                            if (Wenable)
                                            {
                                                Enable();
                                                break;
                                            }
                                            if (reset)
                                            {
                                                Reset();
                                                break;
                                            }
                                        }
                                    }
                                    if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                    {
                                        break;
                                    }
                                }
                            }
                            t = 0;
                            T = false;
                            Dispatcher.Invoke(new Action(delegate
                            {
                                SelectLine1(i);
                            }));
                        }
                        else if (Command == "SDOON")
                        {
                            Dispatcher.Invoke(new Action(delegate
                            {
                                SelectLine(i);
                            }));
                            SDOON = Convert.ToInt16(coordinateAxis[4]);
                            #region ON
                            if (SDOON == 0)
                            {
                                temp1[7] = '1';
                            }
                            else if (SDOON == 1)
                            {
                                temp1[6] = '1';
                            }
                            else if (SDOON == 2)
                            {
                                temp1[5] = '1';
                            }
                            else if (SDOON == 3)
                            {
                                temp1[4] = '1';
                            }
                            else if (SDOON == 4)
                            {
                                temp1[3] = '1';
                            }
                            else if (SDOON == 5)
                            {
                                temp1[2] = '1';
                            }
                            else if (SDOON == 6)
                            {
                                temp1[1] = '1';
                            }
                            else if (SDOON == 7)
                            {
                                temp1[0] = '1';
                            }
                            else if (SDOON == 8)
                            {
                                temp2[7] = '1';
                            }
                            else if (SDOON == 9)
                            {
                                temp2[6] = '1';
                            }
                            else if (SDOON == 10)
                            {
                                temp2[5] = '1';
                            }
                            else if (SDOON == 11)
                            {
                                temp2[4] = '1';
                            }
                            else if (SDOON == 12)
                            {
                                temp2[3] = '1';
                            }
                            else if (SDOON == 13)
                            {
                                temp2[2] = '1';
                            }
                            else if (SDOON == 14)
                            {
                                temp2[1] = '1';
                            }
                            else if (SDOON == 15)
                            {
                                temp2[0] = '1';
                            }
                            #endregion
                            build_data();
                            short[] value = new short[2];
                            value[0] = BinaryToShort(data_write1);
                            value[1] = BinaryToShort(data_Write2);
                            WRITE_IO[0] = value[0];
                            WRITE_IO[1] = value[1];
                            _RS485.SendFc15(6, 0, 16, WRITE_IO);
                            Dispatcher.Invoke(new Action(delegate
                            {
                                SelectLine1(i);
                            }));
                        }
                        else if (Command == "SDOOF")
                        {
                            Dispatcher.Invoke(new Action(delegate
                            {
                                SelectLine(i);
                            }));
                            SDOOF = Convert.ToInt16(coordinateAxis[4]);
                            #region OF
                            if (SDOOF == 0)
                            {
                                temp1[7] = '0';
                            }
                            else if (SDOOF == 1)
                            {
                                temp1[6] = '0';
                            }
                            else if (SDOOF == 2)
                            {
                                temp1[5] = '0';
                            }
                            else if (SDOOF == 3)
                            {
                                temp1[4] = '0';
                            }
                            else if (SDOOF == 4)
                            {
                                temp1[3] = '0';
                            }
                            else if (SDOOF == 5)
                            {
                                temp1[2] = '0';
                            }
                            else if (SDOOF == 6)
                            {
                                temp1[1] = '0';
                            }
                            else if (SDOOF == 7)
                            {
                                temp1[0] = '0';
                            }
                            else if (SDOOF == 8)
                            {
                                temp2[7] = '0';
                            }
                            else if (SDOOF == 9)
                            {
                                temp2[6] = '0';
                            }
                            else if (SDOOF == 10)
                            {
                                temp2[5] = '0';
                            }
                            else if (SDOOF == 11)
                            {
                                temp2[4] = '0';
                            }
                            else if (SDOOF == 12)
                            {
                                temp2[3] = '0';
                            }
                            else if (SDOOF == 13)
                            {
                                temp2[2] = '0';
                            }
                            else if (SDOOF == 14)
                            {
                                temp2[1] = '0';
                            }
                            else if (SDOOF == 15)
                            {
                                temp2[0] = '0';
                            }
                            #endregion
                            build_data();
                            short[] value = new short[2];
                            value[0] = BinaryToShort(data_write1);
                            value[1] = BinaryToShort(data_Write2);
                            WRITE_IO[0] = value[0];
                            WRITE_IO[1] = value[1];
                            _RS485.SendFc15(6, 0, 16, WRITE_IO);
                            Dispatcher.Invoke(new Action(delegate
                            {
                                SelectLine1(i);
                            }));
                        }
                        else if (Command == "WDION")
                        {
                            Dispatcher.Invoke(new Action(delegate
                            {
                                SelectLine(i);
                            }));
                            WDION = Convert.ToInt16(coordinateAxis[4]);
                            _RS485.SendFc02(6, 0, 16, ref READ_IO);
                            Dispatcher.Invoke(new Action(delegate
                            {
                                var output1 = Convert.ToString(READ_IO[0], 2);
                                var output2 = Convert.ToString(READ_IO[1], 2);
                                string out1 = output1.PadLeft(8, '0');
                                string out2 = output2.PadLeft(8, '0');
                                for (int O = 0; O < 8; O++)
                                {
                                    out_put1[O] = out1[O];
                                    out_put2[O] = out2[O];
                                }
                            }));
                            if (Stop == 1 || Pause == 1)
                            {
                                while (Pause == 1)
                                {
                                    if (Stop == 1)
                                    {
                                        break;
                                    }
                                    if (Whome)
                                    {
                                        HOME();
                                        break;
                                    }
                                    if (Wshutdown)
                                    {
                                        ShutDown();
                                        break;
                                    }
                                    if (Wenable)
                                    {
                                        Enable();
                                        break;
                                    }
                                    if (reset)
                                    {
                                        Reset();
                                        break;
                                    }
                                }
                                if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                {
                                    break;
                                }
                            }
                            if (WDION <= 7)
                            {
                                if (WDION == 7) WDION = 0;
                                else if (WDION == 6) WDION = 1;
                                else if (WDION == 5) WDION = 2;
                                else if (WDION == 4) WDION = 3;
                                else if (WDION == 3) WDION = 4;
                                else if (WDION == 2) WDION = 5;
                                else if (WDION == 1) WDION = 6;
                                else if (WDION == 0) WDION = 7;
                                while (!(out_put1[WDION] == '1'))
                                {
                                    _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        var output1 = Convert.ToString(READ_IO[0], 2);
                                        string out1 = output1.PadLeft(8, '0');
                                        for (int O = 0; O < 8; O++)
                                        {
                                            out_put1[O] = out1[O];
                                        }
                                    }));
                                    if (Wshutdown)
                                    {
                                        ShutDown();
                                        Dispatcher.Invoke(new Action(delegate
                                        {
                                            SelectLine1(i);
                                        }));
                                        break;
                                    }
                                    if (Wenable)
                                    {
                                        Enable();
                                        Dispatcher.Invoke(new Action(delegate
                                        {
                                            SelectLine1(i);
                                        }));
                                        break;
                                    }
                                    if (reset)
                                    {
                                        Reset();
                                        Dispatcher.Invoke(new Action(delegate
                                        {
                                            SelectLine1(i);
                                        }));
                                        break;
                                    }
                                    if (Stop == 1 || Pause == 1)
                                    {
                                        while (Pause == 1)
                                        {
                                            if (Stop == 1)
                                            {
                                                break;
                                            }
                                            if (Whome)
                                            {
                                                HOME();
                                                break;
                                            }
                                            if (Wshutdown)
                                            {
                                                ShutDown();
                                                break;
                                            }
                                            if (Wenable)
                                            {
                                                Enable();
                                                break;
                                            }
                                            if (reset)
                                            {
                                                Reset();
                                                break;
                                            }
                                        }
                                        if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            if (WDION > 7)
                            {
                                if (WDION == 15) WDION = 0;
                                else if (WDION == 14) WDION = 1;
                                else if (WDION == 13) WDION = 2;
                                else if (WDION == 12) WDION = 3;
                                else if (WDION == 11) WDION = 4;
                                else if (WDION == 10) WDION = 5;
                                else if (WDION == 9) WDION = 6;
                                else if (WDION == 8) WDION = 7;
                                while (!(out_put2[WDION] == '1'))
                                {
                                    _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        var output2 = Convert.ToString(READ_IO[1], 2);
                                        string out2 = output2.PadLeft(8, '0');
                                        for (int O = 0; O < 8; O++)
                                        {
                                            out_put2[O] = out2[O];
                                        }
                                    }));
                                    if (Wshutdown)
                                    {
                                        ShutDown();
                                        Dispatcher.Invoke(new Action(delegate
                                        {
                                            SelectLine1(i);
                                        }));
                                        break;
                                    }
                                    if (Wenable)
                                    {
                                        Enable();
                                        Dispatcher.Invoke(new Action(delegate
                                        {
                                            SelectLine1(i);
                                        }));
                                        break;
                                    }
                                    if (reset)
                                    {
                                        Reset();
                                        Dispatcher.Invoke(new Action(delegate
                                        {
                                            SelectLine1(i);
                                        }));
                                        break;
                                    }
                                    if (Stop == 1 || Pause == 1)
                                    {
                                        while (Pause == 1)
                                        {
                                            if (Stop == 1)
                                            {
                                                break;
                                            }
                                            if (Whome)
                                            {
                                                HOME();
                                                break;
                                            }
                                            if (Wshutdown)
                                            {
                                                ShutDown();
                                                break;
                                            }
                                            if (Wenable)
                                            {
                                                Enable();
                                                break;
                                            }
                                            if (reset)
                                            {
                                                Reset();
                                                break;
                                            }
                                        }
                                        if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            Dispatcher.Invoke(new Action(delegate
                            {
                                SelectLine1(i);
                            }));
                        }
                        else if (Command == "WDIOF")
                        {
                            Dispatcher.Invoke(new Action(delegate
                            {
                                SelectLine(i);
                            }));
                            WDIOF = Convert.ToInt16(coordinateAxis[4]);
                            _RS485.SendFc02(6, 0, 16, ref READ_IO);
                            Dispatcher.Invoke(new Action(delegate
                            {
                                var output1 = Convert.ToString(READ_IO[0], 2);
                                var output2 = Convert.ToString(READ_IO[1], 2);

                                string out1 = output1.PadLeft(8, '0');
                                string out2 = output2.PadLeft(8, '0');
                                for (int O = 0; O < 8; O++)
                                {
                                    out_put1[O] = out1[O];
                                    out_put2[O] = out2[O];
                                }
                            }));
                            if (Stop == 1 || Pause == 1)
                            {
                                while (Pause == 1)
                                {
                                    if (Stop == 1)
                                    {
                                        break;
                                    }
                                    if (Whome)
                                    {
                                        HOME();
                                        break;
                                    }
                                    if (Wshutdown)
                                    {
                                        ShutDown();
                                        break;
                                    }
                                    if (Wenable)
                                    {
                                        Enable();
                                        break;
                                    }
                                    if (reset)
                                    {
                                        Reset();
                                        break;
                                    }
                                }
                                if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                {
                                    break;
                                }
                            }
                            if (WDIOF <= 7)
                            {
                                if (WDIOF == 7) WDIOF = 0;
                                else if (WDIOF == 6) WDIOF = 1;
                                else if (WDIOF == 5) WDIOF = 2;
                                else if (WDIOF == 4) WDIOF = 3;
                                else if (WDIOF == 3) WDIOF = 4;
                                else if (WDIOF == 2) WDIOF = 5;
                                else if (WDIOF == 1) WDIOF = 6;
                                else if (WDIOF == 0) WDIOF = 7;
                                while (!(out_put1[WDIOF] == '0'))
                                {
                                    _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        var output1 = Convert.ToString(READ_IO[0], 2);

                                        string out1 = output1.PadLeft(8, '0');
                                        for (int O = 0; O < 8; O++)
                                        {
                                            out_put1[O] = out1[O];
                                        }
                                    }));
                                    if (Wshutdown)
                                    {
                                        ShutDown();
                                        Dispatcher.Invoke(new Action(delegate
                                        {
                                            SelectLine1(i);
                                        }));
                                        break;
                                    }
                                    if (Wenable)
                                    {
                                        Enable();
                                        Dispatcher.Invoke(new Action(delegate
                                        {
                                            SelectLine1(i);
                                        }));
                                        break;
                                    }
                                    if (reset)
                                    {
                                        Reset();
                                        Dispatcher.Invoke(new Action(delegate
                                        {
                                            SelectLine1(i);
                                        }));
                                        break;
                                    }
                                    if (Stop == 1 || Pause == 1)
                                    {
                                        while (Pause == 1)
                                        {
                                            if (Stop == 1)
                                            {
                                                break;
                                            }
                                            if (Whome)
                                            {
                                                HOME();
                                                break;
                                            }
                                            if (Wshutdown)
                                            {
                                                ShutDown();
                                                break;
                                            }
                                            if (Wenable)
                                            {
                                                Enable();
                                                break;
                                            }
                                            if (reset)
                                            {
                                                Reset();
                                                break;
                                            }
                                        }
                                        if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            if (WDIOF > 7)
                            {
                                if (WDIOF == 15) WDIOF = 0;
                                else if (WDIOF == 14) WDIOF = 1;
                                else if (WDIOF == 13) WDIOF = 2;
                                else if (WDIOF == 12) WDIOF = 3;
                                else if (WDIOF == 11) WDIOF = 4;
                                else if (WDIOF == 10) WDIOF = 5;
                                else if (WDIOF == 9) WDIOF = 6;
                                else if (WDIOF == 8) WDIOF = 7;
                                while (!(out_put2[WDIOF] == '0'))
                                {
                                    _RS485.SendFc02(6, 0, 16, ref READ_IO);
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        var output2 = Convert.ToString(READ_IO[1], 2);

                                        string out2 = output2.PadLeft(8, '0');
                                        for (int O = 0; O < 8; O++)
                                        {
                                            out_put2[O] = out2[O];
                                        }
                                    }));
                                    if (Wshutdown)
                                    {
                                        ShutDown();
                                        Dispatcher.Invoke(new Action(delegate
                                        {
                                            SelectLine1(i);
                                        }));
                                        break;
                                    }
                                    if (Wenable)
                                    {
                                        Enable();
                                        Dispatcher.Invoke(new Action(delegate
                                        {
                                            SelectLine1(i);
                                        }));
                                        break;
                                    }
                                    if (reset)
                                    {
                                        Reset();
                                        Dispatcher.Invoke(new Action(delegate
                                        {
                                            SelectLine1(i);
                                        }));
                                        break;
                                    }
                                    if (Stop == 1 || Pause == 1)
                                    {
                                        while (Pause == 1)
                                        {
                                            if (Stop == 1)
                                            {
                                                break;
                                            }
                                            if (Whome)
                                            {
                                                HOME();
                                                break;
                                            }
                                            if (Wshutdown)
                                            {
                                                ShutDown();
                                                break;
                                            }
                                            if (Wenable)
                                            {
                                                Enable();
                                                break;
                                            }
                                            if (reset)
                                            {
                                                Reset();
                                                break;
                                            }
                                        }
                                        if (Stop == 1 || Whome || Wshutdown | Wenable || reset)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            Dispatcher.Invoke(new Action(delegate
                            {
                                SelectLine1(i);
                            }));
                        }
                    }
                    Dispatcher.Invoke(new Action(delegate
                    {
                        PlayRun.IsEnabled = true;
                        OpenFileRun.IsEnabled = true;
                        PauseRun.IsEnabled = false;
                        StopRun.IsEnabled = false;
                    }));
                }
            }
            while (Loop == true);
        }
        private void ReadLIST()
        {

            do
            {
                int i = -1;
                if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                {
                    break;
                }

                foreach (string item in testprogram)
                {
                    i++;
                    if (Whome || Wshutdown || Wenable || reset || Stop == 1) { Whome = false; Wshutdown = false; Wenable = false; reset = false; break; }
                    string[] coordinateAxis = item.Split('.', ':', ' ');
                    Command = coordinateAxis[3];
                    if (Command == "MOVE")
                    {
                        X = Convert.ToInt16(coordinateAxis[5]);
                        Y1 = Convert.ToInt16(coordinateAxis[7]);
                        Y2 = Convert.ToInt16(coordinateAxis[9]);
                        Z1 = Convert.ToInt16(coordinateAxis[11]);
                        Z2 = Convert.ToInt16(coordinateAxis[13]);
                        SpeedRun[0] = Convert.ToInt16(coordinateAxis[15]);
                        AccelRun[0] = Convert.ToInt16(((SpeedRun[0]) * 30) / 100);
                        DecelRun[0] = Convert.ToInt16(((SpeedRun[0]) * 30) / 100);
                        short[] PointdataRunX = Get_PointData(X);
                        short[] PointdataRunY1 = Get_PointData(Y1);
                        short[] PointdataRunY2 = Get_PointData(Y2);
                        short[] PointdataRunZ1 = Get_PointData(Z1);
                        short[] PointdataRunZ2 = Get_PointData(Z2);
                        PointX[0] = PointdataRunX[0];
                        PointX[1] = PointdataRunX[1];
                        PointY1[0] = PointdataRunY1[0];
                        PointY1[1] = PointdataRunY1[1];
                        PointY2[0] = PointdataRunY2[0];
                        PointY2[1] = PointdataRunY2[1];
                        PointZ1[0] = PointdataRunZ1[0];
                        PointZ1[1] = PointdataRunZ1[1];
                        PointZ2[0] = PointdataRunZ2[0];
                        PointZ2[1] = PointdataRunZ2[1];
                        Dispatcher.Invoke(new Action(delegate
                        {
                            SelectLine(i);

                        }));

                        _RS485.SendFc16(1, 24640, 1, new short[1] { 15 });
                        _RS485.SendFc16(2, 24640, 1, new short[1] { 15 });
                        //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
                        //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
                        //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });
                        _RS485.SendFc16(1, 24672, 1, new short[1] { -101 });
                        _RS485.SendFc16(2, 24672, 1, new short[1] { -101 });
                        //_RS485.SendFc16(3, 24672, 1, new short[1] { -101 });
                        //_RS485.SendFc16(4, 24672, 1, new short[1] { -101 });
                        //_RS485.SendFc16(5, 24672, 1, new short[1] { -101 });
                        _RS485.SendFc16(1, 10241, 9, new short[9] { 1, PointX[0], PointX[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                        _RS485.SendFc16(2, 10241, 9, new short[9] { 1, PointY1[0], PointY1[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                        //_RS485.SendFc16(3, 10241, 9, new short[9] { 1, PointY2[0], PointY2[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                        //_RS485.SendFc16(4, 10241, 9, new short[9] { 1, PointZ1[0], PointZ1[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                        //_RS485.SendFc16(5, 10241, 9, new short[9] { 1, PointZ2[0], PointZ2[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                        _RS485.SendFc16(1, 11616, 1, new short[1] { 1 });
                        _RS485.SendFc16(2, 11616, 1, new short[1] { 1 });
                        //_RS485.SendFc16(3, 11616, 1, new short[1] { 1 });
                        //_RS485.SendFc16(4, 11616, 1, new short[1] { 1 });
                        //_RS485.SendFc16(5, 11616, 1, new short[1] { 1 });
                        _RS485.SendFc16(1, 24640, 1, new short[1] { 31 });
                        _RS485.SendFc16(2, 24640, 1, new short[1] { 31 });
                        //_RS485.SendFc16(3, 24640, 1, new short[1] { 31 });
                        //_RS485.SendFc16(4, 24640, 1, new short[1] { 31 });
                        //_RS485.SendFc16(5, 24640, 1, new short[1] { 31 });
                        while (!(R_Point_X[0] == PointX[0] && R_Point_X[1] == PointX[1]) || !(R_Point_Y1[0] == PointY1[0] && R_Point_Y1[1] == PointY1[1]))// || !(R_Point_Y2[0] == Y2) || !(R_Point_Z1[0] == Z1) || !(R_Point_Z2[0] == Z2))
                        {
                            _RS485.SendFc3(1, 11055, 2, ref R_Point_X);
                            _RS485.SendFc3(2, 11055, 2, ref R_Point_Y1);
                            //_RS485.SendFc3(3, 11055, 2, ref R_Point_Y2);
                            //_RS485.SendFc3(4, 11055, 2, ref R_Point_Z1);
                            //_RS485.SendFc3(5, 11055, 2, ref R_Point_Z2);
                            Dispatcher.Invoke(new Action(delegate
                            {
                                Point_XRun.Content = GetIntFromBits_Read(R_Point_X[0], R_Point_X[1]);
                                Point_Y1Run.Content = GetIntFromBits_Read(R_Point_Y1[0], R_Point_Y1[1]);
                                //Point_Y2Run.Content = R_Point_Y2[0].ToString();
                                //Point_Z1Run.Content = R_Point_Z1[0].ToString();
                                //Point_Z2Run.Content = R_Point_Z2[0].ToString();
                                VelocityRun.Content = SpeedRun[0].ToString();
                            }));
                            if (Wshutdown)
                            {
                                ShutDown();
                                Dispatcher.Invoke(new Action(delegate
                                {
                                    //    SelectLine1(i);
                                }));
                                break;
                            }
                            if (Wenable)
                            {
                                Enable();
                                Dispatcher.Invoke(new Action(delegate
                                {
                                    // SelectLine1(i);
                                }));
                                break;
                            }
                            if (reset)
                            {
                                Reset();
                                Dispatcher.Invoke(new Action(delegate
                                {
                                    //  SelectLine1(i);
                                }));
                                break;
                            }
                            if (Pause == 1 || Stop == 1)
                            {
                                if (Pause == 1)
                                {
                                    _RS485.SendFc16(1, 24640, 1, new short[1] { 271 });
                                    _RS485.SendFc16(2, 24640, 1, new short[1] { 271 });
                                    //_RS485.SendFc16(3, 24640, 1, new short[1] { 271 });
                                    //_RS485.SendFc16(4, 24640, 1, new short[1] { 271 });
                                    //_RS485.SendFc16(5, 24640, 1, new short[1] { 271 });
                                    while (Pause == 1)
                                    {
                                        _RS485.SendFc3(1, 11055, 2, ref R_Point_X);
                                        _RS485.SendFc3(2, 11055, 2, ref R_Point_Y1);
                                        //_RS485.SendFc3(3, 11055, 2, ref R_Point_Y2);
                                        //_RS485.SendFc3(4, 11055, 2, ref R_Point_Z1);
                                        //_RS485.SendFc3(5, 11055, 2, ref R_Point_Z2);
                                        Dispatcher.Invoke(new Action(delegate
                                        {
                                            Point_XRun.Content = GetIntFromBits_Read(R_Point_X[0], R_Point_X[1]);
                                            Point_Y1Run.Content = GetIntFromBits_Read(R_Point_Y1[0], R_Point_Y1[1]);
                                            //Point_Y2Run.Content = R_Point_Y2[0].ToString();
                                            //Point_Z1Run.Content = R_Point_Z1[0].ToString();
                                            //Point_Z2Run.Content = R_Point_Z2[0].ToString();
                                        }));
                                        if (Pause == 0)
                                        {
                                            _RS485.SendFc16(1, 24640, 1, new short[1] { 31 });
                                            _RS485.SendFc16(2, 24640, 1, new short[1] { 31 });
                                            //_RS485.SendFc16(3, 24640, 1, new short[1] { 31 });
                                            //_RS485.SendFc16(4, 24640, 1, new short[1] { 31 });
                                            //_RS485.SendFc16(5, 24640, 1, new short[1] { 31 });
                                        }
                                        if (Stop == 1)
                                        {
                                            _RS485.SendFc16(1, 24640, 1, new short[1] { 0 });
                                            _RS485.SendFc16(2, 24640, 1, new short[1] { 0 });
                                            //_RS485.SendFc16(3, 24640, 1, new short[1] { 0 });
                                            //_RS485.SendFc16(4, 24640, 1, new short[1] { 0 });
                                            //_RS485.SendFc16(5, 24640, 1, new short[1] { 0 });
                                            _RS485.SendFc16(1, 24640, 1, new short[1] { 15 });
                                            _RS485.SendFc16(2, 24640, 1, new short[1] { 15 });
                                            //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
                                            //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
                                            //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });
                                            Dispatcher.Invoke(new Action(delegate
                                            {
                                                SelectLine1(i);
                                            }));
                                            break;
                                        }
                                        if (Whome == true)
                                        {
                                            HOME();
                                            break;
                                        }
                                        if (Wshutdown)
                                        {
                                            ShutDown();
                                            break;
                                        }
                                        if (Wenable)
                                        {
                                            Enable();
                                            break;
                                        }
                                        if (reset)
                                        {
                                            Reset();
                                            break;
                                        }
                                    }
                                }
                                if (Stop == 1)
                                {
                                    _RS485.SendFc16(1, 24640, 1, new short[1] { 0 });
                                    _RS485.SendFc16(2, 24640, 1, new short[1] { 0 });
                                    //_RS485.SendFc16(3, 24640, 1, new short[1] { 0 });
                                    //_RS485.SendFc16(4, 24640, 1, new short[1] { 0 });
                                    //_RS485.SendFc16(5, 24640, 1, new short[1] { 0 });
                                    _RS485.SendFc16(1, 24640, 1, new short[1] { 15 });
                                    _RS485.SendFc16(2, 24640, 1, new short[1] { 15 });
                                    //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
                                    //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
                                    //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        SelectLine1(i);
                                    }));
                                    break;
                                }
                                if (Whome == true || Wshutdown || Wenable || reset)
                                {
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        SelectLine1(i);
                                    }));
                                    break;
                                }
                            }
                        }
                        if (R_Point_X[0] == PointX[0] && R_Point_X[1] == PointX[1] && R_Point_Y1[0] == PointY1[0] && R_Point_Y1[1] == PointY1[1])// && R_Point_Y2[0] == Y2 && R_Point_Z1[0] == Z1 && R_Point_Z2[0] == Z2)
                        {
                            Dispatcher.Invoke(new Action(delegate
                            {
                                SelectLine1(i);
                            }));
                        }
                    }
                    else if (Command == "LOOP")
                    {
                        do
                        {
                            int j = -1;
                            int k = 1;
                            if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                            {
                                break;
                            }

                            foreach (var iteml in testrun)
                            {
                                j++;
                                if (Whome || Stop == 1 || Wshutdown || Wenable || reset)
                                {
                                    break;
                                }
                                string[] coordinateAxis1 = iteml.Split('.', ':', ' '); ;
                                Command1 = coordinateAxis1[3];
                                if (Command1 == "L_MOVE")
                                {

                                    X = Convert.ToInt16(coordinateAxis1[5]);
                                    Y1 = Convert.ToInt16(coordinateAxis1[7]);
                                    Y2 = Convert.ToInt16(coordinateAxis1[9]);
                                    Z1 = Convert.ToInt16(coordinateAxis1[11]);
                                    Z2 = Convert.ToInt16(coordinateAxis1[13]);
                                    SpeedRun[0] = Convert.ToInt16(coordinateAxis1[15]);
                                    AccelRun[0] = Convert.ToInt16(((SpeedRun[0]) * 30) / 100);
                                    DecelRun[0] = Convert.ToInt16(((SpeedRun[0]) * 30) / 100);
                                    short[] PointdataRunX = Get_PointData(X);
                                    short[] PointdataRunY1 = Get_PointData(Y1);
                                    short[] PointdataRunY2 = Get_PointData(Y2);
                                    short[] PointdataRunZ1 = Get_PointData(Z1);
                                    short[] PointdataRunZ2 = Get_PointData(Z2);
                                    PointX[0] = PointdataRunX[0];
                                    PointX[1] = PointdataRunX[1];
                                    PointY1[0] = PointdataRunY1[0];
                                    PointY1[1] = PointdataRunY1[1];
                                    PointY2[0] = PointdataRunY2[0];
                                    PointY2[1] = PointdataRunY2[1];
                                    PointZ1[0] = PointdataRunZ1[0];
                                    PointZ1[1] = PointdataRunZ1[1];
                                    PointZ2[0] = PointdataRunZ2[0];
                                    PointZ2[1] = PointdataRunZ2[1];
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        SelectLine(j);
                                    }));
                                    _RS485.SendFc16(1, 24640, 1, new short[1] { 15 });
                                    _RS485.SendFc16(2, 24640, 1, new short[1] { 15 });
                                    //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
                                    //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
                                    //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });
                                    _RS485.SendFc16(1, 24672, 1, new short[1] { -101 });
                                    _RS485.SendFc16(2, 24672, 1, new short[1] { -101 });
                                    //_RS485.SendFc16(3, 24672, 1, new short[1] { -101 });
                                    //_RS485.SendFc16(4, 24672, 1, new short[1] { -101 });
                                    //_RS485.SendFc16(5, 24672, 1, new short[1] { -101 });
                                    _RS485.SendFc16(1, 10241, 9, new short[9] { 7, PointX[0], PointX[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                                    _RS485.SendFc16(1, 11616, 1, new short[1] { 1 });

                                    _RS485.SendFc16(2, 10241, 9, new short[9] { 7, PointY1[0], PointY1[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                                    _RS485.SendFc16(2, 11616, 1, new short[1] { 1 });
                                    //_RS485.SendFc16(3, 10241, 9, new short[9] { 1, PointY2[0], PointY2[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                                    //_RS485.SendFc16(4, 10241, 9, new short[9] { 1, PointZ1[0], PointZ1[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                                    //_RS485.SendFc16(5, 10241, 9, new short[9] { 1, PointZ2[0], PointZ2[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });






                                    //_RS485.SendFc16(3, 11616, 1, new short[1] { 1 });
                                    //_RS485.SendFc16(4, 11616, 1, new short[1] { 1 });
                                    //_RS485.SendFc16(5, 11616, 1, new short[1] { 1 });
                                    _RS485.SendFc16(1, 24640, 1, new short[1] { 31 });
                                    _RS485.SendFc16(2, 24640, 1, new short[1] { 31 });
                                    //_RS485.SendFc16(3, 24640, 1, new short[1] { 31 });
                                    //_RS485.SendFc16(4, 24640, 1, new short[1] { 31 });
                                    //_RS485.SendFc16(5, 24640, 1, new short[1] { 31 });
                                    while (!(R_Point_X[0] == PointX[0] && R_Point_X[1] == PointX[1]) || !(R_Point_Y1[0] == PointY1[0] && R_Point_Y1[1] == PointY1[1]))// || !(R_Point_Y2[0] == Y2) || !(R_Point_Z1[0] == Z1) || !(R_Point_Z2[0] == Z2))
                                    {
                                        _RS485.SendFc3(1, 11055, 2, ref R_Point_X);
                                        _RS485.SendFc3(2, 11055, 2, ref R_Point_Y1);

                                        //_RS485.SendFc3(3, 11055, 2, ref R_Point_Y2);
                                        //_RS485.SendFc3(4, 11055, 2, ref R_Point_Z1);
                                        //_RS485.SendFc3(5, 11055, 2, ref R_Point_Z2);
                                        Dispatcher.Invoke(new Action(delegate
                                        {
                                            Point_XRun.Content = GetIntFromBits_Read(R_Point_X[0], R_Point_X[1]);
                                            Point_Y1Run.Content = GetIntFromBits_Read(R_Point_Y1[0], R_Point_Y1[1]);
                                            //Point_Y2Run.Content = R_Point_Y2[0].ToString();
                                            //Point_Z1Run.Content = R_Point_Z1[0].ToString();
                                            //Point_Z2Run.Content = R_Point_Z2[0].ToString();
                                            VelocityRun.Content = SpeedRun[0].ToString();
                                        }));
                                        if (Wshutdown)
                                        {
                                            ShutDown();
                                            Dispatcher.Invoke(new Action(delegate
                                            {
                                                SelectLine1(j);
                                            }));
                                            break;
                                        }
                                        if (Wenable)
                                        {
                                            Enable();
                                            Dispatcher.Invoke(new Action(delegate
                                            {
                                                SelectLine1(j);
                                            }));
                                            break;
                                        }
                                        if (reset)
                                        {
                                            Reset();
                                            Dispatcher.Invoke(new Action(delegate
                                            {
                                                SelectLine1(j);
                                            }));
                                            break;
                                        }
                                        if (Pause == 1 || Stop == 1)
                                        {
                                            if (Pause == 1)
                                            {
                                                _RS485.SendFc16(1, 24640, 1, new short[1] { 271 });
                                                _RS485.SendFc16(2, 24640, 1, new short[1] { 271 });
                                                //_RS485.SendFc16(3, 24640, 1, new short[1] { 271 });
                                                //_RS485.SendFc16(4, 24640, 1, new short[1] { 271 });
                                                //_RS485.SendFc16(5, 24640, 1, new short[1] { 271 });
                                                while (Pause == 1)
                                                {
                                                    _RS485.SendFc3(1, 11055, 2, ref R_Point_X);
                                                    _RS485.SendFc3(2, 11055, 2, ref R_Point_Y1);
                                                    //_RS485.SendFc3(3, 11055, 2, ref R_Point_Y2);
                                                    //_RS485.SendFc3(4, 11055, 2, ref R_Point_Z1);
                                                    //_RS485.SendFc3(5, 11055, 2, ref R_Point_Z2);
                                                    Dispatcher.Invoke(new Action(delegate
                                                    {
                                                        Point_XRun.Content = GetIntFromBits_Read(R_Point_X[0], R_Point_X[1]);
                                                        Point_Y1Run.Content = GetIntFromBits_Read(R_Point_Y1[0], R_Point_Y1[1]);
                                                        //Point_Y2Run.Content = R_Point_Y2[0].ToString();
                                                        //Point_Z1Run.Content = R_Point_Z1[0].ToString();
                                                        //Point_Z2Run.Content = R_Point_Z2[0].ToString();
                                                    }));
                                                    if (Pause == 0)
                                                    {
                                                        _RS485.SendFc16(1, 24640, 1, new short[1] { 31 });
                                                        _RS485.SendFc16(2, 24640, 1, new short[1] { 31 });
                                                        //_RS485.SendFc16(3, 24640, 1, new short[1] { 31 });
                                                        //_RS485.SendFc16(4, 24640, 1, new short[1] { 31 });
                                                        //_RS485.SendFc16(5, 24640, 1, new short[1] { 31 });
                                                    }
                                                    if (Stop == 1)
                                                    {
                                                        _RS485.SendFc16(1, 24640, 1, new short[1] { 0 });
                                                        _RS485.SendFc16(2, 24640, 1, new short[1] { 0 });
                                                        //_RS485.SendFc16(3, 24640, 1, new short[1] { 0 });
                                                        //_RS485.SendFc16(4, 24640, 1, new short[1] { 0 });
                                                        //_RS485.SendFc16(5, 24640, 1, new short[1] { 0 });
                                                        _RS485.SendFc16(1, 24640, 1, new short[1] { 15 });
                                                        _RS485.SendFc16(2, 24640, 1, new short[1] { 15 });
                                                        //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
                                                        //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
                                                        //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });
                                                        Dispatcher.Invoke(new Action(delegate
                                                        {
                                                            SelectLine1(j);
                                                        }));
                                                        break;
                                                    }
                                                    if (Whome)
                                                    {
                                                        HOME();
                                                        break;
                                                    }
                                                    if (Wshutdown)
                                                    {
                                                        ShutDown();
                                                        break;
                                                    }
                                                    if (Wenable)
                                                    {
                                                        Enable();
                                                        break;
                                                    }
                                                    if (reset)
                                                    {
                                                        Reset();
                                                        break;
                                                    }
                                                }
                                            }
                                            if (Stop == 1)
                                            {
                                                _RS485.SendFc16(1, 24640, 1, new short[1] { 0 });
                                                _RS485.SendFc16(2, 24640, 1, new short[1] { 0 });
                                                //_RS485.SendFc16(3, 24640, 1, new short[1] { 0 });
                                                //_RS485.SendFc16(4, 24640, 1, new short[1] { 0 });
                                                //_RS485.SendFc16(5, 24640, 1, new short[1] { 0 });
                                                _RS485.SendFc16(1, 24640, 1, new short[1] { 15 });
                                                _RS485.SendFc16(2, 24640, 1, new short[1] { 15 });
                                                //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
                                                //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
                                                //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                                break;
                                            }
                                            if (Whome || Wshutdown || Wenable || reset)
                                            {
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                                break;
                                            }
                                        }
                                    }
                                    if (R_Point_X[0] == PointX[0] && R_Point_X[1] == PointX[1] && R_Point_Y1[0] == PointY1[0] && R_Point_Y1[1] == PointY1[1])// && R_Point_Y2[0] == Y2 && R_Point_Z1[0] == Z1 && R_Point_Z2[0] == Z2)
                                    {
                                        Dispatcher.Invoke(new Action(delegate
                                        {
                                            SelectLine1(j);
                                        }));
                                    }
                                }
                                else if (Command1 == "L_DELAY")
                                {
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        SelectLine(j);
                                    }));
                                    Delay = Convert.ToInt16(coordinateAxis1[4]);
                                    t = 0;
                                    T = true;
                                    while (!(t >= Delay))
                                    {
                                        if (Wshutdown)
                                        {
                                            ShutDown();
                                            Dispatcher.Invoke(new Action(delegate
                                            {
                                                SelectLine1(j);
                                            }));
                                            break;
                                        }
                                        if (Wenable)
                                        {
                                            Enable();
                                            Dispatcher.Invoke(new Action(delegate
                                            {
                                                SelectLine1(j);
                                            }));
                                            break;
                                        }
                                        if (reset)
                                        {
                                            Reset();
                                            Dispatcher.Invoke(new Action(delegate
                                            {
                                                SelectLine1(j);
                                            }));
                                            break;
                                        }

                                        if (Pause == 1 || Stop == 1)
                                        {
                                            if (Pause == 1)
                                            {
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    Timerdelay.Content = t.ToString();
                                                }));
                                                while (Pause == 1)
                                                {
                                                    if (Stop == 1)
                                                    {
                                                        break;
                                                    }
                                                    if (Whome)
                                                    {
                                                        HOME();
                                                        break;
                                                    }
                                                    if (Wshutdown)
                                                    {
                                                        ShutDown();
                                                        break;
                                                    }
                                                    if (Wenable)
                                                    {
                                                        Enable();
                                                        break;
                                                    }
                                                    if (reset)
                                                    {
                                                        Reset();
                                                        break;
                                                    }
                                                }
                                            }
                                            if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    t = 0;
                                    T = false;
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        SelectLine1(j);
                                    }));
                                }
                                else if (Command1 == "L_SDOON")
                                {
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        SelectLine(j);
                                    }));
                                    SDOON = Convert.ToInt16(coordinateAxis1[4]);
                                    #region ON
                                    if (SDOON == 0)
                                    {
                                        temp1[7] = '1';
                                    }
                                    if (SDOON == 1)
                                    {
                                        temp1[6] = '1';
                                    }
                                    if (SDOON == 2)
                                    {
                                        temp1[5] = '1';
                                    }
                                    if (SDOON == 3)
                                    {
                                        temp1[4] = '1';
                                    }
                                    if (SDOON == 4)
                                    {
                                        temp1[3] = '1';
                                    }
                                    if (SDOON == 5)
                                    {
                                        temp1[2] = '1';
                                    }
                                    if (SDOON == 6)
                                    {
                                        temp1[1] = '1';
                                    }
                                    if (SDOON == 7)
                                    {
                                        temp1[0] = '1';
                                    }
                                    if (SDOON == 8)
                                    {
                                        temp2[7] = '1';
                                    }
                                    if (SDOON == 9)
                                    {
                                        temp2[6] = '1';
                                    }
                                    if (SDOON == 10)
                                    {
                                        temp2[5] = '1';
                                    }
                                    if (SDOON == 11)
                                    {
                                        temp2[4] = '1';
                                    }
                                    if (SDOON == 12)
                                    {
                                        temp2[3] = '1';
                                    }
                                    if (SDOON == 13)
                                    {
                                        temp2[2] = '1';
                                    }
                                    if (SDOON == 14)
                                    {
                                        temp2[1] = '1';
                                    }
                                    if (SDOON == 15)
                                    {
                                        temp2[0] = '1';
                                    }
                                    #endregion
                                    build_data();
                                    short[] value = new short[2];
                                    value[0] = BinaryToShort(data_write1);
                                    value[1] = BinaryToShort(data_Write2);
                                    WRITE_IO[0] = value[0];
                                    WRITE_IO[1] = value[1];
                                    _RS485.SendFc15(6, 0, 16, WRITE_IO);
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        SelectLine1(j);
                                    }));
                                }
                                else if (Command1 == "L_SDOOF")
                                {
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        SelectLine(j);
                                    }));
                                    SDOOF = Convert.ToInt16(coordinateAxis1[4]);
                                    #region OF
                                    if (SDOOF == 0)
                                    {
                                        temp1[7] = '0';
                                    }
                                    if (SDOOF == 1)
                                    {
                                        temp1[6] = '0';
                                    }
                                    if (SDOOF == 2)
                                    {
                                        temp1[5] = '0';
                                    }
                                    if (SDOOF == 3)
                                    {
                                        temp1[4] = '0';
                                    }
                                    if (SDOOF == 4)
                                    {
                                        temp1[3] = '0';
                                    }
                                    if (SDOOF == 5)
                                    {
                                        temp1[2] = '0';
                                    }
                                    if (SDOOF == 6)
                                    {
                                        temp1[1] = '0';
                                    }
                                    if (SDOOF == 7)
                                    {
                                        temp1[0] = '0';
                                    }
                                    if (SDOOF == 8)
                                    {
                                        temp2[7] = '0';
                                    }
                                    if (SDOOF == 9)
                                    {
                                        temp2[6] = '0';
                                    }
                                    if (SDOOF == 10)
                                    {
                                        temp2[5] = '0';
                                    }
                                    if (SDOOF == 11)
                                    {
                                        temp2[4] = '0';
                                    }
                                    if (SDOOF == 12)
                                    {
                                        temp2[3] = '0';
                                    }
                                    if (SDOOF == 13)
                                    {
                                        temp2[2] = '0';
                                    }
                                    if (SDOOF == 14)
                                    {
                                        temp2[1] = '0';
                                    }
                                    if (SDOOF == 15)
                                    {
                                        temp2[0] = '0';
                                    }
                                    #endregion
                                    build_data();
                                    short[] value = new short[2];
                                    value[0] = BinaryToShort(data_write1);
                                    value[1] = BinaryToShort(data_Write2);
                                    WRITE_IO[0] = value[0];
                                    WRITE_IO[1] = value[1];
                                    _RS485.SendFc15(6, 0, 16, WRITE_IO);
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        SelectLine1(j);
                                    }));
                                }
                                else if (Command1 == "L_WDION")
                                {
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        SelectLine(j);
                                    }));
                                    WDION = Convert.ToInt16(coordinateAxis1[4]);
                                    _RS485.SendFc01(6, 32, 16, ref READ_IO);
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        var output1 = Convert.ToString(READ_IO[0], 2);
                                        var output2 = Convert.ToString(READ_IO[1], 2);

                                        string out1 = output1.PadLeft(8, '0');
                                        string out2 = output2.PadLeft(8, '0');
                                        for (int O = 0; O < 8; O++)
                                        {
                                            out_put1[O] = out1[O];
                                            out_put2[O] = out2[O];
                                        }
                                    }));
                                    if (Stop == 1 || Pause == 1)
                                    {
                                        while (Pause == 1)
                                        {
                                            if (Stop == 1)
                                            {
                                                break;
                                            }
                                            if (Whome)
                                            {
                                                HOME();
                                                break;
                                            }
                                            if (Wshutdown)
                                            {
                                                ShutDown();
                                                break;
                                            }
                                            if (Wenable)
                                            {
                                                Enable();
                                                break;
                                            }
                                            if (reset)
                                            {
                                                Reset();
                                                break;
                                            }
                                        }
                                        if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                        {
                                            break;
                                        }
                                    }
                                    if (WDION <= 7)
                                    {
                                        if (WDION == 7) WDION = 0;
                                        else if (WDION == 6) WDION = 1;
                                        else if (WDION == 5) WDION = 2;
                                        else if (WDION == 4) WDION = 3;
                                        else if (WDION == 3) WDION = 4;
                                        else if (WDION == 2) WDION = 5;
                                        else if (WDION == 1) WDION = 6;
                                        else if (WDION == 0) WDION = 7;
                                        while (!(out_put1[WDION] == '1'))
                                        {
                                            _RS485.SendFc01(6, 32, 16, ref READ_IO);
                                            Dispatcher.Invoke(new Action(delegate
                                            {
                                                var output1 = Convert.ToString(READ_IO[0], 2);
                                                string out1 = output1.PadLeft(8, '0');
                                                for (int O = 0; O < 8; O++)
                                                {
                                                    out_put1[O] = out1[O];
                                                }
                                            }));
                                            if (Wshutdown)
                                            {
                                                ShutDown();
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                                break;
                                            }
                                            if (Wenable)
                                            {
                                                Enable();
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                                break;
                                            }
                                            if (reset)
                                            {
                                                Reset();
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                                break;
                                            }
                                            if (Stop == 1 || Pause == 1)
                                            {
                                                while (Pause == 1)
                                                {
                                                    if (Stop == 1)
                                                    {
                                                        break;
                                                    }
                                                    if (Whome)
                                                    {
                                                        HOME();
                                                        break;
                                                    }
                                                    if (Wshutdown)
                                                    {
                                                        ShutDown();
                                                        break;
                                                    }
                                                    if (Wenable)
                                                    {
                                                        Enable();
                                                        break;
                                                    }
                                                    if (reset)
                                                    {
                                                        Reset();
                                                        break;
                                                    }
                                                }
                                                if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (WDION > 7)
                                    {
                                        if (WDION == 15) WDION = 0;
                                        else if (WDION == 14) WDION = 1;
                                        else if (WDION == 13) WDION = 2;
                                        else if (WDION == 12) WDION = 3;
                                        else if (WDION == 11) WDION = 4;
                                        else if (WDION == 10) WDION = 5;
                                        else if (WDION == 9) WDION = 6;
                                        else if (WDION == 8) WDION = 7;
                                        while (!(out_put2[WDION] == '1'))
                                        {
                                            _RS485.SendFc01(6, 32, 16, ref READ_IO);
                                            Dispatcher.Invoke(new Action(delegate
                                            {

                                                var output2 = Convert.ToString(READ_IO[1], 2);


                                                string out2 = output2.PadLeft(8, '0');
                                                for (int O = 0; O < 8; O++)
                                                {

                                                    out_put2[O] = out2[O];
                                                }
                                            }));
                                            if (Wshutdown)
                                            {
                                                ShutDown();
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                                break;
                                            }
                                            if (Wenable)
                                            {
                                                Enable();
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                                break;
                                            }
                                            if (reset)
                                            {
                                                Reset();
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                                break;
                                            }
                                            if (Stop == 1 || Pause == 1)
                                            {
                                                while (Pause == 1)
                                                {
                                                    if (Stop == 1)
                                                    {
                                                        break;
                                                    }
                                                    if (Whome)
                                                    {
                                                        HOME();
                                                        break;
                                                    }
                                                    if (Wshutdown)
                                                    {
                                                        ShutDown();
                                                        break;
                                                    }
                                                    if (Wenable)
                                                    {
                                                        Enable();
                                                        break;
                                                    }
                                                    if (reset)
                                                    {
                                                        Reset();
                                                        break;
                                                    }
                                                }
                                                if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        SelectLine1(j);
                                    }));
                                }
                                else if (Command1 == "L_WDIOF")
                                {
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        SelectLine(j);
                                    }));
                                    WDIOF = Convert.ToInt16(coordinateAxis1[4]);
                                    _RS485.SendFc01(6, 32, 16, ref READ_IO);
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        var output1 = Convert.ToString(READ_IO[0], 2);
                                        var output2 = Convert.ToString(READ_IO[1], 2);

                                        string out1 = output1.PadLeft(8, '0');
                                        string out2 = output2.PadLeft(8, '0');
                                        for (int O = 0; O < 8; O++)
                                        {
                                            out_put1[O] = out1[O];
                                            out_put2[O] = out2[O];
                                        }
                                    }));
                                    if (Stop == 1 || Pause == 1)
                                    {
                                        while (Pause == 1)
                                        {
                                            if (Stop == 1)
                                            {
                                                break;
                                            }
                                            if (Whome)
                                            {
                                                HOME();
                                                break;
                                            }
                                            if (Wshutdown)
                                            {
                                                ShutDown();
                                                break;
                                            }
                                            if (Wenable)
                                            {
                                                Enable();
                                                break;
                                            }
                                            if (reset)
                                            {
                                                Reset();
                                                break;
                                            }
                                        }
                                        if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                        {
                                            break;
                                        }
                                    }
                                    if (WDIOF <= 7)
                                    {
                                        if (WDIOF == 7) WDIOF = 0;
                                        else if (WDIOF == 6) WDIOF = 1;
                                        else if (WDIOF == 5) WDIOF = 2;
                                        else if (WDIOF == 4) WDIOF = 3;
                                        else if (WDIOF == 3) WDIOF = 4;
                                        else if (WDIOF == 2) WDIOF = 5;
                                        else if (WDIOF == 1) WDIOF = 6;
                                        else if (WDIOF == 0) WDIOF = 7;
                                        while (!(out_put1[WDIOF] == '0'))
                                        {
                                            _RS485.SendFc01(6, 32, 16, ref READ_IO);
                                            Dispatcher.Invoke(new Action(delegate
                                            {
                                                var output1 = Convert.ToString(READ_IO[0], 2);
                                                string out1 = output1.PadLeft(8, '0');
                                                for (int O = 0; O < 8; O++)
                                                {
                                                    out_put1[O] = out1[O];
                                                }
                                            }));
                                            if (Wshutdown)
                                            {
                                                ShutDown();
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                                break;
                                            }
                                            if (Wenable)
                                            {
                                                Enable();
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                                break;
                                            }
                                            if (reset)
                                            {
                                                Reset();
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                                break;
                                            }
                                            if (Stop == 1 || Pause == 1)
                                            {
                                                while (Pause == 1)
                                                {
                                                    if (Stop == 1)
                                                    {
                                                        break;
                                                    }
                                                    if (Whome)
                                                    {
                                                        HOME();
                                                        break;
                                                    }
                                                    if (Wshutdown)
                                                    {
                                                        ShutDown();
                                                        break;
                                                    }
                                                    if (Wenable)
                                                    {
                                                        Enable();
                                                        break;
                                                    }
                                                    if (reset)
                                                    {
                                                        Reset();
                                                        break;
                                                    }
                                                }
                                                if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (WDIOF > 7)
                                    {
                                        if (WDIOF == 15) WDIOF = 0;
                                        else if (WDIOF == 14) WDIOF = 1;
                                        else if (WDIOF == 13) WDIOF = 2;
                                        else if (WDIOF == 12) WDIOF = 3;
                                        else if (WDIOF == 11) WDIOF = 4;
                                        else if (WDIOF == 10) WDIOF = 5;
                                        else if (WDIOF == 9) WDIOF = 6;
                                        else if (WDIOF == 8) WDIOF = 7;
                                        while (!(out_put2[WDIOF] == '0'))
                                        {
                                            _RS485.SendFc01(6, 32, 16, ref READ_IO);
                                            Dispatcher.Invoke(new Action(delegate
                                            {
                                                var output2 = Convert.ToString(READ_IO[1], 2);
                                                string out2 = output2.PadLeft(8, '0');
                                                for (int O = 0; O < 8; O++)
                                                {
                                                    out_put2[O] = out2[O];
                                                }
                                            }));
                                            if (Wshutdown)
                                            {
                                                ShutDown();
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                                break;
                                            }
                                            if (Wenable)
                                            {
                                                Enable();
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                                break;
                                            }
                                            if (reset)
                                            {
                                                Reset();
                                                Dispatcher.Invoke(new Action(delegate
                                                {
                                                    SelectLine1(j);
                                                }));
                                                break;
                                            }
                                            if (Stop == 1 || Pause == 1)
                                            {
                                                while (Pause == 1)
                                                {
                                                    if (Stop == 1)
                                                    {
                                                        break;
                                                    }
                                                    if (Whome)
                                                    {
                                                        HOME();
                                                        break;
                                                    }
                                                    if (Wshutdown)
                                                    {
                                                        ShutDown();
                                                        break;
                                                    }
                                                    if (Wenable)
                                                    {
                                                        Enable();
                                                        break;
                                                    }
                                                    if (reset)
                                                    {
                                                        Reset();
                                                        break;
                                                    }
                                                }
                                                if (Stop == 1 || Whome || Wshutdown || Wenable || reset)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        SelectLine1(j);
                                    }));
                                }
                                else if (Command1 == "ENDLOOP")
                                {
                                    break;
                                }

                            }

                        }
                        while (true);


                    }
                }

            }
            while (Loop == true);
        }
        #endregion
        #region ClickRun
        int Pause = 0;
        int Stop = 0;
        bool Loop = false;
        bool Wrun = false;
        bool Wrunprogram;
        private void PlayRun_Click(object sender, RoutedEventArgs e)
        {
            if (WPlayRun)
            {
                Wrun = true;
                TimerDelay.IsEnabled = true;
                PlayRun.IsEnabled = false;
                PauseRun.IsEnabled = true;
                StopRun.IsEnabled = true;
                OpenFileRun.IsEnabled = false;
                Pause = 0;
                Stop = 0;
                Home.IsEnabled = false;

            }
            else
            {
                MessageBox.Show("Home");
            }

        }
        private void PauseRun_Click(object sender, RoutedEventArgs e)
        {
            Pause = 1 - Pause;
            Home.IsEnabled = true;
            if (Pause == 1)
            {
                TimerDelay.IsEnabled = false;
            }
            if (Pause == 0)
            {
                TimerDelay.IsEnabled = true;
            }
        }
        private void StopRun_Click(object sender, RoutedEventArgs e)
        {
            Stop = 1;
            Wrun = false;
            PauseRun.IsEnabled = false;
            PlayRun.IsEnabled = true;
            WPlayRun = false;
            StopRun.IsEnabled = false;
            OpenFileRun.IsEnabled = true;
            Home.IsEnabled = true;
        }
        #endregion
        #region selectline
        public void SelectLine(int line)
        {
            int l = 0;
            TextRange r;
            foreach (var item in RichTextBoxRun.Document.Blocks)
            {
                if (line == l)
                {
                    r = new TextRange(item.ContentStart, item.ContentEnd);
                    if (r.Text.Trim().Equals(""))
                    {
                        continue;
                    }
                    r.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.OrangeRed);
                    r.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.White);
                    return;
                }
                l++;
            }
        }
        public void SelectLine1(int line)
        {
            int l = 0;
            TextRange r;
            foreach (var item in RichTextBoxRun.Document.Blocks)
            {
                if (line == l)
                {
                    r = new TextRange(item.ContentStart, item.ContentEnd);
                    if (r.Text.Trim().Equals(""))
                    {
                        continue;
                    }
                    r.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.White);
                    r.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
                    return;
                }
                l++;
            }
        }
        public void SelectLineL(int line)
        {
            int l = 0;
            TextRange r;
            foreach (var item in RichTextBoxRun.Document.Blocks)
            {
                if (line == l)
                {
                    r = new TextRange(item.ContentStart, item.ContentEnd);
                    if (r.Text.Trim().Equals(""))
                    {
                        continue;
                    }
                    r.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Green);
                    r.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.White);
                    return;
                }
                l++;
            }
        }
        #endregion
        #region TagClick
        private void Setup_Click(object sender, RoutedEventArgs e)
        {
            SetupItem.IsSelected = true;
        }
        private void ManualControl_Click(object sender, RoutedEventArgs e)
        {
            ManualControlItem.IsSelected = true;
        }
        private void IOControl_Click(object sender, RoutedEventArgs e)
        {
            IOControlItem.IsSelected = true;
        }
        private void RobotProgram_Click(object sender, RoutedEventArgs e)
        {
            RobotProgramItem.IsSelected = true;
        }
        private void AutoRun_Click(object sender, RoutedEventArgs e)
        {
            AutoRunItem.IsSelected = true;
        }
        private void VelocityMinus_Click(object sender, RoutedEventArgs e)
        {
            Velocity.Value = Velocity.Value - 5;
        }
        private void MoveRobot_Click(object sender, RoutedEventArgs e)
        {
            ManualControlItem.IsSelected = true;
            FinishSetPoint.IsEnabled = true;
            FinishSetPoint1.IsEnabled = true;
        }
        private void Moving_Click(object sender, RoutedEventArgs e)
        {
            MovingItem.IsSelected = true;
        }
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            SettingItem.IsSelected = true;
        }
        private void Block_Click(object sender, RoutedEventArgs e)
        {
            BlockItem.IsSelected = true;
        }
        #endregion

        /// <summary>
        /// /////////////
        /// </summary>
        public static ObservableCollection<string> testprogram = new ObservableCollection<string>();
        public static ObservableCollection<string> testrun = new ObservableCollection<string>();
        private List<string> SetHOME = new List<string>();
        private List<string> IONAME = new List<string>();
        #region Click
        private void delayAdd_Click(object sender, RoutedEventArgs e)
        {
            int timeradd;
            timeradd = Convert.ToInt16(Timer.Text);
            timeradd++;
            Timer.Text = timeradd.ToString();
        }
        private void delayMinus_Click(object sender, RoutedEventArgs e)
        {

            int timerminus;
            timerminus = Convert.ToInt16(Timer.Text);
            if (timerminus > 0)
            {
                timerminus--;
                Timer.Text = timerminus.ToString();
            }

        }
        private void timesadd_Click(object sender, RoutedEventArgs e)
        {
            int timesAdd;
            timesAdd = Convert.ToInt16(Times.Text);
            timesAdd++;
            Times.Text = timesAdd.ToString();
        }
        private void timesminus_Click(object sender, RoutedEventArgs e)
        {
            int timesMinus;
            timesMinus = Convert.ToInt16(Times.Text);
            if (timesMinus > 2)
            {
                timesMinus--;
                Times.Text = timesMinus.ToString();
            }

        }

        bool addspeed = false;
        private void VelocityAdd_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            addspeed = true;
            TimerSpeed.IsEnabled = true;
        }
        private void VelocityAdd_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            addspeed = false;
            TimerSpeed.IsEnabled = false;
        }
        bool minusspeed = false;
        private void VelocityMinus_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            minusspeed = true;
            TimerSpeed.IsEnabled = true;
        }
        private void VelocityMinus_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            minusspeed = false;
            TimerSpeed.IsEnabled = false;
        }
        #endregion
        #region LOAD
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string Systemconfig = Directory.GetCurrentDirectory() + @"\" + "HOME.txt";
            string Systemconfig1 = Directory.GetCurrentDirectory() + @"\" + "IO.txt";

            Speed[0] = Convert.ToInt16(Velocity.Value);
            if (Speed[0] == 100)
            {
                Speed[0] = 102;
            }
            else if (Speed[0] == 90)
            {
                Speed[0] = 92;
            }
            else if (Speed[0] == 200)
            {
                Speed[0] = 202;
            }
            Accel[0] = Convert.ToInt16(Speed[0] * 10 / 100);
            Decel[0] = Convert.ToInt16(Speed[0] * 10 / 100);

            using (StreamReader sr = new StreamReader(Systemconfig1))
            {
                for (int i = 0; i <= 20; i++)
                {
                    IONAME.Add(sr.ReadLine());
                }
            }

            using (StreamReader sr = new StreamReader(Systemconfig))
            {
                for (int i = 0; i <= 20; i++)
                {
                    SetHOME.Add(sr.ReadLine());
                }
            }
            if (SetHOME != null)
            {
                SpeedHomeX = Convert.ToInt16(SetHOME[5]);
                AccelHomeX = Convert.ToInt16(SetHOME[10]);
                DecelHomeX = Convert.ToInt16(SetHOME[15]);
                PositioHomeX = Convert.ToInt16(SetHOME[0]);

                SpeedHomeY1 = Convert.ToInt16(SetHOME[6]);
                AccelHomeY1 = Convert.ToInt16(SetHOME[11]);
                DecelHomeY1 = Convert.ToInt16(SetHOME[16]);
                PositioHomeY1 = Convert.ToInt16(SetHOME[1]);

                SpeedHomeY2 = Convert.ToInt16(SetHOME[7]);
                AccelHomeY2 = Convert.ToInt16(SetHOME[12]);
                DecelHomeY2 = Convert.ToInt16(SetHOME[17]);
                PositioHomeY2 = Convert.ToInt16(SetHOME[2]);

                SpeedHomeZ1 = Convert.ToInt16(SetHOME[8]);
                AccelHomeZ1 = Convert.ToInt16(SetHOME[13]);
                DecelHomeZ1 = Convert.ToInt16(SetHOME[18]);
                PositioHomeZ1 = Convert.ToInt16(SetHOME[3]);

                SpeedHomeZ2 = Convert.ToInt16(SetHOME[9]);
                AccelHomeZ2 = Convert.ToInt16(SetHOME[14]);
                DecelHomeZ2 = Convert.ToInt16(SetHOME[19]);
                PositioHomeZ2 = Convert.ToInt16(SetHOME[4]);

                PositionX.Text = PositioHomeX.ToString();
                PositionY1.Text = PositioHomeY1.ToString();
                PositionY2.Text = PositioHomeY2.ToString();
                PositionZ1.Text = PositioHomeZ1.ToString();
                PositionZ2.Text = PositioHomeZ2.ToString();

                VelocityX.Text = SpeedHomeX.ToString();
                VelocityY1.Text = SpeedHomeY1.ToString();
                VelocityY2.Text = SpeedHomeY2.ToString();
                VelocityZ1.Text = SpeedHomeZ1.ToString();
                VelocityZ2.Text = SpeedHomeZ2.ToString();

                AccelX.Text = AccelHomeX.ToString();
                AccelY1.Text = AccelHomeY1.ToString();
                AccelY2.Text = AccelHomeY2.ToString();
                AccelZ1.Text = AccelHomeZ1.ToString();
                AccelZ2.Text = AccelHomeZ2.ToString();

                DecelX.Text = DecelHomeX.ToString();
                DecelY1.Text = DecelHomeY1.ToString();
                DecelY2.Text = DecelHomeY2.ToString();
                DecelZ1.Text = DecelHomeZ1.ToString();
                DecelZ2.Text = DecelHomeZ2.ToString();

            }
            if (IONAME != null)
            {
                ioname0.Text = IONAME[0];
                ioname1.Text = IONAME[1];
                ioname2.Text = IONAME[2];
                ioname3.Text = IONAME[3];
                ioname4.Text = IONAME[4];
                ioname5.Text = IONAME[5];
                ioname6.Text = IONAME[6];
                ioname7.Text = IONAME[7];
                ioname8.Text = IONAME[8];
                ioname9.Text = IONAME[9];
                ioname10.Text = IONAME[10];
                ioname11.Text = IONAME[11];
                ioname12.Text = IONAME[12];
                ioname13.Text = IONAME[13];
                ioname14.Text = IONAME[14];
                ioname15.Text = IONAME[15];
            }

            // Process.Start(@"C:\Windows\WinSxS\amd64_microsoft-windows-osk_31bf3856ad364e35_10.0.19041.1_none_60ade0eff94c37fc\osk.exe");

        }
        #endregion
        #region LOLA
        private void FinishSetPoint_Click(object sender, RoutedEventArgs e)
        {
            RobotProgramItem.IsSelected = true;
            MovingItem.IsSelected = true;
            FinishSetPoint.IsEnabled = false;
            FinishSetPoint1.IsEnabled = false;
        }
        private void FinishSetPoint1_Click(object sender, RoutedEventArgs e)
        {
            RobotProgramItem.IsSelected = true;
            MovingItem.IsSelected = true;
            FinishSetPoint1.IsEnabled = false;
            FinishSetPoint.IsEnabled = false;
        }
        private void PositionX_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PositionX.SelectAll();
        }
        private void PositionY1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PositionY1.SelectAll();
        }
        private void PositionY2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PositionY2.SelectAll();
        }
        private void PositionZ1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PositionZ1.SelectAll();
        }
        private void PositionZ2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PositionZ2.SelectAll();
        }
        private void VelocityX_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VelocityX.SelectAll();
        }
        private void VelocityY1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VelocityY1.SelectAll();
        }
        private void VelocityY2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VelocityY2.SelectAll();
        }
        private void VelocityZ1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VelocityZ1.SelectAll();
        }
        private void VelocityZ2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VelocityZ2.SelectAll();
        }
        private void AccelX_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AccelX.SelectAll();
        }
        private void AccelY1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AccelY1.SelectAll();
        }
        private void AccelY2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AccelY2.SelectAll();
        }
        private void AccelZ1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AccelZ1.SelectAll();
        }
        private void AccelZ2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AccelZ2.SelectAll();
        }
        private void DecelX_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DecelX.SelectAll();
        }
        private void DecelY1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DecelY1.SelectAll();
        }
        private void DecelY2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DecelY2.SelectAll();
        }
        private void DecelZ1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DecelZ1.SelectAll();
        }
        private void DecelZ2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DecelZ2.SelectAll();
        }
        private void Text_PointX_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Text_PointX.SelectAll();
        }
        private void Text_PointY1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Text_PointY1.SelectAll();
        }
        private void Text_PointY2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Text_PointY2.SelectAll();
        }
        private void Text_PointZ1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Text_PointZ1.SelectAll();
        }
        private void Text_PointZ2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Text_PointZ2.SelectAll();
        }
        private void Speedrun_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Speedrun.SelectAll();
        }

        #endregion
        #region RUNLINE
        private void Runline()
        {
            try
            {

                string[] coordinateAxis = Readline.Split('.', ':', ' ');
                Command = coordinateAxis[0];
                if (Command == "MOVE")
                {
                    X = Convert.ToInt16(coordinateAxis[2]);
                    Y1 = Convert.ToInt16(coordinateAxis[4]);
                    Y2 = Convert.ToInt16(coordinateAxis[6]);
                    Z1 = Convert.ToInt16(coordinateAxis[8]);
                    Z2 = Convert.ToInt16(coordinateAxis[10]);
                    SpeedRun[0] = Convert.ToInt16(coordinateAxis[12]);
                    AccelRun[0] = Convert.ToInt16(((SpeedRun[0]) * 30) / 100);
                    DecelRun[0] = Convert.ToInt16(((SpeedRun[0]) * 30) / 100);
                    short[] PointdataRunX = Get_PointData(X);
                    short[] PointdataRunY1 = Get_PointData(Y1);
                    short[] PointdataRunY2 = Get_PointData(Y2);
                    short[] PointdataRunZ1 = Get_PointData(Z1);
                    short[] PointdataRunZ2 = Get_PointData(Z2);
                    PointX[0] = PointdataRunX[0];
                    PointX[1] = PointdataRunX[1];
                    PointY1[0] = PointdataRunY1[0];
                    PointY1[1] = PointdataRunY1[1];
                    PointY2[0] = PointdataRunY2[0];
                    PointY2[1] = PointdataRunY2[1];
                    PointZ1[0] = PointdataRunZ1[0];
                    PointZ1[1] = PointdataRunZ1[1];
                    PointZ2[0] = PointdataRunZ2[0];
                    PointZ2[1] = PointdataRunZ2[1];

                    _RS485.SendFc16(ID1, 24640, 1, new short[1] { 15 });
                    _RS485.SendFc16(ID2, 24640, 1, new short[1] { 15 });
                    //_RS485.SendFc16(3, 24640, 1, new short[1] { 15 });
                    //_RS485.SendFc16(4, 24640, 1, new short[1] { 15 });
                    //_RS485.SendFc16(5, 24640, 1, new short[1] { 15 });
                    _RS485.SendFc16(ID1, 24672, 1, new short[1] { -101 });
                    _RS485.SendFc16(ID2, 24672, 1, new short[1] { -101 });
                    //_RS485.SendFc16(3, 24672, 1, new short[1] { -101 });
                    //_RS485.SendFc16(4, 24672, 1, new short[1] { -101 });
                    //_RS485.SendFc16(5, 24672, 1, new short[1] { -101 });
                    _RS485.SendFc16(ID1, 10241, 9, new short[9] { 7, PointX[0], PointX[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                    _RS485.SendFc16(ID2, 10241, 9, new short[9] { 7, PointY1[0], PointY1[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                    //_RS485.SendFc16(3, 10241, 9, new short[9] { 1, PointY2[0], PointY2[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                    //_RS485.SendFc16(4, 10241, 9, new short[9] { 1, PointZ1[0], PointZ1[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                    //_RS485.SendFc16(5, 10241, 9, new short[9] { 1, PointZ2[0], PointZ2[1], SpeedRun[0], AccelRun[0], DecelRun[0], 0, 0, 0 });
                    _RS485.SendFc16(ID1, 11616, 1, new short[1] { 1 });
                    _RS485.SendFc16(ID2, 11616, 1, new short[1] { 1 });
                    //_RS485.SendFc16(3, 11616, 1, new short[1] { 1 });
                    //_RS485.SendFc16(4, 11616, 1, new short[1] { 1 });
                    //_RS485.SendFc16(5, 11616, 1, new short[1] { 1 });
                    _RS485.SendFc16(ID1, 24640, 1, new short[1] { 31 });
                    _RS485.SendFc16(ID2, 24640, 1, new short[1] { 31 });

                    //_RS485.SendFc16(3, 24640, 1, new short[1] { 31 });
                    //_RS485.SendFc16(4, 24640, 1, new short[1] { 31 });
                    //_RS485.SendFc16(5, 24640, 1, new short[1] { 31 });
                    while (!(R_Point_X[0] == PointX[0] && R_Point_X[1] == PointX[1]) || !(R_Point_Y1[0] == PointY1[0] && R_Point_Y1[1] == PointY1[1]))// || !(R_Point_Y2[0] == Y2) || !(R_Point_Z1[0] == Z1) || !(R_Point_Z2[0] == Z2))
                    {

                        _RS485.SendFc3(ID1, 11055, 2, ref R_Point_X);
                        _RS485.SendFc3(ID2, 11055, 2, ref R_Point_Y1);

                        //_RS485.SendFc3(3, 11055, 2, ref R_Point_Y2);
                        //_RS485.SendFc3(4, 11055, 2, ref R_Point_Z1);
                        //_RS485.SendFc3(5, 11055, 2, ref R_Point_Z2);

                        Dispatcher.Invoke(new Action(delegate
                        {
                            Point_XRun.Content = GetIntFromBits_Read(R_Point_X[0], R_Point_X[1]);
                            Point_Y1Run.Content = GetIntFromBits_Read(R_Point_Y1[0], R_Point_Y1[1]);
                            //Point_Y2Run.Content = R_Point_Y2[0].ToString();
                            //Point_Z1Run.Content = R_Point_Z1[0].ToString();
                            //Point_Z2Run.Content = R_Point_Z2[0].ToString();
                            VelocityRun.Content = SpeedRun[0].ToString();
                        }));
                    }
                    if (R_Point_X[0] == PointX[0] && R_Point_X[1] == PointX[1] && R_Point_Y1[0] == PointY1[0] && R_Point_Y1[1] == PointY1[1])// && R_Point_Y1[0] == Y1 && R_Point_Y2[0] == Y2 && R_Point_Z1[0] == Z1 && R_Point_Z2[0] == Z2)
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            testl.UnselectAll();
                        }));
                    }
                }
                if (Command == "DELAY")
                {

                    Delay = Convert.ToInt16(coordinateAxis[1]);
                    t = 0;
                    T = true;
                    while (!(t >= Delay))
                    {

                    }
                    t = 0;
                    T = false;
                    Dispatcher.Invoke(new Action(delegate
                    {
                        testl.UnselectAll();
                        TimerDelay.IsEnabled = false;
                    }));
                }
                if (Command == "SDOON")
                {

                    SDOON = Convert.ToInt16(coordinateAxis[1]);
                    #region ON
                    if (SDOON == 0)
                    {
                        temp1[7] = '1';
                    }
                    else if (SDOON == 1)
                    {
                        temp1[6] = '1';
                    }
                    else if (SDOON == 2)
                    {
                        temp1[5] = '1';
                    }
                    else if (SDOON == 3)
                    {
                        temp1[4] = '1';
                    }
                    else if (SDOON == 4)
                    {
                        temp1[3] = '1';
                    }
                    else if (SDOON == 5)
                    {
                        temp1[2] = '1';
                    }
                    else if (SDOON == 6)
                    {
                        temp1[1] = '1';
                    }
                    else if (SDOON == 7)
                    {
                        temp1[0] = '1';
                    }
                    else if (SDOON == 8)
                    {
                        temp2[7] = '1';
                    }
                    else if (SDOON == 9)
                    {
                        temp2[6] = '1';
                    }
                    else if (SDOON == 10)
                    {
                        temp2[5] = '1';
                    }
                    else if (SDOON == 11)
                    {
                        temp2[4] = '1';
                    }
                    else if (SDOON == 12)
                    {
                        temp2[3] = '1';
                    }
                    else if (SDOON == 13)
                    {
                        temp2[2] = '1';
                    }
                    else if (SDOON == 14)
                    {
                        temp2[1] = '1';
                    }
                    else if (SDOON == 15)
                    {
                        temp2[0] = '1';
                    }
                    #endregion
                    build_data();
                    short[] value = new short[2];
                    value[0] = BinaryToShort(data_write1);
                    value[1] = BinaryToShort(data_Write2);
                    WRITE_IO[0] = value[0];
                    WRITE_IO[1] = value[1];
                    _RS485.SendFc15(6, 0, 16, WRITE_IO);
                    Dispatcher.Invoke(new Action(delegate
                    {
                        testl.UnselectAll();
                    }));
                }
                if (Command == "SDOOF")
                {

                    SDOOF = Convert.ToInt16(coordinateAxis[1]);
                    #region OF
                    if (SDOOF == 0)
                    {
                        temp1[7] = '0';
                    }
                    else if (SDOOF == 1)
                    {
                        temp1[6] = '0';
                    }
                    else if (SDOOF == 2)
                    {
                        temp1[5] = '0';
                    }
                    else if (SDOOF == 3)
                    {
                        temp1[4] = '0';
                    }
                    else if (SDOOF == 4)
                    {
                        temp1[3] = '0';
                    }
                    else if (SDOOF == 5)
                    {
                        temp1[2] = '0';
                    }
                    else if (SDOOF == 6)
                    {
                        temp1[1] = '0';
                    }
                    else if (SDOOF == 7)
                    {
                        temp1[0] = '0';
                    }
                    else if (SDOOF == 8)
                    {
                        temp2[7] = '0';
                    }
                    else if (SDOOF == 9)
                    {
                        temp2[6] = '0';
                    }
                    else if (SDOOF == 10)
                    {
                        temp2[5] = '0';
                    }
                    else if (SDOOF == 11)
                    {
                        temp2[4] = '0';
                    }
                    else if (SDOOF == 12)
                    {
                        temp2[3] = '0';
                    }
                    else if (SDOOF == 13)
                    {
                        temp2[2] = '0';
                    }
                    else if (SDOOF == 14)
                    {
                        temp2[1] = '0';
                    }
                    else if (SDOOF == 15)
                    {
                        temp2[0] = '0';
                    }
                    #endregion
                    build_data();
                    short[] value = new short[2];
                    value[0] = BinaryToShort(data_write1);
                    value[1] = BinaryToShort(data_Write2);
                    WRITE_IO[0] = value[0];
                    WRITE_IO[1] = value[1];
                    _RS485.SendFc15(6, 0, 16, WRITE_IO);
                    Dispatcher.Invoke(new Action(delegate
                    {
                        testl.UnselectAll();
                    }));
                }
                if (Command == "WDION")
                {

                    WDION = Convert.ToInt16(coordinateAxis[1]);
                    _RS485.SendFc02(6, 0, 16, ref READ_IO);
                    Dispatcher.Invoke(new Action(delegate
                    {
                        var output1 = Convert.ToString(READ_IO[0], 2);
                        var output2 = Convert.ToString(READ_IO[1], 2);
                        string out1 = output1.PadLeft(8, '0');
                        string out2 = output2.PadLeft(8, '0');
                        for (int O = 0; O < 8; O++)
                        {
                            out_put1[O] = out1[O];
                            out_put2[O] = out2[O];
                        }
                    }));
                    if (WDION <= 7)
                    {
                        if (WDION == 7) WDION = 0;
                        else if (WDION == 6) WDION = 1;
                        else if (WDION == 5) WDION = 2;
                        else if (WDION == 4) WDION = 3;
                        else if (WDION == 3) WDION = 4;
                        else if (WDION == 2) WDION = 5;
                        else if (WDION == 1) WDION = 6;
                        else if (WDION == 0) WDION = 7;
                        while (!(out_put1[WDION] == '1'))
                        {
                            _RS485.SendFc02(6, 0, 16, ref READ_IO);
                            Dispatcher.Invoke(new Action(delegate
                            {
                                var output1 = Convert.ToString(READ_IO[0], 2);
                                string out1 = output1.PadLeft(8, '0');
                                for (int O = 0; O < 8; O++)
                                {
                                    out_put1[O] = out1[O];
                                }
                            }));
                        }
                    }
                    if (WDION > 7)
                    {
                        if (WDION == 15) WDION = 0;
                        else if (WDION == 14) WDION = 1;
                        else if (WDION == 13) WDION = 2;
                        else if (WDION == 12) WDION = 3;
                        else if (WDION == 11) WDION = 4;
                        else if (WDION == 10) WDION = 5;
                        else if (WDION == 9) WDION = 6;
                        else if (WDION == 8) WDION = 7;
                        while (!(out_put2[WDION] == '1'))
                        {
                            _RS485.SendFc02(6, 0, 16, ref READ_IO);
                            Dispatcher.Invoke(new Action(delegate
                            {
                                var output2 = Convert.ToString(READ_IO[1], 2);
                                string out2 = output2.PadLeft(8, '0');
                                for (int O = 0; O < 8; O++)
                                {
                                    out_put2[O] = out2[O];
                                }
                            }));
                        }
                    }
                    Dispatcher.Invoke(new Action(delegate
                    {
                        testl.UnselectAll();
                    }));
                }
                if (Command == "WDIOF")
                {

                    WDIOF = Convert.ToInt16(coordinateAxis[1]);
                    _RS485.SendFc02(6, 0, 16, ref READ_IO);
                    Dispatcher.Invoke(new Action(delegate
                    {
                        var output1 = Convert.ToString(READ_IO[0], 2);
                        var output2 = Convert.ToString(READ_IO[1], 2);

                        string out1 = output1.PadLeft(8, '0');
                        string out2 = output2.PadLeft(8, '0');
                        for (int O = 0; O < 8; O++)
                        {
                            out_put1[O] = out1[O];
                            out_put2[O] = out2[O];
                        }
                    }));
                    if (WDIOF <= 7)
                    {
                        if (WDIOF == 7) WDIOF = 0;
                        else if (WDIOF == 6) WDIOF = 1;
                        else if (WDIOF == 5) WDIOF = 2;
                        else if (WDIOF == 4) WDIOF = 3;
                        else if (WDIOF == 3) WDIOF = 4;
                        else if (WDIOF == 2) WDIOF = 5;
                        else if (WDIOF == 1) WDIOF = 6;
                        else if (WDIOF == 0) WDIOF = 7;
                        while (!(out_put1[WDIOF] == '0'))
                        {
                            _RS485.SendFc02(6, 0, 16, ref READ_IO);
                            Dispatcher.Invoke(new Action(delegate
                            {
                                var output1 = Convert.ToString(READ_IO[0], 2);

                                string out1 = output1.PadLeft(8, '0');
                                for (int O = 0; O < 8; O++)
                                {
                                    out_put1[O] = out1[O];
                                }
                            }));
                        }
                    }
                    if (WDIOF > 7)
                    {
                        if (WDIOF == 15) WDIOF = 0;
                        else if (WDIOF == 14) WDIOF = 1;
                        else if (WDIOF == 13) WDIOF = 2;
                        else if (WDIOF == 12) WDIOF = 3;
                        else if (WDIOF == 11) WDIOF = 4;
                        else if (WDIOF == 10) WDIOF = 5;
                        else if (WDIOF == 9) WDIOF = 6;
                        else if (WDIOF == 8) WDIOF = 7;
                        while (!(out_put2[WDIOF] == '0'))
                        {
                            _RS485.SendFc02(6, 0, 16, ref READ_IO);
                            Dispatcher.Invoke(new Action(delegate
                            {
                                var output2 = Convert.ToString(READ_IO[1], 2);

                                string out2 = output2.PadLeft(8, '0');
                                for (int O = 0; O < 8; O++)
                                {
                                    out_put2[O] = out2[O];
                                }
                            }));

                        }
                    }
                    Dispatcher.Invoke(new Action(delegate
                    {
                        testl.UnselectAll();
                    }));
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        public static bool WRunline = false;
        public static string Readline;

        #endregion
        int bn5 = 0;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bn5 = 1 - bn5;
            if (bn5 == 1)
            {
                sLanguage.Content = "English";
                Setup.Content = "Thiết Lập";
                ManualControl.Content = "Điều Khiển Bằng Tay";
                IOControl.Content = "Điều Khiển IO";
                RobotProgram.Content = "Chương Trình Robot";
                AutoRun.Content = "Chạy Tự Động";
                Interface.Content = "Giao Diện Điều Khiển";
                start.Content = "Khởi Động Robot";
                Sestop.Content = "1: Nhấn 'Estop' Để Kích Hoạt Robot";
                Sroboton.Content = "2: Nhấn 'Robot On' Để Điều Khiển Robot";
                Shome.Content = "3: Nhấn 'Home' Để Về Vị Trí Ban Đầu";
                MainPageIteam.Header = "Trang Chính";
                SetupItem.Header = "Thiết Lập";
                ManualControlItem.Header = "Điều Khiển Bằng Tay";
                IOControlItem.Header = "Điều Khiển IO";
                RobotProgramItem.Header = "Chương Trình Robot";
                AutoRunItem.Header = "Chạy Tự Động";
                sJoint.Content = "Khớp";
                sPosition.Content = "Vị Trí (mm)";
                sAcceleration.Content = "Tăng Tốc (ms)";
                sDeceleration.Content = "Giảm Tốc (ms)";
                sVelocity.Content = "Vận Tốc (r/min)";
                sJointName.Content = "Tên Khớp";
                sJoint0.Content = "Khớp 0";
                sJoint1.Content = "Khớp 1";
                sJoint2.Content = "Khớp 2";
                sJoint3.Content = "Khớp 3";
                sJoint4.Content = "Khớp 4";
                sHomeSetting.Header = "Thiết Lập Home";
                sHomeParameter.Content = "Thông Số Home";
                UpdateHome.Content = "Cập Nhật Thông Số Home";
                sJogParameter.Header = "Thông Số Jog";
                sVelocity1.Content = "Vận Tốc (r/min)";
                sMoveJog.Header = "Chạy Jog";
                sMovePoint.Header = "Chạy Điểm";
                MovingItem.Header = "Di chuyển";
                SettingItem.Header = "Thiết Lập";
                BlockItem.Header = "Khối Lệnh";
                Moving.Content = "Di Chuyển";
                Setting.Content = "Thiết Lập";
                Block.Content = "Khối Lệnh";
                MoveRobot.Content = "Di Chuyển Robot";
                SetPoint.Content = "Thiết Lập Điểm";
                sSpeed.Text = "Tốc Độ (mm/ph) Max: 3000";
                sRobotPsogram1.Header = "Chương Trình Robot";
                sCommand.Header = "Điều Khiển";
                sRun.Header = "Chạy";
                sCoordinate.Header = "Tọa Độ";
                sAxisX.Content = "Trục X:";
                sAxisY1.Content = "Trục Y1:";
                sAxisY2.Content = "Trục Y2:";
                sAxisZ1.Content = "Trục Z1:";
                sAxisZ2.Content = "Trục Z2:";
                sStatus1.Header = "Trạng Thái";
                sCurrentVelocity.Content = "Vận Tốc Hiện Tại:";
                sTimerDelay.Content = "Thời Gian Chờ:";
                sCommand1.Header = "Điều Khiển";
                sStatus.Header = "Trạng Thái";
                SDelay.Content = "Chọn";
                SDO.Content = "Chọn";
                WDI.Content = "Chờ";
                SAO.Content = "Chọn";
               

            }
            else
            {
                sLanguage.Content = "Tiếng Việt";
                Setup.Content = "Setup";
                ManualControl.Content = "Manual Control";
                IOControl.Content = "IO Control";
                RobotProgram.Content = "Robot Program";
                AutoRun.Content = "Auto Run";
                Interface.Content = "Robot control interface";
                start.Content = "To Start Robot:";
                Sestop.Content = "1: Press 'Estop' to enable robot.";
                Sroboton.Content = "2: Press 'Robot On' to the alble to control to robot in joint mode.";
                Shome.Content = "3: Press 'Home' to the alble to control to robot in full mode.";
                MainPageIteam.Header = "Main Page";
                SetupItem.Header = "Setup";
                ManualControlItem.Header = "Manual Control";
                IOControlItem.Header = "IO Control";
                RobotProgramItem.Header = "Robot Program";
                AutoRunItem.Header = "Auto Run";
                sJoint.Content = "Joint";
                sPosition.Content = "Position (mm)";
                sAcceleration.Content = "Acceleration (ms)";
                sDeceleration.Content = "Deceleration (ms)";
                sVelocity.Content = "Velocity (r/min)";
                sJointName.Content = "Joint Name";
                sJoint0.Content = "Joint 0";
                sJoint1.Content = "Joint 1";
                sJoint2.Content = "Joint 2";
                sJoint3.Content = "Joint 3";
                sJoint4.Content = "Joint 4";
                sHomeSetting.Header = "Home Setting";
                sHomeParameter.Content = "Home Parameter";
                UpdateHome.Content = "Update Home Parameter";
                sJogParameter.Header = "Jog Parameter";
                sVelocity1.Content = "Velocity (r/min)";
                sMoveJog.Header = "Move Jog";
                sMovePoint.Header = "Move Point";
                MovingItem.Header = "Moving";
                SettingItem.Header = "Setting";
                BlockItem.Header = "Block";
                Moving.Content = "Moving";
                Setting.Content = "Setting";
                Block.Content = "Block";
                MoveRobot.Content = "Move Robot";
                SetPoint.Content = "Set Point";
                sSpeed.Text = "Speed (mm/ph) Max: 3000";
                sRobotPsogram1.Header = "Robot Program";
                sCommand.Header = "Command";
                sRun.Header = "Run";
                sCoordinate.Header = "World coordinate";
                sAxisX.Content = "Axis X:";
                sAxisY1.Content = "Axis Y1:";
                sAxisY2.Content = "Axis Y2:";
                sAxisZ1.Content = "Axis Z1:";
                sAxisZ2.Content = "Axis Z2:";
                sStatus1.Header = "Status";
                sCurrentVelocity.Content = "Current Velocity:";
                sTimerDelay.Content = "TimerDelay:";
                sCommand1.Header = "Command";
                sStatus.Header = "STATUS";
                SDelay.Content = "Set";
                SDO.Content = "Set";
                WDI.Content = "Wait";
                SAO.Content = "Set";
               
            }

        }
        private void KeyBoard_Click(object sender, RoutedEventArgs e)
        {
            Process[] pname = Process.GetProcessesByName("BanPhim");
            if (pname.Length == 0)
            {
                KeyBoard.Background = new SolidColorBrush(Color.FromArgb(200, 255, 0, 0));
                Process.Start(@"F:\Cong_Viec\BanPhim\BanPhim\bin\Debug\BanPhim.exe");
            }
                

            else
            {
                KeyBoard.Background = new SolidColorBrush(Color.FromArgb(255, 52, 142, 246));
                foreach (Process worker in pname)
                {
                    worker.Kill();
                    worker.WaitForExit();
                    worker.Dispose();
                }
            }




        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WWhome = true;
            PlayRun.IsEnabled = true;
            WPlayRun = true;

        }
        #region UPDATE
        private void UpdateHome_Click(object sender, RoutedEventArgs e)
        {
            string Systemconfig = Directory.GetCurrentDirectory() + @"\" + "HOME.txt";
            string text = "";
            SpeedHomeX = Convert.ToInt16(VelocityX.Text);
            AccelHomeX = Convert.ToInt16(AccelX.Text);
            DecelHomeX = Convert.ToInt16(DecelX.Text);
            PositioHomeX = Convert.ToInt16(PositionX.Text);
            SpeedHomeY1 = Convert.ToInt16(VelocityY1.Text);
            AccelHomeY1 = Convert.ToInt16(AccelY1.Text);
            DecelHomeY1 = Convert.ToInt16(DecelY1.Text);
            PositioHomeY1 = Convert.ToInt16(PositionY1.Text);
            SpeedHomeY2 = Convert.ToInt16(VelocityY2.Text);
            AccelHomeY2 = Convert.ToInt16(AccelY2.Text);
            DecelHomeY2 = Convert.ToInt16(DecelY2.Text);
            PositioHomeY2 = Convert.ToInt16(PositionY2.Text);
            SpeedHomeZ1 = Convert.ToInt16(VelocityZ1.Text);
            AccelHomeZ1 = Convert.ToInt16(AccelZ1.Text);
            DecelHomeZ1 = Convert.ToInt16(DecelZ1.Text);
            PositioHomeZ1 = Convert.ToInt16(PositionZ1.Text);
            SpeedHomeZ2 = Convert.ToInt16(VelocityZ2.Text);
            AccelHomeZ2 = Convert.ToInt16(AccelZ2.Text);
            DecelHomeZ2 = Convert.ToInt16(DecelZ2.Text);
            PositioHomeZ2 = Convert.ToInt16(PositionZ2.Text);
            text += PositionX.Text + "\r\n";
            text += PositionY1.Text + "\r\n";
            text += PositionY2.Text + "\r\n";
            text += PositionZ1.Text + "\r\n";
            text += PositionZ2.Text + "\r\n";
            text += VelocityX.Text + "\r\n";
            text += VelocityY1.Text + "\r\n";
            text += VelocityY2.Text + "\r\n";
            text += VelocityZ1.Text + "\r\n";
            text += VelocityZ2.Text + "\r\n";
            text += AccelX.Text + "\r\n";
            text += AccelY1.Text + "\r\n";
            text += AccelY2.Text + "\r\n";
            text += AccelZ1.Text + "\r\n";
            text += AccelZ2.Text + "\r\n";
            text += DecelX.Text + "\r\n";
            text += DecelY1.Text + "\r\n";
            text += DecelY2.Text + "\r\n";
            text += DecelZ1.Text + "\r\n";
            text += DecelZ2.Text;
            File.WriteAllText(Systemconfig, text);
        }
        private void UpdateIOname_Click(object sender, RoutedEventArgs e)
        {
            string Systemconfig1 = Directory.GetCurrentDirectory() + @"\" + "IO.txt";
            string text = "";
            text += ioname0.Text + "\r\n";
            text += ioname1.Text + "\r\n";
            text += ioname2.Text + "\r\n";
            text += ioname3.Text + "\r\n";
            text += ioname4.Text + "\r\n";
            text += ioname5.Text + "\r\n";
            text += ioname6.Text + "\r\n";
            text += ioname7.Text + "\r\n";
            text += ioname8.Text + "\r\n";
            text += ioname9.Text + "\r\n";
            text += ioname10.Text + "\r\n";
            text += ioname11.Text + "\r\n";
            text += ioname12.Text + "\r\n";
            text += ioname13.Text + "\r\n";
            text += ioname14.Text + "\r\n";
            text += ioname15.Text;
            File.WriteAllText(Systemconfig1, text);
        }
        #endregion
        private void PlayProgram_Click(object sender, RoutedEventArgs e)
        {

        }
        #region CHECK
        private void Check_IO()
        {
            if (out_put1[0] == '1') In7.IsChecked = true;
            else In7.IsChecked = false;
            if (out_put1[1] == '1') In6.IsChecked = true;
            else In6.IsChecked = false;
            if (out_put1[2] == '1') In5.IsChecked = true;
            else In5.IsChecked = false;
            if (out_put1[3] == '1') In4.IsChecked = true;
            else In4.IsChecked = false;
            if (out_put1[4] == '1') In3.IsChecked = true;
            else In3.IsChecked = false;
            if (out_put1[5] == '1') In2.IsChecked = true;
            else In2.IsChecked = false;
            if (out_put1[6] == '1') In1.IsChecked = true;
            else In1.IsChecked = false;
            if (out_put1[7] == '1') In0.IsChecked = true;
            else In0.IsChecked = false;

            if (out_put2[0] == '1') In15.IsChecked = true;
            else In15.IsChecked = false;
            if (out_put2[1] == '1') In14.IsChecked = true;
            else In14.IsChecked = false;
            if (out_put2[2] == '1') In13.IsChecked = true;
            else In13.IsChecked = false;
            if (out_put2[3] == '1') In12.IsChecked = true;
            else In12.IsChecked = false;
            if (out_put2[4] == '1') In11.IsChecked = true;
            else In11.IsChecked = false;
            if (out_put2[5] == '1') In10.IsChecked = true;
            else In10.IsChecked = false;
            if (out_put2[6] == '1') In9.IsChecked = true;
            else In9.IsChecked = false;
            if (out_put2[7] == '1') In8.IsChecked = true;
            else In8.IsChecked = false;
        }
        private void Check_DO()
        {
            if (out_put3[0] == '1') Out7.IsChecked = true;
            else Out7.IsChecked = false;
            if (out_put3[1] == '1') Out6.IsChecked = true;
            else Out6.IsChecked = false;
            if (out_put3[2] == '1') Out5.IsChecked = true;
            else Out5.IsChecked = false;
            if (out_put3[3] == '1') Out4.IsChecked = true;
            else Out4.IsChecked = false;
            if (out_put3[4] == '1') Out3.IsChecked = true;
            else Out3.IsChecked = false;
            if (out_put3[5] == '1') Out2.IsChecked = true;
            else Out2.IsChecked = false;
            if (out_put3[6] == '1') Out1.IsChecked = true;
            else Out1.IsChecked = false;
            if (out_put3[7] == '1') Out0.IsChecked = true;
            else Out0.IsChecked = false;

            if (out_put4[0] == '1') Out15.IsChecked = true;
            else Out15.IsChecked = false;
            if (out_put4[1] == '1') Out14.IsChecked = true;
            else Out14.IsChecked = false;
            if (out_put4[2] == '1') Out13.IsChecked = true;
            else Out13.IsChecked = false;
            if (out_put4[3] == '1') Out12.IsChecked = true;
            else Out12.IsChecked = false;
            if (out_put4[4] == '1') Out11.IsChecked = true;
            else Out11.IsChecked = false;
            if (out_put4[5] == '1') Out10.IsChecked = true;
            else Out10.IsChecked = false;
            if (out_put4[6] == '1') Out9.IsChecked = true;
            else Out9.IsChecked = false;
            if (out_put4[7] == '1') Out8.IsChecked = true;
            else Out8.IsChecked = false;
        }
        #endregion
    }
}
