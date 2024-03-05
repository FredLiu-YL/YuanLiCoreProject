using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
using YuanliCore.Interface;


namespace YuanliCore.Motion
{
    /// <summary>
    /// AxisMotionUC.xaml 的互動邏輯
    /// </summary>
    public partial class AxisMotionUC : UserControl, INotifyPropertyChanged
    {
        private bool isGetStatus = false;
        private VelocityParams moveVelParams = new VelocityParams(5000);
        private System.Windows.Threading.DispatcherTimer dispatcherTimerFeedback;
        private string axisName;
        private double position;
        private double accMoveVelTime = 0.1, decMoveVelTime = 0.1, accHomVelTime = 0.1, decHomeVelTime = 0.1;
        private Brush pELBackground = Brushes.White, nELBackground = Brushes.White, oRGBackground = Brushes.White;
        private double moveFinalVelocity, moveAccTime, moveDecTime, homeFinalVelocity, homeAccTime, homeDecTime;



        public AxisMotionUC()
        {
            InitializeComponent();
            if (HomeModeList.Count == 0) foreach (HomeModes item in Enum.GetValues(typeof(HomeModes))) { HomeModeList.Add(item); };
        }

        private void AxisMotion_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //var c = Thread.CurrentThread;
                //StartAxisStatus();
                if (Axis == null) return;
                dispatcherTimerFeedback = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimerFeedback.Tick += new EventHandler(dispatcherTimerFeedback_Tick);
                dispatcherTimerFeedback.Interval = TimeSpan.FromMilliseconds(200);
                dispatcherTimerFeedback.Start();

                if (Axis.IsOpen) OnBackground = Brushes.LightGreen;
                else OffBackground = Brushes.LightGreen;

                SetHomeMode(Axis.HomeMode, AxisSetConfig.Direction);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    MessageBox.Show(ex.Message);
                });
            }
        }

        private void AxisMotion_Unloaded(object sender, RoutedEventArgs e)
        {
            isGetStatus = false;

            if (dispatcherTimerFeedback != null)
            {
                dispatcherTimerFeedback.Stop();
                dispatcherTimerFeedback.Tick -= new EventHandler(dispatcherTimerFeedback_Tick);
            }
        }

        /// <summary>
        /// 軸的原點、正負極限燈號
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimerFeedback_Tick(object sender, EventArgs e)
        {
            GetAxisStatusTest();
        }

        public static readonly DependencyProperty AxisProperty = DependencyProperty.Register("Axis", typeof(Axis), typeof(AxisMotionUC), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty AxisSetConfigProperty = DependencyProperty.Register("AxisSetConfig", typeof(AxisConfig), typeof(AxisMotionUC), new FrameworkPropertyMetadata(new AxisConfig(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnConfigChanged)));
        public static readonly DependencyProperty HomeAsycProperty = DependencyProperty.Register("HomeAsyc", typeof(Action<Axis>), typeof(AxisMotionUC), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Action<Axis> HomeAsyc
        {
            get => (Action<Axis>)GetValue(HomeAsycProperty);
            set => SetValue(HomeAsycProperty, value);
        }

        public Axis Axis
        {
            get
            {
                var axis = (Axis)GetValue(AxisProperty);
                return axis;
            }
            set => SetValue(AxisProperty, value);
        }

        public AxisConfig AxisSetConfig
        {
            get => (AxisConfig)GetValue(AxisSetConfigProperty);
            set => SetValue(AxisSetConfigProperty, value);
        }

        /// <summary>
        /// 取得或設定 軸名稱
        /// </summary>
        public string AxisName
        {
            get => axisName; set => SetValue(ref axisName, value);
        }



        /// <summary>
        /// 取得或設定 原點速度
        /// </summary>
        public double HomeVel { get; set; } = 2000;

        /// <summary>
        /// 取得或設定 移動加速度時間
        /// </summary>
        public double AccMoveVelTime { get => accMoveVelTime; set => SetValue(ref accMoveVelTime, value); }

        /// <summary>
        /// 取得或設定 移動減速度時間
        /// </summary>
        public double DecMoveVelTime { get => decMoveVelTime; set => SetValue(ref decMoveVelTime, value); }

        /// <summary>
        /// 取得或設定 原點加速度時間
        /// </summary>
        public double AccHomeVelTime { get => accHomVelTime; set => SetValue(ref accHomVelTime, value); }

        /// <summary>
        /// 取得或設定 原點減速度時間
        /// </summary>
        public double DecHomeVelTime { get => decHomeVelTime; set => SetValue(ref decHomeVelTime, value); }

        /// <summary>
        /// 取得或設定 位移量
        /// </summary>
        public string MoveDistance { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string HomeModeString { get; set; } = "";

        /// <summary>
        /// 當前位置
        /// </summary>
        public double Position { get => position; set => SetValue(ref position, value); }

        /// <summary>
        /// 取得或設定 軸在正極限上顏色
        /// </summary>
        public Brush PELBackground { get => pELBackground; set => SetValue(ref pELBackground, value); }

        /// <summary>
        /// 取得或設定 軸在負極限上顏色
        /// </summary>
        public Brush NELBackground { get => nELBackground; set => SetValue(ref nELBackground, value); }

        /// <summary>
        /// 取得或設定 軸在原點上顏色
        /// </summary>
        public Brush ORGBackground { get => oRGBackground; set => SetValue(ref oRGBackground, value); }


        public double MoveFinalVelocity { get => moveFinalVelocity; set => SetValue(ref moveFinalVelocity, value); }
        public double MoveAccTime { get => moveAccTime; set => SetValue(ref moveAccTime, value); }
        public double MoveDecTime { get => moveDecTime; set => SetValue(ref moveDecTime, value); }
        public double HomeFinalVelocity { get => homeFinalVelocity; set => SetValue(ref homeFinalVelocity, value); }
        public double HomeAccTime { get => homeAccTime; set => SetValue(ref homeAccTime, value); }
        public double HomeDecTime { get => homeDecTime; set => SetValue(ref homeDecTime, value); }

        /// <summary>
        /// 取得或設定 Home 模式
        /// </summary>
        public System.Collections.ObjectModel.ObservableCollection<HomeModes> HomeModeList { get; set; } = new System.Collections.ObjectModel.ObservableCollection<HomeModes>();

        /// <summary>
        /// 取得或設定 On顏色
        /// </summary>
        public Brush OnBackground { get; set; } = Brushes.Transparent;

        /// <summary>
        /// 取得或設定 Off顏色
        /// </summary>
        public Brush OffBackground { get; set; } = Brushes.Transparent;

        /// <summary>
        /// 設定軸參數
        /// </summary>
        public ICommand SetAxisCommand => new RelayCommand(() =>
        {
            //var moveV = ConvertMoveVelocity();

            //設定軸參數
            Axis.PositionNEL = AxisSetConfig.LimitNEL;
            Axis.PositionPEL = AxisSetConfig.LimitPEL;
            /* if(AxisSetConfig.MoveVel != null)
                 AxisSetConfig.MoveVel = new VelocityParams(0, AxisSetConfig.MoveVel.FinalVel, AccMoveVelTime, DecMoveVelTime);
             if(AxisSetConfig.HomeVel != null)
                 AxisSetConfig.HomeVel = new VelocityParams(0, AxisSetConfig.HomeVel.FinalVel, AccHomeVelTime, DecHomeVelTime);*/

            AxisSetConfig.MoveVel = new VelocityParams(0, MoveFinalVelocity, MoveAccTime, MoveDecTime);
            AxisSetConfig.HomeVel = new VelocityParams(0, HomeFinalVelocity, HomeAccTime, HomeDecTime);


            /* if (AxisSetConfig.MoveVel != null)
                 AxisSetConfig.MoveVel = new VelocityParams(0, AxisSetConfig.MoveVel.MaxVel, AccMoveVelTime, DecMoveVelTime);
             if (AxisSetConfig.HomeVel != null)
                 AxisSetConfig.HomeVel = new VelocityParams(0, AxisSetConfig.HomeVel.MaxVel, AccHomeVelTime, DecHomeVelTime);
            */
            switch (HomeModeString)
            {
                case "原點":
                    Axis.HomeMode = HomeModes.ORG;
                    AxisSetConfig.HomeMode = HomeModes.ORG;
                    AxisSetConfig.HomeDirection = HomeDirection.Backward;
                    AxisSetConfig.Direction = MotionDirections.Backward;
                    break;
                case "正極限":
                    Axis.HomeMode = HomeModes.EL;
                    AxisSetConfig.HomeMode = HomeModes.EL;
                    AxisSetConfig.HomeDirection = HomeDirection.Forward;
                    AxisSetConfig.Direction = MotionDirections.Forward;
                    break;
                case "負極限":
                    Axis.HomeMode = HomeModes.EL;
                    AxisSetConfig.HomeMode = HomeModes.EL;
                    AxisSetConfig.HomeDirection = HomeDirection.Backward;
                    AxisSetConfig.Direction = MotionDirections.Backward;
                    break;
                case "CurPos":
                    Axis.HomeMode = HomeModes.CurPos;
                    AxisSetConfig.HomeMode = HomeModes.CurPos;
                    AxisSetConfig.HomeDirection = HomeDirection.Backward;
                    AxisSetConfig.Direction = MotionDirections.Backward;
                    break;
                case "原點+Index":
                    Axis.HomeMode = HomeModes.ORGAndIndex;
                    AxisSetConfig.HomeMode = HomeModes.ORGAndIndex;
                    AxisSetConfig.HomeDirection = HomeDirection.Backward;
                    AxisSetConfig.Direction = MotionDirections.Backward;
                    break;
                case "原點+正極限":
                    Axis.HomeMode = HomeModes.ELAndIndex;
                    AxisSetConfig.HomeMode = HomeModes.ELAndIndex;
                    AxisSetConfig.HomeDirection = HomeDirection.Forward;
                    AxisSetConfig.Direction = MotionDirections.Forward;
                    break;
                case "原點+負極限":
                    Axis.HomeMode = HomeModes.ELAndIndex;
                    AxisSetConfig.HomeMode = HomeModes.ELAndIndex;
                    AxisSetConfig.HomeDirection = HomeDirection.Backward;
                    AxisSetConfig.Direction = MotionDirections.Backward;
                    break;
                default:
                    break;
            }
            // Axis.MotionVelParams = AxisSetConfig.MoveVel;


            Axis.AxisVelocity = AxisSetConfig.MoveVel;
            Axis.HomeVelocity = AxisSetConfig.HomeVel;



        });

        public ICommand SetAxisOnCommand => new RelayCommand(() =>
        {
            try
            {
                if (Axis.IsOpen) { MessageBox.Show("軸已開啟"); return; }
                Axis.Open();
                OnBackground = Brushes.LightGreen;
                OffBackground = Brushes.Transparent;
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    MessageBox.Show(ex.Message);
                });
            }
        });

        public ICommand SetAxisOffCommand => new RelayCommand(() =>
        {
            try
            {
                if (!Axis.IsOpen) { MessageBox.Show("軸未開啟，無法關閉"); return; }

                Axis.Close();
                OffBackground = Brushes.LightGreen;
                OnBackground = Brushes.Transparent;
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    MessageBox.Show(ex.Message);
                });
            }
        });

        public ICommand AxisHomeAsycCommand => new RelayCommand(async () =>
        {
            try
            {
                //HomeAsyc?.Invoke(Axis);
                await Axis.HomeAsync();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    //     MessageBox.Show(ex.Message);
                });
            }

        });

        public ICommand AxisStopAsycCommand => new RelayCommand(() =>
        {
            try
            {
                Axis.Stop();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    //     MessageBox.Show(ex.Message);
                });
            }
        });

        public ICommand AxisMoveMouseDownCommand => new RelayCommand<string>(async param =>
        {
            try
            {
                double dis = ConvertDistance();
                if (dis != 0 || Axis.IsRunning) return;

                switch (param)
                {
                    case "Move-":
                        await Axis.MoveToAsync(AxisSetConfig.LimitNEL);
                        break;
                    case "Move+":
                        await Axis.MoveToAsync(AxisSetConfig.LimitPEL);
                        break;
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    //  MessageBox.Show(ex.Message);
                });
            }
        });

        public ICommand AxisMoveMouseUpCommand => new RelayCommand<string>(async param =>
        {
            try
            {
                double distance = ConvertDistance();
                if (distance == 0)
                    Axis.Stop();
                else
                {
                    switch (param)
                    {
                        case "Move-":
                            await Axis.MoveAsync(-distance);
                            break;
                        case "Move+":
                            await Axis.MoveAsync(distance);
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    //     MessageBox.Show(ex.Message);
                });
            }
        });



        private double ConvertDistance()
        {
            double distance = 0;
            try
            {
                if (MoveDistance == "連續運動")
                    distance = 0;
                else
                    distance = Convert.ToDouble(MoveDistance);
            }
            catch (Exception)
            {
                throw new Exception("數值錯誤");
            }

            return distance;
        }

        private HomeModes ConvertHomeMode(string modeString)
        {
            HomeModes mode = HomeModes.ORG;

            switch (modeString)
            {
                case "負極限+Index":
                    break;

                case "負極限":
                    break;

                case "原點":
                    break;

                case "原點+Index":
                    break;

                case "CurPos":
                    break;

                case "正極限":
                    break;

                case "正極限+Index":
                    break;
            }
            return mode;
        }

        private void SetHomeMode(HomeModes homeModes, MotionDirections motionDirections = MotionDirections.Backward)
        {
            switch (homeModes)
            {
                case HomeModes.ORG:
                    HomeModeString = "原點";
                    break;
                case HomeModes.EL:
                    if (motionDirections == MotionDirections.Backward)
                        HomeModeString = "負極限";
                    else if (motionDirections == MotionDirections.Forward)
                        HomeModeString = "正極限";
                    break;
                case HomeModes.ORGAndIndex:
                    HomeModeString = "原點+Index";
                    break;
                case HomeModes.Block:
                    break;
                case HomeModes.CurPos:
                    HomeModeString = "CurPos";
                    break;
                case HomeModes.ELAndIndex:
                    if (motionDirections == MotionDirections.Backward)
                        HomeModeString = "負極限+Index";
                    else if (motionDirections == MotionDirections.Forward)
                        HomeModeString = "正極限+Index";
                    break;
                default:
                    break;
            }
        }

        private static void OnConfigChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as AxisMotionUC;
            dp.SetVelocity();
        }
        private void SetVelocity()
        {
            if (AxisSetConfig != null)
            {
                MoveFinalVelocity = AxisSetConfig.MoveVel.MaxVel;
                MoveAccTime = AxisSetConfig.MoveVel.AccelerationTime;
                MoveDecTime = AxisSetConfig.MoveVel.DecelerationTime;

                HomeFinalVelocity = AxisSetConfig.HomeVel.MaxVel;
                HomeAccTime = AxisSetConfig.HomeVel.AccelerationTime;
                HomeDecTime = AxisSetConfig.HomeVel.DecelerationTime;

            }


        }


        //沒在用
        private async void StartAxisStatus()
        {
            var b = Thread.CurrentThread.ManagedThreadId;
            Task.Run(GetAxisStatus);
        }
        //沒在用
        private async Task GetAxisStatus()
        {
            isGetStatus = true;
            int count = 0;
            var a = Thread.CurrentThread.ManagedThreadId;
            while (isGetStatus)
            {
                if (Axis == null)
                {
                    await Task.Delay(500);
                    continue;
                }
                AxisName = Axis.AxisName;
                Position = Axis.Position;

                if (Axis.AxisState == AxisSensor.NEL)
                    NELBackground = Brushes.Red;
                else
                    NELBackground = Brushes.White;

                if (Axis.AxisState == AxisSensor.PEL)
                    PELBackground = Brushes.Red;
                else
                    PELBackground = Brushes.White;

                if (Axis.AxisState == AxisSensor.ORG)
                    ORGBackground = Brushes.DarkGreen;
                else
                    ORGBackground = Brushes.White;
                await Task.Delay(50);
            }
        }

        private async Task GetAxisStatusTest()
        {
            var a = Thread.CurrentThread.ManagedThreadId;

            if (Axis == null)
            {
                return;
            }
            AxisName = Axis.AxisName;
            Position = Axis.Position;
            if (Axis.AxisState == AxisSensor.NEL)
                NELBackground = Brushes.Red;
            else
                NELBackground = Brushes.White;

            if (Axis.AxisState == AxisSensor.PEL)
                PELBackground = Brushes.Red;
            else
                PELBackground = Brushes.White;

            if (Axis.AxisState == AxisSensor.ORG)
                ORGBackground = Brushes.DarkGreen;
            else
                ORGBackground = Brushes.White;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            T oldValue = field;
            field = value;
            OnPropertyChanged(propertyName, oldValue, value);
        }

        protected virtual void OnPropertyChanged<T>(string name, T oldValue, T newValue)
        {
            // oldValue 和 newValue 目前沒有用到，代爾後需要再實作。
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class VelValueConver : IValueConverter
    {
        //当值从绑定源传播给绑定目标时，调用方法Convert
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valpar = (VelocityParams)value;
            if (valpar != null)
                return valpar.MaxVel;
            return null;


        }

        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var val = (string)value;
            var dVal = System.Convert.ToDouble(val);
            var moveVel = new VelocityParams(0, dVal, 0.2, 0.2);
            return moveVel;
        }


    }
}
