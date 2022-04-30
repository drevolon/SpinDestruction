using System;

internal interface IReadOnlySubscriptionAction
{
    void SubscribeOnChange(Action subscriptionAction);
    void UnSubscriptionOnChange(Action unsubscriptionAction);
}