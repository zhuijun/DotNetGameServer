using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace AccountServer.Models
{
    public class UserAccountWx
    {
        public string Id { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [Description("用户Id")]
        public long UserId { get; set; }

        /// <summary>
        /// OpenId
        /// </summary>
        [Description("OpenId")]
        public string OpenId { get; set; }

        /// <summary>
        /// UnionId
        /// </summary>
        [Description("UnionId")]
        public string UnionId { get; set; }
    }
}
