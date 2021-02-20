using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace AccountServer.Models
{
    public class UserAccount
    {
        public string Id { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [Description("用户Id")]
        public long UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Description("用户名")]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Description("密码")]
        public string Password { get; set; }

        /// <summary>
        /// 用户Token
        /// </summary>
        [Description("用户Token")]
        public string Token { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [Description("用户昵称")]
        public string NickName { get; set; }

        /// <summary>
        /// 用户头像URL
        /// </summary>
        [Description("用户头像URL")]
        public string HeadIcon { get; set; }

        /// <summary>
        /// 0--普通玩家, 1--代理玩家, 2--游客玩家
        /// </summary>
        [Description("0--普通玩家, 1--代理玩家, 2--游客玩家")]
        public short UserType { get; set; }
    }
}
