/**
 * XF Color Picker — ES module for interactive HSV color picking.
 *
 * Provides pointer-based drag handling for the saturation/value (SV) area
 * and hue slider, reporting normalized coordinates back to .NET via
 * DotNetObjectReference.
 *
 * Usage from C# via IJSRuntime:
 *   var module = await JS.InvokeAsync<IJSObjectReference>("import", "/_content/XForge.DesignSystem/js/xf-color-picker.js");
 *   await module.InvokeVoidAsync("xfColorPicker.initSvPicker", svAreaRef, dotNetRef);
 *   await module.InvokeVoidAsync("xfColorPicker.initHueSlider", hueRef, dotNetRef);
 */

const xfColorPicker = {
    /**
     * Initialize pointer tracking on the saturation/value (SV) picker area.
     * Calls dotNetRef.OnSvDrag(saturation, value) on pointer move,
     * and dotNetRef.OnSvDragEnd() on pointer up.
     *
     * @param {HTMLElement} el — SV area element
     * @param {object} dotNetRef — .NET DotNetObjectReference
     */
    initSvPicker(el, dotNetRef) {
        if (!el || !dotNetRef) return;
        this._initPointer(el, dotNetRef, 'sv');
    },

    /**
     * Initialize pointer tracking on the hue slider.
     * Calls dotNetRef.OnHueDrag(hue) on pointer move,
     * and dotNetRef.OnHueDragEnd() on pointer up.
     *
     * @param {HTMLElement} el — Hue slider element
     * @param {object} dotNetRef — .NET DotNetObjectReference
     */
    initHueSlider(el, dotNetRef) {
        if (!el || !dotNetRef) return;
        this._initPointer(el, dotNetRef, 'hue');
    },

    /**
     * Initialize HSV picker by finding elements via component root ID.
     * Used when @ref timing is unreliable (e.g., inside nested @if blocks).
     * @param {string} componentId — The component root element ID
     * @param {object} dotNetRef — .NET DotNetObjectReference
     */
    initBySelector(componentId, dotNetRef) {
        if (!componentId || !dotNetRef) return;
        const root = document.getElementById(componentId);
        if (!root) return;
        
        const svArea = root.querySelector('[role="slider"][aria-label*="aturation"], [role="slider"][aria-label*="aturac"]');
        const hueSlider = root.querySelectorAll('[role="slider"]');
        // SV area is the first slider (has the gradient), hue is the second
        const sliders = root.querySelectorAll('[role="slider"]');
        if (sliders.length >= 2) {
            this.initSvPicker(sliders[0], dotNetRef);
            this.initHueSlider(sliders[1], dotNetRef);
        } else if (sliders.length === 1) {
            // Only one slider - could be SV or hue depending on config
            this.initSvPicker(sliders[0], dotNetRef);
        }
    },

    /**
     * Clean up event listeners for an element.
     * @param {HTMLElement} el
     */
    dispose(el) {
        if (el && el._xfColorPickerCleanup) {
            el._xfColorPickerCleanup();
            delete el._xfColorPickerCleanup;
        }
    },

    /**
     * @param {HTMLElement} el
     * @param {object} dotNetRef
     * @param {'sv'|'hue'} mode
     */
    _initPointer(el, dotNetRef, mode) {
        // Clean up any previous listener
        if (el._xfColorPickerCleanup) {
            el._xfColorPickerCleanup();
        }

        const self = this;

        const onPointerDown = (e) => {
            e.preventDefault();
            el.setPointerCapture(e.pointerId);
            self._handleMove(e, el, dotNetRef, mode);
        };

        const onPointerMove = (e) => {
            // Only respond when pointer is captured (button pressed)
            if (!(el.hasPointerCapture && el.hasPointerCapture(e.pointerId))) return;
            self._handleMove(e, el, dotNetRef, mode);
        };

        const onPointerUp = (e) => {
            if (el.hasPointerCapture && el.hasPointerCapture(e.pointerId)) {
                el.releasePointerCapture(e.pointerId);
            }
            if (mode === 'sv') {
                dotNetRef.invokeMethodAsync('OnSvDragEnd').catch(() => {});
            } else {
                dotNetRef.invokeMethodAsync('OnHueDragEnd').catch(() => {});
            }
        };

        el.addEventListener('pointerdown', onPointerDown);
        el.addEventListener('pointermove', onPointerMove);
        el.addEventListener('pointerup', onPointerUp);

        // Store cleanup function
        el._xfColorPickerCleanup = () => {
            el.removeEventListener('pointerdown', onPointerDown);
            el.removeEventListener('pointermove', onPointerMove);
            el.removeEventListener('pointerup', onPointerUp);
        };
    },

    /**
     * Calculate normalized position and invoke the appropriate .NET callback.
     * @param {PointerEvent} e
     * @param {HTMLElement} el
     * @param {object} dotNetRef
     * @param {'sv'|'hue'} mode
     */
    _handleMove(e, el, dotNetRef, mode) {
        const rect = el.getBoundingClientRect();
        const x = Math.max(0, Math.min(1, (e.clientX - rect.left) / rect.width));
        const y = Math.max(0, Math.min(1, (e.clientY - rect.top) / rect.height));

        if (mode === 'sv') {
            // Saturation = x (0 left, 1 right), Value = 1-y (1 top, 0 bottom)
            dotNetRef.invokeMethodAsync('OnSvDrag', x, 1 - y).catch(() => {});
        } else {
            // Hue = x * 360
            dotNetRef.invokeMethodAsync('OnHueDrag', x * 360).catch(() => {});
        }
    }
};

export { xfColorPicker };
