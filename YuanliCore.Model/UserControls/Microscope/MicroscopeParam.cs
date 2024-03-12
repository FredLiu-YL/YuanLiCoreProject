using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace YuanliCore.Model
{
    public class MicroscopeParam : INotifyPropertyChanged
    {
        private int lensIndex;
        private int cubeIndex;
        private int filter1Index;
        private int filter2Index;
        private int filter3Index;
        private int lightValue;
        private int apertureValue;
        private int position;
        private int nEL;
        private int pEL;
        private int aberationPosition;
        private int aFNEL;
        private int aFPEL;
        private int timeOutRetryCount;

        /// <summary>
        /// 目前Lens在第幾孔
        /// </summary>
        public int LensIndex
        {
            get => lensIndex;
            set => SetValue(ref lensIndex, value);
        }
        /// <summary>
        /// 目前Cube在第幾孔
        /// </summary>
        public int CubeIndex
        {
            get => cubeIndex;
            set => SetValue(ref cubeIndex, value);
        }
        /// <summary>
        /// 目前Filter1在第幾孔
        /// </summary>
        public int Filter1Index
        {
            get => filter1Index;
            set => SetValue(ref filter1Index, value);
        }
        /// <summary>
        /// 目前Filter2在第幾孔
        /// </summary>
        public int Filter2Index
        {
            get => filter2Index;
            set => SetValue(ref filter2Index, value);
        }
        /// <summary>
        /// 目前Filter3在第幾孔
        /// </summary>
        public int Filter3Index
        {
            get => filter3Index;
            set => SetValue(ref filter3Index, value);
        }
        /// <summary>
        /// 目前光強度
        /// </summary>
        public int LightValue
        {
            get => lightValue;
            set => SetValue(ref lightValue, value);
        }
        /// <summary>
        /// 目前光圈
        /// </summary>
        public int ApertureValue
        {
            get => apertureValue;
            set => SetValue(ref apertureValue, value);
        }
        /// <summary>
        /// 目前Z軸位置
        /// </summary>
        public int Position
        {
            get => position;
            set => SetValue(ref position, value);
        }
        /// <summary>
        /// Z軟體負極限
        /// </summary>
        public int NEL
        {
            get => nEL;
            set => SetValue(ref nEL, value);
        }
        /// <summary>
        /// Z軟體正極限
        /// </summary>
        public int PEL
        {
            get => pEL;
            set => SetValue(ref pEL, value);
        }
        /// <summary>
        /// 準焦位置
        /// </summary>
        public int AberationPosition
        {
            get => aberationPosition;
            set => SetValue(ref aberationPosition, value);
        }
        /// <summary>
        /// 自動對焦負極限
        /// </summary>
        public int AFNEL
        {
            get => aFNEL;
            set => SetValue(ref aFNEL, value);
        }
        /// <summary>
        /// 自動對焦正極限
        /// </summary>
        public int AFPEL
        {
            get => aFPEL;
            set => SetValue(ref aFPEL, value);
        }
        /// <summary>
        /// TimeOut重送次數
        /// </summary>
        public int TimeOutRetryCount
        {
            get => timeOutRetryCount;
            set => SetValue(ref timeOutRetryCount, value);
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
