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
    [Comment("合成大西瓜大转盘配置表")]
    public class TruntableConfig : IdEntity
    {
        [Comment("奖励描述")]
        [MaxLength(100)]
        public string AwardDesc { get; set; }

        [Comment("图片路径")]
        [MaxLength(100)]
        public string ImagePath { get; set; }

        [Comment("价值（单位：分）")]
        public long Price { get; set; }

        [Comment("是否开启")]
        public bool IsValid { get; set; }

        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateTime { get; set; }

        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpateTime { get; set; }
    }
}
