﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YuanliApplication.Tool
{
    /// <summary>
    /// MethodUC.xaml 的互動邏輯
    /// </summary>
    public partial class MethodDisplayUC : UserControl, INotifyPropertyChanged
    {
        private string sn ="0";
        private string methodName = "找尋方法";
        private string resultName = "找尋方法";

        public MethodDisplayUC()
        {
            InitializeComponent();
        }



        public string SN { get => sn; set => SetValue(ref sn, value); }
        public string MethodName { get => methodName; set => SetValue(ref methodName, value); }
        public string ResultName { get => resultName; set => SetValue(ref resultName, value); }





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
