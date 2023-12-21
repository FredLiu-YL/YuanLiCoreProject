using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuanliCore.Motion
{
    //暫不使用
    public struct MotionVelocity
    {
    
        public MotionVelocity(double initialVel, double fainalVelocity, double accVelocityTime, double decVelocityTime)
        {
            InitialVelocity =  initialVel;
            FainalVelocity = fainalVelocity;
            AccVelocityTime = accVelocityTime;
            DecVelocityTime = decVelocityTime;
        }
        /// <summary>
        /// 運動軸初速度
        /// </summary>
        public double InitialVelocity { get; set; }
        /// <summary>
        /// 運動軸最高速度
        /// </summary>
        public double FainalVelocity { get; set; }
        /// <summary>
        /// 運動軸加速度 到最高速的時間
        /// </summary>
        public double AccVelocityTime { get; set; }
        /// <summary>
        /// 運動軸減速度 到0的時間
        /// </summary>
        public double DecVelocityTime { get; set; }
    }
}
