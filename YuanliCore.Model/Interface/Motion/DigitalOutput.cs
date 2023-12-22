using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuanliCore.Interface
{
    public class DigitalOutput
    {
        private IMotionController motionController;
    
        public DigitalOutput(int id ,IMotionController motionController) 
        {
            ID = id;
            this.motionController = motionController;

        }
        public bool IsSwitchOn; //因沒實作 IMotionController的模擬動作  先暫放這

        public string Name { get; set; }

        public int ID { get; set; }

        public void On()
        {
            motionController.IutputSignals[ID].IsSignal = true;
            IsSwitchOn = true;

        }
        public void Off()
        {
            motionController.IutputSignals[ID].IsSignal = false;
            IsSwitchOn = false;
        }


    }
}
