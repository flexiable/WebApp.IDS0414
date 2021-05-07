using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace IDS.Entity
{
    public class ClientTokenLog : Entity
    {
        public ClientTokenLog()
        {
            ZId = Guid.NewGuid().ToString();
        }
        [DisplayName("客户端ID")]
        public string ClientId { get; set; }
        [DisplayName("客户端名称")]
        public string ClientName { get; set; }
        [DisplayName("客户端请求参数")]
        public string ClientRequestParam { get; set; }
        [DisplayName("客户端返回参数")]
        public string ClientResponseBody { get; set; }
        [DisplayName("客户端用户ID")]
        public string UserId { get; set; }
        [DisplayName("客户端使用状态")]
        public bool Enabled
        { get; set; }

        [DisplayName("客户端token有效时长")]
        public int? AccessTokenLifetime { get; set; }
        public string LocalIpAddress { get; set; }
        public string RemoteIpAddress { get; set; }
    }
}
