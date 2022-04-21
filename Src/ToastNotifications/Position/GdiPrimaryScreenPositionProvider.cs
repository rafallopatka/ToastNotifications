using System;
using System.Windows;
using System.Windows.Forms;
using ToastNotifications.Core;

namespace ToastNotifications.Position
{
    public class GdiPrimaryScreenPositionProvider : PositionProvider
    {
        private readonly float _dpiRatioX;
        private readonly float _dpiRatioY;

        private double ScreenHeight => Screen.PrimaryScreen.Bounds.Height / _dpiRatioY;
        private double ScreenWidth => Screen.PrimaryScreen.Bounds.Width / _dpiRatioX;

        private double WorkAreaHeight => Screen.PrimaryScreen.WorkingArea.Height / _dpiRatioY;
        private double WorkAreaWidth => Screen.PrimaryScreen.WorkingArea.Width / _dpiRatioX;

        public GdiPrimaryScreenPositionProvider(
            Corner corner,
            double offsetX,
            double offsetY) : base(null, corner, offsetX, offsetY)
        {

            using (var gfx = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
            {
                _dpiRatioX = gfx.DpiX / 96F;
                _dpiRatioY = gfx.DpiY / 96F;
            }
        }

        public override double GetHeight()
        {
            return ScreenHeight;
        }

        protected override Point GetPositionForBottomLeftCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight)
        {
            double pointX = _offsetX;
            double pointY = WorkAreaHeight - _offsetY - actualPopupHeight;

            switch (GetTaskBarLocation())
            {
                case WindowsTaskBarLocation.Left:
                    pointX = (ScreenWidth - WorkAreaWidth) + _offsetX;
                    break;

                case WindowsTaskBarLocation.Top:
                    pointY = ScreenHeight - _offsetY - actualPopupHeight;
                    break;
            }

            return new Point(pointX, pointY);
        }

        protected override Point GetPositionForBottomCenterCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight)
        {
            double pointX = (WorkAreaWidth - _offsetX - actualPopupWidth) / 2;
            double pointY = WorkAreaHeight - _offsetY - actualPopupHeight;

            switch (GetTaskBarLocation())
            {
                case WindowsTaskBarLocation.Left:
                    pointX = (ScreenWidth - _offsetX - actualPopupWidth) / 2;
                    break;

                case WindowsTaskBarLocation.Top:
                    pointY = ScreenHeight - _offsetY - actualPopupHeight;
                    break;
            }

            return new Point(pointX, pointY);
        }


        protected override Point GetPositionForBottomRightCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight)
        {
            double pointX = WorkAreaWidth - _offsetX - actualPopupWidth;
            double pointY = WorkAreaHeight - _offsetY - actualPopupHeight;

            switch (GetTaskBarLocation())
            {
                case WindowsTaskBarLocation.Left:
                    pointX = ScreenWidth - _offsetX - actualPopupWidth;
                    break;

                case WindowsTaskBarLocation.Top:
                    pointY = ScreenHeight - _offsetY - actualPopupHeight;
                    break;
            }

            return new Point(pointX, pointY);
        }

        protected override Point GetPositionForTopLeftCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight)
        {
            double pointX = _offsetX;
            double pointY = _offsetY;

            switch (GetTaskBarLocation())
            {
                case WindowsTaskBarLocation.Left:
                    pointX = ScreenWidth - WorkAreaWidth + _offsetX;
                    break;

                case WindowsTaskBarLocation.Top:
                    pointY = ScreenHeight - WorkAreaHeight + _offsetY;
                    break;
            }

            return new Point(pointX, pointY);
        }

        protected override Point GetPositionForTopRightCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight)
        {
            double pointX = WorkAreaWidth - _offsetX - actualPopupWidth;
            double pointY = _offsetY;

            switch (GetTaskBarLocation())
            {
                case WindowsTaskBarLocation.Left:
                    pointX = ScreenWidth - actualPopupWidth - _offsetX;
                    break;

                case WindowsTaskBarLocation.Top:
                    pointY = ScreenHeight - WorkAreaHeight + _offsetY;
                    break;
            }

            return new Point(pointX, pointY);
        }

        protected override Point GetPositionForTopCenterCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight)
        {
            double pointX = (WorkAreaWidth - _offsetX - actualPopupWidth) / 2;
            double pointY = _offsetY;

            switch (GetTaskBarLocation())
            {
                case WindowsTaskBarLocation.Left:
                    pointX = (ScreenWidth - _offsetX - actualPopupWidth) / 2;
                    break;

                case WindowsTaskBarLocation.Top:
                    pointY = ScreenHeight - WorkAreaHeight + _offsetY;
                    break;
            }

            return new Point(pointX, pointY);
        }

        private WindowsTaskBarLocation GetTaskBarLocation()
        {
            if (SystemParameters.WorkArea.Left > 0)
                return WindowsTaskBarLocation.Left;

            if (SystemParameters.WorkArea.Top > 0)
                return WindowsTaskBarLocation.Top;

            if (SystemParameters.WorkArea.Left == 0 &&
                SystemParameters.WorkArea.Width < SystemParameters.PrimaryScreenWidth)
                return WindowsTaskBarLocation.Right;

            return WindowsTaskBarLocation.Bottom;
        }

    }
}
