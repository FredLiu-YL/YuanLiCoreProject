using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace YuanliCore.Interface
{
    public class DigitalInput : INotifyPropertyChanged
    {
        private IMotionController motionController;

        private int id;
        public DigitalInput(string name, int id, IMotionController motionController)
        {
            Name = name;
            this.motionController = motionController;
            this.id = id;
        }
        public string Name { get; set; }
        public bool IsSignal { get => GetInput(); }
        private bool GetInput()
        {
            return motionController.GetInputCommand(id);
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
