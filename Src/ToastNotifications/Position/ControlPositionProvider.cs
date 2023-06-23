using System;
using System.Windows;
using System.Windows.Media;
using ToastNotifications.Core;

namespace ToastNotifications.Position
{
    public class ControlPositionProvider : IPositionProvider
    {
        private readonly double _offsetX;
        private readonly double _offsetY;
        private readonly Corner _corner;
        private readonly FrameworkElement _element;

        public Window ParentWindow { get; }
        public EjectDirection EjectDirection { get; private set; }

        public ControlPositionProvider(Window parentWindow, FrameworkElement trackingElement, Corner corner, double offsetX, double offsetY)
        {
            _corner = corner;
            _offsetX = offsetX;
            _offsetY = offsetY;
            _element = trackingElement;

            ParentWindow = parentWindow;

            parentWindow.SizeChanged += ParentWindowOnSizeChanged;
            parentWindow.LocationChanged += ParentWindowOnLocationChanged;

            SetEjectDirection(corner);
        }

        public Point GetPosition(double actualPopupWidth, double actualPopupHeight)
        {
            var source = PresentationSource.FromVisual(ParentWindow);
            if (source?.CompositionTarget == null)
                return new Point(0, 0);

            Matrix transform = source.CompositionTarget.TransformFromDevice;
            Point location = transform.Transform(_element.PointToScreen(new Point(0, 0)));

            switch (_corner)
            {
                case Corner.TopRight:
                    return GetPositionForTopRightCorner(location, actualPopupWidth, actualPopupHeight);
                case Corner.TopLeft:
                    return GetPositionForTopLeftCorner(location, actualPopupWidth, actualPopupHeight);
                case Corner.BottomRight:
                    return GetPositionForBottomRightCorner(location, actualPopupWidth, actualPopupHeight);
                case Corner.BottomCenter:
                    return GetPositionForBottomCenterCorner(location, actualPopupWidth, actualPopupHeight);
                case Corner.BottomLeft:
                    return GetPositionForBottomLeftCorner(location, actualPopupWidth, actualPopupHeight);
                case Corner.TopCenter:
                    return GetPositionForTopCenterCorner(location, actualPopupWidth, actualPopupHeight);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public double GetHeight()
        {
            return ParentWindow.ActualHeight;
        }

        private void SetEjectDirection(Corner corner)
        {
            switch (corner)
            {
                case Corner.TopRight:
                case Corner.TopLeft:
                case Corner.TopCenter:
                    EjectDirection = EjectDirection.ToBottom;
                    break;
                case Corner.BottomRight:
                case Corner.BottomLeft:
                case Corner.BottomCenter:
                    EjectDirection = EjectDirection.ToTop;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(corner), corner, null);
            }
        }

        private Point GetPositionForBottomLeftCorner(Point location, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(location.X + _offsetX, location.Y + _element.ActualHeight - _offsetY - actualPopupHeight);
        }

        private Point GetPositionForBottomRightCorner(Point location, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(location.X + _element.ActualWidth - _offsetX - actualPopupWidth, location.Y + _element.ActualHeight - _offsetY - actualPopupHeight);
        }

        private Point GetPositionForBottomCenterCorner(Point location, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(location.X + (_element.ActualWidth - _offsetX - actualPopupWidth) / 2, location.Y + _element.ActualHeight - _offsetY - actualPopupHeight);
        }


        private Point GetPositionForTopLeftCorner(Point location, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(location.X + _offsetX, location.Y + _offsetY);
        }

        private Point GetPositionForTopRightCorner(Point location, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(location.X + _element.ActualWidth - _offsetX - actualPopupWidth, location.Y + _offsetY);
        }

        private Point GetPositionForTopCenterCorner(Point location, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(location.X + (_element.ActualWidth - _offsetX - actualPopupWidth) / 2, location.Y + _offsetY);
        }

        public void Dispose()
        {
            ParentWindow.LocationChanged -= ParentWindowOnLocationChanged;
            ParentWindow.SizeChanged -= ParentWindowOnSizeChanged;
        }

        protected virtual void RequestUpdatePosition()
        {
            UpdatePositionRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ParentWindowOnLocationChanged(object sender, EventArgs eventArgs)
        {
            RequestUpdatePosition();
        }

        private void ParentWindowOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            RequestUpdatePosition();
        }
#pragma warning disable CS0067
		public event EventHandler UpdatePositionRequested;

        public event EventHandler UpdateEjectDirectionRequested;

        public event EventHandler UpdateHeightRequested;
#pragma warning restore CS0067
	}
}