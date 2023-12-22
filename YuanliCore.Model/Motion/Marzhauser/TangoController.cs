using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Interface;
using YuanliCore.Interface.Motion;

namespace YuanliCore.Motion.Marzhauser
{
    public class TangoController : IMotionController
    {
  
        private string comPort;
        private int baudRate = 57600;


        public TangoController(string comPort)
        {

            this.comPort = comPort;

        }


        public bool IsOpen { get; set; }



        public DigitalInput[] IutputSignals => throw new NotImplementedException();

        public IEnumerable<DigitalOutput> OutputSignals => throw new NotImplementedException();

        public  Axis[] Axes => GetDefaultAxes();



        public async void InitializeCommand()
        {

            try
            {
                #region 初始化流程
                //------------設定參數----------------
                //   Mechanical parameters
                //   SetPitch();
                //   SetGear();
                //   SetDimensions();
                //   SetActiveAxis();
                //   SetAxisDirection();
                //   SetXYComp();
                //------------設定極限開關-------------
                //  Hardware limit switches
                //  SetSwitchActive();
                //  SetSwitchPolarity();
                //------------設定軟體極限-------------
                // Software limit switches
                // SetLmit();
                // SetLimitControl();
                //------------運動設定-------------
                //  Motor Configuration
                //  SetCurrent();
                //  SetReduction();
                //------------運動設定-------------
                //  Encoder Configuration
                //  SetEncoderActive();
                //  SetEncoderPeriod();
                //  SetEncoderRefSignal();
                //  SetEncoderPosition();
                #endregion


                Int32 ShowProt = 0;

                Int32 loc_err = TangoLib.LS_ConnectSimple(1, comPort, baudRate, 0);
                if (loc_err == 0)
                {
                    IsOpen = true;
                    //  var axisinfo_ = GetAxisVel();
                }
                else
                    throw new Exception("Tango Initialize Fail");

            }
            catch (Exception ex)
            {
                // 可能是 執行檔資料夾沒有 Tango_DLL_x64.DLL
                throw ex;

            }

        }


        public void MoveToCommand(int id, double position)
        {
            /*TangoLib.LS_GetPos(out Double pdX, out Double pdY, out Double pdZ, out Double pdA);
            switch (id)
            {
                case 1:
                    var r1 = TangoLib.LS_MoveAbs(position, pdY, pdZ, pdA, 1);
                    break;

                case 2:
                    var r2 = TangoLib.LS_MoveAbs(pdX, position, pdZ, pdA, 1);
                    break;


                case 3:

                    break;


                case 4:

                    break;
                default:
                    break;
            }*/


        
            var r1 = TangoLib.LS_MoveAbsSingleAxis(id, position, 0);
          
        }


        public void HomeCommand(int id)
        {
            //  TangoLib.LS_MoveAbsSingleAxis(id, position, 1);
            var r1 = TangoLib.LS_Calibrate();
            var r2 = TangoLib.LS_RMeasure();
        }

        public double GetPositionCommand(int id)
        {
            try
            {
                UpdatePosition(out double posX, out double posY, out double posZ, out double posA);
                switch (id)
                {
                    case 1:
                        return posX;
                       

                    case 2:
                        return posY;
                    


                    case 3:
                        return posZ;
                


                    case 4:
                        return posA;
                    

                    default:
                        throw new Exception("Id Error");
                       
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public Axis[] SetAxesParam(IEnumerable<AxisInfo> axisInfos)
        {
            throw new NotImplementedException();
        }

        public DigitalOutput[] SetOutputs(IEnumerable<string> names)
        {
            throw new NotImplementedException();
        }

        public DigitalInput[] SetInputs(IEnumerable<string> names)
        {
            throw new NotImplementedException();
        }
        public void StopCommand(int id)
        {
            TangoLib.LS_StopAxes();
        }
        public void MoveCommand(int id, double distance)
        {
            try
            {
                var r1 = TangoLib.LS_MoveRelSingleAxis(id, distance, 0);

            }
            catch (Exception ex)
            {


            }
        }


        public void GetLimitCommand(int id, out double limitN, out double limitP)
        {
            Int32 loc_err = TangoLib.LS_GetLimit(id, out double pdMinRange, out double pdMaxRange);
            limitN = pdMinRange;
            limitP = pdMaxRange;
        }
        public void SetLimitCommand(int id, double limitN, double limitP)
        {
            Int32 loc_err = TangoLib.LS_SetLimit(id, limitN, limitP);

        }

        public AxisDirection GetAxisDirectionCommand(int id)
        {
            Int32 loc_err = TangoLib.LSX_GetAxisDirection(1, out int plXD, out int plYD, out int plZD, out int plAD);

         /*   switch (id)
            {
                case 1:
                    return (AxisDirection)plXD;
                    break;

                case 2:
                    return (AxisDirection)plYD;
                    break;


                case 3:
                    return (AxisDirection)plZD;
                    break;


                case 4:
                    return (AxisDirection)plAD;
                    break;

                default:
                    break;
            }*/


            return AxisDirection.Forward;
        }

        public void SetAxisDirectionCommand(int id, AxisDirection direction)
        {
            /*
                0  禁用操縱桿的軸（偏轉忽略）
                1  正軸方向，不使用電流減少
               -1  負軸方向，不使用電流減少
                2  電流減少的正軸方向（默認）
               -2  負軸方向，電流減小
            */
            //沒搞懂 電流減少意義 ， 先以 2 與 -2 來做切換 
            //int dir = 2;
            // if (direction == AxisDirection.Backward)
            //     dir = -2;

            Int32 loc_err = 0;
            //先獲取原本方向 ， 並將 搖桿控制設定 1 啟用
            TangoLib.LSX_GetAxisDirection(1, out int plXD, out int plYD, out int plZD, out int plAD);

            switch (id)
            {
                case 1:
                    loc_err = TangoLib.LSX_SetAxisDirection(1, (int)direction, plYD, plZD, plAD);
                    break;

                case 2:
                    loc_err = TangoLib.LSX_SetAxisDirection(1, plXD, (int)direction, plZD, plAD);
                    break;

              case 3:        
                    loc_err = TangoLib.LSX_SetAxisDirection(1, plXD, plYD, (int)direction, plAD);
              
                    break;

                 case 4:       
                    loc_err = TangoLib.LSX_SetAxisDirection(1, plXD, plYD, plZD, (int)direction);
            
                    break;

                default:
                    break;
            }

            if (loc_err != 0)
                throw new Exception("Set Direction Fail");


        }


        private void UpdatePosition(out double posX, out double posY, out double posZ, out double posA)
        {
            Double xx, yy, zz, aa;
            try
            {
                TangoLib.LS_GetPos(out xx, out yy, out zz, out aa);
                posX = Math.Round(xx, 4);
                posY = Math.Round(yy, 4);
                posZ = Math.Round(zz, 4);
                posA = Math.Round(aa, 4);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private Axis[] GetDefaultAxes()
        {
            List<Axis> axesList = new List<Axis>();

            for (int i = 1; i <= 4; i++)
            {
                axesList.Add(new Axis(this, i));
            }
            return axesList.ToArray();
        }

        /// <summary>
        /// 搖桿開關
        /// </summary>
        private void JoysticSwitch()
        {


        }

        public VelocityParams GetSpeedCommand(int id)
        {
            TangoLib.LS_GetVel(out double motionVelX, out double motionVelY, out double motionVelZ, out double motionVelA);
            TangoLib.LS_GetAccel(out double motionAccVelX, out double motionAccVelY, out double motionAccVelZ, out double motionAccVelA);
            TangoLib.LS_GetStopAccel(out double motionDecVelX, out double motionDecVelY, out double motionDecVelZ, out double motionDecVelA);

            switch (id)
            {
                case 1:
                    VelocityParams velocityX = new VelocityParams(0,motionVelX, motionAccVelX, motionDecVelX);
                    return velocityX;
                 

                case 2:
                    VelocityParams velocityY = new VelocityParams(0,motionVelY, motionAccVelY, motionDecVelY);
                    return velocityY;
                 


                case 3:
                    VelocityParams velocityZ = new VelocityParams(0,motionVelZ, motionAccVelZ, motionDecVelZ);
                    return velocityZ;
                 


                case 4:
                    VelocityParams velocityA = new VelocityParams(0,motionVelA, motionAccVelA, motionDecVelA);
                    return velocityA;
                   

                default:
                    throw new NotImplementedException("Axis  does not exist");
               
            }
        }

        public void SetSpeedCommand(int id, VelocityParams motionVelocity)
        {
            TangoLib.LS_GetVel(out double motionVelX, out double motionVelY, out double motionVelZ, out double motionVelA);
            TangoLib.LS_GetAccel(out double motionAccVelX, out double motionAccVelY, out double motionAccVelZ, out double motionAccVelA);
            TangoLib.LS_GetStopAccel(out double motionDecVelX, out double motionDecVelY, out double motionDecVelZ, out double motionDecVelA);

            double acc = motionVelocity.FinalVel / motionVelocity.AccelerationTime;
            double dec = motionVelocity.FinalVel / motionVelocity.DecelerationTime;

            switch (id)
            {
                case 1:

                 
                    TangoLib.LS_SetVel(motionVelocity.FinalVel, motionVelY,  motionVelZ,  motionVelA);
                    TangoLib.LS_SetAccel(acc,  motionAccVelY,  motionAccVelZ,  motionAccVelA);
                    TangoLib.LS_SetStopAccel(dec,  motionDecVelY,  motionDecVelZ,  motionDecVelA);

                    break;

                case 2:

                    TangoLib.LS_SetVel(motionVelX, motionVelocity.FinalVel, motionVelZ, motionVelA);
                    TangoLib.LS_SetAccel(motionAccVelX, acc, motionAccVelZ, motionAccVelA);
                    TangoLib.LS_SetStopAccel(motionDecVelX, dec, motionDecVelZ, motionDecVelA);
                    break;


                case 3:
                    TangoLib.LS_SetVel(motionVelX, motionVelY, motionVelZ, motionVelA);
                    TangoLib.LS_SetAccel(motionAccVelX, motionAccVelY, motionAccVelZ, motionAccVelA);
                    TangoLib.LS_SetStopAccel(motionDecVelX, motionDecVelY, motionDecVelZ, motionDecVelA);
                    break;


                case 4:

                    TangoLib.LS_SetVel(motionVelX, motionVelY, motionVelZ, motionVelA);
                    TangoLib.LS_SetAccel(motionAccVelX, motionAccVelY, motionAccVelZ, motionAccVelA);
                    TangoLib.LS_SetStopAccel(motionDecVelX, motionDecVelY, motionDecVelZ, motionDecVelA);
                    break;

                default:
                    throw new NotImplementedException("Axis  does not exist");
                    
            }
        }

        private AxisInfo[] GetAxisVel()
        {

            TangoLib.LS_GetVel(out double motionVelX, out double motionVelY, out double motionVelZ, out double motionVelA);
            TangoLib.LS_GetAccel(out double motionAccVelX, out double motionAccVelY, out double motionAccVelZ, out double motionAccVelA);
            TangoLib.LS_GetStopAccel(out double motionDecVelX, out double motionDecVelY, out double motionDecVelZ, out double motionDecVelA);

            return new AxisInfo[] {
                    new AxisInfo(){AxisName="AxisX" , AxisID=1,Velocity= new VelocityParams(0, motionVelX,motionAccVelX , motionDecVelX)},
                    new AxisInfo(){AxisName="AxisY" , AxisID=2, Velocity=new VelocityParams(0, motionVelY,motionAccVelY , motionDecVelY)},
                    new AxisInfo(){AxisName="AxisZ" , AxisID=3, Velocity=new VelocityParams(0, motionVelZ,motionAccVelZ , motionDecVelZ)},
                    new AxisInfo(){AxisName="AxisA" , AxisID=4, Velocity=new VelocityParams(0, motionVelA,motionAccVelA , motionDecVelA)},
            };

        }

        public AxisSensor GetSensorCommand(int id)
        {
            throw new NotImplementedException();
        }
    }

}
