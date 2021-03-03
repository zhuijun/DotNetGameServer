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
    [Comment("游戏宝箱表")]
    public class GameBox : IdEntity
    {
        [Comment("角色Id")]
        public long RoleId { get; set; }

        [Comment("优惠券Id")]
        [MaxLength(50)]
        public string CouponsId { get; set; }

        [Comment("优惠金额")]
        public long Amount { get; set; }

        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateTime { get; set; }

        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpateTime { get; set; }
    }
}
