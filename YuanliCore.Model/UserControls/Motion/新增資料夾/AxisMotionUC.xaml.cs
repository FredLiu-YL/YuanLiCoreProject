using GalaSoft.MvvmLight.Command;
using goonØ;
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

namespace goonδ.Bonding
{
    /// <summary>
    /// AxisMotionUC.xaml 的互動邏輯
    /// </summary>
    public partial class AxisMotionUC : UserControl, INotifyPropertyChanged
    {
        private bool isGetStatus = false;
        private VelocityParams moveVelParams = new VelocityParams(5000);
        private System.Windows.Threading.DispatcherTimer dispatcherTimerFeedback;

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
        public static readonly DependencyProperty AxisSetConfigProperty = DependencyProperty.Register("AxisSetConfig", typeof(BondingAxisConfig), typeof(AxisMotionUC), new FrameworkPropertyMetadata(new BondingAxisConfig(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
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

        public BondingAxisConfig AxisSetConfig
        {
            get => (BondingAxisConfig)GetValue(AxisSetConfigProperty);
            set => SetValue(AxisSetConfigProperty, value);
        }

        /// <summary>
        /// 取得或設定 軸名稱
        /// </summary>
        public string AxisName { get; set; } = "";

        /// <summary>
        /// 取得或設定 負極限
        /// </summary>
        public double LimitNEL { get; set; } = 0;

        /// <summary>
        /// 取得或設定 正極限
        /// </summary>
        public double LimitPEL { get; set; } = 200;

        /// <summary>
        /// 取得或設定 移動速度
        /// </summary>
        public double MoveVel { get; set; } = 2000;

        /// <summary>
        /// 取得或設定 原點速度
        /// </summary>
        public double HomeVel { get; set; } = 2000;

        /// <summary>
        /// 取得或設定 移動加速度時間
        /// </summary>
        public double AccMoveVelTime { get; set; } = 0.1;

        /// <summary>
        /// 取得或設定 移動減速度時間
        /// </summary>
        public double DecMoveVelTime { get; set; } = 0.1;

        /// <summary>
        /// 取得或設定 原點加速度時間
        /// </summary>
        public double AccHomeVelTime { get; set; } = 0.1;

        /// <summary>
        /// 取得或設定 原點減速度時間
        /// </summary>
        public double DecHomeVelTime { get; set; } = 0.1;

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
        public double Position { get; set; } = 0;

        /// <summary>
        /// 取得或設定 軸在正極限上顏色
        /// </summary>
        public Brush PELBackground { get; set; } = Brushes.White;

        /// <summary>
        /// 取得或設定 軸在負極限上顏色
        /// </summary>
        public Brush NELBackground { get; set; } = Brushes.White;

        /// <summary>
        /// 取得或設定 軸在原點上顏色
        /// </summary>
        public Brush ORGBackground { get; set; } = Brushes.White;

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
            if(AxisSetConfig.MoveVel != null)
                AxisSetConfig.MoveVel = new VelocityParams(0, AxisSetConfig.MoveVel.FinalVel, AccMoveVelTime, DecMoveVelTime);
            if(AxisSetConfig.HomeVel != null)
                AxisSetConfig.HomeVel = new VelocityParams(0, AxisSetConfig.HomeVel.FinalVel, AccHomeVelTime, DecHomeVelTime);
            switch (HomeModeString)
            {
                case "原點":
                    Axis.HomeMode = HomeModes.ORG;
                    AxisSetConfig.HomeMode = HomeModes.ORG;
                    AxisSetConfig.Direction = MotionDirections.Backward;
                    break;
                case "正極限":
                    Axis.HomeMode = HomeModes.EL;
                    AxisSetConfig.HomeMode = HomeModes.EL;
                    AxisSetConfig.Direction = MotionDirections.Forward;
                    break;
                case "負極限":
                    Axis.HomeMode = HomeModes.EL;
                    AxisSetConfig.HomeMode = HomeModes.EL;
                    AxisSetConfig.Direction = MotionDirections.Backward;
                    break;
                case "CurPos":
                    Axis.HomeMode = HomeModes.CurPos;
                    AxisSetConfig.HomeMode = HomeModes.CurPos;
                    AxisSetConfig.Direction = MotionDirections.Backward;
                    break;
            }
            Axis.MotionVelParams = AxisSetConfig.MoveVel;

            //存入參數
            //      AxisConfig axisConfig = new AxisConfig();
            //        axisConfig.LimitNEL = LimitNEL;
            //        axisConfig.LimitPEL = LimitPEL;
            //         axisConfig.HomeMode = mode;
            //         axisConfig.HomeVel = homeV;
            //   AxisSetConfig.MoveVel = moveV;
            //          AxisSetConfig = axisConfig;
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
            HomeAsyc?.Invoke(Axis);
        });

        public ICommand AxisStopAsycCommand => new RelayCommand(() =>
        {
            Axis.Stop();
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

        private VelocityParams ConvertHomeVelocity()
        {
            var homeVel = new VelocityParams(1000, HomeVel, AccHomeVelTime, DecHomeVelTime);
            return homeVel;
        }

        private VelocityParams ConvertMoveVelocity()
        {
            var moveVel = new VelocityParams(0, MoveVel, AccMoveVelTime, DecMoveVelTime);
            return moveVel;
        }

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
                case "負極限":

                    break;
                case "原點":

                    break;
                case "正極限":

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
                    if(motionDirections == MotionDirections.Backward)
                        HomeModeString = "負極限";
                    else if(motionDirections == MotionDirections.Forward) 
                        HomeModeString = "正極限";
                    break;
                case HomeModes.Index:
                    break;
                case HomeModes.Block:
                    break;
                case HomeModes.CurPos:
                    HomeModeString = "CurPos";
                    break;
                case HomeModes.ELAndIndex:
                    break;
                default:
                    break;
            }
        }

        private async void StartAxisStatus()
        {
            var b = Thread.CurrentThread.ManagedThreadId;
            Task.Run(GetAxisStatus);
        }

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
                AxisName = Axis.Name;
                Position = Axis.Position;

                if (Axis.Sensor.HasFlag(Sensors.NEL))
                    NELBackground = Brushes.Red;
                else
                    NELBackground = Brushes.White;
                if (Axis.Sensor.HasFlag(Sensors.PEL))
                    PELBackground = Brushes.Red;
                else
                    PELBackground = Brushes.White;
                if (Axis.Sensor.HasFlag(Sensors.ORG))
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
            AxisName = Axis.Name;
            Position = Axis.Position;

            if (Axis.Sensor.HasFlag(Sensors.NEL))
                NELBackground = Brushes.Red;
            else
                NELBackground = Brushes.White;
            if (Axis.Sensor.HasFlag(Sensors.PEL))
                PELBackground = Brushes.Red;
            else
                PELBackground = Brushes.White;
            if (Axis.Sensor.HasFlag(Sensors.ORG))
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
                return valpar.FinalVel;
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
