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
using YuanliCore.Model;

namespace YuanliCore.Account
{
    /// <summary>
    /// UserRightsControl.xaml 的互動邏輯
    /// </summary>
    public partial class UserRightsControl : UserControl, INotifyPropertyChanged
    {
        private static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(UserAccount), typeof(UserRightsControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        //private static readonly DependencyProperty LoggerProperty = DependencyProperty.Register(nameof(Logger), typeof(Logger), typeof(UserRightsControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private static readonly DependencyProperty IsLineFeedProperty = DependencyProperty.Register(nameof(IsLineFeed), typeof(bool), typeof(UserRightsControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private static readonly DependencyProperty PackIconForegroundProperty = DependencyProperty.Register(nameof(PackIconForeground), typeof(Brush), typeof(UserRightsControl), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private static readonly DependencyProperty PackIconWidthProperty = DependencyProperty.Register(nameof(PackIconWidth), typeof(double), typeof(UserRightsControl), new FrameworkPropertyMetadata(25.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private static readonly DependencyProperty PackIconHeightProperty = DependencyProperty.Register(nameof(PackIconHeight), typeof(double), typeof(UserRightsControl), new FrameworkPropertyMetadata(25.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private static readonly DependencyProperty ButtonWidthProperty = DependencyProperty.Register(nameof(ButtonWidth), typeof(double), typeof(UserRightsControl), new FrameworkPropertyMetadata(35.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private static readonly DependencyProperty ButtonHeightProperty = DependencyProperty.Register(nameof(ButtonHeight), typeof(double), typeof(UserRightsControl), new FrameworkPropertyMetadata(35.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private static readonly DependencyProperty ButtonBorderBrushProperty = DependencyProperty.Register(nameof(ButtonBorderBrush), typeof(Brush), typeof(UserRightsControl), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private static readonly DependencyProperty ButtonFontFamilyProperty = DependencyProperty.Register(nameof(LabelFontFamily), typeof(FontFamily), typeof(UserRightsControl), new FrameworkPropertyMetadata(new FontFamily("Arial Rounded MT Bold"), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private static readonly DependencyProperty ButtonFontSizeProperty = DependencyProperty.Register(nameof(LabelFontSize), typeof(double), typeof(UserRightsControl), new FrameworkPropertyMetadata(15.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private static readonly DependencyProperty ButtonForegroundProperty = DependencyProperty.Register(nameof(LabelForeground), typeof(Brush), typeof(UserRightsControl), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        /// <summary>
        /// 使用者資訊
        /// </summary>
        public UserAccount Account { get => (UserAccount)GetValue(AccountProperty); set => SetValue(AccountProperty, value); }
        
        /// <summary>
        /// Logger
        /// </summary>
      //  public Logger Logger { get => (Logger)GetValue(LoggerProperty); set => SetValue(LoggerProperty, value); }


        /// <summary>
        /// 是否要換行
        /// </summary>
        public bool IsLineFeed { get => (bool)GetValue(IsLineFeedProperty); set => SetValue(IsLineFeedProperty, value); }

        public double PackIconWidth { get => (double)GetValue(PackIconWidthProperty); set => SetValue(PackIconWidthProperty, value); }
        public double PackIconHeight { get => (double)GetValue(PackIconHeightProperty); set => SetValue(PackIconHeightProperty, value); }
        public double ButtonWidth { get => (double)GetValue(ButtonWidthProperty); set => SetValue(ButtonWidthProperty, value); }
        public double ButtonHeight { get => (double)GetValue(ButtonHeightProperty); set => SetValue(ButtonHeightProperty, value); }
        public Brush PackIconForeground { get => (Brush)GetValue(PackIconForegroundProperty); set => SetValue(PackIconForegroundProperty, value); }
        public Brush ButtonBorderBrush { get => (Brush)GetValue(ButtonBorderBrushProperty); set => SetValue(ButtonBorderBrushProperty, value); }
        public FontFamily LabelFontFamily { get => (FontFamily)GetValue(ButtonFontFamilyProperty); set => SetValue(ButtonFontFamilyProperty, value); }
        public double LabelFontSize { get => (double)GetValue(ButtonFontSizeProperty); set => SetValue(ButtonFontSizeProperty, value); }
        public Brush LabelForeground { get => (Brush)GetValue(ButtonForegroundProperty); set => SetValue(ButtonForegroundProperty, value); }


        public UserRightsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 顯示權限/帳號
        /// </summary>
        public string RightAndName { get; set; }

        /// <summary>
        /// 顯示 資訊
        /// </summary>
        public string RightAndNameToolTip { get; set; } = "[權限]\n[帳號]";

        /// <summary>
        /// 使用者管理
        /// </summary>
        public ICommand UserManagerCommand => new RelayCommand(() =>
        {
            if (Account == null) return;
            if (Account.CurrentAccount.Right != RightsModel.Administrator)
            {
                MessageBox.Show("Insufficient permissions");
                return;
            }
            UserManagerWindow win = new UserManagerWindow(Account);

            win.ShowDialog();
        });
        /// <summary>
        /// 登入登出
        /// </summary>
        public ICommand LogoutCommand => new RelayCommand(() =>
        {
            if (Account == null) return;

         //   Logger?.Debug($"[{Account.CurrentAccount.Right}][{Account.CurrentAccount.Name}] 登出");
            Account.Logout();

            SignInWindow win = new SignInWindow(Account);
            win.ShowDialog();

            Account = win.Account;
         //   Logger?.Debug($"[{Account.CurrentAccount.Right}][{Account.CurrentAccount.Name}] 登入");

            string lineFeed = "";
            if (IsLineFeed)
                lineFeed = "\n";

            RightAndName = $"[{Account.CurrentAccount.Right}]{lineFeed}[{Account.CurrentAccount.Name}]";
            RightAndNameToolTip = $"[權限] [{Account.CurrentAccount.Right}]{lineFeed}[帳號] [{Account.CurrentAccount.Name}]";
        });

        public event PropertyChangedEventHandler PropertyChanged;
    }

    [ValueConversion(typeof(Enum), typeof(bool))]
    public class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null || !(value is Enum))
                return Visibility.Collapsed;

            var currentState = value.ToString();
            var stateStrings = parameter.ToString();
            var found = false;

            foreach (var state in stateStrings.Split(','))
            {
                found = (currentState == state.Trim());

                if (found)
                    break;
            }

            return found;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(Enum), typeof(Visibility))]
    public class EnumToVisibleCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null || !(value is Enum))
                return Visibility.Collapsed;

            var currentState = value.ToString();
            var found = Visibility.Collapsed;

            foreach (var state in parameter.ToString().Split(','))
                if (currentState == state.Trim()) found = Visibility.Visible;

            return found;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
