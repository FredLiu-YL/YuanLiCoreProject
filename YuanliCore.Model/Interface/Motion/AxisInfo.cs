using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Motion;

namespace YuanliCore.Interface.Motion
{
    public class AxisInfo
    {
        /// <summary>
        /// 運動軸在卡的號碼
        /// </summary>
        public int AxisID { get; set; }
        /// <summary>
        /// 運動軸名稱
        /// </summary>
        public string AxisName { get; set; }
        /// <summary>
        /// 運動軸速度
        /// </summary>
        public VelocityParams Velocity { get; set; }
  
       
        /// <summary>
        /// 運動軸方向
        /// </summary>
        public AxisDirection AxisDir { get; set; }

    }


}
