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
using YuanliCore.Model;

namespace YuanliCore.Account
{
    /// <summary>
    /// UserManagerWindow.xaml 的互動邏輯
    /// </summary>
    public partial class UserManagerWindow : Window, INotifyPropertyChanged
    {
        private UserAccount userAccount;
        public UserManagerWindow(UserAccount account)
        {
            InitializeComponent();

            UserAccount = account;
        }


        public UserAccount UserAccount { get => userAccount; set => SetValue(ref userAccount, value); }

        public ICommand AddUserCommand => new RelayCommand(() =>
        {
            UserAddWindow win = new UserAddWindow();
            bool? result = win.ShowDialog();

            if (result.HasValue && result.Value && UserAccount.Accounts.Where(_ => _.Name == win.UserName).Select((_) => false).ToArray().Count() == 0)
                UserAccount.Add(win.UserName, win.RightsModel, win.Password);
        });

        public ICommand EditUserCommand => new RelayCommand<int>((selectIndex) =>
        {
            if (selectIndex == -1 || selectIndex == UserAccount.Accounts.Count) return;
            if (UserAccount.Accounts[selectIndex].Name == "yuanli") return;

            UserEditWindow win = new UserEditWindow(UserAccount, selectIndex);
            bool? result = win.ShowDialog();

            if (result.HasValue && result.Value)
                UserAccount.Edit(win.UserName, win.OldPassword, win.NewPassword);
        });

        public ICommand DeleteUserCommand => new RelayCommand<int>((selectIndex) =>
        {
            if (selectIndex == -1 || selectIndex == UserAccount.Accounts.Count) return;
            if (UserAccount.Accounts[selectIndex].Name == "yuanli") return;

            string name = UserAccount.Accounts[selectIndex].Name;

            UserAccount.Delete(name);
        });

        public ICommand LogUserCommand => new RelayCommand(() =>
        {
            UserLogWindow win = new UserLogWindow(UserAccount);
            win.ShowDialog();
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
