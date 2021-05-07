using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.Entity
{
    public class Entity : Entity<string>, IEntity
    {
    }

    public class Entity<TPrimaryKey> : IEntity<TPrimaryKey> 
    {
        public TPrimaryKey ZId { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime? ModifyDateTime { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? DeleteTime { get; set; }
      
    }
}
