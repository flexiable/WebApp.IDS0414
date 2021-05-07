using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace IDS.Entity
{
    public interface ITrack
    {

        /// <summary>
        /// 创建时间
        /// </summary>
        [DisplayName("创建时间")]
        DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [DisplayName("更新时间")]
        DateTime? ModifyDateTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [DisplayName("是否删除")]
        bool IsDelete { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        [DisplayName("删除时间")]
        DateTime? DeleteTime { get; set; }
    }
}
