//------------------------------------------------------------------------------
//
//
//
//
//
//
//
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HttpServer.Request
{
    /// <summary>
	/// 
	/// </summary>
    public partial class RecordUserPointDetailReq
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 运算符类型：0加、1减
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 增减积分值
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string Sign { get; set; }
    }
}