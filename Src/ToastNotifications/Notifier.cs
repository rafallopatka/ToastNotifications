using System;
using System.Windows;
using ToastNotifications.Core;
using ToastNotifications.Display;
using ToastNotifications.Events;
using ToastNotifications.Lifetime;
using ToastNotifications.Lifetime.Clear;

// ReSharper disable LocalizableElement

namespace ToastNotifications
{
    public class Notifier : IDisposable
    {
        private readonly object _syncRoot = new object();

        private readonly Action<NotifierConfiguration> _configureAction;
        private NotifierConfiguration _configuration;
        private INotificationsLifetimeSupervisor _lifetimeSupervisor;
        private NotificationsDisplaySupervisor _displaySupervisor;

        public Notifier(Action<NotifierConfiguration> configureAction)
        {
            _configureAction = configureAction;
        }

        public void Notify<T>(Func<T> createNotificationFunc)
            where T: INotification
        {
            Configure();
            _lifetimeSupervisor.PushNotification(createNotificationFunc());
        }

        private void Configure()
        {
            lock (_syncRoot)
            {
                if (_configuration != null)
                    return;

                var cfg = CreateConfiguration();

                var keyboardEventHandler = cfg.KeyboardEventHandler ?? new BlockAllKeyInputEventHandler();

                _configuration = cfg;
                _lifetimeSupervisor = cfg.LifetimeSupervisor;
                _lifetimeSupervisor.UseDispatcher(cfg.Dispatcher);

                _displaySupervisor = new NotificationsDisplaySupervisor(
                    cfg.Dispatcher,
                    cfg.PositionProvider,
                    cfg.LifetimeSupervisor,
                    cfg.DisplayOptions,
                    keyboardEventHandler);
            }
        }

        private NotifierConfiguration CreateConfiguration()
        {
            var cfg = new NotifierConfiguration
            {
                Dispatcher = Application.Current.Dispatcher
            };
            _configureAction(cfg);

            if (cfg.LifetimeSupervisor == null)
                throw new ArgumentNullException(nameof(cfg.LifetimeSupervisor), "Missing configuration argument");

            if (cfg.PositionProvider == null)
                throw new ArgumentNullException(nameof(cfg.PositionProvider), "Missing configuration argument");
            return cfg;
        }

        public void ClearMessages(IClearStrategy clearStrategy)
        {
            _lifetimeSupervisor?.ClearMessages(clearStrategy);
        }

        public void UpdateLifetimeSupervisor(INotificationsLifetimeSupervisor lifetimeSupervisor)
        {
            if (_configuration != null)
            {
                lock (_syncRoot)
                {
                    _configuration.LifetimeSupervisor = lifetimeSupervisor;
                    _lifetimeSupervisor = lifetimeSupervisor;
                    _lifetimeSupervisor.UseDispatcher(_configuration.Dispatcher);

                    _displaySupervisor.UpdateLifetimeSupervisor(_lifetimeSupervisor);
                }

            }
        }

        private bool _disposed = false;

        public object SyncRoot => _syncRoot;

        public void Dispose()
        {
            if (_disposed == false)
            {
                _disposed = true;
                _configuration?.PositionProvider?.Dispose();
                _displaySupervisor?.Dispose();
                _lifetimeSupervisor?.Dispose();
            }
        }
    }
}
