using Microsoft.EntityFrameworkCore;
using Repository.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    [Comment("大转盘记录表")]
    public class GameTruntable : IdEntity
    {
        [Comment("角色Id")]
        public long RoleId { get; set; }

        [Comment("奖励Id")]
        [MaxLength(50)]
        public string AwardId { get; set; }

        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateTime { get; set; }

        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpateTime { get; set; }

    }
}
