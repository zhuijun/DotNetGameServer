using Microsoft.EntityFrameworkCore;
using Repository.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Models
{
    [Comment("游戏角色表")]
    public class GameRole : IdEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Comment("角色Id")]
        public long RoleId { get; set; }

        [Required]
        [Comment("用户Id")]
        public long UserId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        [Comment("昵称")]
        public string NickName { get; set; }

        [DataType(DataType.DateTime)]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateTime { get; set; }

        [DataType(DataType.DateTime)]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpateTime { get; set; }
    }
}
