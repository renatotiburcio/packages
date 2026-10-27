(function () {
    const traps = new Map();
    const focusableSelector = [
        'a[href]',
        'area[href]',
        'button:not([disabled])',
        'input:not([disabled])',
        'select:not([disabled])',
        'textarea:not([disabled])',
        '[tabindex]:not([tabindex="-1"])',
        '[contenteditable="true"]'
    ].join(',');

    function getFocusable(element) {
        return Array.from(element.querySelectorAll(focusableSelector))
            .filter(item => !item.hasAttribute('disabled') && (item.offsetParent !== null || item === document.activeElement));
    }

    function focusTarget(element, selector) {
        const requested = selector ? element.querySelector(selector) : null;
        const focusable = getFocusable(element);
        const target = requested || focusable[0] || element;

        if (target && typeof target.focus === 'function') {
            target.focus({ preventScroll: true });
        }
    }

    window.xfFocusTrap = {
        activate(elementId, options) {
            const element = document.getElementById(elementId);
            if (!element) {
                return;
            }

            if (traps.has(elementId)) {
                this.release(elementId, false);
            }

            const settings = options || {};
            const previous = settings.restoreFocus !== false && document.activeElement instanceof HTMLElement
                ? document.activeElement
                : null;

            const onKeyDown = event => {
                if (event.key !== 'Tab') {
                    return;
                }

                const focusable = getFocusable(element);
                if (focusable.length === 0) {
                    event.preventDefault();
                    element.focus({ preventScroll: true });
                    return;
                }

                const first = focusable[0];
                const last = focusable[focusable.length - 1];

                if (event.shiftKey && document.activeElement === first) {
                    event.preventDefault();
                    last.focus({ preventScroll: true });
                } else if (!event.shiftKey && document.activeElement === last) {
                    event.preventDefault();
                    first.focus({ preventScroll: true });
                }
            };

            element.addEventListener('keydown', onKeyDown);
            traps.set(elementId, { element, previous, onKeyDown });

            if (settings.focusOnActivate !== false) {
                requestAnimationFrame(() => focusTarget(element, settings.initialFocusSelector));
            }
        },

        focusInitial(elementId, selector) {
            const element = document.getElementById(elementId);
            if (element) {
                requestAnimationFrame(() => focusTarget(element, selector));
            }
        },

        release(elementId, restoreFocus) {
            const trap = traps.get(elementId);
            if (!trap) {
                return;
            }

            trap.element.removeEventListener('keydown', trap.onKeyDown);
            traps.delete(elementId);

            if (restoreFocus !== false && trap.previous && document.contains(trap.previous)) {
                requestAnimationFrame(() => trap.previous.focus({ preventScroll: true }));
            }
        }
    };
})();
