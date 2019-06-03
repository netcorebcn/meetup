using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using System.Text;

namespace Meetup.Api
{
    public class EventDeserializer
    {
        private IDictionary<string, Type> _typeMap;

        public void WithEventsFrom(Assembly eventsAssembly)
        {
            _typeMap = eventsAssembly
                .GetExportedTypes()
                .Where(x => x.FullName.Contains("Event", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(x => x.Name);
        }
        public object Deserialize(RecordedEvent evt)
        {
            var targetType = GetTypeForEventName(evt.EventType);
            var json = Encoding.UTF8.GetString(evt.Data);
            return JsonConvert.DeserializeObject(json, targetType);
        }

        private Type GetTypeForEventName(string name) =>
            _typeMap.TryGetValue(name.Replace(" ", string.Empty), out Type eventType)
            ? eventType
            : throw new ArgumentException($"Unable to find suitable type for event name {name}");
    }
}