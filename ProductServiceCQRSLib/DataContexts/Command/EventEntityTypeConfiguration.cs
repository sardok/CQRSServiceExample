using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using ProductServiceCQRSLib.Models.Command;

namespace ProductServiceCQRSLib.DataContexts.Command
{
    public class EventEntityTypeConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.Property(@event => @event.Seq)
                .ValueGeneratedOnAdd();
            builder.Property(@event => @event.Timestamp)
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(@event => @event.Data)
                .HasConversion(
                eventData => JsonConvert.SerializeObject(eventData),
                entry => JsonConvert.DeserializeObject<EventData>(entry));
        }
    }

}