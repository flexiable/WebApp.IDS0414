using IDS.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.Mapping
{
    class ClientTokenLogMap : IEntityTypeConfiguration<ClientTokenLog>
    {
        public void Configure(EntityTypeBuilder<ClientTokenLog> builder)
        {
            
            builder.ToTable("ClientTokenLog");
            builder.HasKey(m => m.ZId);
            builder.Property(m => m.ZId).ValueGeneratedOnAdd();
            //builder.Property(m => m.Name);
        }
    }
}
