using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuanliCore.Motion
{
    [Serializable]
    public class VelocityParams
    {
        public VelocityParams() { }

        public VelocityParams(double max, double accTime = 0.2)
            : this(0, max, accTime, accTime)
        { }

        [JsonConstructor]
        public VelocityParams(double start, double max, double accTime = 0.2, double decTime = 0.2)
        {
            InitialVel = start;
            MaxVel = max;
            AccelerationTime = accTime;
            DecelerationTime = decTime;
        }

        public static implicit operator VelocityParams(double maxVel)
            => new VelocityParams(maxVel);

        public static VelocityParams operator /(VelocityParams vel, double factor)
            => new VelocityParams(
             vel.InitialVel / factor,
             vel.MaxVel / factor,
             vel.AccelerationTime,
             vel.DecelerationTime)
            {
                JerkAcceleration = vel.JerkAcceleration,
                JerkDeceleration = vel.JerkDeceleration
            };

        public static bool operator ==(VelocityParams vel1, VelocityParams vel2)
        {
            if (vel1 is null && vel2 is null) return true;
            if (vel1 is null || vel2 is null) return false;

            return
                vel1.InitialVel == vel2.InitialVel &&
                vel1.AccelerationTime == vel2.AccelerationTime &&
                vel1.DecelerationTime == vel2.DecelerationTime &&
                vel1.MaxVel == vel2.MaxVel;
        }

        public static bool operator !=(VelocityParams vel1, VelocityParams vel2)
        {
            if (vel1 is null && vel2 is null) return false;
            if (vel1 is null || vel2 is null) return true;

            return
                vel1.InitialVel != vel2.InitialVel ||
                vel1.AccelerationTime != vel2.AccelerationTime ||
                vel1.DecelerationTime != vel2.DecelerationTime ||
                vel1.MaxVel != vel2.MaxVel;
        }

        /// <summary>
        /// 取得或設定加減速時的曲線型式。
        /// </summary>
        public VelCurves Curve { get; set; } = VelCurves.T;

        /// <summary>
        /// 取得或設定初速度。
        /// </summary>
        public double InitialVel { get; set; } = 0;

        /// <summary>
        /// 取得或設定最大速度。
        /// </summary>
        public double MaxVel { get; set; } = 10000;

        /// <summary>
        /// 取得或設定由初速加速至最高速的加速時間。
        /// </summary>
        public double AccelerationTime { get; set; } = 0.2;

        /// <summary>
        /// 取得或設定由最高速減速至初速的減速時間。
        /// </summary>
        public double DecelerationTime { get; set; } = 0.2;

        /// <summary>
        /// 取得或設定複合加加速度。
        /// </summary>
        public double JerkAcceleration { get; set; } = 1;

        /// <summary>
        /// 取得或設定複合減減速度。
        /// </summary>
        public double JerkDeceleration { get; set; } = 1;
    }

    public enum VelCurves
    {
        S = 0,

        T = 1,
    }

    /* public static partial class goonØ
     {
         public static double GetAcceleration(this VelocityParams vel)
             => (vel.FinalVel - vel.InitialVel) / vel.AccelerationTime;

         public static double GetDeceleration(this VelocityParams vel)
             => (vel.FinalVel - vel.InitialVel) / vel.DecelerationTime;
     }*/
}
