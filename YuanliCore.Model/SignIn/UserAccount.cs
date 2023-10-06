using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace goonγ
{
    public class UserAccount
    {
        private List<string> changedProperties = new List<string>();// 紀錄變動過值的屬性。
        private static readonly string fileName = "Authenticate.aut";
        private static readonly string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Assembly.GetEntryAssembly().GetName().Name);

        private void ThrowIfAuthorityNotEnought()
        {
            if (CurrentAccount.Right != RightsModel.Administrator)
            {
                MessageBox.Show("Member's authority is not enough, only Administrator can edit");
                return;
            }
        }


        public UserAccount()
        {
            Accounts = new ObservableCollection<Account>();

            Register = new List<Tuple<string, string, string>>();

            CurrentAccount = new Account();
        }


        /// <summary>
        /// 權限檔案副檔名
        /// </summary>
        public static string Extension { get; set; } = ".aut";

        /// <summary>
        /// 檔案名稱
        /// </summary>
        public string Name { get; set; }

        [JsonIgnore]
        public bool BeenSaved { get; set; } = false;

        [JsonIgnore]
        public string FilePath { get; set; }

        [JsonIgnore]
        public bool IsPropertyChanged { get => changedProperties.Any(); }

        [JsonIgnore]
        private DateTime TimeLogin { get; set; }

        [JsonIgnore]
        private DateTime TimeLogout { get; set; }

        [JsonIgnore]
        private Tuple<string, string, string> Info { get; set; }


        /// <summary>
        /// 當前登入使用者資訊
        /// </summary>
        public Account CurrentAccount { get; set; }

        /// <summary>
        /// 顯示權限管理
        /// </summary>
        public ObservableCollection<Account> Accounts { get; set; }

        /// <summary>
        /// 歷史紀錄登入登出
        /// </summary>
        public List<Tuple<string, string, string>> Register { get; private set; }



        /// <summary>
        /// 儲存當前 Recpie 內容至指定路徑，副檔名若不是 iwi 則自動加入此附檔名。
        /// </summary>
        /// <param name="fileName"></param>
        public void Save()
        {
            try
            {
                var path = Path.Combine(dir, fileName);
                string fileFullPath = Path.GetFullPath(path);
                string dirFullPath = Path.GetDirectoryName(fileFullPath);

                DirectoryInfo directory = new DirectoryInfo(dirFullPath);
                if (!directory.Exists) throw new DirectoryNotFoundException($"Directory not exists {directory.FullName}");

                Name = Path.GetFileNameWithoutExtension(path);

                JsonSerializerSettings settings = new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Error,
                    TypeNameHandling = TypeNameHandling.All
                };

                if (File.Exists(path) && File.GetAttributes(path).HasFlag(FileAttributes.Hidden))
                    File.SetAttributes(path, FileAttributes.Normal);

                using (FileStream fs = File.Open(path, FileMode.Create))
                using (StreamWriter sw = new StreamWriter(fs))
                using (JsonWriter jw = new JsonTextWriter(sw))
                {
                    JsonSerializer serializer = JsonSerializer.Create(settings);
                    serializer.Serialize(jw, this);
                    File.SetAttributes(path, FileAttributes.Hidden);
                }

                BeenSaved = true;
                FilePath = path;
                changedProperties.Clear(); // 存檔後清空紀錄變動值屬性的列表。
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Save IWIGroup failed.", ex);
            }
        }
        /// <summary>
        /// 讀檔
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static UserAccount Load()
        {
            try
            {
                if (!File.Exists(dir)) Directory.CreateDirectory(dir);
                var path = Path.Combine(dir, fileName);
                string extension = Path.GetExtension(path);
                if (!File.Exists(path)) throw new FileNotFoundException($"Not found IWIGroup file", path);
                string dirPath = new DirectoryInfo(path).FullName;
                JsonSerializerSettings settings = new JsonSerializerSettings()
                {
                    DefaultValueHandling = DefaultValueHandling.Populate,
                    TypeNameHandling = TypeNameHandling.Auto
                };

                using (FileStream fs = File.Open(Path.GetFullPath(path), FileMode.Open))
                using (StreamReader sr = new StreamReader(fs))
                using (JsonReader jr = new JsonTextReader(sr))
                {
                    File.SetAttributes(Path.GetFullPath(path), FileAttributes.Hidden);
                    JsonSerializer serializer = JsonSerializer.Create(settings);
                    var account = (UserAccount)serializer.Deserialize(jr);
                    account.FilePath = path;
                    account.BeenSaved = true;
                    return account;
                }
            }
            catch (FileNotFoundException exception)
            {
                var account = new UserAccount();
                account.Accounts.Add(new Account() { Name = "Administrator", Right = RightsModel.Administrator, Password = "0000" });
                account.Accounts.Add(new Account() { Name = "Admin", Right = RightsModel.Administrator, Password = "0000" });
                account.Save();
                return UserAccount.Load();
            }

            catch (JsonReaderException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 登出
        /// </summary>
        public void Logout()
        {
            TimeLogout = DateTime.Now;

            if (CurrentAccount.Right != RightsModel.Visitor)
                Register.Add(Tuple.Create(CurrentAccount.Name, TimeLogin.ToString("yyyy/MM/dd HH:mm:ss"), TimeLogout.ToString("yyyy/MM/dd HH:mm:ss")));

            LogOut?.Invoke(this, new EventArgs());
            CurrentAccount.Right = RightsModel.Visitor;
            CurrentAccount.Name = "Visitor";
            CurrentAccount.Password = "";
            LogIn?.Invoke(this, new EventArgs());

            Save();
        }
        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Login(string username, string password)
        {
            if (!Accounts.Any(account => account.Name == username)) return false;

            var member = Accounts.First(account => account.Name == username);

            if (password != member.Password) return false;

            CurrentAccount.Right = member.Right;
            CurrentAccount.Name = member.Name;
            CurrentAccount.Password = member.Password;
            TimeLogin = DateTime.Now;
            LogIn?.Invoke(this, new EventArgs());
            return true;
        }

        /// <summary>
        /// 刪除使用者
        /// </summary>
        /// <param name="username"></param>
        public void Delete(string username)
        {
            ThrowIfAuthorityNotEnought();

            if (!Accounts.Any(account => account.Name == username)) return;

            var deleteAccount = Accounts.First(account => account.Name == username);

            Accounts.Remove(deleteAccount);
            Save();
        }
        /// <summary>
        /// 新增使用者
        /// </summary>
        /// <param name="username"></param>
        /// <param name="rights"></param>
        /// <param name="password"></param>
        public void Add(string username, RightsModel rights, string password)
        {
            ThrowIfAuthorityNotEnought();

            var compare = Accounts.FirstOrDefault(account => account.Name == username);
            if (compare != null) Accounts.Remove(compare);

            Accounts.Add(new Account() { Name = username, Right = rights, Password = password });
            Save();
        }
        /// <summary>
        /// 修改使用者密碼
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        public void Edit(string username, string oldPassword, string newPassword)
        {
            ThrowIfAuthorityNotEnought();

            if (!Accounts.Any(accunt => accunt.Name == username)) return;

            var editAccount = Accounts.First(account => account.Name == username);

            if (editAccount.Password != oldPassword)
            {
                MessageBox.Show("Old password incorrect\nChanged password fail!");
                return;
            }

            editAccount.Password = newPassword;
            Save();
        }



        public event LogInEventHandler LogIn;
        public event LogOutEventHandler LogOut;
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Account : INotifyPropertyChanged
    {
        /// <summary>
        /// 取得或設定 名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或設定 權限
        /// </summary>
        public RightsModel Right { get; set; } = RightsModel.Visitor;

        /// <summary>
        /// 取得或設定 密碼
        /// </summary>
        public string Password { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public enum RightsModel
    {
        /// <summary>
        /// 管理使用者
        /// </summary>
        [Description("管理使用者")]
        Administrator,

        ///// <summary>
        ///// 工程模式
        ///// </summary>
        //[Description("工程模式")]
        //Engineer,

        /// <summary>
        /// 進階使用者
        /// </summary>
        [Description("進階使用者")]
        AdvancedOperator,
        
        /// <summary>
        /// 使用者
        /// </summary>
        [Description("使用者")]
        Operator,

        /// <summary>
        /// 登出後的狀態
        /// </summary>
        [Description("訪客")]
        Visitor,
    }

    public delegate void LogInEventHandler(object sender, EventArgs e);
    public delegate void LogOutEventHandler(object sender, EventArgs e);
}
