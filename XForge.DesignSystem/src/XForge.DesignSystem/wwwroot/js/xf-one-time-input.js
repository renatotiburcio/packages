(function () {
    'use strict';

    function focusSlot(id) {
        if (!id) {
            return;
        }

        var element = document.getElementById(id);
        if (!element || typeof element.focus !== 'function') {
            return;
        }

        requestAnimationFrame(function () {
            element.focus({ preventScroll: true });
            if (typeof element.select === 'function') {
                element.select();
            }
        });
    }

    window.xfOneTimeInput = {
        focusSlot: focusSlot
    };
})();
