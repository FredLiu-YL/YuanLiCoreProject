using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace goonγ
{
    /// <summary>
    /// UserAddWindow.xaml 的互動邏輯
    /// </summary>
    public partial class UserAddWindow : Window, INotifyPropertyChanged
    {
        public UserAddWindow()
        {
            InitializeComponent();

            foreach (RightsModel order in Enum.GetValues(typeof(RightsModel))) 
            {
                if (order == RightsModel.Visitor) continue;
                RightsSource.Add(new ComboNames() { Rights = order.ToString() }); 
            }
        }

        /// <summary>
        /// 取得或設定 輸入帳號內文
        /// </summary>
        public string UserName { get; set; } = "";

        /// <summary>
        /// 取得或設定 輸入密碼內文
        /// </summary>
        public string Password { get; set; } = "";

        /// <summary>
        /// ComboBox 集合
        /// </summary>
        public ObservableCollection<ComboNames> RightsSource { get; set; } = new ObservableCollection<ComboNames>();

        /// <summary>
        /// 選擇權限
        /// </summary>
        public RightsModel RightsModel { get; set; } = RightsModel.Operator;


        /// <summary>
        /// 選擇權限
        /// </summary>
        public RightsModel RightsModelItem { get; set; } = RightsModel.Operator;

        /// <summary>
        /// 取得或設定 輸入錯誤顯示
        /// </summary>
        public bool IsMessageDisplay { get; set; } = false;

        /// <summary>
        /// 確認按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserName == "" || Password == "") IsMessageDisplay = true;
            else
            {
                IsMessageDisplay = false;
                this.DialogResult = true;
            }
        }

        /// <summary>
        /// 輸入帳號 按下Enter事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AccountNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                UIElement element = Keyboard.FocusedElement as UIElement;
                element.MoveFocus(request);
            }
        }

        /// <summary>
        /// 輸入密碼 按下Enter事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                UIElement element = Keyboard.FocusedElement as UIElement;
                element.MoveFocus(request);
                OKButton_Click(sender, e);
            }
        }

        /// <summary>
        /// 拖曳視窗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
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


    public class ComboNames
    {
        public string Rights { get; set; }
    }

    public class RightsNameConverter : IValueConverter
    {
        //当值从绑定源传播给绑定目标时，调用方法Convert
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }

        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (RightsModel)value;
        }
    }

}
