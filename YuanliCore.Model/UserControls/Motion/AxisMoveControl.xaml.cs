using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using YuanliCore.Interface;

namespace YuanliCore.Motion
{
    /// <summary>
    /// AxisMoveControl.xaml 的互動邏輯
    /// </summary>
    public partial class AxisMoveControl : UserControl, INotifyPropertyChanged
    {
        public AxisMoveControl()
        {
            InitializeComponent();
        }


        private string moveDistance;
        private bool isContinueMouseDown = false;
        private bool isDownCommandPressed;
       
        public static readonly DependencyProperty AxisXProperty = DependencyProperty.Register("AxisX", typeof(Axis), typeof(AxisMoveControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty AxisYProperty = DependencyProperty.Register("AxisY", typeof(Axis), typeof(AxisMoveControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty AxisRProperty = DependencyProperty.Register("AxisR", typeof(Axis), typeof(AxisMoveControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty IsForwardXProperty = DependencyProperty.Register("IsForwardX", typeof(bool), typeof(AxisMoveControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsForwardYProperty = DependencyProperty.Register("IsForwardY", typeof(bool), typeof(AxisMoveControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsForwardRProperty = DependencyProperty.Register("IsForwardR", typeof(bool), typeof(AxisMoveControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty IsvisibleXProperty = DependencyProperty.Register("IsvisibleX", typeof(Visibility), typeof(AxisMoveControl), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsvisibleYProperty = DependencyProperty.Register("IsvisibleY", typeof(Visibility), typeof(AxisMoveControl), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsvisibleRProperty = DependencyProperty.Register("IsvisibleR", typeof(Visibility), typeof(AxisMoveControl), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty UnitRateXProperty = DependencyProperty.Register("UnitRateX", typeof(double), typeof(AxisMoveControl), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty UnitRateYProperty = DependencyProperty.Register("UnitRateY", typeof(double), typeof(AxisMoveControl), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty UnitRateRProperty = DependencyProperty.Register("UnitRateR", typeof(double), typeof(AxisMoveControl), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public Axis AxisX
        {
            get => (Axis)GetValue(AxisXProperty);
            set => SetValue(AxisXProperty, value);
        }
        public Axis AxisY
        {
            get => (Axis)GetValue(AxisYProperty);
            set => SetValue(AxisYProperty, value);
        }
        public Axis AxisR
        {
            get => (Axis)GetValue(AxisRProperty);
            set => SetValue(AxisRProperty, value);
        }
        public bool IsForwardX
        {
            get => (bool)GetValue(IsForwardXProperty);
            set => SetValue(IsForwardXProperty, value);
        }
        public bool IsForwardY
        {
            get => (bool)GetValue(IsForwardYProperty);
            set => SetValue(IsForwardYProperty, value);
        }
        public bool IsForwardR
        {
            get => (bool)GetValue(IsForwardRProperty);
            set => SetValue(IsForwardRProperty, value);
        }

        public double UnitRateX
        {
            get => (double)GetValue(UnitRateXProperty);
            set => SetValue(UnitRateXProperty, value);
        }
        public double UnitRateY
        {
            get => (double)GetValue(UnitRateYProperty);
            set => SetValue(UnitRateYProperty, value);
        }
        public double UnitRateR
        {
            get => (double)GetValue(UnitRateRProperty);
            set => SetValue(UnitRateRProperty, value);
        }

        public Visibility IsvisibleX
        {
            get => (Visibility)GetValue(IsvisibleXProperty);
            set => SetValue(IsvisibleXProperty, value);
        }
        public Visibility IsvisibleY
        {
            get => (Visibility)GetValue(IsvisibleYProperty);
            set => SetValue(IsvisibleYProperty, value);
        }
        public Visibility IsvisibleR
        {
            get => (Visibility)GetValue(IsvisibleRProperty);
            set => SetValue(IsvisibleRProperty, value);
        }

        public string MoveDistance { get => moveDistance; set => SetValue(ref moveDistance, value); }

        public ICommand AxisMoveMouseDownCommand => new RelayCommand<string>(async param =>
        {

            try
            {
                isDownCommandPressed = true;
                double dis = ConvertDistance();
                //if (dis != 0 || AxisX.IsRunning || AxisY.IsRunning||AxisR.IsRunning)return;

                if (dis != 0 ||
                AxisX != null && AxisX.IsRunning ||
                     AxisY != null && AxisY.IsRunning ||
                     AxisR != null && AxisR.IsRunning) return;
                isContinueMouseDown = true;//表示連續移動被按下

                switch (param)
                {

                    case "MoveX+":
                        if (IsForwardX)
                            await AxisX.MoveToAsync(AxisX.PositionPEL);
                        else
                            await AxisX.MoveToAsync(AxisX.PositionNEL);
                        break;
                    case "MoveX-":
                        if (IsForwardX)
                            await AxisX.MoveToAsync(AxisX.PositionNEL);
                        else
                            await AxisX.MoveToAsync(AxisX.PositionPEL);
                        break;
                    case "MoveY+":
                        if (IsForwardY)
                            await AxisY.MoveToAsync(AxisY.PositionPEL);
                        else
                            await AxisY.MoveToAsync(AxisY.PositionNEL);
                        break;
                    case "MoveY-":
                        if (IsForwardY)
                            await AxisY.MoveToAsync(AxisY.PositionNEL);
                        else
                            await AxisY.MoveToAsync(AxisY.PositionPEL);
                        break;

                    case "RotateRight":
                        if (IsForwardR)
                            await AxisR.MoveToAsync(AxisR.PositionNEL);
                        else
                            await AxisR.MoveToAsync(AxisR.PositionPEL);
                        break;

                    case "RotateLeft":
                        if (IsForwardR)
                            await AxisR.MoveToAsync(AxisR.PositionPEL);
                        else
                            await AxisR.MoveToAsync(AxisR.PositionNEL);
                        break;
                }
            }
            catch (Exception ex)
            {
                //     MessageBox.Show(ex.Message);

            }

        });



        public ICommand AxisMoveMouseUpCommand => new RelayCommand<string>(async param =>
        {
            if (!isDownCommandPressed) return;
            try
            {
                double distance = ConvertDistance();
                if (distance == 0)
                {
                    isContinueMouseDown = false;//表示連續移動被按下
                    if (AxisX != null) AxisX.Stop(); if (AxisY != null) AxisY.Stop(); if (AxisR != null) AxisR.Stop();
                    return;
                }

                if (AxisX != null && AxisX.IsRunning ||
                    AxisY != null && AxisY.IsRunning ||
                    AxisR != null && AxisR.IsRunning) return;
                double d;
                double distUnit;
                switch (param)
                {
                    case "MoveX+":

                        d = IsForwardX ? distance : -distance;
                        distUnit = (d * UnitRateX);
                        if (AxisX.IsRunning) return;
                        await AxisX.MoveAsync(distUnit);
                        break;
                    case "MoveX-":
                        d = IsForwardX ? distance : -distance;
                        distUnit = (d * UnitRateX);

                        if (AxisX.IsRunning) return;
                        await AxisX.MoveAsync(-distUnit);
                        break;
                    case "MoveY+":
                        d = IsForwardY ? distance : -distance;
                        distUnit = (d * UnitRateY);
                        if (AxisY.IsRunning) return;
                        await AxisY.MoveAsync(distUnit);
                        break;
                    case "MoveY-":
                        d = IsForwardY ? distance : -distance;
                        distUnit = (d * UnitRateY);
                        if (AxisY.IsRunning) return;
                        await AxisY.MoveAsync(-distUnit);
                        break;

                    case "RotateLeft":
                        d = IsForwardR ? distance : -distance;
                        distUnit = (d * UnitRateR);
                        if (AxisR.IsRunning) return;
                        await AxisR.MoveAsync(distUnit);
                        break;
                    case "RotateRight":
                        d = IsForwardR ? distance : -distance;
                        distUnit = (d * UnitRateR);
                        if (AxisR.IsRunning) return;
                        await AxisR.MoveAsync(-distUnit);
                        break;
                }
                isDownCommandPressed = false;
            }
            catch (Exception ex)
            {
             //   MessageBox.Show(ex.Message);

            }

        });
        public ICommand AxisMoveMouseLeaveCommand => new RelayCommand(async () =>
        {
            if (!isContinueMouseDown) return;
            if (AxisX != null) AxisX.Stop();
            if (AxisY != null) AxisY.Stop();
            if (AxisR != null) AxisR.Stop();

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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
