using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using System.Text;

namespace Meetup.Api
{
    internal class EventDeserializer
    {

        private readonly IDictionary<string, Type> _typeMap;

        public EventDeserializer(Assembly eventsAssembly)
        {
            _typeMap = eventsAssembly
                .GetExportedTypes()
                .Where(x => x.Name.EndsWith("Event", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(x => x.Name);
        }

        private Type GetTypeForEventName(string name)
        {
            Type eventType;
            var strippedName = name.Replace(" ", string.Empty);
            if (!_typeMap.TryGetValue(strippedName, out eventType))
            {
                throw new ArgumentException($"Unable to find suitable type for event name {name}. Expected to find a type named {strippedName}Event");
            }

            return eventType;
        }

        public object Deserialize(RecordedEvent evt)
        {
            var targetType = GetTypeForEventName(evt.EventType);
            var json = Encoding.UTF8.GetString(evt.Data);
            return JsonConvert.DeserializeObject(json, targetType);
        }
    }
}