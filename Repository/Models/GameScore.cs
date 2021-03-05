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
    [Comment("游戏积分表")]
    public class GameScore : IdEntity
    {
        [Comment("角色Id")]
        public long RoleId { get; set; }

        [Comment("积分")]
        public long Score { get; set; }

        [DataType(DataType.DateTime)]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateTime { get; set; }

        [DataType(DataType.DateTime)]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpateTime { get; set; }

    }
}
