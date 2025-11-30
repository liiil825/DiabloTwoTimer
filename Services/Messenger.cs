using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.Services;

public class Messenger : IMessenger
{
    // 线程安全的字典，存储消息类型 -> 订阅者列表
    private readonly ConcurrentDictionary<Type, List<WeakSubscription>> _subscriptions = new();

    public void Subscribe<TMessage>(Action<TMessage> action)
        where TMessage : class
    {
        var messageType = typeof(TMessage);
        var subscription = new WeakSubscription(action);

        _subscriptions.AddOrUpdate(
            messageType,
            // 如果不存在，创建新列表
            _ => new List<WeakSubscription> { subscription },
            // 如果存在，添加到现有列表
            (_, list) =>
            {
                lock (list)
                {
                    list.Add(subscription);
                }
                return list;
            }
        );
    }

    public void Unsubscribe<TMessage>(Action<TMessage> action)
        where TMessage : class
    {
        var messageType = typeof(TMessage);
        if (_subscriptions.TryGetValue(messageType, out var list))
        {
            lock (list)
            {
                // 找到并移除对应的订阅
                var target = list.FirstOrDefault(s => s.IsHandler(action));
                if (target != null)
                {
                    list.Remove(target);
                }
            }
        }
    }

    public void Publish<TMessage>(TMessage message)
        where TMessage : class
    {
        var messageType = typeof(TMessage);
        if (_subscriptions.TryGetValue(messageType, out var list))
        {
            List<WeakSubscription> toExecute = [];

            lock (list)
            {
                // 清理已失效的订阅（垃圾回收）
                list.RemoveAll(s => !s.IsAlive);
                // 复制一份快照用于执行，避免锁内执行造成死锁
                toExecute = list.ToList();
            }

            foreach (var subscription in toExecute)
            {
                subscription.Invoke(message);
            }
        }
    }

    // 内部类：封装弱引用委托
    private class WeakSubscription
    {
        private readonly WeakReference? _target;
        private readonly System.Reflection.MethodInfo _method;
        private readonly Type _delegateType;

        public bool IsAlive => _target == null || _target.IsAlive;

        public WeakSubscription(Delegate handler)
        {
            // 如果是静态方法，Target 为 null
            _target = handler.Target != null ? new WeakReference(handler.Target) : null;
            _method = handler.Method;
            _delegateType = handler.GetType();
        }

        public bool IsHandler(Delegate handler)
        {
            var target = _target?.Target;
            return target == handler.Target && _method == handler.Method;
        }

        public void Invoke(object message)
        {
            var target = _target?.Target;
            // 如果 Target 还是活着的（或者本来就是静态方法），则执行
            if (_target == null || target != null)
            {
                try
                {
                    _method.Invoke(target, [message]);
                }
                catch (Exception ex)
                {
                    LogManager.WriteErrorLog("Messenger", "Error invoking subscription", ex);
                }
            }
        }
    }
}
