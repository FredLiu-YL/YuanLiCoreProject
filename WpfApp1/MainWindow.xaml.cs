
using Cognex.Vision;
using System;
using System.Collections.Generic;
using System.Linq;
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
using YuanliCore.ImageProcess;
using YuanliCore.ImageProcess.AI;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
 
 
        public MainWindow()
        {

            InitializeComponent();

        }


        private void ReadImage()
        {



        }


        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 讀取BMP檔案
            BitmapImage bitmap = new BitmapImage(new Uri("D:\\AA.bmp", UriKind.RelativeOrAbsolute));

            //      CogImageConvertWindow window = new CogImageConvertWindow(bitmap);
            //      window.ShowDialog();


            Startup.Initialize(Startup.ProductKey.VProX);

            CogSegmentWindow cogSegmentWindow = new CogSegmentWindow(bitmap);
            cogSegmentWindow.ShowDialog();
            Startup.Shutdown();


        }

 
    }

 
}
