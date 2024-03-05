using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuanliCore.Machine.Base
{
    public enum MachineStates
    {
        IDLE,
        RUNNING,
        PAUSED,//對位異常
        Alarm,//(紅色 紅燈) 真空異常 上船失敗 等...
        Emergency//(紅色 紅燈)緊急停止 、軸卡異常 不能往下做，需重開設備
    }
}
