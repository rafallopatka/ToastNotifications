using System;
using System.Windows;
using ToastNotifications.Core;

namespace ToastNotifications.Position
{
    public class WindowPositionProvider : PositionProvider
    {

        public WindowPositionProvider(
            Window parentWindow,
            Corner corner,
            double offsetX,
            double offsetY) : base(parentWindow, corner, offsetX, offsetY)
        {

        }

        public override double GetHeight()
        {//ToDo fix null exception here.
            var actualHeight = (ParentWindow.Content as FrameworkElement)?.ActualHeight ?? ParentWindow.ActualHeight;

            return actualHeight;
        }

        private double GetWindowWidth()
        {
            var actualWidth = (ParentWindow.Content as FrameworkElement)?.ActualWidth ?? ParentWindow.ActualWidth;

            return actualWidth;
        }

        protected override Point GetPositionForBottomLeftCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(parentPosition.X + _offsetX, parentPosition.Y - _offsetY);
        }

        protected override Point GetPositionForBottomRightCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(parentPosition.X + GetWindowWidth() - _offsetX - actualPopupWidth, parentPosition.Y - _offsetY);
        }

        protected override Point GetPositionForBottomCenterCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(parentPosition.X + (GetWindowWidth() - actualPopupWidth) / 2, parentPosition.Y - _offsetY);
        }

        protected override Point GetPositionForTopLeftCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(parentPosition.X + _offsetX, parentPosition.Y + _offsetY);
        }

        protected override Point GetPositionForTopRightCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(parentPosition.X + GetWindowWidth() - _offsetX - actualPopupWidth, parentPosition.Y + _offsetY);
        }

        protected override Point GetPositionForTopCenterCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(parentPosition.X + (GetWindowWidth() - actualPopupWidth) / 2, parentPosition.Y + _offsetY);
        }

    }
}