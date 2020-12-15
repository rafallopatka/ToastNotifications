# Thank you for contributing

Any help, suggestions, finding issues, pull requests are really appreciated.

If you crate an enhancement or bug fix to the library, and you would like to share it with the community, please send a pull request
I will test, merge and release nuget

Author: Alexandre José Heinen
Feature: Allowing multiple LifetimeSupervisors with different behaviors
I need to use two different LifetimeSupervisor, one with CountBasedLifetimeSupervisor and other with TimeAndCountBasedLifetimeSupervisor. If I create 2 separated Notifiers the notifications can be displayed over the notifications that are already opened.
For this I create a way to set multiple LifetimeSupervisors to same Notifiers and same DisplaySupervisor.
To work correctly the LifetimeSupervisors need to be cached, to pass the same instance.
Example of how to use:

```
private static Notifier GenerateNotifier(double notificationLifeTime, Corner notificationPosition)
{
    //here I check if the LifetimeSupervisor to use is based on time
    //if it is null I create new instance
    if (notificationLifeTime > 0 && _lifetimeTimeAndCount == null)
    {
        _lifetimeTimeAndCount = new TimeAndCountBasedLifetimeSupervisor(
            notificationLifetime: TimeSpan.FromSeconds(notificationLifeTime),
            maximumNotificationCount: MaximumNotificationCount.FromCount(5));
    }
    
    //here I check if the LifetimeSupervisor to use is based only on count
    //if it is null I create new instance
    if (notificationLifeTime == 0 && _lifetimeCount == null)
    {
        _lifetimeCount = new CountBasedLifetimeSupervisor(maximumNotificationCount: MaximumNotificationCount.FromCount(5));
    }

    //get the correct LifetimeSupervisor to use
    INotificationsLifetimeSupervisor lifetimeSupervisor = notificationLifeTime > 0 ? (INotificationsLifetimeSupervisor)_lifetimeTimeAndCount : (INotificationsLifetimeSupervisor)_lifetimeCount;

    if (_notifier == null)
    {
        _notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: notificationPosition,
                offsetX: 10,
                offsetY: 49);

            cfg.LifetimeSupervisor = lifetimeSupervisor;

            cfg.Dispatcher = Application.Current.Dispatcher;
        });
    }

    //update the current LifetimeSupervisor in the notifier
    _notifier.UpdateLifetimeSupervisor(lifetimeSupervisor);

    return _notifier;
}
```

In this case I'm not checking if the notificationLifeTime is different, but if you need you can cache into a dictionary with notificationLifeTime as the key.
