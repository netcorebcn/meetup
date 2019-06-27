using System;

namespace Meetup.Api
{
    public class OutboxEvent
    {
        public Guid Id { get; set; }
        public Guid StreamId { get; set; }
        public object Data { get; set; }
        public string Type { get; set; }
    }
}