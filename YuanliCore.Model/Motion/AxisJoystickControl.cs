using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using YuanliCore.Motion;


namespace YuanliCore.Motion
{
    public class AxisJoystickControl : IDisposable
    {

        #region private

        private DirectInput directInput;
        private Joystick joystick;

        private CancellationTokenSource cancellationTokenSource;
        private bool[] buttonState = new bool[12];

        //X軸
        private Axis axisX;
        //Y軸
        private Axis axisY;
        //Z軸
        private Axis axisZ;

        //左右反轉
        private bool reverseRL;
        //上下反轉
        private bool reverseUD;
        //Z軸反轉
        private bool reverseZ;

        /// <summary>
        /// X軸速度
        /// </summary>
        private double xMaxVel;

        /// <summary>
        /// Y軸速度
        /// </summary>
        private double yMaxVel;

        /// <summary>
        /// Z軸速度
        /// </summary>
        private double zMaxVel;

        //移動模式
        private moveMode MoveMode;

        //高速模式
        private bool highSpeedMode;

        #endregion private

        #region public

        /// <summary>
        /// 啟用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 按鈕事件
        /// </summary>
        public delegate void ButtonEventHandler();
        /// <summary>
        /// 按鈕1按下事件
        /// </summary>
        public event ButtonEventHandler Button1Down;
        /// <summary>
        /// 按鈕1放開事件
        /// </summary>
        public event ButtonEventHandler Button1Up;
        /// <summary>
        /// 按鈕2按下事件
        /// </summary>
        public event ButtonEventHandler Button2Down;
        /// <summary>
        /// 按鈕2放開事件
        /// </summary>
        public event ButtonEventHandler Button2Up;
        /// <summary>
        /// 按鈕3按下事件
        /// </summary>
        public event ButtonEventHandler Button3Down;
        /// <summary>
        /// 按鈕3放開事件
        /// </summary>
        public event ButtonEventHandler Button3Up;
        /// <summary>
        /// 按鈕4按下事件
        /// </summary>
        public event ButtonEventHandler Button4Down;
        /// <summary>
        /// 按鈕4放開事件
        /// </summary>
        public event ButtonEventHandler Button4Up;
        /// <summary>
        /// 按鈕5按下事件
        /// </summary>
        public event ButtonEventHandler Button5Down;
        /// <summary>
        /// 按鈕5放開事件
        /// </summary>
        public event ButtonEventHandler Button5Up;
        /// <summary>
        /// 按鈕6按下事件
        /// </summary>
        public event ButtonEventHandler Button6Down;
        /// <summary>
        /// 按鈕6放開事件
        /// </summary>
        public event ButtonEventHandler Button6Up;
        /// <summary>
        /// 按鈕7按下事件
        /// </summary>
        public event ButtonEventHandler Button7Down;
        /// <summary>
        /// 按鈕7放開事件
        /// </summary>
        public event ButtonEventHandler Button7Up;
        /// <summary>
        /// 按鈕8按下事件
        /// </summary>
        public event ButtonEventHandler Button8Down;
        /// <summary>
        /// 按鈕8放開事件
        /// </summary>
        public event ButtonEventHandler Button8Up;
        /// <summary>
        /// 按鈕9按下事件
        /// </summary>
        public event ButtonEventHandler Button9Down;
        /// <summary>
        /// 按鈕9放開事件
        /// </summary>
        public event ButtonEventHandler Button9Up;
        /// <summary>
        /// 按鈕10按下事件
        /// </summary>
        public event ButtonEventHandler Button10Down;
        /// <summary>
        /// 按鈕10放開事件
        /// </summary>
        public event ButtonEventHandler Button10Up;
        /// <summary>
        /// 按鈕11按下事件
        /// </summary>
        public event ButtonEventHandler Button11Down;
        /// <summary>
        /// 按鈕11放開事件
        /// </summary>
        public event ButtonEventHandler Button11Up;
        /// <summary>
        /// 按鈕12按下事件
        /// </summary>
        public event ButtonEventHandler Button12Down;
        /// <summary>
        /// 按鈕12放開事件
        /// </summary>
        public event ButtonEventHandler Button12Up;

        /// <summary>
        /// 回傳搖桿XY位置事件
        /// </summary>
        /// <param name="x">X百分比(0~1)</param>
        /// <param name="y">Y百分比(0~1)</param>
        public delegate void XYChangedEventHandler(moveMode MoveMode,bool highSpeed);
        public event XYChangedEventHandler XYZChanged;

        #endregion public

        /// <summary>
        /// 搖桿建構式
        /// </summary>
        /// <param name="reverseRL">左右反轉</param>
        /// <param name="reverseUD">上下反轉</param>
        /// <param name="reverseZ">Z軸反轉</param>
        public AxisJoystickControl(bool reverseRL = false, bool reverseUD = false, bool reverseZ = false)
        {
            this.reverseRL = reverseRL;
            this.reverseUD = reverseUD;
            this.reverseZ = reverseZ;

            xMaxVel = 0;
            yMaxVel = 0;
            zMaxVel = 0;

            InitDirectInput();
            StartCatchAxis();
            StartCatchButton();

            Enable = true;
        }

        /// <summary>
        /// 搖桿建構式
        /// </summary>
        /// <param name="xAxis">X軸</param>
        /// <param name="yAxis">Y軸</param>
        /// <param name="zAxis">Z軸</param>
        /// <param name="reverseRL">左右反轉</param>
        /// <param name="reverseUD">上下反轉</param>
        /// <param name="reverseZ">Z軸反轉</param>
        public AxisJoystickControl(Axis xAxis, Axis yAxis, Axis zAxis, bool reverseRL = false, bool reverseUD = false, bool reverseZ = false)
        {
            this.reverseRL = reverseRL;
            this.reverseUD = reverseUD;
            this.reverseZ = reverseZ;

            this.axisX = xAxis;
            this.axisY = yAxis;
            this.axisZ = zAxis;

            if (axisX != null)

                xMaxVel = axisX.AxisVelocity.MaxVel;
            if (axisY != null)
                yMaxVel = axisY.AxisVelocity.MaxVel;
            if (axisZ != null)
                zMaxVel = axisZ.AxisVelocity.MaxVel;

            InitDirectInput();
            StartCatchAxis();
            StartCatchButton();

            Enable = true;
        }

        /// <summary>
        /// 搖桿初始化
        /// </summary>
        private void InitDirectInput()
        {
            directInput = new DirectInput();
            var joystickGuid = Guid.Empty;

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
            {
                joystickGuid = deviceInstance.InstanceGuid;
            }

            if (joystickGuid == Guid.Empty)
            {
                foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
                {
                    joystickGuid = deviceInstance.InstanceGuid;
                }
            }

            if (joystickGuid != Guid.Empty)
            {
                joystick = new Joystick(directInput, joystickGuid);
                joystick.Properties.BufferSize = 128;
                joystick.Acquire();
            }
        }

        /// <summary>
        /// 軸執行緒
        /// </summary>
        private void StartCatchAxis()
        {
            MoveMode = moveMode.Stop;

            if (cancellationTokenSource == null)
                cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            Task.Run(async () =>
            {
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        if (joystick == null) continue;

                        // 取得搖桿資訊
                        joystick.Poll();
                        var datas = joystick.GetCurrentState();

                        // 搖桿控制軸移動
                        ProcessJoystickPosition(datas.X, datas.Y, datas.Z);


                        await Task.Delay(10, token); // Update every 10 milliseconds
                    }
                }
                catch (TaskCanceledException)
                {
                    // Task was canceled, ignore the exception
                }
            }, token);

        }

        /// <summary>
        /// 按鈕執行緒
        /// </summary>
        private void StartCatchButton()
        {
            if (cancellationTokenSource == null)
                cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            Task.Run(async () =>
            {
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        if (joystick == null) continue;

                        // 取得搖桿資訊
                        joystick.Poll();
                        var datas = joystick.GetCurrentState();

                        // 取得按鈕狀態
                        var buttons = datas.Buttons;

                        // 按鈕矩陣迴圈
                        for (int i = 0; i < buttonState.Length; i++)
                        {
                            if (buttons.Length > i)
                                UpdateButtonState(buttons[i], i); //更新按鈕
                        }

                        await Task.Delay(10, token); // Update every 10 milliseconds
                    }
                }
                catch (TaskCanceledException)
                {
                    // Task was canceled, ignore the exception
                }
            }, token);

        }

        /// <summary>
        /// 搖桿控制軸移動
        /// </summary>
        /// <param name="x">搖桿X值</param>
        /// <param name="y">搖桿Y值</param>
        /// <param name="z">搖桿Z值</param>
        /// <returns></returns>
        private void ProcessJoystickPosition(int x, int y, int z)
        {

            const int maxthreshold = 65535 / 2; //X、Y、Z值為0~65535，maxthreshold為中間值
            const int threshold1 = maxthreshold / 15; //threshold1為15分之1的值，小於這個移動量當作不移動
            const int threshold2 = maxthreshold * 2 / 3; //threshold2為3分之2的值，大於這個移動量作為高速移動

            //計算到中點的距離
            double dx = x - maxthreshold;
            double dy = maxthreshold - y;
            double dz = z - maxthreshold;

            //反轉
            if (reverseRL)
            {
                dx = -dx;
            }

            if (reverseUD)
            {
                dy = -dy;
            }

            if (reverseZ)
            {
                dz = -dz;
            }

            //計算角度
            double angle = Math.Atan2(dy, dx) * 180 / Math.PI;

            //計算距離
            double distance = Math.Sqrt(dx * dx + dy * dy);

            //bool nowHighSpeed= distance > threshold2;
            //if (highSpeedMode != nowHighSpeed)
            //{
            //    highSpeedMode = nowHighSpeed;
            //    //距離超過切為高速模式
            //    SwitchHighSpeed(highSpeedMode);
            //    RunMove();
            //}

            if(!Enable)
            {
                //不啟動將軸停止
                if (MoveMode != moveMode.Stop)
                {
                    MoveMode = moveMode.Stop;
                    RunMove();
                }
                return;
            }


            if (MoveMode == moveMode.Stop)
            {
                //停止模式改變的時候
                if ((Math.Abs(dx) >= threshold1 || Math.Abs(dy) >= threshold1)) //搖桿的X或Y超過臨界值
                {
                    //正右方為0度，上半圓為負，下半圓為正。
                    if (angle >= -22.5 && angle < 22.5)
                    {
                        MoveMode = moveMode.Right;
                    }
                    else if (angle >= 22.5 && angle < 67.5)
                    {
                        MoveMode = moveMode.UpRight;
                    }
                    else if (angle >= 67.5 && angle < 112.5)
                    {
                        MoveMode = moveMode.Up;
                    }
                    else if (angle >= 112.5 && angle < 157.5)
                    {
                        MoveMode = moveMode.UpLeft;
                    }
                    else if (angle >= 157.5 || angle < -157.5)
                    {
                        MoveMode = moveMode.Left;
                    }
                    else if (angle >= -157.5 && angle < -112.5)
                    {
                        MoveMode = moveMode.DownLeft;
                    }
                    else if (angle >= -112.5 && angle < -67.5)
                    {
                        MoveMode = moveMode.Down;
                    }
                    else if (angle >= -67.5 && angle < -22.5)
                    {
                        MoveMode = moveMode.DownRight;

                    }

                    RunMove();

                }
                else if (Math.Abs(dz) >= threshold1) //搖桿的Z超過臨界值
                {
                    if (dz > 0)
                    {
                        //Z軸前進
                        MoveMode = moveMode.ZPlus;
                    }
                    else
                    {
                        //Z軸後退
                        MoveMode = moveMode.ZMinus;
                    }

                    RunMove();
                }
            }
            else
            {
                //移動模式改變的時候
                if (Math.Abs(dx) < threshold1 && Math.Abs(dy) < threshold1 && Math.Abs(dz) < threshold1)
                {
                    MoveMode = moveMode.Stop;
                    RunMove();
                }
            }



        }
        private void RunMove()
        {
            XYZChanged?.Invoke(MoveMode, highSpeedMode);

            switch (MoveMode)
            {
                case moveMode.Stop:
                    XYStop();
                    ZStop();
                    break;
                case moveMode.ZPlus:
                    ZMoveForward();
                    break;
                case moveMode.ZMinus:
                    ZMoveReverse();
                    break;
                case moveMode.Right:
                    MoveRight();
                    break;
                case moveMode.UpRight:
                    MoveUpRight();
                    break;
                case moveMode.Up:
                    MoveUp();
                    break;
                case moveMode.UpLeft:
                    MoveUpLeft();
                    break;
                case moveMode.Left:
                    MoveLeft();
                    break;
                case moveMode.DownLeft:
                    MoveDownLeft();
                    break;
                case moveMode.Down:
                    MoveDown();
                    break;
                case moveMode.DownRight:
                    MoveDownRight();
                    break;
            }
        }


        /// <summary>
        /// Z軸停止
        /// </summary>
        private void ZStop()
        {
            if (axisZ != null)
            {
                axisZ.Stop();
            }
        }
        /// <summary>
        /// Z軸前進
        /// </summary>
        /// <returns></returns>
        private void ZMoveForward()
        {
            if (axisZ != null)
            {
                axisZ.MoveToAsync(axisZ.PositionPEL);
            }
        }
        /// <summary>
        /// Z軸後退
        /// </summary>
        /// <returns></returns>
        private void ZMoveReverse()
        {
            if (axisZ != null)
            {
                axisZ.MoveToAsync(axisZ.PositionNEL);
            }
        }
        /// <summary>
        /// XY軸停止
        /// </summary>
        private void XYStop()
        {
            //X軸停止
            if (axisX != null)
            {
                axisX.Stop();
            }

            //Y軸停止
            if (axisY != null)
            {
                axisY.Stop();
            }
        }

        /// <summary>
        /// 改變軸速度模式
        /// </summary>
        /// <param name="highspeed">高速模式</param>
        private void SwitchHighSpeed(bool highspeed)
        {
            if (highspeed) //高速模式
            {
                if (axisX != null)
                    axisX.AxisVelocity.MaxVel = xMaxVel;
                if (axisY != null)
                    axisY.AxisVelocity.MaxVel = yMaxVel;
            }
            else //低速模式
            {
                if (axisX != null)
                    axisX.AxisVelocity.MaxVel = xMaxVel / 2;
                if (axisY != null)
                    axisY.AxisVelocity.MaxVel = yMaxVel / 2;
            }

        }


        private void MoveRight()
        {
            if (axisX != null)
            {
                axisX.MoveToAsync(axisX.PositionPEL);
            }
        }

        private void MoveUpRight()
        {
            if (axisX != null)
            {
                Task.WhenAll(axisX.MoveToAsync(axisX.PositionPEL), axisY.MoveToAsync(axisY.PositionPEL));
            }
        }

        private void MoveUp()
        {
            if (axisX != null)
            {
                axisY.MoveToAsync(axisY.PositionPEL);
            }
        }

        private void MoveUpLeft()
        {
            if (axisX != null)
            {
                Task.WhenAll(axisX.MoveToAsync(axisX.PositionNEL), axisY.MoveToAsync(axisY.PositionPEL));
            }
        }

        private void MoveLeft()
        {
            if (axisX != null)
            {
                axisX.MoveToAsync(axisX.PositionNEL);
            }
        }
        private void MoveDownLeft()
        {
            if (axisX != null)
            {
                Task.WhenAll(axisX.MoveToAsync(axisX.PositionNEL), axisY.MoveToAsync(axisY.PositionNEL));
            }
        }
        private void MoveDown()
        {
            if (axisX != null)
            {
                axisY.MoveToAsync(axisY.PositionNEL);
            }
        }
        private void MoveDownRight()
        {
            if (axisX != null)
            {
                Task.WhenAll(axisX.MoveToAsync(axisX.PositionPEL), axisY.MoveToAsync(axisY.PositionNEL));
            }
        }




        /// <summary>
        /// 改變按鈕狀態
        /// </summary>
        /// <param name="isPressed">按壓狀態</param>
        /// <param name="index">改變的按鈕編號</param>
        private void UpdateButtonState(bool isPressed, int index)
        {
            if (index >= buttonState.Length)
            {
                return;
            }

            if (isPressed && !buttonState[index])
            {
                buttonState[index] = true;
                InvokeButtonDownEvent(index);
            }
            else if (!isPressed && buttonState[index])
            {
                buttonState[index] = false;
                InvokeButtonUpEvent(index);
            }
        }

        private void InvokeButtonDownEvent(int index)
        {
            switch (index)
            {
                case 0:
                    Button1Down?.Invoke();
                    break;
                case 1:
                    Button2Down?.Invoke();
                    break;
                case 2:
                    Button3Down?.Invoke();
                    break;
                case 3:
                    Button4Down?.Invoke();
                    break;
                case 4:
                    Button5Down?.Invoke();
                    break;
                case 5:
                    Button6Down?.Invoke();
                    break;
                case 6:
                    Button7Down?.Invoke();
                    break;
                case 7:
                    Button8Down?.Invoke();
                    break;
                case 8:
                    Button9Down?.Invoke();
                    break;
                case 9:
                    Button10Down?.Invoke();
                    break;
                case 10:
                    Button11Down?.Invoke();
                    break;
                case 11:
                    Button12Down?.Invoke();
                    break;
            }
        }

        private void InvokeButtonUpEvent(int index)
        {
            switch (index)
            {
                case 0:
                    Button1Up?.Invoke();
                    break;
                case 1:
                    Button2Up?.Invoke();
                    break;
                case 2:
                    Button3Up?.Invoke();
                    break;
                case 3:
                    Button4Up?.Invoke();
                    break;
                case 4:
                    Button5Up?.Invoke();
                    break;
                case 5:
                    Button6Up?.Invoke();
                    break;
                case 6:
                    Button7Up?.Invoke();
                    break;
                case 7:
                    Button8Up?.Invoke();
                    break;
                case 8:
                    Button9Up?.Invoke();
                    break;
                case 9:
                    Button10Up?.Invoke();
                    break;
                case 10:
                    Button11Up?.Invoke();
                    break;
                case 11:
                    Button12Up?.Invoke();
                    break;
            }
        }

        public void Dispose()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            joystick?.Unacquire();
            joystick?.Dispose();
            directInput?.Dispose();
        }


        public enum moveMode
        {
            Stop = 0,
            ZPlus = 1, ZMinus = 2,
            Right = 3, UpRight = 4, Up = 5, UpLeft = 6, Left = 7, DownLeft = 8, Down = 9, DownRight = 10
        }
    }
}
