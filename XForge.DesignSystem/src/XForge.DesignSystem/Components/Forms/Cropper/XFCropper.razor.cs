using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using XForge.DesignSystem.Core.Base;
using XForge.Localization;

namespace XForge.DesignSystem.Components.Forms.Cropper
{
    public partial class XFCropper : XForgeComponentBase
    {
        [Inject] private IXFLocalizer? Localizer { get; set; }
        private string L(string key, string fallback) => Localizer?[key] ?? fallback;

        protected override string BaseClass => "xf-cropper";

        // Image state
        private bool HasImage => !string.IsNullOrEmpty(Source);
        private bool IsLoading;
        private string? ErrorMessage;
        private double _naturalWidth;
        private double _naturalHeight;

        // Transform state
        private double _zoom = 100;
        private double _rotation = 0;

        // Crop area state
        private double _cropX;
        private double _cropY;
        private double _cropWidth;
        private double _cropHeight;

        // Drag state
        private bool _isDragging;
        private CropDragMode _dragMode;
        private double _dragStartX;
        private double _dragStartY;
        private double _dragOriginalX;
        private double _dragOriginalY;
        private double _dragOriginalWidth;
        private double _dragOriginalHeight;
        private CropResizeDirection _resizeDirection;

        // Preview
        private byte[]? _previewImageBytes;

        #region Parameters

        /// <summary>Image source URL.</summary>
        [Parameter]
        public string Source { get; set; } = "";

        /// <summary>Maximum display width in pixels.</summary>
        [Parameter]
        public int MaxWidth { get; set; } = 600;

        /// <summary>Maximum display height in pixels.</summary>
        [Parameter]
        public int MaxHeight { get; set; } = 400;

        /// <summary>Initial crop aspect ratio. null = free form.</summary>
        [Parameter]
        public double? InitialAspectRatio { get; set; }

        /// <summary>Minimum zoom percentage.</summary>
        [Parameter]
        public int MinZoom { get; set; } = 25;

        /// <summary>Maximum zoom percentage.</summary>
        [Parameter]
        public int MaxZoom { get; set; } = 400;

        /// <summary>Output format for cropped image.</summary>
        [Parameter]
        public XFCropperOutputFormat OutputFormat { get; set; } = XFCropperOutputFormat.Png;

        /// <summary>Output quality (1-100) for JPEG/WebP.</summary>
        [Parameter]
        public int OutputQuality { get; set; } = 90;

        /// <summary>Whether to show the preview.</summary>
        [Parameter]
        public bool ShowPreview { get; set; } = true;

        /// <summary>Whether to show the rule-of-thirds grid.</summary>
        [Parameter]
        public bool ShowGrid { get; set; } = true;

        /// <summary>Whether to allow free-form crop (no aspect ratio lock).</summary>
        [Parameter]
        public bool AllowFreeCrop { get; set; } = true;

        /// <summary>Custom aspect ratios.</summary>
        [Parameter]
        public IReadOnlyList<XFCropperAspectRatio> CustomAspectRatios { get; set; } = Array.Empty<XFCropperAspectRatio>();

        /// <summary>Alt text for the image.</summary>
        [Parameter]
        public string ImageAlt { get; set; } = "Image to crop";

        /// <summary>Crop callback with result.</summary>
        [Parameter]
        public EventCallback<XFCropperResult> OnCropComplete { get; set; }

        /// <summary>Cancel callback.</summary>
        [Parameter]
        public EventCallback OnCancel { get; set; }

        /// <summary>Zoom change callback.</summary>
        [Parameter]
        public EventCallback<double> OnZoomChanged2 { get; set; }

        /// <summary>Rotation change callback.</summary>
        [Parameter]
        public EventCallback<double> OnRotationChanged2 { get; set; }

        #endregion

        #region Labels (localizable)

        private string AriaLabel => L("cropper.aria_label", "Image cropper");
        private string LoadingText => L("cropper.loading", "Loading image...");
        private string ZoomText => L("cropper.zoom", "Zoom");
        private string ZoomInText => L("cropper.zoom_in", "Zoom in");
        private string ZoomOutText => L("cropper.zoom_out", "Zoom out");
        private string RotationText => L("cropper.rotation", "Rotation");
        private string RotateLeftText => L("cropper.rotate_left", "Rotate left");
        private string RotateRightText => L("cropper.rotate_right", "Rotate right");
        private string AspectRatioText => L("cropper.aspect_ratio", "Aspect ratio");
        private string FreeText => L("cropper.free", "Free");
        private string CropText => L("cropper.crop", "Crop");
        private string CancelText => L("cropper.cancel", "Cancel");
        private string PreviewText => L("cropper.preview", "Preview");
        private string NoImageText => L("cropper.no_image", "No image loaded");
        private string UploadText => L("cropper.upload", "Load image");

        #endregion

        #region Computed

        private IReadOnlyList<XFCropperAspectRatio> AspectRatios
        {
            get
            {
                var ratios = new List<XFCropperAspectRatio>();
                if (AllowFreeCrop)
                    ratios.Add(new XFCropperAspectRatio(null, FreeText));
                ratios.Add(new XFCropperAspectRatio(1, "1:1"));
                ratios.Add(new XFCropperAspectRatio(4.0 / 3, "4:3"));
                ratios.Add(new XFCropperAspectRatio(16.0 / 9, "16:9"));
                ratios.Add(new XFCropperAspectRatio(3.0 / 2, "3:2"));
                ratios.Add(new XFCropperAspectRatio(2.0 / 3, "2:3"));
                if (CustomAspectRatios.Count > 0)
                    ratios.AddRange(CustomAspectRatios);
                return ratios;
            }
        }

        private bool IsCropping;

        private double ImageDisplayWidth
        {
            get
            {
                if (_naturalWidth == 0 || _naturalHeight == 0) return MaxWidth;
                var ratio = _naturalWidth / _naturalHeight;
                if (ratio > (double)MaxWidth / MaxHeight)
                    return MaxWidth;
                return MaxHeight * ratio;
            }
        }

        private double ImageDisplayHeight
        {
            get
            {
                if (_naturalWidth == 0 || _naturalHeight == 0) return MaxHeight;
                var ratio = _naturalWidth / _naturalHeight;
                if (ratio > (double)MaxWidth / MaxHeight)
                    return MaxWidth / ratio;
                return MaxHeight;
            }
        }

        private string CropAreaClass => string.Join(" ",
            "relative overflow-hidden rounded-lg border border-border-light bg-gray-900 cursor-crosshair",
            $"w-[{ImageDisplayWidth:F0}px] h-[{ImageDisplayHeight:F0}px]");

        private string ZoomButtonClass => "inline-flex h-8 w-8 items-center justify-center rounded-lg border border-border-light bg-bg-surface text-text-secondary hover:border-[var(--color-primary)] hover:text-primary disabled:opacity-50 disabled:cursor-not-allowed transition-colors";

        private string RotateButtonClass => "inline-flex h-8 w-8 items-center justify-center rounded-lg border border-border-light bg-bg-surface text-text-secondary hover:border-[var(--color-primary)] hover:text-primary transition-colors";

        private string AspectRatioButtonClass(XFCropperAspectRatio ratio)
        {
            var isActive = _currentAspectRatio?.Equals(ratio) == true;
            return isActive
                ? "rounded-md bg-[var(--color-primary)] px-2 py-1 text-xs font-bold text-white"
                : "rounded-md bg-surface-100 px-2 py-1 text-xs font-semibold text-text-secondary hover:bg-surface-200 transition-colors";
        }

        private string CancelButtonClass => "inline-flex items-center justify-center rounded-lg border border-border-light bg-bg-surface px-4 py-2 text-sm font-semibold text-text-secondary hover:border-[var(--color-primary)] hover:text-primary transition-colors disabled:opacity-50";

        private string CropButtonClass => "inline-flex items-center justify-center rounded-lg bg-[var(--color-primary)] px-4 py-2 text-sm font-bold text-white hover:opacity-90 transition-opacity disabled:opacity-50";

        private string EmptyStateClass => "flex flex-col items-center justify-center gap-3 rounded-lg border-2 border-dashed border-border-light p-8 text-center";

        private string UploadButtonClass => "rounded-lg bg-[var(--color-primary)] px-4 py-2 text-sm font-bold text-white hover:opacity-90 transition-opacity";

        private XFCropperAspectRatio? _currentAspectRatio;

        #endregion

        #region Lifecycle

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(Source) && _naturalWidth == 0)
            {
                await LoadImage();
            }
        }

        #endregion

        #region Image Loading

        private async Task LoadImage()
        {
            IsLoading = true;
            ErrorMessage = null;
            StateHasChanged();

            try
            {
                // Use JS interop to get natural dimensions
                // For now, set defaults and let the image load event handle it
                _naturalWidth = MaxWidth;
                _naturalHeight = MaxHeight;

                // Initialize crop area
                InitializeCropArea();

                IsLoading = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                IsLoading = false;
                ErrorMessage = L("cropper.load_error", $"Failed to load image: {ex.Message}");
                StateHasChanged();
            }
        }

        private void InitializeCropArea()
        {
            var displayW = ImageDisplayWidth;
            var displayH = ImageDisplayHeight;

            if (InitialAspectRatio.HasValue)
            {
                var ratio = InitialAspectRatio.Value;
                if (ratio > displayW / displayH)
                {
                    _cropWidth = displayW * 0.8;
                    _cropHeight = _cropWidth / ratio;
                }
                else
                {
                    _cropHeight = displayH * 0.8;
                    _cropWidth = _cropHeight * ratio;
                }
            }
            else
            {
                _cropWidth = displayW * 0.8;
                _cropHeight = displayH * 0.8;
            }

            _cropX = (displayW - _cropWidth) / 2;
            _cropY = (displayH - _cropHeight) / 2;
        }

        #endregion

        #region Crop Interaction

        private void OnCropPointerDown(PointerEventArgs e)
        {
            if (IsCropping) return;

            _isDragging = true;
            _dragMode = CropDragMode.Move;
            _dragStartX = e.ClientX;
            _dragStartY = e.ClientY;
            _dragOriginalX = _cropX;
            _dragOriginalY = _cropY;
        }

        private void OnCropPointerMove(PointerEventArgs e)
        {
            if (!_isDragging || IsCropping) return;

            var dx = e.ClientX - _dragStartX;
            var dy = e.ClientY - _dragStartY;

            if (_dragMode == CropDragMode.Move)
            {
                _cropX = Math.Clamp(_dragOriginalX + dx, 0, ImageDisplayWidth - _cropWidth);
                _cropY = Math.Clamp(_dragOriginalY + dy, 0, ImageDisplayHeight - _cropHeight);
            }
            else if (_dragMode == CropDragMode.Resize)
            {
                ApplyResize(dx, dy);
            }

            StateHasChanged();
        }

        private void OnCropPointerUp(PointerEventArgs e)
        {
            _isDragging = false;
            _dragMode = CropDragMode.None;
        }

        private void StartResize(CropResizeDirection direction)
        {
            _isDragging = true;
            _dragMode = CropDragMode.Resize;
            _resizeDirection = direction;
            _dragStartX = 0; // Will be set on next move
            _dragStartY = 0;
            _dragOriginalX = _cropX;
            _dragOriginalY = _cropY;
            _dragOriginalWidth = _cropWidth;
            _dragOriginalHeight = _cropHeight;
        }

        private void ApplyResize(double dx, double dy)
        {
            const double minSize = 30;
            var newWidth = _dragOriginalWidth;
            var newHeight = _dragOriginalHeight;
            var newX = _dragOriginalX;
            var newY = _dragOriginalY;

            if (_resizeDirection.HasFlag(CropResizeDirection.Right))
                newWidth = Math.Clamp(_dragOriginalWidth + dx, minSize, ImageDisplayWidth - _dragOriginalX);
            if (_resizeDirection.HasFlag(CropResizeDirection.Left))
            {
                newWidth = Math.Clamp(_dragOriginalWidth - dx, minSize, _dragOriginalX + _dragOriginalWidth);
                newX = _dragOriginalX + _dragOriginalWidth - newWidth;
            }
            if (_resizeDirection.HasFlag(CropResizeDirection.Bottom))
                newHeight = Math.Clamp(_dragOriginalHeight + dy, minSize, ImageDisplayHeight - _dragOriginalY);
            if (_resizeDirection.HasFlag(CropResizeDirection.Top))
            {
                newHeight = Math.Clamp(_dragOriginalHeight - dy, minSize, _dragOriginalY + _dragOriginalHeight);
                newY = _dragOriginalY + _dragOriginalHeight - newHeight;
            }

            // Maintain aspect ratio if locked
            if (_currentAspectRatio?.Ratio.HasValue == true)
            {
                var ratio = _currentAspectRatio.Value.Ratio.Value;
                if (_resizeDirection is CropResizeDirection.Left or CropResizeDirection.Right)
                    newHeight = newWidth / ratio;
                else
                    newWidth = newHeight * ratio;
            }

            _cropWidth = newWidth;
            _cropHeight = newHeight;
            _cropX = newX;
            _cropY = newY;
        }

        #endregion

        #region Zoom & Rotation

        private void OnZoomChanged(ChangeEventArgs e)
        {
            if (double.TryParse(e.Value?.ToString(), out var value))
            {
                _zoom = Math.Clamp(value, MinZoom, MaxZoom);
                UpdatePreview();
                StateHasChanged();
            }
        }

        private void ZoomIn()
        {
            _zoom = Math.Min(_zoom + 10, MaxZoom);
            UpdatePreview();
        }

        private void ZoomOut()
        {
            _zoom = Math.Max(_zoom - 10, MinZoom);
            UpdatePreview();
        }

        private void OnRotationChanged(ChangeEventArgs e)
        {
            if (double.TryParse(e.Value?.ToString(), out var value))
            {
                _rotation = Math.Clamp(value, -180, 180);
                UpdatePreview();
                StateHasChanged();
            }
        }

        private void RotateLeft()
        {
            _rotation = Math.Max(_rotation - 90, -180);
            UpdatePreview();
        }

        private void RotateRight()
        {
            _rotation = Math.Min(_rotation + 90, 180);
            UpdatePreview();
        }

        #endregion

        #region Aspect Ratio

        private void SetAspectRatio(XFCropperAspectRatio ratio)
        {
            _currentAspectRatio = ratio;
            if (ratio.Ratio.HasValue)
            {
                var r = ratio.Ratio.Value;
                var centerX = _cropX + _cropWidth / 2;
                var centerY = _cropY + _cropHeight / 2;

                if (_cropWidth / _cropHeight > r)
                    _cropWidth = _cropHeight * r;
                else
                    _cropHeight = _cropWidth / r;

                _cropX = Math.Clamp(centerX - _cropWidth / 2, 0, ImageDisplayWidth - _cropWidth);
                _cropY = Math.Clamp(centerY - _cropHeight / 2, 0, ImageDisplayHeight - _cropHeight);
            }
            StateHasChanged();
        }

        #endregion

        #region Actions

        private async Task OnCrop()
        {
            if (IsCropping) return;
            IsCropping = true;
            StateHasChanged();

            try
            {
                // Calculate actual crop coordinates relative to natural image
                var scaleX = _naturalWidth / ImageDisplayWidth;
                var scaleY = _naturalHeight / ImageDisplayHeight;

                var result = new XFCropperResult
                {
                    X = (int)(_cropX * scaleX),
                    Y = (int)(_cropY * scaleY),
                    Width = (int)(_cropWidth * scaleX),
                    Height = (int)(_cropHeight * scaleY),
                    Zoom = _zoom,
                    Rotation = _rotation,
                    OutputFormat = OutputFormat,
                    Quality = OutputQuality,
                    PreviewImage = _previewImageBytes
                };

                await OnCropComplete.InvokeAsync(result);
            }
            finally
            {
                IsCropping = false;
                StateHasChanged();
            }
        }

        private async Task HandleCancel()
        {
            await OnCancel.InvokeAsync();
        }

        private void UpdatePreview()
        {
            // Preview generation would happen client-side via JS interop
            // For now, null preview - the component will work with server-side rendering
            // and the actual crop will be handled by the consumer
        }

        private static string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            return $"{bytes / (1024.0 * 1024.0):F1} MB";
        }

        #endregion
    }

    #region Supporting types

    public enum XFCropperOutputFormat
    {
        Png,
        Jpeg,
        WebP
    }

    public enum CropDragMode
    {
        None,
        Move,
        Resize
    }

    [Flags]
    public enum CropResizeDirection
    {
        None = 0,
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8
    }

    public record struct XFCropperAspectRatio(double? Ratio, string Label);

    public class XFCropperResult
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double Zoom { get; set; }
        public double Rotation { get; set; }
        public XFCropperOutputFormat OutputFormat { get; set; }
        public int Quality { get; set; }
        public byte[]? PreviewImage { get; set; }
    }

    #endregion
}
