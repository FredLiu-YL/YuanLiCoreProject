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
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuanliCore.Model.Interface;

namespace YuanliCore.Model.Microscope
{

    /// <summary>
    /// BXFMUC.xaml 的互動邏輯
    /// </summary>
    public partial class BXFMUC : UserControl, INotifyPropertyChanged
    {

        private static readonly DependencyProperty MicroscopeProperty = DependencyProperty.Register(nameof(Microscope), typeof(IMicroscope), typeof(BXFMUC), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

     
        public BXFMUC()
        {
            InitializeComponent();
         
        }

        public IMicroscope Microscope
        {
            get => (IMicroscope)GetValue(MicroscopeProperty);
            set => SetValue(MicroscopeProperty, value);
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
