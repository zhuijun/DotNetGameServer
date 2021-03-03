using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Repository.Models.Core
{
    /// <summary>
    /// ID 基础实体类
    /// </summary>
    public abstract class IdEntity : Entity
    {
        /// <summary>
        /// 唯一标识 ID
        /// </summary>
        [Browsable(false)]
        [MaxLength(50)]
        public string Id { get; set; }

        /// <summary>
        /// ID 基础实体类
        /// </summary>
        public IdEntity()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
