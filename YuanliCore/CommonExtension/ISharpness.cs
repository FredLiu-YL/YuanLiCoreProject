using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuanliCore.Interface
{
   public  interface ISharpness
    {
    }

    public class CogSharpnessResult
    {
        public CogSharpnessResult( double score)
        {
            Score = score;

        }
        public double Score { get; }

     
    }
}
