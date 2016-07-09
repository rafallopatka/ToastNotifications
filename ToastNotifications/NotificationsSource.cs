using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ToastNotifications
{
    public class NotificationsSource : INotifyPropertyChanged
    {
        private readonly DispatcherTimer _timer;
        private bool _isOpen;
        private bool _isTopmost;

        public const int UnlimitedNotifications = -1;

        public ObservableCollection<NotificationViewModel> NotificationMessages { get; private set; }

        public long MaximumNotificationCount { get; set; }

        public TimeSpan NotificationLifeTime { get; set; }

        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                _isOpen = value;
                OnPropertyChanged(nameof(IsOpen));
            }
        }

        public bool IsTopmost
        {
            get { return _isTopmost; }
            set
            {
                _isTopmost = value;
                OnPropertyChanged(nameof(IsTopmost));
            }
        }

        public NotificationsSource()
        {
            NotificationMessages = new ObservableCollection<NotificationViewModel>();

            MaximumNotificationCount = 5;
            NotificationLifeTime = TimeSpan.FromSeconds(6);

            _timer = new DispatcherTimer(DispatcherPriority.Normal);
            _timer.Interval = TimeSpan.FromMilliseconds(200);
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            var currentTime = DateTime.Now;

            var itemsToRemove = NotificationMessages.Where(x => currentTime - x.CreateTime >= NotificationLifeTime).Select(x => x.Id).ToList();

            foreach (var id in itemsToRemove)
            {
                Hide(id);
            }
        }

        public void Show(string message, NotificationType type)
        {
            if (NotificationMessages.Any() == false)
            {
                InternalStartTimer();
                IsOpen = true;
            }

            if (MaximumNotificationCount != UnlimitedNotifications)
            {
                if (NotificationMessages.Count >= MaximumNotificationCount)
                {
                    int removeCount = (int)(NotificationMessages.Count - MaximumNotificationCount) + 1;

                    var itemsToRemove = NotificationMessages.OrderBy(x => x.CreateTime).Take(removeCount).Select(x => x.Id).ToList();
                    foreach (var id in itemsToRemove)
                        Hide(id);
                }
            }

            NotificationMessages.Add(new NotificationViewModel { Message = message, Type = type });
        }

        public void Hide(Guid id)
        {
            var n = NotificationMessages.Single(x => x.Id == id);
            n.InvokeHideAnimation();

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(200);
            }).ContinueWith(t =>
            {
                NotificationMessages.Remove(n);

                if (NotificationMessages.Any() == false)
                {
                    InternalStopTimer();
                    IsOpen = false;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void InternalStartTimer()
        {
            _timer.Tick += TimerOnTick;
            _timer.Start();
        }

        private void InternalStopTimer()
        {
            _timer.Stop();
            _timer.Tick -= TimerOnTick;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}