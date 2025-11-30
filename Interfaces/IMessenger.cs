using System;

namespace DiabloTwoMFTimer.Interfaces;

public interface IMessenger
{
    // 订阅消息
    void Subscribe<TMessage>(Action<TMessage> action)
        where TMessage : class;

    // 取消订阅 (通常用于组件销毁时，虽然我们将实现弱引用，但显式取消是个好习惯)
    void Unsubscribe<TMessage>(Action<TMessage> action)
        where TMessage : class;

    // 发送消息
    void Publish<TMessage>(TMessage message)
        where TMessage : class;
}
