using System;
using System.Windows;
using System.Windows.Media;
using ToastNotifications.Core;

namespace ToastNotifications.Position
{
    public class ControlPositionProvider : PositionProvider
    {

        private readonly FrameworkElement _element;

        public ControlPositionProvider(Window parentWindow,
            FrameworkElement trackingElement,
            Corner corner,
            double offsetX,
            double offsetY) : base(parentWindow, corner, offsetX, offsetY)
        {
            _element = trackingElement;
        }

        public override Point GetPosition(double actualPopupWidth, double actualPopupHeight)
        {
            var source = PresentationSource.FromVisual(ParentWindow);
            if (source?.CompositionTarget == null)
                return new Point(0, 0);

            Matrix transform = source.CompositionTarget.TransformFromDevice;
            Point location = transform.Transform(_element.PointToScreen(new Point(0, 0)));

            return GetPositionForCorner(location, actualPopupWidth, actualPopupHeight);
        }

        public override double GetHeight()
        {
            return ParentWindow.ActualHeight;
        }

        protected override Point GetPositionForBottomLeftCorner(Point location, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(location.X + _offsetX, location.Y + _element.ActualHeight - _offsetY - actualPopupHeight);
        }

        protected override Point GetPositionForBottomRightCorner(Point location, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(location.X + _element.ActualWidth - _offsetX - actualPopupWidth, location.Y + _element.ActualHeight - _offsetY - actualPopupHeight);
        }

        protected override Point GetPositionForBottomCenterCorner(Point location, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(location.X + (_element.ActualWidth - _offsetX - actualPopupWidth) / 2, location.Y + _element.ActualHeight - _offsetY - actualPopupHeight);
        }

        protected override Point GetPositionForTopLeftCorner(Point location, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(location.X + _offsetX, location.Y + _offsetY);
        }

        protected override Point GetPositionForTopRightCorner(Point location, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(location.X + _element.ActualWidth - _offsetX - actualPopupWidth, location.Y + _offsetY);
        }

        protected override Point GetPositionForTopCenterCorner(Point location, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(location.X + (_element.ActualWidth - _offsetX - actualPopupWidth) / 2, location.Y + _offsetY);
        }

    }
}