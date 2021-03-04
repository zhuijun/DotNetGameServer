using Microsoft.EntityFrameworkCore;
using Repository.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Repository.Models
{
    [Index(nameof(FruitId), IsUnique = true)]
    [Comment("合成大西瓜水果配置表")]
    public class FruitConfig : IdEntity
    {
        [Comment("水果Id")]
        public int FruitId { get; set; }

        [Comment("概率")]
        public int Rate { get; set; }

        [Comment("名称")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Comment("图片")]
        [MaxLength(50)]
        public string Image { get; set; }

        [Comment("合成可得的积分")]
        public int Score { get; set; }

        [Comment("合成后的水果Id")]
        public int CombineFruitId { get; set; }
    }
}
