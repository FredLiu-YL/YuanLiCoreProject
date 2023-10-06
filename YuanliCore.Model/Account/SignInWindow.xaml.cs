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

namespace YuanliCore.Account
{
    /// <summary>
    /// SignInWindow.xaml 的互動邏輯
    /// </summary>
    public partial class SignInWindow : Window, INotifyPropertyChanged
    {
        private bool isMessageDisplay = false;
        private bool isErrorMessageDisplay = false;
        private string userSignInTitle;

        public SignInWindow(UserAccount account, string userSignInTitle = "User Sign In")
        {
            InitializeComponent();
            Account = account;
            UserSignInTitle = userSignInTitle;
        }

        public UserAccount Account { get; set; }

        /// <summary>
        /// 取得或設定 輸入帳號內文
        /// </summary>
        public string Username { get; set; } = "";

        /// <summary>
        /// 取得或設定 輸入密碼內文
        /// </summary>
        public string Password { get; set; } = "";

        /// <summary>
        /// 取得或設定 帳號或密碼未輸入顯示
        /// </summary>
        public bool IsMessageDisplay { get => isMessageDisplay; set => SetValue(ref isMessageDisplay, value); } 

        /// <summary>
        /// 取得或設定 帳號或密碼輸入錯誤顯示
        /// </summary>
        public bool IsErrorMessageDisplay { get => isErrorMessageDisplay; set => SetValue(ref isErrorMessageDisplay, value); }

        /// <summary>
        /// 取得或設定 視窗標題訊息顯示
        /// </summary>
        public string UserSignInTitle { get => userSignInTitle; set => SetValue(ref userSignInTitle, value); }

        /// <summary>
        /// 確認按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (Username == "" || Password == "")
            {
                IsErrorMessageDisplay = false;
                IsMessageDisplay = true;
                return;
            }

            bool isSucess = Authenticate(Username, Password);
            if (!isSucess)
            {
                IsMessageDisplay = false;
                IsErrorMessageDisplay = true;
                return;
            }

            IsMessageDisplay = false;
            IsErrorMessageDisplay = false;
            this.DialogResult = true;
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

        /// <summary>
        /// 驗證使用者帳號及密碼。
        /// </summary>
        /// <returns>驗證結果是否成功。</returns>
        private bool Authenticate(string username, string password)
        {
            return Account.Login(username, password);
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

    public static class PasswordHelper
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password",
            typeof(string), typeof(PasswordHelper),
            new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach",
            typeof(bool), typeof(PasswordHelper), new PropertyMetadata(false, Attach));

        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached("IsUpdating", typeof(bool),
            typeof(PasswordHelper));


        public static void SetAttach(DependencyObject dp, bool value)
        {
            dp.SetValue(AttachProperty, value);
        }

        public static bool GetAttach(DependencyObject dp)
        {
            return (bool)dp.GetValue(AttachProperty);
        }

        public static string GetPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }

        private static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }

        private static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            passwordBox.PasswordChanged -= PasswordChanged;

            if (!(bool)GetIsUpdating(passwordBox))
            {
                passwordBox.Password = (string)e.NewValue;
            }
            passwordBox.PasswordChanged += PasswordChanged;
        }

        private static void Attach(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            if (passwordBox == null)
                return;

            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
            }

            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }
    }

}
