
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
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

namespace YuanliCore.UserControls
{
    /// <summary>
    /// FileInfoWindow.xaml 的互動邏輯
    /// </summary>
    public partial class FileInfoWindow : Window, INotifyPropertyChanged
    {
        private string filenameExtension = ".json";
        private string fileName;
     //   private string folderPath;
        private bool isInput;
        public string MachineName { get; private set; }
        public string RecipeDirectory { get; private set; }
       // public bool IsInput { get; private set; }
        public ObservableCollection<RecipeInfo> DataCollection { get; private set; } = new ObservableCollection<RecipeInfo>();
        public int SelectedIndex { get; set; } = -1;
        public string FilePathName { get; private set; }
        public string FileName { get => fileName; set => SetValue(ref fileName, value); }
        public bool IsInput { get => isInput; set => SetValue(ref isInput, value); }
        /// <summary>
        /// FileInfoWindow 
        /// (整體路徑為 文件/machineName/directoryName(預設Recipe)/ )
        /// </summary>
        /// <param name="isInputTextBox">需給是否需要可以輸入檔案名稱</param>
        /// <param name="machineName">機台名稱</param>
        /// <param name="directoryName">讀取路徑資料夾</param>
        /// <param name="filenameExtension">副檔名 預設 .JSON</param>
        public FileInfoWindow(bool isInputTextBox, string machineName, string directoryPath, string filenameExtension = ".json")
        {
            InitializeComponent();

            IsInput = isInputTextBox;
            MachineName = machineName;
            RecipeDirectory = directoryPath;
            this.filenameExtension = filenameExtension;

 
        }

        public FileInfoWindow(bool isInputTextBox, string machineName, DirectoryNames directoryName, string filenameExtension = ".json")
            :this(isInputTextBox, machineName, directoryName.ToString(), filenameExtension)
        {
        }

        private void RespiceFileControl_Loaded(object sender, RoutedEventArgs e)
        {
          //  string temp = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
          //  folderPath = System.IO.Path.Combine(temp, MachineName, RecipeDirectory);
            
            if (!Directory.Exists(RecipeDirectory))
            {
                Directory.CreateDirectory(RecipeDirectory);
            }

            DataCollection.Clear();
        //    var fileNameList = Directory.GetFileSystemEntries(RecipeDirectory, $"*{filenameExtension}").ToList(); //找尋資料夾內的 .JSON檔案
            var fileNameList = Directory.GetDirectories(RecipeDirectory).ToList(); //找尋資料夾內的 所有資料夾
           
            fileNameList.ForEach(file =>
            {
                var path = System.IO.Path.Combine(RecipeDirectory, file);
                string name = System.IO.Path.GetFileNameWithoutExtension(path);
                RecipeInfo info = new RecipeInfo();
                info.Name = name;
                info.CreationTime = File.GetCreationTime(path).ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
                info.LastWriteTime = File.GetLastWriteTime(path).ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
                DataCollection.Add(info);
            });
        }

        /// <summary>
        /// 刪除檔案按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedIndex < 0) return;
            var index = SelectedIndex;
            MessageBoxResult msg = MessageBox.Show("是否刪除檔案?", "再次確認", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (msg == MessageBoxResult.Cancel) return;

            var path = System.IO.Path.Combine(RecipeDirectory, $"{DataCollection[index].Name}{filenameExtension}");
            var dirPath = System.IO.Path.Combine(RecipeDirectory, $"{DataCollection[index].Name}");

            if (Directory.Exists(dirPath))
                Directory.Delete(dirPath, true);
            File.Delete(path);
            DataCollection.RemoveAt(index);
        }

        /// <summary>
        /// 確認按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKRecipe_Click(object sender, RoutedEventArgs e)
        {
            OKClick();
        }

        /// <summary>
        /// 雙擊事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKRecipe_Click(object sender, MouseButtonEventArgs e)
        {
            OKClick();
        }

        private void OKClick()
        {
            if (IsInput)
            {
                if (FileName == "" || FileName == null)
                {
                    MessageBox.Show("檔名不可空白");
                    return;
                }

                var hasSameName = DataCollection.FirstOrDefault(data => data.Name == FileName);
                if (hasSameName != null)
                {
                    MessageBoxResult msg = MessageBox.Show("是否覆蓋檔案?", "再次確認", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (msg == MessageBoxResult.Cancel) return;
                }

                FilePathName = System.IO.Path.Combine(RecipeDirectory, $"{FileName}{filenameExtension}");
            }
            else
            {
                if (SelectedIndex < 0) return;

                //HOperatorSet.ClearAllShapeModels();
                //HOperatorSet.ClearAllNccModels();

                FilePathName = System.IO.Path.Combine(RecipeDirectory, $"{DataCollection[SelectedIndex].Name}{filenameExtension}");
            }

            this.DialogResult = true;
        }

        /// <summary>
        /// 選擇事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedIndex < 0) { FileName = string.Empty; return; }
            FileName = DataCollection[SelectedIndex].Name;
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

    /// <summary>
    /// 顯示檔案資訊
    /// </summary>
    public class RecipeInfo : INotifyPropertyChanged
    {
        private string name;
        private string creationTime = DateTime.Now.ToString("yyyy-MM-dd (ddd) tt hh:mm:ss", System.Globalization.CultureInfo.InstalledUICulture);
        private string lastWriteTime;

        public string Name { get => name; set => SetValue(ref name, value); }
        public string CreationTime { get => creationTime; set => SetValue(ref creationTime, value); }
        public string LastWriteTime { get => lastWriteTime; set => SetValue(ref lastWriteTime, value); }

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

    /// <summary>
    /// 讀取資料夾位置
    /// </summary>
    public enum DirectoryNames
    {
        Recipe,

        Layout,

        TransformData,
    }
}
