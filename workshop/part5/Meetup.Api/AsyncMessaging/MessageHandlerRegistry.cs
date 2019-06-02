using System;
using System.Collections;
using System.Collections.Generic;

namespace Meetup.Api
{
    public class MessageHandlerRegistry : IEnumerable<KeyValuePair<Type, List<Type>>>
    {
        private readonly Dictionary<Type, List<Type>> _observers = new Dictionary<Type, List<Type>>();

        public IEnumerable<Type> Get<TRequest>()
        {
            var observed = _observers.ContainsKey(typeof(TRequest));
            return observed ? _observers[typeof(TRequest)] : new List<Type>();
        }

        public void Add(Type requestType, Type handlerType)
        {
            var observed = _observers.ContainsKey(requestType);
            if (!observed)
                _observers.Add(requestType, new List<Type> { handlerType });
            else
                _observers[requestType].Add(handlerType);
        }

        public IEnumerator<KeyValuePair<Type, List<Type>>> GetEnumerator() => _observers.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
