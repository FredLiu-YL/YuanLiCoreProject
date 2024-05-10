
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Interface;

namespace YuanliCore.Motion
{
    public class AxisConfig : INotifyPropertyChanged
    {

        private double limitNEL = 0, limitPEL = 1000000;
        private double initialPos;
        private VelocityParams moveVel = new VelocityParams(50000);
        private VelocityParams homeVel = new VelocityParams(50000);
        private string axisName="";

        /// <summary>
        /// 運動軸在卡的號碼
        /// </summary>
        public int AxisID { get; set; }
        /// <summary>
        /// 運動軸名稱
        /// </summary>
        public string AxisName { get => axisName; set => SetValue(ref axisName, value); }

        /// <summary>
        /// 取得或設定 軟體負極限
        /// </summary>
        public double LimitNEL { get => limitNEL; set => SetValue(ref limitNEL, value); }

        /// <summary>
        /// 取得或設定 軟體正極限
        /// </summary>
        public double LimitPEL { get => limitPEL; set => SetValue(ref limitPEL, value); }


        /// <summary>
        /// 取得或設定 運動速度
        /// </summary>
        public VelocityParams MoveVel { get => moveVel; set => SetValue(ref moveVel, value); }

        /// <summary>
        /// 取得或設定 回原點速度
        /// </summary>
        public VelocityParams HomeVel { get => homeVel; set => SetValue(ref homeVel, value); }

        /// <summary>
        /// 取得或設定 初始化後位置
        /// </summary>
        public double InitialPos { get => initialPos; set => SetValue(ref initialPos, value); }

        /// <summary>
        /// 取得或設定 原點模式
        /// </summary>
        public HomeModes HomeMode { get; set; } = HomeModes.ORG;
        /// <summary>
        /// 取得或設定 原點方向
        /// </summary>
        public HomeDirection HomeDirection { get; set; } = HomeDirection.Backward;
        /// <summary>
        /// 取得或設定 方向
        /// </summary>
        public AxisDirection Direction { get; set; } = AxisDirection.Forward;

        /// <summary>
        /// 取得或設定 軸解析度
        /// </summary>
        public double Ratio { get; set; } = 1;
        /// <summary>
        /// 到位整定容許量(um)
        /// </summary>
        public double Tolerance { get; set; } = 3;

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
