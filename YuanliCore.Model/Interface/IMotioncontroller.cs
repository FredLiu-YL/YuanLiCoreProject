using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Interface.Motion;
using YuanliCore.Motion;

namespace YuanliCore.Interface
{
    public interface IMotionController
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsOpen { get; }


        IEnumerable<Axis> Axes { get; }
        IEnumerable<DigitalInput> IutputSignals { get; }
        IEnumerable<DigitalOutput> OutputSignals { get; }



        void InitializeCommand();


        Axis[] SetAxesParam(IEnumerable<AxisInfo> axisInfos);
        DigitalOutput[] SetOutputs(IEnumerable<string> names);
        DigitalInput[] SetInputs(IEnumerable<string> names);

        void StopCommand(int id);
        void MoveCommand( int id,  double distance);

        void MoveToCommand( int id ,double position);

        void HomeCommand(int id);

        double GetPositionCommand(int id);

        void GetLimitCommand(int id, out double limitN, out double limitP);
        void SetLimitCommand(int id, double minPos , double maxPos);
        MotionVelocity GetSpeedCommand(int id);
        void SetSpeedCommand(int id, MotionVelocity motionVelocity);
        void SetAxisDirectionCommand(int id ,AxisDirection direction);
        AxisDirection GetAxisDirectionCommand(int id);
    }
}
