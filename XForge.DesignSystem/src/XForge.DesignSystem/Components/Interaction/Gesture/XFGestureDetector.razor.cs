using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Interaction.Gesture
{
    public partial class XFGestureDetector : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        protected override string BaseClass => "xf-gesture-detector";

        private double _startX;
        private double _startY;
        private DateTime _startTime;
        private bool _isTracking;
        private bool _isLongPressTriggered;
        private long _pointerId = -1;
        private Timer? _longPressTimer;

        // Pinch state
        private double _initialPinchDistance;
        private bool _isPinching;

        #region Parameters

        /// <summary>Whether tap detection is enabled.</summary>
        [Parameter] public bool EnableTap { get; set; } = true;

        /// <summary>Whether double-tap detection is enabled.</summary>
        [Parameter] public bool EnableDoubleTap { get; set; }

        /// <summary>Whether long-press detection is enabled.</summary>
        [Parameter] public bool EnableLongPress { get; set; }

        /// <summary>Whether swipe detection is enabled.</summary>
        [Parameter] public bool EnableSwipe { get; set; } = true;

        /// <summary>Whether pinch/zoom detection is enabled.</summary>
        [Parameter] public bool EnablePinch { get; set; }

        /// <summary>Whether pan (drag) detection is enabled.</summary>
        [Parameter] public bool EnablePan { get; set; }

        /// <summary>Long-press duration in milliseconds.</summary>
        [Parameter] public int LongPressDelay { get; set; } = 500;

        /// <summary>Minimum swipe distance in pixels.</summary>
        [Parameter] public double SwipeThreshold { get; set; } = 50;

        /// <summary>Tap callback.</summary>
        [Parameter] public EventCallback<XFGestureTapEventArgs> OnTap { get; set; }

        /// <summary>Double-tap callback.</summary>
        [Parameter] public EventCallback<XFGestureTapEventArgs> OnDoubleTap { get; set; }

        /// <summary>Long-press callback.</summary>
        [Parameter] public EventCallback<XFGestureLongPressEventArgs> OnLongPress { get; set; }

        /// <summary>Swipe callback.</summary>
        [Parameter] public EventCallback<XFGestureSwipeEventArgs> OnSwipe { get; set; }

        /// <summary>Pinch callback.</summary>
        [Parameter] public EventCallback<XFGesturePinchEventArgs> OnPinch { get; set; }

        /// <summary>Pan callback.</summary>
        [Parameter] public EventCallback<XFGesturePanEventArgs> OnPan { get; set; }

        /// <summary>Child content to wrap.</summary>
        [Parameter] public RenderFragment? ChildContent { get; set; }

        // assinatura de render: pula render redundante quando o pai re-renderiza sem mudar nada aqui
        private string? _renderSignature;

        protected override bool ShouldRender()
        {
            var signature = string.Join('', EnableTap, EnableDoubleTap, EnableLongPress, EnableSwipe, EnablePinch, EnablePan, LongPressDelay, SwipeThreshold, OnTap, OnDoubleTap, OnLongPress, OnSwipe, OnPinch, OnPan, ChildContent);

            if (_renderSignature == signature)
            {
                return false;
            }

            _renderSignature = signature;
            return true;
        }

        #endregion

        #region Pointer Events

        private void OnPointerDown(PointerEventArgs e)
        {
            if (_pointerId >= 0) return;

            _pointerId = e.PointerId;
            _startX = e.ClientX;
            _startY = e.ClientY;
            _startTime = DateTime.UtcNow;
            _isTracking = true;
            _isLongPressTriggered = false;

            if (EnableLongPress)
            {
                _longPressTimer?.Dispose();
                _longPressTimer = new Timer(_ =>
                {
                    if (_isTracking && !_isLongPressTriggered)
                    {
                        _isLongPressTriggered = true;
                        InvokeAsync(() => OnLongPress.InvokeAsync(new XFGestureLongPressEventArgs
                        {
                            X = _startX,
                            Y = _startY,
                            Duration = LongPressDelay
                        }));
                    }
                }, null, LongPressDelay, Timeout.Infinite);
            }
        }

        private async Task OnPointerUp(PointerEventArgs e)
        {
            if (!_isTracking || e.PointerId != _pointerId) return;

            _longPressTimer?.Dispose();
            _isTracking = false;
            var pointerId = _pointerId;
            _pointerId = -1;

            if (_isLongPressTriggered) return;

            var deltaX = e.ClientX - _startX;
            var deltaY = e.ClientY - _startY;
            var distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            var duration = (DateTime.UtcNow - _startTime).TotalMilliseconds;

            // Swipe
            if (EnableSwipe && distance >= SwipeThreshold)
            {
                var direction = GetSwipeDirection(deltaX, deltaY);
                await OnSwipe.InvokeAsync(new XFGestureSwipeEventArgs
                {
                    Direction = direction,
                    DistanceX = deltaX,
                    DistanceY = deltaY,
                    Velocity = distance / Math.Max(duration, 1) * 1000,
                    Duration = duration
                });
                return;
            }

            // Tap
            if (EnableTap && distance < SwipeThreshold && duration < LongPressDelay)
            {
                await OnTap.InvokeAsync(new XFGestureTapEventArgs
                {
                    X = e.ClientX,
                    Y = e.ClientY,
                    Duration = duration
                });
            }
        }

        private async Task OnPointerMove(PointerEventArgs e)
        {
            if (!_isTracking || e.PointerId != _pointerId) return;

            var deltaX = e.ClientX - _startX;
            var deltaY = e.ClientY - _startY;

            if (EnablePan && !_isLongPressTriggered)
            {
                await OnPan.InvokeAsync(new XFGesturePanEventArgs
                {
                    DeltaX = deltaX,
                    DeltaY = deltaY,
                    TotalX = deltaX,
                    TotalY = deltaY
                });
            }
        }

        private void OnPointerCancel(PointerEventArgs e)
        {
            _longPressTimer?.Dispose();
            _isTracking = false;
            _pointerId = -1;
        }

        #endregion

        #region Wheel

        private async Task OnWheel(WheelEventArgs e)
        {
            if (!EnablePinch) return;

            await OnPinch.InvokeAsync(new XFGesturePinchEventArgs
            {
                Scale = e.DeltaY > 0 ? 0.9 : 1.1,
                CenterX = e.ClientX,
                CenterY = e.ClientY
            });
        }

        #endregion

        #region Helpers

        private static XFGestureSwipeDirection GetSwipeDirection(double deltaX, double deltaY)
        {
            if (Math.Abs(deltaX) > Math.Abs(deltaY))
                return deltaX > 0 ? XFGestureSwipeDirection.Right : XFGestureSwipeDirection.Left;
            else
                return deltaY > 0 ? XFGestureSwipeDirection.Down : XFGestureSwipeDirection.Up;
        }

        #endregion
    }

    #region Supporting types

    public enum XFGestureSwipeDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    public class XFGestureTapEventArgs
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Duration { get; set; }
    }

    public class XFGestureLongPressEventArgs
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int Duration { get; set; }
    }

    public class XFGestureSwipeEventArgs
    {
        public XFGestureSwipeDirection Direction { get; set; }
        public double DistanceX { get; set; }
        public double DistanceY { get; set; }
        public double Velocity { get; set; }
        public double Duration { get; set; }
    }

    public class XFGesturePinchEventArgs
    {
        public double Scale { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
    }

    public class XFGesturePanEventArgs
    {
        public double DeltaX { get; set; }
        public double DeltaY { get; set; }
        public double TotalX { get; set; }
        public double TotalY { get; set; }
    }

    #endregion
}
