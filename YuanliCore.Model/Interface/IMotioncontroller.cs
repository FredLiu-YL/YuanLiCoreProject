using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Motion;

namespace YuanliCore.Interface
{
    public interface IMotionController
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsOpen { get; }


        Axis[] Axes { get; }
        DigitalInput[] IutputSignals { get; }
        IEnumerable<DigitalOutput> OutputSignals { get; }



        void InitializeCommand();


        Axis[] SetAxesParam(IEnumerable<AxisConfig> axisConfig);
        DigitalOutput[] SetOutputs(IEnumerable<string> names);
        DigitalInput[] SetInputs(IEnumerable<string> names);

        void StopCommand(int id);
        void MoveCommand( int id,  double distance);

        void MoveToCommand( int id ,double position);

        void HomeCommand(int id);

        double GetPositionCommand(int id);

        AxisSensor GetSensorCommand(int id);

        void GetLimitCommand(int id, out double limitN, out double limitP);
        void SetLimitCommand(int id, double minPos , double maxPos);
        VelocityParams GetSpeedCommand(int id);
        void SetSpeedCommand(int id, VelocityParams motionVelocity);
        void SetAxisDirectionCommand(int id ,AxisDirection direction);
        AxisDirection GetAxisDirectionCommand(int id);
    }
}
