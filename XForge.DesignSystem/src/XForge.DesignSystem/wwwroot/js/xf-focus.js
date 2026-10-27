/**
 * XF Focus utilities — ES module for programmatic focus/select/blur.
 *
 * Usage from C# via IJSRuntime:
 *   await JS.InvokeVoidAsync("xfFocus.focus", elementRef);
 *   await JS.InvokeVoidAsync("xfFocus.select", elementRef);
 *   await JS.InvokeVoidAsync("xfFocus.blur", elementRef);
 */

const xfFocus = {
    /**
     * Focus an element.
     * @param {HTMLElement} el
     * @param {object} [options]
     * @param {boolean} [options.preventScroll]
     */
    focus(el, options) {
        if (el && typeof el.focus === 'function') {
            el.focus(options);
        }
    },

    /**
     * Select all text in an input or textarea.
     * @param {HTMLInputElement|HTMLTextAreaElement} el
     */
    select(el) {
        if (el && typeof el.select === 'function') {
            el.select();
        }
    },

    /**
     * Remove focus from an element.
     * @param {HTMLElement} el
     */
    blur(el) {
        if (el && typeof el.blur === 'function') {
            el.blur();
        }
    }
};

export default xfFocus;
