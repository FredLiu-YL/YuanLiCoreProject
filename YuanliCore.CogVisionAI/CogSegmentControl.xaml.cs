using Cognex.Vision;
using Cognex.VisionPro;
using Cognex.VisionPro.Blob;
using Cognex.VisionPro.ViDiEL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace YuanliCore.ImageProcess.AI
{
    /// <summary>
    /// PatMaxControl.xaml 的互動邏輯
    /// </summary>
    public  partial class CogSegmentToolControl : UserControl
    {
        private static readonly DependencyProperty ImageProperty = DependencyProperty.Register(nameof(Image), typeof(ICogImage), typeof(CogSegmentToolControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnImageChanged)));
        private static readonly DependencyProperty ToolProperty = DependencyProperty.Register(nameof(Tool), typeof(CogSegmentTool), typeof(CogSegmentToolControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnToolChanged)));
        //     private static readonly DependencyProperty ImageConvertParamProperty = DependencyProperty.Register(nameof(ImageConvertParam), typeof(CogImageConvertParams), typeof(CogSegmentToolControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnPatmaxParamChanged)));

        //private CogSegmentTool tool;
        private CogSegmentEditV2 editor;

    //    private CogBlobTool tool;
    //    private CogBlobEditV2 editor;


        public CogSegmentToolControl()
        {
          
            InitializeComponent();
            //    tool = new CogBlobTool();
            //    editor = new CogBlobEditV2();
            editor = new CogSegmentEditV2();
            Tool = new CogSegmentTool();
      

     //      ImageConvertParam = CogImageConvertParams.Default(tool, 0);

            //Tool.Changed += PatMaxTool_Changed;

        //    ((System.ComponentModel.ISupportInitialize)(editor)).BeginInit();

            wfcontrol.SuspendLayout();

            editor.AllowDrop = true;
            editor.Dock = System.Windows.Forms.DockStyle.Fill;
            editor.Name = "CogSegmentEditV2";
     //       editor.SuspendElectricRuns = false;
            editor.TabIndex = 0;

            wfcontrol.Controls.Add(editor);
            wfcontrol.ResumeLayout();
            wfcontrol.PerformLayout();

        //    ((System.ComponentModel.ISupportInitialize)(editor)).EndInit();

            editor.Subject = Tool;

        }

        private void SearchRegion_Changed(object sender, CogChangedEventArgs e)
        {
            var flagName = e.GetStateFlagNames(sender);

    //        ParameterChangedEvent?.Invoke(this, new PatmaxParamsEventArgs(PatmaxParam));
            Trace.WriteLine($"SearchRegion_Changed => {flagName}");
        }

     //   public event EventHandler<PatmaxParamsEventArgs> PatternTrainedEvent;

      //  public event EventHandler<PatmaxParamsEventArgs> ParameterChangedEvent;

        public ICogImage Image
        {
            get => (ICogImage)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public CogSegmentTool Tool
        {
            get => (CogSegmentTool)GetValue(ToolProperty);
            set => SetValue(ToolProperty, value);

        }

        //    public CogImageConvertParams ImageConvertParam
        //     {
        //        get => (CogImageConvertParams)GetValue(ImageConvertParamProperty);
        //         set => SetValue(ImageConvertParamProperty, value);
        //    }

        private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as CogSegmentToolControl;
            dp.SetImage();
        }
        private static void OnToolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as CogSegmentToolControl;
            dp.SetTool();
        }

        //private static void OnPatmaxParamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var dp = d as CogSegmentToolControl;
        //    dp.RefreshPatmaxParam();
        //}

        private void SetImage()
        {
            Tool.InputImage = Image as ICogVisionData;
        }
        private void SetTool()
        {
            editor.Subject = Tool;
        }
        //private void RefreshPatmaxParam()
        //{
        //    //if (tool == null) { throw new Exception("123"); }// 先測試是否有可能會發生

        //    // 移除前次參數事件

        //    //       tool.RunParams.Changed -= RunParams_Changed;
        //    //       if (tool.Region != null) tool.Region.Changed -= SearchRegion_Changed;

        //    // 更新 tool 內的 Pax 參數

        //    //        tool.RunParams = ImageConvertParam.RunParams;
        //    //       tool.Region = ImageConvertParam.Region;

        //    // 將新參數委派事件           
        //    //        if (tool.RunParams != null) tool.RunParams.Changed += RunParams_Changed;

        //    //        if (tool.Region != null) tool.Region.Changed += SearchRegion_Changed;
        //}

        #region internal event
        //  private void Pattern_Changed(object sender, CogChangedEventArgs e)
        //  {
        //      var flagName = e.GetStateFlagNames(sender);

        //  //    if (flagName.Contains("SfTrained"))
        //  //        PatternTrainedEvent?.Invoke(this, new PatmaxParamsEventArgs(PatmaxParam));

        //      Trace.WriteLine($"Pattern_Changed => {flagName}");
        //  }

        //  private void RunParams_Changed(object sender, CogChangedEventArgs e)
        //  {
        //      var flagName = e.GetStateFlagNames(sender);

        ////      ParameterChangedEvent?.Invoke(this, new PatmaxParamsEventArgs(PatmaxParam));
        //      Trace.WriteLine($"RunParams_Changed => {flagName}");
        //  }

        //private void PatMaxTool_Changed(object sender, CogChangedEventArgs e)
        //{
        //    var flagName = e.GetStateFlagNames(sender);
        //    Trace.WriteLine($"PatMaxTool_Changed => {flagName}");
        //}

        //public void SetToParam()
        //{

        //    //        ImageConvertParam.Region = tool.Region;
        //    //        ImageConvertParam.RunParams = tool.RunParams;
        //}
        #endregion

        //private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    bool isVisible = (bool)e.NewValue;

        //    //if (!isVisible) SetToParam();
        //}
        public void Dispose()
        {
            // 釋放 editor 的資源
            if (editor != null)
            {
                editor.Dispose();
                editor = null;
            }

        }
       
    }

}
