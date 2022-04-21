using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ToastNotifications.Core;

namespace ToastNotifications.Position
{
    public abstract class PositionProvider : IPositionProvider
    {
        protected readonly Corner _corner;
        protected readonly double _offsetX;
        protected readonly double _offsetY;

        public Window ParentWindow { get; }
        public EjectDirection EjectDirection { get; protected set; }

        public PositionProvider( 
            Window parentWindow,
            Corner corner, 
            double offsetX, 
            double offsetY)
        {
            _corner = corner;
            _offsetX = offsetX;
            _offsetY = offsetY;
            if(parentWindow != null)
            {
                ParentWindow = parentWindow;
                parentWindow.SizeChanged += ParentWindowOnSizeChanged;
                parentWindow.LocationChanged += ParentWindowOnLocationChanged;
                parentWindow.StateChanged += ParentWindowOnStateChanged;
                parentWindow.Activated += ParentWindowOnActivated;
            }

            SetEjectDirection(corner);
        }

        public virtual Point GetPosition(double actualPopupWidth, double actualPopupHeight)
        {
            Point parentPosition;
            if (ParentWindow == null)
                //inheriting classes can ignore this value if they do not use a parent;
                parentPosition = new Point(0, 0);
            else
                parentPosition = ParentWindow.GetActualPosition();

            return GetPositionForCorner(parentPosition,actualPopupWidth, actualPopupHeight);
        }

        protected virtual Point GetPositionForCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight)
        {
            switch (_corner)
            {
                case Corner.TopRight:
                    return GetPositionForTopRightCorner(parentPosition, actualPopupWidth, actualPopupHeight);
                case Corner.TopLeft:
                    return GetPositionForTopLeftCorner(parentPosition, actualPopupWidth, actualPopupHeight);
                case Corner.TopCenter:
                    return GetPositionForTopCenterCorner(parentPosition, actualPopupWidth, actualPopupHeight);
                case Corner.BottomRight:
                    return GetPositionForBottomRightCorner(parentPosition, actualPopupWidth, actualPopupHeight);
                case Corner.BottomLeft:
                    return GetPositionForBottomLeftCorner(parentPosition, actualPopupWidth, actualPopupHeight);
                case Corner.BottomCenter:
                    return GetPositionForBottomCenterCorner(parentPosition, actualPopupWidth, actualPopupHeight);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public abstract double GetHeight();

        protected virtual void SetEjectDirection(Corner corner)
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

        protected abstract Point GetPositionForBottomLeftCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight);

        protected abstract Point GetPositionForBottomRightCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight);

        protected abstract Point GetPositionForBottomCenterCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight);

        protected abstract Point GetPositionForTopLeftCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight);

        protected abstract Point GetPositionForTopRightCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight);

        protected abstract Point GetPositionForTopCenterCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight);

        public void Dispose()
        {
            if(ParentWindow != null)
            {
                ParentWindow.LocationChanged -= ParentWindowOnLocationChanged;
                ParentWindow.SizeChanged -= ParentWindowOnSizeChanged;
                ParentWindow.StateChanged -= ParentWindowOnStateChanged;
                ParentWindow.Activated -= ParentWindowOnActivated;
            }
        }

        protected virtual void RequestUpdatePosition()
        {
            UpdateHeightRequested?.Invoke(this, EventArgs.Empty);
            UpdateEjectDirectionRequested?.Invoke(this, EventArgs.Empty);
            UpdatePositionRequested?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void ParentWindowOnLocationChanged(object sender, EventArgs eventArgs)
        {
            RequestUpdatePosition();
        }

        protected virtual void ParentWindowOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            RequestUpdatePosition();
        }

        protected virtual void ParentWindowOnStateChanged(object sender, EventArgs eventArgs)
        {
            RequestUpdatePosition();
        }

        protected virtual void ParentWindowOnActivated(object sender, EventArgs eventArgs)
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
