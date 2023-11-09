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

namespace YuanliCore.Model.LoadPort
{
    /// <summary>
    /// CassetteUC.xaml 的互動邏輯
    /// </summary>
    /// 
    public partial class CassetteUC : UserControl, INotifyPropertyChanged
    {
        public bool Top_IsClik { get => top_IsClik; set => SetValue(ref top_IsClik, value); }
        public bool Back_IsClik { get => back_IsClik; set => SetValue(ref back_IsClik, value); }
        public bool Micro_IsClik { get => micro_IsClik; set => SetValue(ref micro_IsClik, value); }
        public bool Wafer_IsEnable { get => wafer_IsEnable; set => SetValue(ref wafer_IsEnable, value); }
        public Brush Top_Background { get => top_Background; set => SetValue(ref top_Background, value); }
        public Brush Back_Background { get => back_Background; set => SetValue(ref back_Background, value); }
        public Brush Micro_Background { get => micro_Background; set => SetValue(ref micro_Background, value); }
        public string WaferInfo { get => waferInfo; set => SetValue(ref waferInfo, value); }

        public Brush Click_On;

        public Brush Click_Off;
        public CassetteUC(bool isEnable, string pwaferInfo)
        {
            InitializeComponent();

            Wafer_IsEnable = isEnable;
            WaferInfo = pwaferInfo;
        }
        private bool top_IsClik = true;

        private bool back_IsClik = true;

        private bool micro_IsClik = true;

        private bool wafer_IsEnable;

        private Brush top_Background;

        private Brush back_Background;

        private Brush micro_Background;

        private string waferInfo;

        private SolidColorBrush PrimaryHueLightBrush = Application.Current.Resources["PrimaryHueLightBrush"] as SolidColorBrush;

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

                Top_Background = Click_Off;
                Back_Background = Click_Off;
                Micro_Background = Click_Off;

                Top_IsClik = false;
                Back_IsClik = false;
                Micro_IsClik = false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ICommand Top_Command => new RelayCommand(async () =>
        {
            try
            {
                if (Top_IsClik == true)
                {
                    Top_IsClik = false;
                    Top_Background = Click_Off;
                }
                else
                {
                    Top_IsClik = true;
                    Top_Background = Click_On;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        });
        public ICommand Back_Command => new RelayCommand(async () =>
        {
            try
            {
                if (Back_IsClik == true)
                {
                    Back_IsClik = false;
                    Back_Background = Click_Off;
                }
                else
                {
                    Back_IsClik = true;
                    Back_Background = Click_On;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        });
        public ICommand Micro_Command => new RelayCommand(async () =>
        {
            try
            {
                if (Micro_IsClik == true)
                {
                    Micro_IsClik = false;
                    Micro_Background = Click_Off;
                }
                else
                {
                    Micro_IsClik = true;
                    Micro_Background = Click_On;
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
