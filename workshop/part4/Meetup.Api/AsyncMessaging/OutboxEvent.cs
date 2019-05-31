using System;

namespace Meetup.Api
{
    public class OutboxEvent
    {
        public Guid Id { get; set; }
        public Guid StreamId { get; set; }
        public object Data { get; set; }
        public string EventType { get; set; }
        public string ClrEventType { get; set; }
    }
}