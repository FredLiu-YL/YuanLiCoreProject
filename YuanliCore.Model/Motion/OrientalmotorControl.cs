using Modbus.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Interface;
using YuanliCore.Motion;

namespace YuanliCore.Motion
{
    public class OrientalmotorControl : IMotionController
    {
        private byte slaveAddress;
        private ModbusSerialMaster serialMasterDevice;

        public OrientalmotorControl(SerialPort serialPort, int driverID)
        {

            var id = BitConverter.GetBytes(driverID);

            slaveAddress = id[0];

            serialMasterDevice = ModbusSerialMaster.CreateRtu(serialPort);
            serialMasterDevice.Transport.Retries = 3;
            serialMasterDevice.Transport.WaitToRetryMilliseconds = 500;
            serialMasterDevice.Transport.ReadTimeout = 1000;
            serialMasterDevice.Transport.WriteTimeout = 1000;
        }


        public bool IsOpen => throw new NotImplementedException();

        public Axis[] Axes => throw new NotImplementedException();

        public DigitalInput[] InputSignals => throw new NotImplementedException();

        public IEnumerable<DigitalOutput> OutputSignals => throw new NotImplementedException();

        public AxisDirection GetAxisDirectionCommand(int id)
        {
            throw new NotImplementedException();
        }

        public void GetLimitCommand(int id, out double limitN, out double limitP)
        {
            throw new NotImplementedException();
        }

        public double GetPositionCommand(int id)
        {
            //現階段實作只有1軸  所以 ID不使用
            var pos = GetPosition();
            return pos;
        }

        public AxisSensor GetSensorCommand(int id)
        {
            throw new NotImplementedException();
        }

        public VelocityParams GetSpeedCommand(int id)
        {
            throw new NotImplementedException();
        }

        public void HomeCommand(int id)
        {
            //現階段實作只有1軸  所以 ID不使用
            Home();
        }

        public void InitializeCommand()
        {
            throw new NotImplementedException();
        }

        public void MoveCommand(int id, double distance)
        {
            SetOperationMode(OperationMode.Relative);
           //定義位置移動需要的 基準座標  地址
            byte[] up = new byte[] { 0x82, 0x18 };//按照陣列排列 下位 需要放前面 ， 上位在後面
            ushort upAddress = BitConverter.ToUInt16(up, 0);

            //定義位置移動需要的 目標座標  地址
            byte[] add = new byte[] { 0x83, 0x18 };//按照陣列排列 下位 需要放前面 ， 上位在後面 P.363頁
            ushort Address = BitConverter.ToUInt16(add, 0);

         
            if (distance>=0)
            {

                byte[] distanceBytes = BitConverter.GetBytes((int)distance);
                ushort distValueH = BitConverter.ToUInt16(distanceBytes, 2);
                ushort distValueL = BitConverter.ToUInt16(distanceBytes, 0);
                

                //正向
                serialMasterDevice.WriteSingleRegister(slaveAddress, upAddress, distValueH);
                serialMasterDevice.WriteSingleRegister(slaveAddress, Address, (ushort)(distValueL -1));

            }
            else
            {
              
                var dis  = -distance; //東方沒有 方向移動不能吃負號 ，所以反向必須調整成 正值 
                byte[] distanceBytes = BitConverter.GetBytes((int)dis);
                ushort distValueH = BitConverter.ToUInt16(distanceBytes, 2);
                ushort distValueL = BitConverter.ToUInt16(distanceBytes, 0);

                //負向
                serialMasterDevice.WriteSingleRegister(slaveAddress, upAddress, (ushort)(65535- distValueH));     
               // serialMasterDevice.WriteSingleRegister(slaveAddress, Address, (ushort)(65535 - dis + 1));
                serialMasterDevice.WriteSingleRegister(slaveAddress, Address, (ushort)(65535 - distValueL));
            }


           // SetSpeed((ushort)AxisSpeed);
           //馬達運行  啟動第2組設定
            serialMasterDevice.WriteSingleRegister(slaveAddress, 0x7D, 0x0A);

            CommandReset();
        }

        public void MoveToCommand(int id, double position)
        {
            SetOperationMode(OperationMode.Absolute);

            byte[] posBytes = BitConverter.GetBytes((int)position);
            ushort posValueH = BitConverter.ToUInt16(posBytes, 2);
            ushort posValueL = BitConverter.ToUInt16(posBytes, 0);


            byte[] up = new byte[] { 0x82, 0x18 };//按照陣列排列 下位 需要放前面 ， 上位在後面
            byte[] add = new byte[] { 0x83, 0x18 };

            ushort upAddress = BitConverter.ToUInt16(up, 0);
            serialMasterDevice.WriteSingleRegister(slaveAddress, upAddress, posValueH);

            ushort Address = BitConverter.ToUInt16(add, 0);

            serialMasterDevice.WriteSingleRegister(slaveAddress, Address, (ushort)(posValueL - 1));
         
            serialMasterDevice.WriteSingleRegister(slaveAddress, 0x7D, 0x0A);
            CommandReset();

        }

        public Axis[] SetAxesParam(IEnumerable<AxisConfig> axisConfig)
        {
            throw new NotImplementedException();
        }

        public void SetAxisDirectionCommand(int id, AxisDirection direction)
        {
            throw new NotImplementedException();
        }

        public DigitalInput[] SetInputNames(IEnumerable<string> names)
        {
            throw new NotImplementedException();
        }

        public void SetLimitCommand(int id, double minPos, double maxPos)
        {
            throw new NotImplementedException();
        }

        public void SetOutputCommand(int id, bool isOn)
        {
            throw new NotImplementedException();
        }

        public DigitalOutput[] SetOutputNames(IEnumerable<string> names)
        {
            throw new NotImplementedException();
        }

        public void SetServoCommand(int id, bool isOn)
        {
            throw new NotImplementedException();
        }
        public void ResetAlarmCommand()
        {
            ResetAlarmCode();
        }

        public void SetSpeedCommand(int id, VelocityParams motionVelocity)
        {

            SetFinalSpeed(motionVelocity.MaxVel);
            //要在幾秒內到達最高速度  例0.1秒到達10000的速度  Max= 10000  加速就是10萬  = 10000 /0.1   
            var accSpeed = motionVelocity.MaxVel / motionVelocity.AccelerationTime;
            var decSpeed = motionVelocity.MaxVel / motionVelocity.DecelerationTime;

            SetACCSpeed((int)accSpeed, (int)decSpeed);

        }

        public void StopCommand(int id)
        {  
            //現階段實作只有1軸  所以 ID不使用
            Stop();
        }


        private void Home()
        {
            try
            {
                if (serialMasterDevice == null) return;
                ushort[] m_zhome = { 0x01, 0x06, 0x00, 0x7D, 0x00, 0x10, 0x18, 0x1E }; //ZHOME


                byte[] byte1 = BitConverter.GetBytes(0x00);
                byte[] byte2 = BitConverter.GetBytes(0x7D);
                byte[] byteAddress = new byte[] { byte1[0], byte1[1], byte2[0], byte2[2] };
                ushort Address = BitConverter.ToUInt16(byteAddress, 0);

                serialMasterDevice.WriteSingleRegister(slaveAddress, 0x7D, 0x10);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SetOperationMode(OperationMode mode)
        {
            byte[] add = new byte[] { 0x81, 0x18 };//設定運轉模式 位置
            ushort address1 = BitConverter.ToUInt16(add, 0);

            switch (mode)
            {

                case OperationMode.Absolute:
                    serialMasterDevice.WriteSingleRegister(slaveAddress, address1, 0x01);//絕對位置
                    break;

                case OperationMode.Relative:
                    serialMasterDevice.WriteSingleRegister(slaveAddress, address1, 0x03);//相對位置
                    break;


            }


        }
        private void SetFinalSpeed(double speed)
        {

            var uspeedBytes = BitConverter.GetBytes((int)speed);
           

            byte[] addH = new byte[] { 0x84, 0x18 };  //速度由 32位元組成  [84,85] 但1次只能寫16位元 所以要分兩次下參數
            byte[] addL = new byte[] { 0x85, 0x18 };

            ushort addressH = BitConverter.ToUInt16(addH, 0);
            ushort addressL = BitConverter.ToUInt16(addL, 0);

            ushort valueH = BitConverter.ToUInt16(uspeedBytes, 2);
            ushort valueL = BitConverter.ToUInt16(uspeedBytes, 0);


            serialMasterDevice.WriteSingleRegister(slaveAddress, addressH, valueH);
            serialMasterDevice.WriteSingleRegister(slaveAddress, addressL, valueL);
        }
        private void SetACCSpeed(int accSpeed, int decSpeed)
        {
            var accspeedByte = BitConverter.GetBytes(accSpeed);
            var decspeedByte = BitConverter.GetBytes(decSpeed);

            //加速度由 32位元組成  [86,87] 但1次只能寫16位元 所以要分兩次下參數
            byte[] accSpeedAddressH = new byte[] { 0x86, 0x18 }; //高位元
            byte[] accSpeedAddressL = new byte[] {0x87 ,0x18 };  //低位元
            ushort accaddressH = BitConverter.ToUInt16(accSpeedAddressH, 0);
            ushort accaddressL = BitConverter.ToUInt16(accSpeedAddressL, 0);
            ushort accValueH = BitConverter.ToUInt16(accspeedByte, 2);
            ushort accValueL = BitConverter.ToUInt16(accspeedByte, 0);

            serialMasterDevice.WriteSingleRegister(slaveAddress, accaddressH, Convert.ToUInt16(accValueH));
            serialMasterDevice.WriteSingleRegister(slaveAddress, accaddressL, Convert.ToUInt16(accValueL));



            //減速度由 32位元組成  [88,89] 但1次只能寫16位元 所以要分兩次下參數
            byte[] decSpeedAddressH = new byte[] { 0x88, 0x18 };//高位元
            byte[] decSpeedAddressL = new byte[] { 0x89, 0x18 };//低位元
            ushort decaddressH = BitConverter.ToUInt16(decSpeedAddressH, 0);
            ushort decaddressL = BitConverter.ToUInt16(decSpeedAddressL, 0);
            ushort decValueH = BitConverter.ToUInt16(decspeedByte, 2);
            ushort decValueL = BitConverter.ToUInt16(decspeedByte, 0);
            serialMasterDevice.WriteSingleRegister(slaveAddress, decaddressH, Convert.ToUInt16(decValueH));
            serialMasterDevice.WriteSingleRegister(slaveAddress, decaddressL, Convert.ToUInt16(decValueL));
        }
        private void CommandReset()
        {
            serialMasterDevice.WriteSingleRegister(slaveAddress, 0x7D, 0x00);
        }

        private int GetPosition()
        {
            var datas = serialMasterDevice.ReadHoldingRegisters(slaveAddress, 0xCC, 0x02);

            var pos = datas[0] * 65536 + datas[1];

            return pos;
        }
     
        private void ResetAlarmCode()
        {

            serialMasterDevice.WriteSingleRegister(slaveAddress, 384, 12480);
            serialMasterDevice.WriteSingleRegister(slaveAddress, 385, 12480);

        }
        private int GetAlarmCode()
        {
            //回傳  01 03 02 00 31 79 90   CODE在陣列4的位置 例30 過負載 ， 31超速   P425頁
            var temp = serialMasterDevice.ReadHoldingRegisters(slaveAddress, 0x81, 0x01);



            return temp[4];
        }

        private void Stop()
        {
            try
            {
                if (serialMasterDevice == null) throw new Exception("Device is null") ;
    

                serialMasterDevice.WriteSingleRegister(slaveAddress, 0x7D, 0x20);

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public bool GetInputCommand(int id)
        {
            throw new NotImplementedException();
        }
    }

    public enum OperationMode
    {
        Relative,
        Absolute


    }
}
