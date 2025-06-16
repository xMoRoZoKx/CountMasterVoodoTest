using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace UniTools.Reactive
{
    [System.Serializable]
    public enum CollectionEventType
    {
        Removed,
        Added,
        Replace,
        None
    }

    [System.Serializable]
    public class ReactiveList<T> : List<T>, IReactiveList<T>
    {
        [NonSerialized] UnityEvent<(T, CollectionEventType, int)> _eventsForEach;
        UnityEvent<(T, CollectionEventType, int)> eventsForEach => _eventsForEach ??= new UnityEvent<(T, CollectionEventType, int)>();
        [NonSerialized] UnityEvent<List<T>> _eventStream;
        UnityEvent<List<T>> mainEvent => _eventStream ??= new UnityEvent<List<T>>();
        [NonSerialized] UnityEvent _forceInvoke = new();
        UnityEvent forceInvoked => _forceInvoke ??= new UnityEvent();
        [HideInInspector] public int lastSetedHash = 0;
        public new T this[int index]
        {
            get => base[index];
            set
            {
                if (value == null || value.GetHashCode() != base[index].GetHashCode())
                {
                    base[index] = value;
                    InvokeElementEvents(value, CollectionEventType.Added, index);
                    InvokeListEvents();
                    lastSetedHash = this.GetHashCode();
                }
            }
        }
        private void InvokeListEvents()
        {
            // actionsAndKeys.ForEach(actAndKey => actAndKey.Item1.Invoke(this));
            mainEvent.Invoke(this);
        }
        private void InvokeElementEvents(T value, CollectionEventType type, int idx)
        {
            eventsForEach.Invoke((value, type, idx));
        }
        public new void Add(T item)
        {
            base.Add(item);
            InvokeElementEvents(item, CollectionEventType.Added, Count - 1);
            InvokeListEvents();
        }
        public void AddWithoutNotification(T item)
        {
            base.Add(item);
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            foreach (var element in collection)
            {
                Add(element);
            }
        }
        public bool AddIfNotContains(T element)
        {
            if (!this.Contains(element))
            {
                this.Add(element);
                return true;
            }
            return false;
        }

        public void AddRangeWithoutNotification(IEnumerable<T> collection, bool notificationAfterCompleting = false)
        {
            foreach (var element in collection)
            {
                AddWithoutNotification(element);
            }

            if (notificationAfterCompleting)
                InvokeListEvents();
        }
        public new void Clear()
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                RemoveAt(0);
            }
        }

        public void ClearWithoutNotification(bool notificationAfterCompleting = false)
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                RemoveAtWithoutNotification(0);
            }

            if (notificationAfterCompleting)
                InvokeListEvents();
        }
        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            InvokeElementEvents(base[index], CollectionEventType.Added, index);
            InvokeListEvents();
        }
        public void RemoveAll(Func<T, bool> condition)
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                if (condition(this[i]))
                {
                    RemoveAt(i);
                }
            }
        }
        public new bool Remove(T item)
        {
            var idx = IndexOf(item);
            if (base.Remove(item))
            {
                InvokeElementEvents(item, CollectionEventType.Removed, idx);
                InvokeListEvents();
                return true;
            }
            return false;
        }

        public bool RemoveWithoutNotification(T item)
        {
            return base.Remove(item);
        }

        public new void RemoveAt(int index)
        {
            InvokeElementEvents(base[index], CollectionEventType.Removed, index);
            base.RemoveAt(index);
            InvokeListEvents();
        }

        public void RemoveAtWithoutNotification(int index)
        {
            base.RemoveAt(index);
        }
        public void UpdateFromWithoutNotification(IEnumerable<T> fromCollection, bool notificationAfterCompleting = false)
        {
            ClearWithoutNotification();
            AddRangeWithoutNotification(fromCollection);

            if (notificationAfterCompleting)
                InvokeListEvents();
        }


        public IDisposable SubscribeForEachAndInvoke(Action<T, CollectionEventType, int> onChangeElement)
        {
            for (int i = 0; i < Count; i++)
            {
                onChangeElement.Invoke(this[i], CollectionEventType.Added, i);
            }
            return eventsForEach.Subscribe(value => onChangeElement.Invoke(value.Item1, value.Item2, value.Item3));
        }
        public IDisposable SubscribeForEach(Action<T, CollectionEventType, int> onChangeElement) => eventsForEach.Subscribe(value => onChangeElement.Invoke(value.Item1, value.Item2, value.Item3));
        public List<T> GetValue() => this.ToList();

        public void SetValue(List<T> value)
        {
            Clear();
            value?.ForEach(v => Add(v));
        }

        public IDisposable SubscribeAndInvoke(Action<List<T>> onChangedEvent)
        {
            onChangedEvent.Invoke(this);
            return Subscribe(onChangedEvent);
        }
        public void UnsubscribeAll() => mainEvent.RemoveAllListeners();//.DisonnectAll();
        public IDisposable Subscribe(Action<List<T>> onChangedEvent) => mainEvent.Subscribe(onChangedEvent); //SubscribeWithKey(onChangedEvent, onChangedEvent.GetHashCode().ToString());
        public IDisposable Subscribe(Action onChangedEvent) => Subscribe(val => onChangedEvent?.Invoke());

        public void InvokeEvents()
        {
            for (int i = 0; i < Count; i++)
            {
                InvokeElementEvents(this[i], CollectionEventType.None, i);
            }
            InvokeListEvents();
            forceInvoked?.Invoke();
        }

        public IDisposable SubscribeOnForceInvoke(Action onInvokedEvent) => forceInvoked.Subscribe(onInvokedEvent);
    }
    public interface IReactiveList<T> : IList<T>, IReadOnlyReactiveList<T>
    {
        public void RemoveAll(Func<T, bool> condition);
    }

    public interface IReadOnlyReactiveList<T> : IReadOnlyCollection<T>, IReadOnlyList<T>, IReactive<List<T>>, IEnumerable<T>, IEnumerable
    {
        public IDisposable SubscribeForEach(Action<T, CollectionEventType, int> onChangeElement);
        public IDisposable SubscribeForEachAndInvoke(Action<T, CollectionEventType, int> onChangeElement);
    }
}
