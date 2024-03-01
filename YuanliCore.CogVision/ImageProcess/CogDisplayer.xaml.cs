using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace YuanliCore.ImageProcess
{

    /// <summary>
    /// CogDisplayer.xaml 的互動邏輯
    /// </summary>
    public partial class CogDisplayer : UserControl
    {
        private CogRecordDisplay cogDisplay;
        private CogGraphicLabel cogGraphicLabel = new CogGraphicLabel();

        private static readonly DependencyProperty RecordProperty = DependencyProperty.Register(nameof(Record), typeof(ICogRecord), typeof(CogDisplayer), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnImageChanged)));
        private static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register(nameof(ImageWidth), typeof(int), typeof(CogDisplayer), new FrameworkPropertyMetadata(800, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnImageSizeChanged)));
        private static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register(nameof(ImageHeight), typeof(int), typeof(CogDisplayer), new FrameworkPropertyMetadata(600, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnImageSizeChanged)));
        private static readonly DependencyProperty TextLsitProperty = DependencyProperty.Register(nameof(TextLsit), typeof(List<DisplayLable>), typeof(CogDisplayer), new FrameworkPropertyMetadata(new List<DisplayLable>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnTextChanged)));
      
        public CogDisplayer()
        {
            InitializeComponent();

            cogDisplay = new CogRecordDisplay();
            cogDisplay.Size = new System.Drawing.Size(ImageWidth, ImageHeight);


            ((System.ComponentModel.ISupportInitialize)(cogDisplay)).BeginInit();

            wfcontrol.SuspendLayout();

            //editor.AllowDrop = true;
            //editor.Dock = System.Windows.Forms.DockStyle.Fill;
            //editor.Name = "PMAlignEditV2";
            //editor.SuspendElectricRuns = false;
            //editor.TabIndex = 0;

            wfcontrol.Controls.Add(cogDisplay);
            wfcontrol.ResumeLayout();
            wfcontrol.PerformLayout();

            ((System.ComponentModel.ISupportInitialize)(cogDisplay)).EndInit();

            cogDisplay.MouseMode = CogDisplayMouseModeConstants.Touch;

        }

        public ICogRecord Record
        {
            get => (ICogRecord)GetValue(RecordProperty);
            set => SetValue(RecordProperty, value);
        }

        public int ImageWidth
        {
            get => (int)GetValue(ImageWidthProperty);
            set => SetValue(ImageWidthProperty, value);
        }
        public int ImageHeight
        {
            get => (int)GetValue(ImageHeightProperty);
            set => SetValue(ImageHeightProperty, value);
        }
        public List<DisplayLable> TextLsit
        {
            get => (List<DisplayLable>)GetValue(TextLsitProperty);
            set => SetValue(TextLsitProperty, value);
        }


        private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as CogDisplayer;
            dp.SetImage();


        }
        private static void OnImageSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as CogDisplayer;
            dp.SetSize();


        }
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as CogDisplayer;
            dp.SetTextLsit();


        }
        private void SetImage()
        {
            cogDisplay.Record = Record;


        }
        private void SetSize()
        {
            cogDisplay.Size = new System.Drawing.Size(ImageWidth, ImageHeight);
        }

        private void SetTextLsit()
        {

            foreach (var item in TextLsit) {

                cogDisplay.AddGraphicLabel(item);

              /*  cogGraphicLabel.SetXYText(item.Pos.X, item.Pos.Y, item.Text);
                cogGraphicLabel.Color = CogColorConstants.Red;
                cogGraphicLabel.Font = new System.Drawing.Font("Microsoft Sans Serif" ,18);        
                cogDisplay.StaticGraphics.Add(cogGraphicLabel, "");*/
            }

        }

    }
}
