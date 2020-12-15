using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ToastNotifications.Core;

namespace ToastNotifications.Display
{
    public class NotificationsItemsControl : ItemsControl
    {
        public static readonly DependencyProperty ShouldReverseItemsProperty = DependencyProperty.Register(nameof(ShouldReverseItems), typeof(bool), typeof(NotificationsItemsControl), new FrameworkPropertyMetadata(default(bool), ShouldReverseItemsPropertyChanged));

        public bool ShouldReverseItems
        {
            get { return (bool)GetValue(ShouldReverseItemsProperty); }
            set { SetValue(ShouldReverseItemsProperty, value); }
        }

        public NotificationsItemsControl()
        {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            PrepareItemsControl(this, ShouldReverseItems);
        }

        private static void ShouldReverseItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var itemsControl = d as NotificationsItemsControl;

            if (itemsControl == null)
                return;

            bool shouldReverse = (bool)e.NewValue;

            PrepareItemsControl(itemsControl, shouldReverse);
        }

        private static void PrepareItemsControl(ItemsControl itemsControl, bool reverse)
        {
            Panel itemPanel = GetItemsPanel(itemsControl);
			if(itemPanel == null)
				return;

            int scaleY = reverse ? -1 : 1;

            itemPanel.LayoutTransform = new ScaleTransform(1, scaleY);
            Style itemContainerStyle;
            if (itemsControl.ItemContainerStyle == null)
            {
                itemContainerStyle = new Style();
            }
            else
            {
                itemContainerStyle = CopyStyle(itemsControl.ItemContainerStyle);
            }
            Setter setter = new Setter();
            setter.Property = ItemsControl.LayoutTransformProperty;
            setter.Value = new ScaleTransform(1, scaleY);
            itemContainerStyle.Setters.Add(setter);
            itemsControl.ItemContainerStyle = itemContainerStyle;
        }

        private static Panel GetItemsPanel(ItemsControl itemsControl)
        {
            ItemsPresenter itemsPresenter = GetVisualChild<ItemsPresenter>(itemsControl);
            if (itemsPresenter == null)
                return null;
            return GetVisualChild<Panel>(itemsControl);
        }

        private static Style CopyStyle(Style style)
        {
            Style styleCopy = new Style();
            foreach (SetterBase currentSetter in style.Setters)
            {
                styleCopy.Setters.Add(currentSetter);
            }
            foreach (TriggerBase currentTrigger in style.Triggers)
            {
                styleCopy.Triggers.Add(currentTrigger);
            }
            return styleCopy;
        }

        private static T GetVisualChild<T>(DependencyObject parent) where T : Visual
        {
            T child = default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        public void AddNotification(NotificationDisplayPart notification)
        {
            //Can't find why notifications are tried to add more than one time
            //Without this condition an error was thrown
            if (notification.Parent == null)
            {
                Items.Add(notification);
            }

        }

        public void RemoveNotification(NotificationDisplayPart notification)
        {
            Items.Remove(notification);
        }

        public int GetItemCount()
        {
            return Items.Count;
        }
    }
}
