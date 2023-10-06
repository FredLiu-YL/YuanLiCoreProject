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
using System.Windows.Shapes;

namespace goonγ
{
    /// <summary>
    /// UserEditWindow.xaml 的互動邏輯
    /// </summary>
    public partial class UserEditWindow : Window, INotifyPropertyChanged
    {
        public UserEditWindow(UserAccount userAccount, int index)
        {
            InitializeComponent();

            UserName = userAccount.Accounts[index].Name;
        }

        /// <summary>
        /// 取得或設定 輸入帳號內文
        /// </summary>
        public string UserName { get; set; } = "";

        /// <summary>
        /// 取得或設定 輸入舊密碼內文
        /// </summary>
        public string OldPassword { get; set; } = "";

        /// <summary>
        /// 取得或設定 輸入新密碼內文
        /// </summary>
        public string NewPassword { get; set; } = "";

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
            if (OldPassword == "" || NewPassword == "") IsMessageDisplay = true;
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
}
