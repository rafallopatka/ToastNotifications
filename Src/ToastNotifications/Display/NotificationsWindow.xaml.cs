using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using ToastNotifications.Core;
using ToastNotifications.Events;
using ToastNotifications.Utilities;

namespace ToastNotifications.Display
{
    /// <summary>
    /// Interaction logic for NotificationsWindow.xaml
    /// </summary>
    public partial class NotificationsWindow : Window
    {
        private IKeyboardEventHandler _keyboardEventHandler;

        public NotificationsWindow()
        {
            InitializeComponent();

            Loaded += NotificationsWindow_Loaded;
            Closing += NotificationsWindow_Closing;

            ShowInTaskbar = false;
            Visibility = Visibility.Hidden;
        }

        public NotificationsWindow(Window owner)
        {
            InitializeComponent();

            Loaded += NotificationsWindow_Loaded;
            Closing += NotificationsWindow_Closing;

            Owner = owner;
        }

        public void SetPosition(Point position)
        {
            Left = position.X;
            Top = position.Y;
        }

        public void ShowNotification(NotificationDisplayPart notification)
        {
            NotificationsList.AddNotification(notification);
            RecomputeLayout();
        }

        public void CloseNotification(NotificationDisplayPart notification)
        {
            NotificationsList.RemoveNotification(notification);
            RecomputeLayout();
        }

        private void RecomputeLayout()
        {
            Dispatcher.Invoke(((Action)(() =>
            {
                if (NotificationsList.GetItemCount() == 0)
                {
                    this.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                }
    
            })), DispatcherPriority.Render);
        }

        public void SetEjectDirection(EjectDirection ejectDirection)
        {
            NotificationsList.ShouldReverseItems = ejectDirection == EjectDirection.ToTop;
        }

        public double GetWidth()
        {
            return Width;
        }

        public double GetHeight()
        {
            return Height;
        }

        internal void SetDisplayOptions(DisplayOptions displayOptions)
        {
            Topmost = displayOptions.TopMost;
            NotificationsList.Width = displayOptions.Width;
        }

        public void SetKeyboardEventHandler(IKeyboardEventHandler keyboardEventHandler)
        {
            _keyboardEventHandler = keyboardEventHandler;
        }

        private void NotificationsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);

            int exStyle = (int)WinApi.GetWindowLong(wndHelper.Handle, (int)WinApi.GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (int)WinApi.ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            WinApi.SetWindowLong(wndHelper.Handle, (int)WinApi.GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }

        private void NotificationsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            _keyboardEventHandler.Handle(e);
        }

        public new void Close()
        {
            this.Closing -= NotificationsWindow_Closing;
            base.Close();
        }
    }
}