using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MaterialDesignThemes.Wpf;

namespace YuanliCore.Model.Information
{
    /// <summary>
    /// CassetteUC.xaml 的互動邏輯
    /// </summary>
    /// 
    public partial class CassetteUC : UserControl, INotifyPropertyChanged
    {

        private SolidColorBrush PrimaryHueLightBrush = Application.Current.Resources["PrimaryHueLightBrush"] as SolidColorBrush;

        //Color blueGreyColor2 = (Color)Application.Current.FindResource(new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.bluegrey.xaml", UriKind.RelativeOrAbsolute));


        //Color blueGreyColor = (Color)Application.Current.FindResource("PrimaryHueMidBrush");


        private bool btn1_IsClik = true;
        public bool Btn1_IsClik { get => btn1_IsClik; set => SetValue(ref btn1_IsClik, value); }

        private bool btn2_IsClik = true;
        public bool Btn2_IsClik { get => btn2_IsClik; set => SetValue(ref btn2_IsClik, value); }

        private bool btn3_IsClik = true;
        public bool Btn3_IsClik { get => btn3_IsClik; set => SetValue(ref btn3_IsClik, value); }


        private Brush btn1_Background;
        public Brush Btn1_Background { get => btn1_Background; set => SetValue(ref btn1_Background, value); }

        private Brush btn2_Background;
        public Brush Btn2_Background { get => btn2_Background; set => SetValue(ref btn2_Background, value); }

        private Brush btn3_Background;
        public Brush Btn3_Background { get => btn3_Background; set => SetValue(ref btn3_Background, value); }

        public Brush Click_On = Brushes.Red;

        public Brush Click_Off = Brushes.Yellow;

        public CassetteUC()
        {
            InitializeComponent();
        }
        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ResourceDictionary dictionary = new ResourceDictionary();
                dictionary.Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.bluegrey.xaml", UriKind.RelativeOrAbsolute);

                if (dictionary.Count > 0)
                {
                    // 资源字典已加载
                    object blueGreyColorResource1 = dictionary["PrimaryHueLightBrush"];
                    if (blueGreyColorResource1 != null && blueGreyColorResource1 is SolidColorBrush)
                    {
                        SolidColorBrush brush = (SolidColorBrush)blueGreyColorResource1;
                        Color blueGreyColor = brush.Color;
                        Click_Off = brush;
                        // 现在你可以使用 blueGreyColor
                    }
                    // 资源字典已加载
                    object blueGreyColorResource2 = dictionary["PrimaryHueDarkBrush"]; //PrimaryHueDarkForegroundBrush
                    if (blueGreyColorResource2 != null && blueGreyColorResource2 is SolidColorBrush)
                    {
                        SolidColorBrush brush = (SolidColorBrush)blueGreyColorResource2;
                        Color blueGreyColor = brush.Color;
                        Click_On = brush;
                        // 现在你可以使用 blueGreyColor
                    }
                }

                Btn1_Background = Click_Off;
                Btn2_Background = Click_Off;
                Btn3_Background = Click_Off;

                Btn1_IsClik = false;
                Btn2_IsClik = false;
                Btn3_IsClik = false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ICommand Btn1_Command => new RelayCommand(async () =>
        {
            try
            {
                if (Btn1_IsClik == true)
                {
                    Btn1_IsClik = false;
                    Btn1_Background = Click_Off;
                }
                else
                {
                    Btn1_IsClik = true;
                    Btn1_Background = Click_On;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        });
        public ICommand Btn2_Command => new RelayCommand(async () =>
        {
            try
            {
                if (Btn2_IsClik == true)
                {
                    Btn2_IsClik = false;
                    Btn2_Background = Click_Off;
                }
                else
                {
                    Btn2_IsClik = true;
                    Btn2_Background = Click_On;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        });
        public ICommand Btn3_Command => new RelayCommand(async () =>
        {
            try
            {
                if (Btn3_IsClik == true)
                {
                    Btn3_IsClik = false;
                    Btn3_Background = Click_Off;
                }
                else
                {
                    Btn3_IsClik = true;
                    Btn3_Background = Click_On;
                }
            }
            catch (Exception ex)
            {

                throw ex;
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
}
