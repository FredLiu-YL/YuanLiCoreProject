using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using YuanliCore.Model.Interface;

namespace YuanliCore.Model.Microscope
{

    /// <summary>
    /// BXFMUC.xaml 的互動邏輯
    /// </summary>
    public partial class BXFMUC : UserControl, INotifyPropertyChanged
    {

        private static readonly DependencyProperty MicroscopeProperty = DependencyProperty.Register(nameof(Microscope), typeof(IMicroscope), typeof(BXFMUC),
                                                                                    new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty BXFMUIShowProperty = DependencyProperty.Register(nameof(BXFMUIShow), typeof(BXFMUI), typeof(BXFMUC),
                                                                                     new FrameworkPropertyMetadata(new BXFMUI(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public BXFMUC()
        {
            InitializeComponent();
            //Loaded += UserControl_Loaded;
        }
        public IMicroscope Microscope
        {
            get => (IMicroscope)GetValue(MicroscopeProperty);
            set => SetValue(MicroscopeProperty, value);
        }
        public BXFMUI BXFMUIShow
        {
            get => (BXFMUI)GetValue(BXFMUIShowProperty);
            set => SetValue(BXFMUIShowProperty, value);
        }

        private bool isRefresh = false;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                isRefresh = false;
                RefreshStatus();
            });
        }
        private async Task RefreshStatus()
        {
            try
            {
                await Task.Run(async () =>
                {
                    while (true)
                    {
                        if (isRefresh == true)
                        {
                            if (Microscope != null)
                            {
                                FocusZ = Convert.ToInt32(await Microscope.GetZPosition());

                            }
                            await Task.Delay(300);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        private int intensitySliderValue;
        private int distanceZ = 1000;
        private bool isFocusZMove = true;
        private int focusZ;
        private int intensityValue;
        private int apertureValue;
        public int IntensitySliderValue
        {
            get
            {
                int returnValue = intensitySliderValue;
                if (Microscope != null)
                {
                    returnValue = Microscope.LightValue;
                }
                return intensitySliderValue;
            }
            set
            {
                //Microscope.ChangeLight(value).Wait();

                Microscope.LightValue = value;
                SetValue(ref intensitySliderValue, value);
                IntensityValue = value;
            }
        }
        public int DistanceZ
        {
            get => distanceZ;
            set
            {
                SetValue(ref distanceZ, value);
            }
        }
        public bool IsFocusZMove
        {
            get => isFocusZMove;
            set
            {
                SetValue(ref isFocusZMove, value);
            }
        }
        public int FocusZ
        {
            get => focusZ;
            set
            {
                Microscope.ZMoveToCommand(value).Wait();
                SetValue(ref focusZ, value);
            }
        }
        public int IntensityValue
        {
            get => intensityValue;
            set => SetValue(ref intensityValue, value);
        }
        public int ApertureValue
        {
            get => apertureValue;
            set => SetValue(ref apertureValue, value);
        }
        private bool isAF;
        public bool IsAF
        {
            get => isAF;
            set
            {
                if (value == true)
                {
                    Microscope.AF_Off().Wait();
                }
                else
                {
                    Microscope.AF_Trace().Wait();
                }
                SetValue(ref isAF, value);
            }
        }


        private bool isLens1;
        public bool IsLens1
        {
            get => isLens1;
            set
            {
                if (value == true)
                {
                    try
                    {
                        Microscope.ChangeLens(1).Wait();
                    }
                    catch (Exception)
                    {

                    }
                }
                SetValue(ref isLens1, value);
            }
        }

        private bool isLens2;
        public bool IsLens2
        {
            get => isLens2;
            set
            {
                if (value == true)
                {
                    try
                    {
                        Microscope.ChangeLens(2).Wait();
                    }
                    catch (Exception)
                    {

                    }
                }
                SetValue(ref isLens2, value);
            }
        }

        private bool isLens3;
        public bool IsLens3
        {
            get => isLens3;
            set
            {
                if (value == true)
                {
                    try
                    {
                        Microscope.ChangeLens(3).Wait();
                    }
                    catch (Exception)
                    {

                    }
                }
                SetValue(ref isLens3, value);
            }
        }

        private bool isLens4;
        public bool IsLens4
        {
            get => isLens4;
            set
            {
                if (value == true)
                {
                    try
                    {
                        Microscope.ChangeLens(4).Wait();
                    }
                    catch (Exception)
                    {

                    }
                }
                SetValue(ref isLens4, value);
            }
        }

        private bool isLens5;
        public bool IsLens5
        {
            get => isLens5;
            set
            {
                if (value == true)
                {
                    try
                    {
                        Microscope.ChangeLens(5).Wait();
                    }
                    catch (Exception)
                    {

                    }
                }
                SetValue(ref isLens5, value);
            }
        }


        private bool isObservation1;
        public bool IsObservation1
        {
            get => isObservation1;
            set
            {
                if (value == true)
                {
                    try
                    {
                        Microscope.ChangeCube(2).Wait();
                        Microscope.ChangeFilter(1, 1).Wait();
                        Microscope.ChangeFilter(2, 1).Wait();
                    }
                    catch (Exception)
                    {

                    }
                }
                SetValue(ref isObservation1, value);
            }
        }

        private bool isObservation2;
        public bool IsObservation2
        {
            get => isObservation2;
            set
            {
                if (value == true)
                {
                    try
                    {
                        Microscope.ChangeCube(1).Wait();
                        Microscope.ChangeFilter(1, 1).Wait();
                        Microscope.ChangeFilter(2, 1).Wait();
                    }
                    catch (Exception)
                    {

                    }
                }
                SetValue(ref isObservation2, value);
            }
        }
        private bool isObservation3;
        public bool IsObservation3
        {
            get => isObservation3;
            set
            {
                if (value == true)
                {
                    try
                    {
                        Microscope.ChangeFilter(1, 2).Wait();
                        Microscope.ChangeFilter(2, 2).Wait();
                    }
                    catch (Exception)
                    {

                    }
                }
                SetValue(ref isObservation3, value);
            }
        }



        public ICommand ApertureChange => new RelayCommand<string>(async key =>
        {
            try
            {
                switch (key)
                {
                    case "1":
                        Microscope.ApertureValue = 100;
                        break;
                    case "2":
                        await Microscope.ChangeAperture(700);
                        Microscope.ApertureValue = 700;
                        break;
                    case "3":
                        await Microscope.ChangeAperture(1300);
                        Microscope.ApertureValue = 1300;
                        break;
                    case "4":
                        await Microscope.ChangeAperture(1900);
                        Microscope.ApertureValue = 1900;
                        break;
                    case "5":
                        await Microscope.ChangeAperture(2500);
                        Microscope.ApertureValue = 2500;
                        break;
                    case "6":
                        await Microscope.ChangeAperture(3113);
                        Microscope.ApertureValue = 3113;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });
        public ICommand LensChange => new RelayCommand<string>(async key =>
        {
            try
            {

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });
        public ICommand FocusZMove => new RelayCommand<string>(async key =>
        {
            try
            {
                IsFocusZMove = false;
                switch (key)
                {
                    case "Up":
                        await Microscope.ZMoveCommand(-DistanceZ);
                        break;
                    case "Down":
                        await Microscope.ZMoveCommand(DistanceZ);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsFocusZMove = true;
            }
        });

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
    public class BXFMUI : INotifyPropertyChanged
    {
        private int focusZ;
        public int FocusZ
        {
            get => focusZ;
            set => SetValue(ref focusZ, value);
        }
        private int lightValue;
        public int LightValue
        {
            get => lightValue;
            set => SetValue(ref lightValue, value);
        }

        private int apertureValue;
        public int ApertureValue
        {
            get => apertureValue;
            set => SetValue(ref apertureValue, value);
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
}
