using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Interface;


namespace YuanliCore.Motion
{
    public class Axis
    {
        private IMotionController controller;
        private bool isBusy;
        private bool isStop;
        private readonly object lockObj = new object();

        public Axis(IMotionController motioncontroller, int axisNum)
        {
            controller = motioncontroller;
            this.AxisID = axisNum;
        }

        public bool IsOpen { get; }
        public bool IsRunning { get; }

        /// <summary>
        /// 運動軸編號
        /// </summary>
        public int AxisID { get; }
        /// <summary>
        /// 運動軸名稱
        /// </summary>
        public string AxisName { get; set; }
        /// <summary>
        /// 當前位置
        /// </summary>
        public double Position { get => GetPositon(); }

        /// <summary>
        /// 比例(轉換成um、度)
        /// </summary>
        public double Ratio { get; set; } = 1;

        /// <summary>
        /// 到位整定容許量(um)
        /// </summary>
        public double Tolerance { get; set; } = 3;

        /// <summary>
        /// 軟體負極限
        /// </summary>
        public double PositionNEL { get => GetLimitN(); set => SetLimitN(value); }
        /// <summary>
        /// 軟體正極限
        /// </summary>
        public double PositionPEL { get => GetLimitP(); set => SetLimitP(value); }
        /// <summary>
        /// 運動軸方向
        /// </summary>
        public AxisDirection AxisDir { get => GetDirection(); set => SetDirection(value); }

        /// <summary>
        /// 運動軸速度
        /// </summary>
        public VelocityParams AxisVelocity { get => GetVelocity(); set => SetVelocity(value); }

        /// <summary>
        /// Axis 燈號狀態
        /// </summary>
        public AxisSensor AxisState { get => ReadSensor(); }

        public HomeDirection HomeDirection { get; set; }
        public HomeModes HomeMode { get; set; }
        /// <summary>
        /// 運動軸速度
        /// </summary>
        public VelocityParams HomeVelocity { get; set; }


        public void Open()
        {


        }
        public void Close()
        {


        }

        public async Task HomeAsync()
        {
            if (isBusy) return;
            isBusy = true;
            await Task.Run(() => { controller.HomeCommand(AxisID); });
            isBusy = false;
        }
        public void Stop()
        {
            controller.StopCommand(AxisID);
            isStop = true;
        }

        public async Task MoveAsync(double distance)
        {
            if (isBusy) throw new Exception($"ID{ AxisID}  {AxisName}  is Busy");
            try
            {
                int i = 0;
                isBusy = true;
                await Task.Run(async () =>
                {
                    double postion = Position + distance;
                    controller.MoveCommand(AxisID, distance);

                    while (Math.Abs(Position - postion) * Ratio > Tolerance && !isStop)
                    {
                        i++;
                        await Task.Delay(50);
                        if (i >= 200) throw new Exception($"ID{ AxisID}  {AxisName}  Time out");

                    }

                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                isBusy = false;
                isStop = false;
            }
        }

        public async Task MoveToAsync(double postion)
        {

            if (isBusy) throw new Exception($"ID{ AxisID}  {AxisName}  is Busy");
            try
            {
                isBusy = true;
                int i = 0;
                await Task.Run(async () =>
                {

                    controller.MoveToCommand(AxisID, postion);
                    double nowPosition = Position;
                    while (Math.Abs(nowPosition - postion) * Ratio > Tolerance && !isStop)
                    {
                        i++;
                        await Task.Delay(50);
                        if (i >= 200) throw new Exception($"ID{ AxisID}  {AxisName}  Time out ,Target:{postion} now:{nowPosition}");
                        nowPosition = Position;
                    }

                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                isBusy = false;
                isStop = false;

            }


        }



        private double GetPositon()
        {
            return controller.GetPositionCommand(AxisID);

        }
        private AxisDirection GetDirection()
        {
            return controller.GetAxisDirectionCommand(AxisID);
        }
        private void SetDirection(AxisDirection direction)
        {
            controller.SetAxisDirectionCommand(AxisID, direction);
        }
        private double GetLimitP()
        {
            controller.GetLimitCommand(AxisID, out double limitN, out double limitP);
            return limitP;
        }
        private double GetLimitN()
        {
            controller.GetLimitCommand(AxisID, out double limitN, out double limitP);
            return limitN;
        }


        private void SetLimitN(double limit)
        {
            controller.SetLimitCommand(AxisID, limit, PositionPEL);
        }
        private void SetLimitP(double limit)
        {
            controller.SetLimitCommand(AxisID, PositionNEL, limit);
        }
        private VelocityParams GetVelocity()
        {
            return controller.GetSpeedCommand(AxisID);

        }
        private void SetVelocity(VelocityParams axisVelocity)
        {
            controller.SetSpeedCommand(AxisID, axisVelocity);
            /*double velocity = axisVelocity.FainalVelocity;
            double accVelocity = axisVelocity.AccVelocity;
            double decVelocity = axisVelocity.DecVelocity;

            controller.SetSpeedCommand(AxisID, axisVelocity);*/


        }
        private AxisSensor ReadSensor()
        {
            return controller.GetSensorCommand(AxisID);
        }
    }


    public enum AxisDirection
    {
        Forward,
        Backward
    }

    public enum MotionDirections
    {
        Backward = -1,
        Forward = 1
    }

    public enum AxisSensor
    {
        ORG = 0,
        PEL = 1,
        NEL = 2,
        NONE = 3,
    }
    public enum HomeDirection
    {
        Forward,
        Backward
    }
    public enum HomeModes
    {
        ORG = 0,
        EL = 1,
        Index = 2,
        Block = 3,
        CurPos = 4,
        ELAndIndex = 5
    }
}
