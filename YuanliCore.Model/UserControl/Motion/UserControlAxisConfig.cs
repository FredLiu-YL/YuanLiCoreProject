﻿
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
    public class UserControlAxisConfig : INotifyPropertyChanged
    {
        /// <summary>
        /// 取得或設定 軟體負極限
        /// </summary>
        public double LimitNEL { get; set; } = 0;

        /// <summary>
        /// 取得或設定 軟體正極限
        /// </summary>
        public double LimitPEL { get; set; } = 1000000;

        /// <summary>
        /// 取得或設定 運動速度
        /// </summary>
        public VelocityParams MoveVel { get; set; } = new VelocityParams(50000);

        /// <summary>
        /// 取得或設定 回原點速度
        /// </summary>
        public VelocityParams HomeVel { get; set; } = new VelocityParams(50000);

        /// <summary>
        /// 取得或設定 初始化後位置
        /// </summary>
        public double InitialPos { get; set; } = 0;

        /// <summary>
        /// 取得或設定 原點模式
        /// </summary>
        public HomeModes HomeMode { get; set; } = HomeModes.ORG;

        /// <summary>
        /// 取得或設定 方向
        /// </summary>
        public MotionDirections Direction { get; set; } = MotionDirections.Backward;

        /// <summary>
        /// 取得或設定 軸解析度
        /// </summary>
        public double Unifactor { get; set; } = 1;

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
