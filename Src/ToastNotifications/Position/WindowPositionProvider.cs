using System;
using System.Windows;
using ToastNotifications.Core;

namespace ToastNotifications.Position
{
    public class WindowPositionProvider : IPositionProvider
    {
        private readonly Corner _corner;
        private readonly double _offsetX;
        private readonly double _offsetY;

        public Window ParentWindow { get; }
        public EjectDirection EjectDirection { get; private set; }

        public WindowPositionProvider(Window parentWindow, Corner corner, double offsetX, double offsetY)
        {
            _corner = corner;
            _offsetX = offsetX;
            _offsetY = offsetY;
            ParentWindow = parentWindow;

            parentWindow.SizeChanged += ParentWindowOnSizeChanged;
            parentWindow.LocationChanged += ParentWindowOnLocationChanged;
            parentWindow.StateChanged += ParentWindowOnStateChanged;
            parentWindow.Activated += ParentWindowOnActivated;

            SetEjectDirection(corner);
        }

        public Point GetPosition(double actualPopupWidth, double actualPopupHeight)
        {
            var parentPosition = ParentWindow.GetActualPosition();

			switch(_corner)
            {
                case Corner.TopRight:
                    return GetPositionForTopRightCorner(parentPosition, actualPopupWidth, actualPopupHeight);
                case Corner.TopLeft:
                    return GetPositionForTopLeftCorner(parentPosition, actualPopupWidth, actualPopupHeight);
                case Corner.BottomRight:
                    return GetPositionForBottomRightCorner(parentPosition, actualPopupWidth, actualPopupHeight);
                case Corner.BottomLeft:
                    return GetPositionForBottomLeftCorner(parentPosition, actualPopupWidth, actualPopupHeight);
                case Corner.BottomCenter:
                    return GetPositionForBottomCenter(parentPosition, actualPopupWidth, actualPopupHeight);
                case Corner.TopCenter:
                    return GetPositionForTopCenter(parentPosition, actualPopupWidth, actualPopupHeight);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public double GetHeight()
        {
            var actualHeight = (ParentWindow.Content as FrameworkElement)?.ActualHeight ?? ParentWindow.ActualHeight;

            return actualHeight;
        }

        private double GetWindowWidth()
        {
            var actualWidth = (ParentWindow.Content as FrameworkElement)?.ActualWidth ?? ParentWindow.ActualWidth;

            return actualWidth;
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

        private Point GetPositionForBottomLeftCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(parentPosition.X + _offsetX, parentPosition.Y - _offsetY);
        }

        private Point GetPositionForBottomRightCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(parentPosition.X + GetWindowWidth() - _offsetX - actualPopupWidth, parentPosition.Y - _offsetY);
        }

        private Point GetPositionForBottomCenter(Point parentPosition, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(parentPosition.X + (GetWindowWidth() - actualPopupWidth) / 2, parentPosition.Y - _offsetY);
        }


        private Point GetPositionForTopLeftCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(parentPosition.X + _offsetX, parentPosition.Y + _offsetY);
        }

        private Point GetPositionForTopRightCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point( parentPosition.X + GetWindowWidth() - _offsetX - actualPopupWidth,  parentPosition.Y + _offsetY);
        }

        private Point GetPositionForTopCenter(Point parentPosition, double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(parentPosition.X + (GetWindowWidth() - actualPopupWidth) / 2, parentPosition.Y + _offsetY);
        }

        public void Dispose()
        {
            ParentWindow.LocationChanged -= ParentWindowOnLocationChanged;
            ParentWindow.SizeChanged -= ParentWindowOnSizeChanged;
            ParentWindow.StateChanged -= ParentWindowOnStateChanged;
            ParentWindow.Activated -= ParentWindowOnActivated;
        }

        protected virtual void RequestUpdatePosition()
        {
            UpdateHeightRequested?.Invoke(this, EventArgs.Empty);
            UpdateEjectDirectionRequested?.Invoke(this, EventArgs.Empty);
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

        private void ParentWindowOnStateChanged(object sender, EventArgs eventArgs)
        {
            RequestUpdatePosition();
        }

        private void ParentWindowOnActivated(object sender, EventArgs eventArgs)
        {
            RequestUpdatePosition();
        }

        public event EventHandler UpdatePositionRequested;

        public event EventHandler UpdateEjectDirectionRequested;

        public event EventHandler UpdateHeightRequested;
    }
}