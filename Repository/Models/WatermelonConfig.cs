using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Models
{
    [Comment("合成大西瓜配置表")]
    public class WatermelonConfig
    {
        public Guid Id { get; set; }

        [Comment("水果Id")]
        public int FruitId { get; set; }

        [Comment("概率")]
        public int Rate { get; set; }

        [Comment("名称")]
        public string Name { get; set; }

        [Comment("图片")]
        public string Image { get; set; }

        [Comment("合成可得的积分")]
        public int Score { get; set; }
    }
}
