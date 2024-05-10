using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuanliCore.Model.Interface.Component
{
    public interface IReader
    {
        /// <summary>
        /// 讀取的圖片
        /// </summary>
        System.Drawing.Bitmap Image { get; }
        /// <summary>
        /// 分數0~100
        /// </summary>
        double Score { get; }
        /// <summary>
        /// 初始化
        /// </summary>
        void Initial();
        /// <summary>
        /// 關閉
        /// </summary>
        void Close();
        /// <summary>
        /// 設定參數
        /// </summary>
        /// <param name="paramID"></param>
        void SetParam(int paramID);
        /// <summary>
        /// Reader讀取
        /// </summary>
        /// <returns></returns>
        Task<string> ReadAsync();
    }
}
