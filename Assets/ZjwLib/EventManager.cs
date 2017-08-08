using System.Collections.Generic;
namespace Events
{
    public class Event<T>
    {
        public static void AddListener(EventAction<T> handler, EventManager manager = null)
        {
            EventManager.AddListener(handler, manager);
        }

        public static void AddListener(IEventHandler<T> handler, EventManager manager = null)
        {
            EventManager.AddListener(handler, manager);
        }

        public static void RemoveListener(EventAction<T> handler, EventManager manager = null)
        {
            EventManager.RemoveListener(handler, manager);
        }

        public static void RemoveListener(IEventHandler<T> handler, EventManager manager = null)
        {
            EventManager.RemoveListener(handler, manager);
        }

        public static void Emit(T evt, EventManager eventManager = null)
        {
            EventManager.Emit(evt, eventManager);
        }
    }


    public static class EventExtension
    {
        public static void Emit<E>(this E evt, EventManager eventManager = null)
        {
            EventManager.Emit(evt, eventManager);
        }
    }
}
namespace Events
{
    public interface IEventHandler
    {

    }

    public interface IEventHandler<T> : IEventHandler
    {
        void HandleEvent(T e);
    }
}
namespace Events
{
    public delegate void EventAction<E>(E e);

    public class EventManager
    {
        static readonly EventManager instance = new EventManager();

        readonly Dictionary<System.Type, System.Delegate> eventListeners = new Dictionary<System.Type, System.Delegate>();

        readonly Dictionary<System.Type, HashSet<IEventHandler>> eventHandlers = new Dictionary<System.Type, HashSet<IEventHandler>>();

        public static EventManager Instance
        {
            get
            {
                return instance;
            }
        }

        private void EmitInternal<T>(T evt)
        {
            var type = typeof(T);

            System.Delegate listener;

            if (eventListeners.TryGetValue(type, out listener) && listener != null)
            {
                var action = (EventAction<T>)listener;
                action(evt);
            }

            HashSet<IEventHandler> eventHandlerSet;
            if (eventHandlers.TryGetValue(type, out eventHandlerSet))
            {
                foreach (var item in eventHandlerSet)
                {
                    ((IEventHandler<T>)item).HandleEvent(evt);
                }
            }
        }

        private void AddListenerInternal<T>(EventAction<T> listener)
        {
            var type = typeof(T);
            System.Delegate listeners;

            if (eventListeners.TryGetValue(type, out listeners) && listener != null)
            {
                var action = (EventAction<T>)listeners;
                action += listener;
                eventListeners[type] = action;
            }
            else
            {
                eventListeners[type] = listener;
            }
        }

        private void RemoveListenerInternal<T>(EventAction<T> listener)
        {
            var type = typeof(T);

            System.Delegate listeners;

            if (eventListeners.TryGetValue(type, out listeners))
            {
                var action = (EventAction<T>)listeners;
                action -= listener;
                eventListeners[type] = action;
            }
        }

        private void AddListenerInternal<T>(IEventHandler<T> handler)
        {
            var type = typeof(T);

            HashSet<IEventHandler> eventHandlerSet;

            if (eventHandlers.TryGetValue(type, out eventHandlerSet))
            {
                eventHandlerSet.Add(handler);
            }
            else
            {
                HashSet<IEventHandler> handlerSet = new HashSet<IEventHandler>
            {
                handler
            };
                eventHandlers.Add(type, handlerSet);
            }
        }

        private void RemoveListenerInternal<T>(IEventHandler<T> handler)
        {
            var type = typeof(T);

            HashSet<IEventHandler> eventHandlerSet;

            if (eventHandlers.TryGetValue(type, out eventHandlerSet))
            {
                eventHandlerSet.Remove(handler);
            }
        }

        public static void AddListener<T>(EventAction<T> handler, EventManager manager = null)
        {
            manager = manager ?? Instance;
            manager.AddListenerInternal(handler);
        }

        public static void AddListener<T>(IEventHandler<T> handler, EventManager manager = null)
        {
            manager = manager ?? Instance;
            manager.AddListenerInternal(handler);
        }

        public static void RemoveListener<T>(EventAction<T> handler, EventManager manager = null)
        {
            manager = manager ?? Instance;
            manager.RemoveListenerInternal(handler);
        }

        public static void RemoveListener<T>(IEventHandler<T> handler, EventManager manager = null)
        {
            manager = manager ?? Instance;
            manager.RemoveListenerInternal(handler);
        }

        public static void Emit<T>(T evt, EventManager manager = null)
        {
            manager = manager ?? instance;
            manager.EmitInternal(evt);
        }
    }
}