using System;
using System.Windows.Threading;
using ToastNotifications.Core;
using ToastNotifications.Lifetime.Clear;

namespace ToastNotifications.Lifetime
{
    public interface INotificationsLifetimeSupervisor : IDisposable
    {
        bool ContainsNotification(INotification notification);
        void PushNotification(INotification notification);
        void CloseNotification(INotification notification);

        void UseDispatcher(Dispatcher dispatcher);
        
        event EventHandler<ShowNotificationEventArgs> ShowNotificationRequested;
        event EventHandler<CloseNotificationEventArgs> CloseNotificationRequested;

        void ClearMessages(IClearStrategy clearStrategy);
    }
}