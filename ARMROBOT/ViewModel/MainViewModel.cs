using ARMROBOT.Class;
using ARMROBOT.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
namespace ARMROBOT.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private ObservableCollection<string> _test;
        public ObservableCollection<string> test
        {
            get { return _test; }
            set
            {
                SetProperty(ref _test, value, nameof(test));
            }
        }
        private ObservableCollection<string> _test1;
        public ObservableCollection<string> test1
        {
            get { return _test1; }
            set
            {
                SetProperty(ref _test1, value, nameof(test1));
            }
        }
        private ObservableCollection<IOname> _IO;
        public ObservableCollection<IOname> IO
        {
            get { return _IO; }
            set
            {
                SetProperty(ref _IO, value, nameof(IO));
            }
        }
        public static string _DisplayedImagePath;
        public string DisplayedImagePath
        {
            get { return _DisplayedImagePath; }
            set { SetProperty(ref _DisplayedImagePath, value, nameof(DisplayedImagePath)); }

        }
        public bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { SetProperty(ref _IsSelected, value, nameof(IsSelected)); }
        }


        public string _Point_X;
        public string Point_X
        {
            get => _Point_X;
            set => SetProperty(ref _Point_X, value, nameof(Point_X));
        }
        public string _Point_Y1;
        public string Point_Y1
        {
            get => _Point_Y1;
            set => SetProperty(ref _Point_Y1, value, nameof(Point_Y1));
        }
        public string _Point_Y2;
        public string Point_Y2
        {
            get => _Point_Y2;
            set => SetProperty(ref _Point_Y2, value, nameof(Point_Y2));
        }
        public string _Point_Z1;
        public string Point_Z1
        {
            get => _Point_Z1;
            set => SetProperty(ref _Point_Z1, value, nameof(Point_Z1));
        }
        public string _Point_Z2;
        public string Point_Z2
        {
            get => _Point_Z2;
            set => SetProperty(ref _Point_Z2, value, nameof(Point_Z2));
        }
        public int _Speedrun;
        public int Speedrun
        {
            get => _Speedrun;
            set => SetProperty(ref _Speedrun, value, nameof(Speedrun));
        }
        public string _Times;
        public string Times
        {
            get => _Times;
            set => SetProperty(ref _Times, value, nameof(Times));
        }
        public string _Timer;
        public string Timer
        {
            get => _Timer;
            set => SetProperty(ref _Timer, value, nameof(Timer));
        }
        public string _Namefile;
        public string Namefile
        {
            get => _Namefile;
            set => SetProperty(ref _Namefile, value, nameof(Namefile));
        }
        public string _tensave;
        public string tensave
        {
            get => _tensave;
            set => SetProperty(ref _tensave, value, nameof(tensave));
        }
       
        public bool openProgram { get; set; }
        public bool openRun { get; set; }
        public bool save { get; set; }
        public bool saves { get; set; }
        FileExPlorer fileExPlorer;
        public static string duongdan { get; set; }
        public static string duongdanrun { get; set; }
        #region icommand
        public ICommand SetPoint { get; set; }
        public ICommand Loaded { get; set; }
        public ICommand ItemDetermine { get; set; }
        public ICommand ItemDetermine1 { get; set; }
        public ICommand Delete { get; set; }
        public ICommand OpenFile { get; set; }
        public ICommand OpenFileRun { get; set; }
        public ICommand SaveFile { get; set; }
        public ICommand Runline { get; set; }
        public ICommand Loop { get; set; }
        public ICommand Endloop { get; set; }
        public ICommand Repeat { get; set; }
        public ICommand Endrepeat { get; set; }
        public ICommand SetDelay { get; set; }
        public ICommand SetOutPut { get; set; }
        public ICommand WaitInPut { get; set; }
        public ICommand DO { get; set; }
        public ICommand DI { get; set; }
        public ICommand Newfile { get; set; }
        public ICommand SavesFile { get; set; }
        public ICommand ok { get; set; }
        public ICommand oksave { get; set; }
        public ICommand deletetxt { get; set; }
        #endregion


        public MainViewModel()
        {

            
            string[] Out = new string[5];
            string[] Input = new string[5];
            MessageBoxManager.Yes = "Down";
            MessageBoxManager.No = "Up";
            MessageBoxManager.Cancel = "Replace";
            string Systemconfig = Directory.GetCurrentDirectory() + @"\Code";
            DisplayedImagePath = Directory.GetCurrentDirectory() + @"\" + "Robot.jpg";

            Loaded = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                Point_Y1 = Point_Y2 = Point_Z1 = Point_Z2 = Point_X = "0";
                Speedrun = 100;
                Times = "2";
                Timer = "1";
                test = new ObservableCollection<string>();

                //IO = new ObservableCollection<IOname>();
                //for(int i=0;i<16; i++)
                //{
                //    IOname ioname = new IOname();
                //    ioname.name = "INPUT " + i.ToString() +": ";
                //    IO.Add(ioname);
                //}    
                
                MainWindow.testprogram = test;

            });

            SetPoint = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                try
                {
                    if ((int)p < 0)
                    {
                        test.Add("MOVE X:" + Point_X +
                            " Y1:" + Point_Y1 + " Y2:" + Point_Y2 + " Z1:" + Point_Z1 +
                            " Z2:" + Point_Z2 + " V:" + Speedrun);
                    }
                    else if ((int)p >= 0)
                    {

                        MessageBoxManager.Register();
                        System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Line SetPoint", "Thông báo", (MessageBoxButton)System.Windows.Forms.MessageBoxButtons.YesNoCancel, MessageBoxImage.Information);
                        MessageBoxManager.Unregister();
                        if (dialogResult == System.Windows.Forms.DialogResult.No)
                        {
                            test.Insert((int)p, "MOVE X:" + Point_X +
                                " Y1:" + Point_Y1 + " Y2:" + Point_Y2 + " Z1:" + Point_Z1 +
                                " Z2:" + Point_Z2 + " V:" + Speedrun);
                            MainWindow.TimerRefrest.IsEnabled = true;


                        }
                        else if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                        {
                            test.Insert((int)p + 1, "MOVE X:" + Point_X +
                                " Y1:" + Point_Y1 + " Y2:" + Point_Y2 + " Z1:" + Point_Z1 +
                                " Z2:" + Point_Z2 + " V:" + Speedrun);
                            MainWindow.TimerRefrest.IsEnabled = true;
                        }
                        else
                        {
                            MessageBoxManager.Yes = "Yes";
                            MessageBoxManager.No = "No";
                            MessageBoxManager.Register();
                            System.Windows.Forms.DialogResult dialogResult1 = (System.Windows.Forms.DialogResult)MessageBox.Show("Thực sự muốn replace", "Thông báo", (MessageBoxButton)System.Windows.Forms.MessageBoxButtons.YesNo, MessageBoxImage.Information);
                            MessageBoxManager.Unregister();
                            if (dialogResult1 == System.Windows.Forms.DialogResult.Yes)
                            {
                                test.RemoveAt((int)p);
                                test.Insert((int)p, "MOVE X:" + Point_X +
                                    " Y1:" + Point_Y1 + " Y2:" + Point_Y2 + " Z1:" + Point_Z1 +
                                    " Z2:" + Point_Z2 + " V:" + Speedrun);
                                MessageBoxManager.Yes = "Down";
                                MessageBoxManager.No = "Up";
                                MainWindow.TimerRefrest.IsEnabled = true;

                            }
                            else
                            {
                                MessageBoxManager.Yes = "Down";
                                MessageBoxManager.No = "Up";
                            }

                        }


                    }

                }
                catch (Exception ex)
                {


                }

            });
            Loop = new RelayCommand<object>((P) => { return true; }, (p) =>
            {
                try
                {
                    if ((int)p < 0)
                    {
                        test.Add("LOOP");
                    }
                    else if ((int)p >= 0)
                    {

                        MessageBoxManager.Register();
                        System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Line SetPoint", "Thông báo", (MessageBoxButton)System.Windows.Forms.MessageBoxButtons.YesNoCancel, MessageBoxImage.Information);
                        MessageBoxManager.Unregister();
                        if (dialogResult == System.Windows.Forms.DialogResult.No)
                        {
                            test.Insert((int)p, "LOOP");
                            MainWindow.TimerRefrest.IsEnabled = true;


                        }
                        else if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                        {
                            test.Insert((int)p + 1, "LOOP");
                            MainWindow.TimerRefrest.IsEnabled = true;
                        }
                        else
                        {
                            test.RemoveAt((int)p);
                            test.Insert((int)p, "LOOP");
                            MainWindow.TimerRefrest.IsEnabled = true;
                        }
                    }

                }
                catch (Exception ex)
                {


                }
            });
            Endloop = new RelayCommand<object>((P) => { return true; }, (p) =>
            {
                try
                {
                    if ((int)p < 0)
                    {
                        test.Add("ENDLOOP");
                    }
                    else if ((int)p >= 0)
                    {

                        MessageBoxManager.Register();
                        System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Line SetPoint", "Thông báo", (MessageBoxButton)System.Windows.Forms.MessageBoxButtons.YesNoCancel, MessageBoxImage.Information);
                        MessageBoxManager.Unregister();
                        if (dialogResult == System.Windows.Forms.DialogResult.No)
                        {
                            test.Insert((int)p, "ENDLOOP");
                            MainWindow.TimerRefrest.IsEnabled = true;


                        }
                        else if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                        {
                            test.Insert((int)p + 1, "ENDLOOP");
                            MainWindow.TimerRefrest.IsEnabled = true;
                        }
                        else
                        {
                            test.RemoveAt((int)p);
                            test.Insert((int)p, "ENDLOOP");
                            MainWindow.TimerRefrest.IsEnabled = true;
                        }
                    }


                }
                catch (Exception ex)
                {


                }
            });
            Repeat = new RelayCommand<object>((P) => { return true; }, (p) =>
            {
                try
                {
                    if ((int)p < 0)
                    {
                        test.Add("REPEAT " + Convert.ToInt16(Times.ToString()));
                    }
                    else if ((int)p >= 0)
                    {

                        MessageBoxManager.Register();
                        System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Line SetPoint", "Thông báo", (MessageBoxButton)System.Windows.Forms.MessageBoxButtons.YesNoCancel, MessageBoxImage.Information);
                        MessageBoxManager.Unregister();
                        if (dialogResult == System.Windows.Forms.DialogResult.No)
                        {
                            test.Insert((int)p, "REPEAT " + Convert.ToInt16(Times.ToString()));
                            MainWindow.TimerRefrest.IsEnabled = true;


                        }
                        else if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                        {
                            test.Insert((int)p + 1, "REPEAT " + Convert.ToInt16(Times.ToString()));
                            MainWindow.TimerRefrest.IsEnabled = true;
                        }
                        else
                        {
                            test.RemoveAt((int)p);
                            test.Insert((int)p, "REPEAT " + Convert.ToInt16(Times.ToString()));
                            MainWindow.TimerRefrest.IsEnabled = true;
                        }
                    }


                }
                catch (Exception ex)
                {


                }
            });
            Endrepeat = new RelayCommand<object>((P) => { return true; }, (p) =>
            {
                try
                {
                    if ((int)p < 0)
                    {
                        test.Add("ENDREPEAT");
                    }
                    else if ((int)p >= 0)
                    {

                        MessageBoxManager.Register();
                        System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Line SetPoint", "Thông báo", (MessageBoxButton)System.Windows.Forms.MessageBoxButtons.YesNoCancel, MessageBoxImage.Information);
                        MessageBoxManager.Unregister();
                        if (dialogResult == System.Windows.Forms.DialogResult.No)
                        {
                            test.Insert((int)p, "ENDREPEAT");
                            MainWindow.TimerRefrest.IsEnabled = true;


                        }
                        else if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                        {
                            test.Insert((int)p + 1, "ENDREPEAT");
                            MainWindow.TimerRefrest.IsEnabled = true;
                        }
                        else
                        {
                            test.RemoveAt((int)p);
                            test.Insert((int)p, "ENDREPEAT");
                            MainWindow.TimerRefrest.IsEnabled = true;
                        }
                    }


                }
                catch (Exception ex)
                {


                }
            });
            SetDelay = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                try
                {
                    if ((int)p < 0)
                    {


                        test.Add("DELAY " + Convert.ToInt16(Timer.ToString()));


                    }
                    else if ((int)p >= 0)
                    {

                        MessageBoxManager.Register();
                        System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Line SetPoint", "Thông báo", (MessageBoxButton)System.Windows.Forms.MessageBoxButtons.YesNoCancel, MessageBoxImage.Information);
                        MessageBoxManager.Unregister();
                        if (dialogResult == System.Windows.Forms.DialogResult.No)
                        {
                            test.Insert((int)p, "DELAY " + Convert.ToInt16(Timer.ToString()));
                            MainWindow.TimerRefrest.IsEnabled = true;


                        }
                        else if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                        {
                            test.Insert((int)p + 1, "DELAY " + Convert.ToInt16(Timer.ToString()));
                            MainWindow.TimerRefrest.IsEnabled = true;
                        }
                        else
                        {
                            test.RemoveAt((int)p);
                            test.Insert((int)p, "DELAY " + Convert.ToInt16(Timer.ToString()));
                            MainWindow.TimerRefrest.IsEnabled = true;
                        }




                    }

                }
                catch (Exception ex)
                {


                }
            });
            SetOutPut = new RelayCommand<object>((p) => { return true; }, (p) =>
            {

                try
                {
                    if ((int)p < 0)
                    {

                        if (Out[4] == "ON")
                        {
                            test.Add("SDOON " + Convert.ToInt16(Out[3]));
                        }
                        else
                        {
                            test.Add("SDOOF " + Convert.ToInt16(Out[3]));
                        }



                    }
                    else if ((int)p >= 0)
                    {

                        MessageBoxManager.Register();
                        System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Line SetPoint", "Thông báo", (MessageBoxButton)System.Windows.Forms.MessageBoxButtons.YesNoCancel, MessageBoxImage.Information);
                        MessageBoxManager.Unregister();
                        if (dialogResult == System.Windows.Forms.DialogResult.No)
                        {
                            if (Out[4] == "ON")
                            {
                                test.Insert((int)p, "SDOON " + Convert.ToInt16(Out[3]));
                            }
                            else
                            {
                                test.Insert((int)p, "SDOOF " + Convert.ToInt16(Out[3]));
                            }
                            MainWindow.TimerRefrest.IsEnabled = true;


                        }
                        else if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                        {
                            if (Out[4] == "ON")
                            {
                                test.Insert((int)p + 1, "SDOON " + Convert.ToInt16(Out[3]));
                            }
                            else
                            {
                                test.Insert((int)p + 1, "SDOOF " + Convert.ToInt16(Out[3]));
                            }
                            MainWindow.TimerRefrest.IsEnabled = true;
                        }
                        else
                        {
                            test.RemoveAt((int)p);
                            if (Out[4] == "ON")
                            {
                                test.Insert((int)p, "SDOON " + Convert.ToInt16(Out[3]));
                            }
                            else
                            {
                                test.Insert((int)p, "SDOOF " + Convert.ToInt16(Out[3]));
                            }
                            MainWindow.TimerRefrest.IsEnabled = true;
                        }


                    }

                }
                catch (Exception ex)
                {


                }

            });
            WaitInPut = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                try
                {
                    if ((int)p < 0)
                    {

                        if (Input[4] == "ON")
                        {
                            test.Add("WDION " + Convert.ToInt16(Input[3]));
                        }
                        else
                        {
                            test.Add("WDIOF " + Convert.ToInt16(Input[3]));
                        }


                    }
                    else if ((int)p >= 0)
                    {

                        MessageBoxManager.Register();
                        System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Line SetPoint", "Thông báo", (MessageBoxButton)System.Windows.Forms.MessageBoxButtons.YesNoCancel, MessageBoxImage.Information);
                        MessageBoxManager.Unregister();
                        if (dialogResult == System.Windows.Forms.DialogResult.No)
                        {
                            if (Input[4] == "ON")
                            {
                                test.Insert((int)p, "WDION " + Convert.ToInt16(Input[3]));
                            }
                            else
                            {
                                test.Insert((int)p, "WDIOF " + Convert.ToInt16(Input[3]));
                            }
                            MainWindow.TimerRefrest.IsEnabled = true;


                        }
                        else if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                        {
                            if (Input[4] == "ON")
                            {
                                test.Insert((int)p + 1, "WDION " + Convert.ToInt16(Input[3]));
                            }
                            else
                            {
                                test.Insert((int)p + 1, "WDIOF " + Convert.ToInt16(Input[3]));
                            }
                            MainWindow.TimerRefrest.IsEnabled = true;
                        }
                        else
                        {
                            test.RemoveAt((int)p);
                            if (Input[4] == "ON")
                            {
                                test.Insert((int)p, "WDION " + Convert.ToInt16(Input[3]));
                            }
                            else
                            {
                                test.Insert((int)p, "WDIOF " + Convert.ToInt16(Input[3]));
                            }
                            MainWindow.TimerRefrest.IsEnabled = true;
                        }


                    }

                }
                catch (Exception ex)
                {


                }
            });
            DO = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                Out = p.ToString().Split(' ');
            });
            DI = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                Input = p.ToString().Split(' ');
            });
            ItemDetermine = new RelayCommand<object>((p) => { return true; }, (p) =>
            {

            });
            Delete = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                test.Remove(p.ToString());

            });
            Newfile = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                test.Clear();

            });
            OpenFile = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                
                try
                {
                    test1 = new ObservableCollection<string>();
                    using (System.Windows.Forms.FolderBrowserDialog a = new System.Windows.Forms.FolderBrowserDialog() { Description = "Select your folder path" })
                    {
                        foreach (var item in Directory.GetFiles(Systemconfig))
                        {
                            FileInfo fi = new FileInfo(item);
                            test1.Add(fi.FullName);
                        }
                    }
                    openProgram = true;
                    openRun = false;
                    fileExPlorer = new FileExPlorer();
                    fileExPlorer.ShowDialog();
                    
                }
                catch (Exception ex)
                {


                }

            });
            OpenFileRun = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                try
                {
                    
                    test1 = new ObservableCollection<string>();
                    using (System.Windows.Forms.FolderBrowserDialog a = new System.Windows.Forms.FolderBrowserDialog() { Description = "Select your folder path" })
                    {
                        foreach (var item in Directory.GetFiles(Systemconfig))
                        {
                            FileInfo fi = new FileInfo(item);
                            test1.Add(fi.FullName);
                        }
                    }
                    openRun = true;
                    openProgram = false;
                    fileExPlorer = new FileExPlorer();
                    fileExPlorer.ShowDialog();
                    
                }
                catch (Exception ex)
                {


                }



            });
            ok = new RelayCommand<object>((p) => { return true; }, (p) =>
            {


                if (openRun == true&&openProgram==false)
                {
                    duongdanrun = p.ToString();
                    MainWindow.TimerRichtextBox.IsEnabled = true;
                    fileExPlorer.Close();
                }
                else if(openProgram=true&&openRun==false)
                {
                    duongdan = p.ToString();
                    if (test.Count > 0)
                    {
                        test.Clear();
                    }
                    int count = 0;
                    using (StreamReader sr = new StreamReader(duongdan))
                    {

                        while (sr.ReadLine() != null)
                        {
                            count++;
                        }
                    }
                    using (StreamReader sr = new StreamReader(duongdan))
                    {
                        for (int i = 0; i < count; i++)
                        {
                            string list = sr.ReadLine();
                            test.Add(list.Substring(list.IndexOf(".") + 3));
                        }
                    }
                    Namefile = duongdan;
                    fileExPlorer.Close();
                }
                
            });
            oksave = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                try
                {
                    if (save == true)
                    {
                        if (MainWindow.newsave)
                        {
                            string text = "";
                            int i = -1;
                            foreach (var item in test)
                            {
                                i++;
                                text += i.ToString() + ".  " + item + "\r\n";

                            }
                            text = text.Substring(0, (text.Length - 2));
                            if(tensave!=null)
                            {
                                File.WriteAllText( Systemconfig+ "\\" + tensave + ".txt", text);
                                Namefile =  Systemconfig+ "\\" + tensave;
                                MainWindow.newsave = false;
                                fileExPlorer.Close();
                                MessageBox.Show("OK");
                            }
                            else
                            {
                                MessageBox.Show("Chưa nhập name");
                            }
                            
                        }
                    }
                    else
                    {
                        string text = "";
                        int i = -1;
                        foreach (var item in test)
                        {
                            i++;
                            text += i.ToString() + ".  " + item + "\r\n";
                        }
                        text = text.Substring(0, (text.Length - 2));
                        if (MainWindow.newsave)
                        {
                            if(tensave!=null)
                            {
                                File.WriteAllText( Systemconfig+ "\\" + tensave + ".txt", text);
                                Namefile = Systemconfig + "\\" + tensave;
                                MainWindow.newsave = false;
                                fileExPlorer.Close();
                                MessageBox.Show("OK");
                            } 
                            else
                            {
                                MessageBox.Show("Chưa nhập name");
                            }    
                            
                        }
                        else
                        {

                            if (MainWindow.pressopen)
                            {
                                if (tensave != null)
                                {
                                    File.Delete(duongdan);
                                    File.WriteAllText( Systemconfig+ "\\" + tensave + ".txt", text);
                                    Namefile =  Systemconfig+ "\\" + tensave;
                                    fileExPlorer.Close();
                                    MessageBox.Show("OK");
                                }
                                else
                                {
                                    MessageBox.Show("Chưa nhập name");
                                }

                            }
                            else
                            {
                                if (tensave != null)
                                {
                                    File.Delete(duongdan);
                                    File.WriteAllText( Systemconfig+ "\\" + tensave + ".txt", text);
                                    Namefile = Systemconfig + "\\" + tensave;
                                    fileExPlorer.Close();
                                    MessageBox.Show("OK");
                                }    
                                else
                                {
                                    MessageBox.Show("Chưa nhập name");
                                }    
                                
                            }
                        }
                    }
                   
                }
                catch (Exception)
                {


                }
            });
            SaveFile = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                try
                {
                    string text = "";
                    int i = -1;
                    foreach (var item in test)
                    {
                        i++;
                        text += i.ToString() + ".  " + item + "\r\n";

                    }
                    text = text.Substring(0, (text.Length - 2));
                    if (MainWindow.newsave)
                    {
                        test1 = new ObservableCollection<string>();
                        using (System.Windows.Forms.FolderBrowserDialog a = new System.Windows.Forms.FolderBrowserDialog() { Description = "Select your folder path" })
                        {
                            foreach (var item in Directory.GetFiles(Systemconfig))
                            {
                                FileInfo fi = new FileInfo(item);
                                test1.Add(fi.FullName);
                            }
                        }
                        fileExPlorer = new FileExPlorer();
                        fileExPlorer.ShowDialog();
                    }
                    else
                    {
                        if (MainWindow.pressopen)
                        {
                            File.WriteAllText(duongdan, text);
                            Namefile = duongdan;
                        }
                        else
                        {
                            File.WriteAllText( Systemconfig+ "\\" + tensave + ".txt", text);
                            Namefile = Systemconfig + "\\" + tensave;
                        }
                        MessageBox.Show("OK");
                    }
                    save = true;
                    saves = false;
                }
                catch (Exception ex)
                {


                }


            });
            SavesFile = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                try
                {
                    test1 = new ObservableCollection<string>();
                    using (System.Windows.Forms.FolderBrowserDialog a = new System.Windows.Forms.FolderBrowserDialog() { Description = "Select your folder path" })
                    {
                        foreach (var item in Directory.GetFiles(Systemconfig))
                        {
                            FileInfo fi = new FileInfo(item);
                            test1.Add(fi.FullName);
                        }
                    }
                    fileExPlorer = new FileExPlorer();
                    fileExPlorer.ShowDialog();
                    if (openProgram)
                    {

                    }
                    else
                    {
                        duongdan = Systemconfig + "\\" + tensave + ".txt";
                    }

                    //string text = "";
                    //int i = -1;
                    //foreach (var item in test)
                    //{
                    //    i++;
                    //    text += i.ToString() + ".  " + item + "\r\n";
                    //}
                    //text = text.Substring(0, (text.Length - 2));
                    //if (MainWindow.newsave)
                    //{
                    //    if (Save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    //    {
                    //        File.WriteAllText(Save.FileName + ".txt", text);
                    //        Namefile = Save.FileName;
                    //        MainWindow.newsave = false;
                    //    }

                    //}
                    //else
                    //{
                    //    if (Save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    //    {

                    //        if (MainWindow.pressopen)
                    //        {
                    //            File.Delete(duongdan);
                    //            File.WriteAllText(Save.FileName + ".txt", text);
                    //            Namefile = Save.FileName;
                    //            duongdan = Save.FileName;
                    //        }
                    //        else
                    //        {
                    //            File.Delete(Save.FileName);
                    //            File.WriteAllText(Save.FileName + ".txt", text);
                    //            Namefile = Save.FileName;
                    //            duongdan = Save.FileName;
                    //        }
                    //    }


                    //}
                    save = false;
                    saves = true;
                }
                catch (Exception ex)
                {

                }


            });
            Runline = new RelayCommand<object>((p) => { return true; }, (p) =>
             {
                 try
                 {

                     MainWindow.Readline = p.ToString();
                     MainWindow.TimerDelay.IsEnabled = true;
                     MainWindow.WRunline = true;
                 }
                 catch (Exception ex)
                 {


                 }

             });
            deletetxt = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                try
                {
                    
                    File.Delete(p.ToString());
                    test1.Remove((string)p);
                    FileExPlorer.timerResfret.IsEnabled = true;
                }
                catch (Exception ex)
                {

                    
                }
                
            });
        }

    }
    public class IOname
    {
        public string name { get; set; }    
        public string conten { get; set; }
    }
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo culture)
        {
            ListViewItem item = (ListViewItem)value;
            ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            int index = listView.ItemContainerGenerator.IndexFromContainer(item);

            return index.ToString();
        }



        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }
}
