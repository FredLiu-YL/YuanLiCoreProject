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
    /// Informaiton.xaml 的互動邏輯
    /// </summary>
    public partial class ProductionUC : UserControl, INotifyPropertyChanged
    {
        //private ListBox topListBoxItem;
        //public ListBox TopListBoxItem { get => topListBoxItem; set => SetValue(ref topListBoxItem, value); }

        private ObservableCollection<ObservableCollection<Button>> topListBoxItemSource = new ObservableCollection<ObservableCollection<Button>>();
        public ObservableCollection<ObservableCollection<Button>> TopListBoxItemSource { get => topListBoxItemSource; set => SetValue(ref topListBoxItemSource, value); }


        private ObservableCollection<Button> outerListBox = new ObservableCollection<Button>();
        public ObservableCollection<Button> OuterListBox { get => outerListBox; set => SetValue(ref outerListBox, value); }
        //    <ListBox.ItemTemplate>
        //        <DataTemplate>
        //            <Button Content = "{Binding}" />
        //        </ DataTemplate >
        //    </ ListBox.ItemTemplate >

        private ObservableCollection<ObservableCollection<string>> buttonRows = new ObservableCollection<ObservableCollection<string>>();
        public ObservableCollection<ObservableCollection<string>> ButtonRows { get => buttonRows; set => SetValue(ref buttonRows, value); }

        private ObservableCollection<ObservableCollection<ButtonState>> buttonRows_New = new ObservableCollection<ObservableCollection<ButtonState>>();
        public ObservableCollection<ObservableCollection<ButtonState>> ButtonRows_New { get => buttonRows_New; set => SetValue(ref buttonRows_New, value); }




        //CassetteUC
        public class ButtonState
        {
            public string Context { get; set; }
            public bool IsClick { get; set; }
        }

        public ProductionUC()
        {
            InitializeComponent();
        }
        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                AddButtonAction = new RelayCommand<(int, int)>(key =>
                {
                    int variable1 = key.Item1;
                    int variable2 = key.Item2;
                    AddButton(variable1, variable2);
                });
                CassetteUC = new ObservableCollection<CassetteUC>();
                AddButton(3, 25);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }







        //public static readonly DependencyProperty AddShapeActionProperty = DependencyProperty.Register(nameof(AddShapeAction), typeof(ICommand), typeof(ProductionUC),
        //                                                                            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty AutoSaveIsCheckedProperty = DependencyProperty.Register(nameof(AutoSaveIsChecked), typeof(bool), typeof(ProductionUC),
                                                                                     new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty CircleIsCheckedProperty = DependencyProperty.Register(nameof(CircleIsChecked), typeof(bool), typeof(ProductionUC),
                                                                                     new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty DieInsideAllCheckedProperty = DependencyProperty.Register(nameof(DieInsideAllChecked), typeof(bool), typeof(ProductionUC),
                                                                                     new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty AddButtonActionProperty = DependencyProperty.Register(nameof(AddButtonAction), typeof(ICommand), typeof(ProductionUC),
                                                                                     new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));



        public bool AutoSaveIsChecked
        {
            get => (bool)GetValue(AutoSaveIsCheckedProperty);
            set => SetValue(AutoSaveIsCheckedProperty, value);
        }
        public bool CircleIsChecked
        {
            get => (bool)GetValue(CircleIsCheckedProperty);
            set => SetValue(CircleIsCheckedProperty, value);
        }
        public bool DieInsideAllChecked
        {
            get => (bool)GetValue(DieInsideAllCheckedProperty);
            set => SetValue(DieInsideAllCheckedProperty, value);
        }
        public ICommand AddButtonAction
        {
            get => (ICommand)GetValue(AddButtonActionProperty);
            set => SetValue(AddButtonActionProperty, value);
        }



        //private ObservableCollection<CassetteUC> cassetteUC = new ObservableCollection<CassetteUC>();
        //public ObservableCollection<CassetteUC> CassetteUC { get => cassetteUC; set => SetValue(ref cassetteUC, value); }

        //--------------------------------

        //public static readonly DependencyProperty CassetteUC2Property = DependencyProperty.Register(nameof(CassetteUC2), typeof(CassetteUC), typeof(ProductionUC),
        //                                                                       new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        //public CassetteUC CassetteUC2
        //{
        //    get => (CassetteUC)GetValue(CassetteUC2Property);
        //    set => SetValue(CassetteUC2Property, value);
        //}

        //--------------------------------

        public static readonly DependencyProperty CassetteUCProperty = DependencyProperty.Register(nameof(CassetteUC), typeof(ObservableCollection<CassetteUC>), typeof(ProductionUC),
                                                                                                     new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));//new PropertyMetadata(null));
                                                                                                     
        public ObservableCollection<CassetteUC> CassetteUC
        {
            get => (ObservableCollection<CassetteUC>)GetValue(CassetteUCProperty);
            set { SetValue(CassetteUCProperty, value); }
        }

        public void AddButton(int Cols, int Rows)
        {
            try
            {
                ButtonRows.Clear();
                TopListBoxItemSource.Clear();
                ButtonRows_New.Clear();
                OuterListBox.Clear();
                for (int i = 1; i <= Rows; i++)
                {
                    var row = new ObservableCollection<string>();
                    var row2 = new ObservableCollection<Button>();
                    var row3 = new ObservableCollection<ButtonState>();
                    for (int j = 1; j <= Cols; j++)
                    {
                        row.Add("Button " + (i * 3 - 3 + j));
                        Button newButton = new Button();
                        newButton.Content = "Button " + (i * 3 - 3 + j);
                        newButton.Command = ListBoxButton_Command2;
                        newButton.Tag = (i * 3 - 3 + j); // 將內容存儲在Tag屬性中
                        newButton.Width = 50;
                        newButton.Height = 20;
                        newButton.Background = Brushes.Black;
                        //newButton.Click += ListBoxButton_Click2;
                        OuterListBox.Add(newButton);
                        row2.Add(newButton);
                        row3.Add(new ButtonState { Context = "Button " + (i * 3 - 3 + j), IsClick = false });
                    }
                    CassetteUC.Add(new CassetteUC { Btn1_IsClik = false, Btn2_IsClik = false, Btn3_IsClik = false });

                    ButtonRows.Add(row);
                    TopListBoxItemSource.Add(row2);
                    ButtonRows_New.Add(row3);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ICommand ListBoxButtonCommand => new RelayCommand(() =>
        {
            //if (parameter != null)
            //{
            //    // parameter 包含按钮的数据，可以根据需要进行处理
            //    // 例如，你可以访问Button的Tag属性来识别按钮
            //    //var buttonData = parameter as YourButtonDataClass;
            //}
        });



        private void ListBoxButton_Click(object sender, RoutedEventArgs e)
        {
            // Click="ListBoxButton_Click"
            try
            {
                Button clickedButton = sender as Button;

                if (clickedButton != null)
                {
                    string buttonText = clickedButton.Tag as string;
                    if (buttonText != null)
                    {
                        Brush now = clickedButton.Background;
                        //MessageBox.Show("Button " + buttonText + "被點擊了！");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void ListBoxButton_Click2(object sender, RoutedEventArgs e)
        {
            // Click="ListBoxButton_Click"
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public ICommand ListBoxButton_Command2 => new RelayCommand(async () =>
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
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
