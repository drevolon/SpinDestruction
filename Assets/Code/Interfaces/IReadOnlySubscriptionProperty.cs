using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReadOnlySubscriptionProperty<T>
{
    T Value { get; }
    void SubscribeOnChange(Action<T> subscriptionAction);
    void UnSubscriptionOnChange(Action<T> unsubscriptionAction);
}
