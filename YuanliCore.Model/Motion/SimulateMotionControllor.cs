using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Interface;

namespace YuanliCore.Motion
{
    public class SimulateMotionControllor : IMotionController
    {
        private double[] simulatePosition; //暫存虛擬座標 模擬驅動器內的位置
        private VelocityParams[] simulateVelocity; //模擬驅動器內的各軸的速度參數
        private double[] simulateLimitN; //模擬驅動器內的各軸的軟體極限
        private double[] simulateLimitP; //模擬驅動器內的各軸的軟體極限


        private Axis[] axes;
        public SimulateMotionControllor(IEnumerable<AxisConfig> axisInfos, IEnumerable<string> doNames, IEnumerable<string> diNames)
        {
            List<double> axesPos = new List<double>();
            List<VelocityParams> axesVel = new List<VelocityParams>();
            List<double> axeslimitN = new List<double>();
            List<double> axeslimitP = new List<double>();


            axes = axisInfos.Select((info, i) =>
            {

                axesPos.Add(0);
                return new Axis(this, info.AxisID)
                {
                    AxisName = info.AxisName

                };

            }).ToArray();
            var axisInfosArray = axisInfos.ToArray();
            //有多少軸就創建多少顆驅動器參數
            for (int i = 0; i < axes.Length; i++)
            {
                axeslimitN.Add(axisInfosArray[i].LimitNEL);
                axeslimitP.Add(axisInfosArray[i].LimitPEL);
                axesVel.Add(axisInfosArray[i].MoveVel);
                axes[i].Ratio = axisInfosArray[i].Ratio;
                axes[i].HomeVelocity = axisInfosArray[i].HomeVel;
                axes[i].HomeMode = axisInfosArray[i].HomeMode;
                axes[i].HomeDirection = axisInfosArray[i].HomeDirection;
            }


            simulatePosition = axesPos.ToArray();
            simulateVelocity = axesVel.ToArray();
            simulateLimitP = axeslimitP.ToArray();
            simulateLimitN = axeslimitN.ToArray();

            OutputSignals = doNames.Select((n, i) => new DigitalOutput(i, this));
            InputSignals = diNames.Select((n, i) => new DigitalInput(n, i, this)).ToArray();

            //Task.Run(ReflashInput);
        }

        public bool IsOpen => throw new NotImplementedException();

        public Axis[] Axes => axes;

        public DigitalInput[] InputSignals { get; private set; }

        public IEnumerable<DigitalOutput> OutputSignals { get; set; }




        public double GetPositionCommand(int id)
        {
            return simulatePosition[id];
        }

        public VelocityParams GetSpeedCommand(int id)
        {
            return simulateVelocity[id];

        }
        public void SetSpeedCommand(int id, VelocityParams motionVelocity)
        {

            simulateVelocity[id] = motionVelocity;
        }
        public void HomeCommand(int id)
        {
            simulatePosition[id] = 0;
        }

        public void InitializeCommand()
        {

        }

        public void MoveCommand(int id, double distance)
        {
            if (simulatePosition[id] + distance >= simulateLimitP[id])
                simulatePosition[id] = simulateLimitP[id];
            else if (simulatePosition[id] + distance <= simulateLimitN[id])
                simulatePosition[id] = simulateLimitN[id];
            else
                simulatePosition[id] += distance;
        }

        public void MoveToCommand(int id, double position)
        {
            simulatePosition[id] = position;
        }

        public Axis[] SetAxesParam(IEnumerable<AxisConfig> axisConfig)
        {
            throw new NotImplementedException();
        }

        public AxisDirection GetAxisDirectionCommand(int id)
        {
            return AxisDirection.Forward;
        }


        public void SetAxisDirectionCommand(int id, AxisDirection direction)
        {

        }

        public DigitalInput[] SetInputNames(IEnumerable<string> names)
        {
            throw new NotImplementedException();
        }
        public void GetLimitCommand(int id, out double limitN, out double limitP)
        {
            limitN = simulateLimitN[id];
            limitP = simulateLimitP[id];
        }
        public void SetLimitCommand(int id, double minPos, double maxPos)
        {
            simulateLimitN[id] = minPos;
            simulateLimitP[id] = maxPos;
        }

        public DigitalOutput[] SetOutputNames(IEnumerable<string> names)
        {
            throw new NotImplementedException();
        }

        public void StopCommand(int id)
        {
            //不實做

        }

        public AxisSensor GetSensorCommand(int id)
        {

            if (simulatePosition[id] == 0)
                return AxisSensor.ORG;
            if (simulatePosition[id] <= simulateLimitN[id])
                return AxisSensor.NEL;
            if (simulatePosition[id] >= simulateLimitP[id])
                return AxisSensor.PEL;

            return AxisSensor.NONE;
        }

        private async Task ReflashInput()
        {
            //try
            //{
            //    var inputCount = InputSignals.Length;

            //    foreach (var item in InputSignals)
            //    {
            //        item.IsSignal = false;
            //    }

            //    while (true)
            //    {
            //        for (int i = 0; i < inputCount; i++)
            //        {
            //            if (i == 0)
            //                InputSignals[inputCount - 1].IsSignal = false;
            //            else
            //                InputSignals[i - 1].IsSignal = false;

            //            InputSignals[i].IsSignal = true;

            //            await Task.Delay(500);
            //        }



            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        public void SetOutputCommand(int id, bool isOn)
        {
            //throw new NotImplementedException();
        }

        public void SetServoCommand(int id, bool isOn)
        {
            throw new NotImplementedException();
        }

        public void ResetAlarmCommand()
        {
            throw new NotImplementedException();
        }

        public bool GetInputCommand(int id)
        {
            return false;
        }
    }

}
