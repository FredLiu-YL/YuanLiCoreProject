using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace YuanliCore.Interface
{
    public class DigitalOutput : INotifyPropertyChanged
    {
        private IMotionController motionController;
        private bool isSwitchOn;
        public DigitalOutput(int id, IMotionController motionController)
        {
            ID = id;
            this.motionController = motionController;
        }
        public bool IsSwitchOn { get => isSwitchOn; set => SetValue(ref isSwitchOn, value); } //因沒實作 IMotionController的模擬動作  先暫放這
        public string Name { get; set; }
        public int ID { get; set; }

        public void On()
        {
            motionController.SetOutputCommand(ID, true);
            //  motionController.IutputSignals[ID].IsSignal = true;
            IsSwitchOn = true;
        }
        public void Off()
        {
            motionController.SetOutputCommand(ID, false);
            //  motionController.IutputSignals[ID].IsSignal = false;
            IsSwitchOn = false;
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
