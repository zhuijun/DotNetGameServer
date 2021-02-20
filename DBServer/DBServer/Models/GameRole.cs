using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DBServer.Models
{
    [Comment("游戏角色表")]
    public class GameRole
    {
        public string Id { get; set; }
        [Comment("角色Id")]
        public long RoleId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Comment("昵称")]
        public string NickName { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpateTime { get; set; }
    }
}
