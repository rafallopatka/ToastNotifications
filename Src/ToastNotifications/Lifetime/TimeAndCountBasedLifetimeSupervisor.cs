using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using ToastNotifications.Core;
using ToastNotifications.Lifetime.Clear;
using ToastNotifications.Utilities;

namespace ToastNotifications.Lifetime
{
    public class TimeAndCountBasedLifetimeSupervisor : INotificationsLifetimeSupervisor
    {
        private readonly TimeSpan _notificationLifetime;
        private readonly int _maximumNotificationCount;

        private Dispatcher _dispatcher;
        private NotificationsList _notifications;
        private Queue<INotification> _notificationsPending;

        private IInterval _interval;

        public TimeAndCountBasedLifetimeSupervisor(TimeSpan notificationLifetime, MaximumNotificationCount maximumNotificationCount)
        {
            _notifications = new NotificationsList();

            _notificationLifetime = notificationLifetime;
            _maximumNotificationCount = maximumNotificationCount.Count;

            _interval = new Interval();
        }

        public void PushNotification(INotification notification)
        {
            if (_disposed)
            {
                Debug.WriteLine($"Warn ToastNotifications {this}.{nameof(PushNotification)} is already disposed");
                return;
            }

            if (_interval.IsRunning == false)
                TimerStart();

            if (_notifications.Count == _maximumNotificationCount)
            {
                if (_notificationsPending == null)
                {
                    _notificationsPending = new Queue<INotification>();
                }
                _notificationsPending.Enqueue(notification);
                return;
            }

            int numberOfNotificationsToClose = Math.Max(_notifications.Count - _maximumNotificationCount + 1, 0);

            var notificationsToRemove = _notifications
                .OrderBy(x => x.Key)
                .Take(numberOfNotificationsToClose)
                .Select(x => x.Value)
                .ToList();

            foreach (var n in notificationsToRemove)
                CloseNotification(n.Notification);

            _notifications.Add(notification);
            RequestShowNotification(new ShowNotificationEventArgs(notification));
        }

        public void CloseNotification(INotification notification)
        {
            _notifications.TryRemove(notification.Id, out var removedNotification);
            RequestCloseNotification(new CloseNotificationEventArgs(removedNotification.Notification));

            if (_notificationsPending != null && _notificationsPending.Any())
            {
                var not = _notificationsPending.Dequeue();
                PushNotification(not);
            }
        }


        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _interval?.Stop();
            _interval = null;
            _notifications?.Clear();
            _notifications = null;
            _notificationsPending?.Clear();
        }



        public void UseDispatcher(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        protected virtual void RequestShowNotification(ShowNotificationEventArgs e)
        {
            ShowNotificationRequested?.Invoke(this, e);
        }

        protected virtual void RequestCloseNotification(CloseNotificationEventArgs e)
        {
            CloseNotificationRequested?.Invoke(this, e);
        }

        private void TimerStart()
        {
            _interval.Invoke(TimeSpan.FromMilliseconds(200), OnTimerTick, _dispatcher);
        }

        private void TimerStop()
        {
            _interval.Stop();
        }

        private void OnTimerTick()
        {
            TimeSpan now = DateTimeNow.Local.TimeOfDay;

            var notificationsToRemove = _notifications
                .Where(x => x.Value.Notification.CanClose && x.Value.CreateTime + _notificationLifetime <= now)
                .Select(x => x.Value)
                .ToList();

            foreach (var n in notificationsToRemove)
                CloseNotification(n.Notification);

            if (_notifications.IsEmpty)
                TimerStop();
        }

        public void ClearMessages(IClearStrategy clearStrategy)
        {
            var notifications = clearStrategy.GetNotificationsToRemove(_notifications);
            foreach (var notification in notifications)
            {
                CloseNotification(notification);
            }
        }

        public bool ContainsNotification(INotification notification)
        {
            var enumerator = _notifications.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Value.Notification == notification)
                {
                    return true;
                }
            }

            return false;
        }

        public event EventHandler<ShowNotificationEventArgs> ShowNotificationRequested;
        public event EventHandler<CloseNotificationEventArgs> CloseNotificationRequested;
    }
}