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
            .filter(item => item.offsetParent !== null || item === document.activeElement);
    }

    function focusFirst(element) {
        const focusable = getFocusable(element);
        const target = focusable[0] || element;
        target.focus({ preventScroll: true });
    }

    window.xfDialog = {
        trap(dialogId) {
            const element = document.getElementById(dialogId);
            if (!element || traps.has(dialogId)) {
                return;
            }

            const previous = document.activeElement instanceof HTMLElement
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
            traps.set(dialogId, { element, previous, onKeyDown });

            requestAnimationFrame(() => focusFirst(element));
        },

        release(dialogId) {
            const trap = traps.get(dialogId);
            if (!trap) {
                return;
            }

            trap.element.removeEventListener('keydown', trap.onKeyDown);
            traps.delete(dialogId);

            if (trap.previous && document.contains(trap.previous)) {
                requestAnimationFrame(() => trap.previous.focus({ preventScroll: true }));
            }
        }
    };
})();
