using System;
using System.ComponentModel;

namespace IDS.Entity
{
    public interface IEntity : IEntity<string>
    {
    }

    public interface IEntity<TPrimaryKey> : ITrack
    {
        [DisplayName("ID")]
        public TPrimaryKey ZId { get; set; }
    }
}
