(function () {
    var registrations = new Map();

    function clamp(value, min, max) {
        return Math.min(Math.max(value, min), max);
    }

    function snap(value, step) {
        if (!step || step <= 0) {
            return value;
        }
        return Math.round(value / step) * step;
    }

    function getValue(root, event, options) {
        var rect = root.getBoundingClientRect();
        var raw = options.orientation === 'vertical'
            ? ((event.clientY - rect.top) / rect.height) * 100
            : ((event.clientX - rect.left) / rect.width) * 100;

        return clamp(snap(raw, options.step), options.min, options.max);
    }

    function applyGrid(root, value, orientation) {
        if (orientation === 'vertical') {
            root.style.gridTemplateRows = value + 'fr auto ' + (100 - value) + 'fr';
        } else {
            root.style.gridTemplateColumns = value + 'fr auto ' + (100 - value) + 'fr';
        }
    }

    function release(handleId) {
        var registration = registrations.get(handleId);
        if (!registration) {
            return;
        }
        registration.handle.removeEventListener('pointerdown', registration.onPointerDown);
        registrations.delete(handleId);
    }

    window.xfSplitter = {
        register: function (rootId, handleId, dotNetReference, options) {
            release(handleId);

            var root = document.getElementById(rootId);
            var handle = document.getElementById(handleId);
            if (!root || !handle) {
                return;
            }

            var orientation = options.orientation || 'horizontal';
            var settings = {
                orientation: orientation,
                min: Number(options.min || 0),
                max: Number(options.max || 100),
                step: Number(options.step || 1)
            };

            var onPointerDown = function (event) {
                if (event.button !== undefined && event.button !== 0) {
                    return;
                }

                event.preventDefault();
                root.classList.add('xf-splitter--resizing');

                if (handle.setPointerCapture) {
                    handle.setPointerCapture(event.pointerId);
                }

                var move = function (moveEvent) {
                    moveEvent.preventDefault();
                    var value = getValue(root, moveEvent, settings);

                    // Apply CSS grid directly for immediate visual feedback
                    applyGrid(root, value, orientation);

                    // Keep C# state in sync without re-render
                    dotNetReference.invokeMethodAsync('SetSizeFromJsAsync', value, false);
                };

                var end = function (endEvent) {
                    root.classList.remove('xf-splitter--resizing');
                    document.removeEventListener('pointermove', move);
                    document.removeEventListener('pointerup', end);
                    document.removeEventListener('pointercancel', end);

                    if (handle.releasePointerCapture) {
                        try {
                            handle.releasePointerCapture(endEvent.pointerId);
                        } catch (_error) {
                            // Pointer capture may already be released by the browser.
                        }
                    }

                    var value = getValue(root, endEvent, settings);

                    // Apply final CSS grid position
                    applyGrid(root, value, orientation);

                    // Notify C# with final value (triggers StateHasChanged for consistency)
                    dotNetReference.invokeMethodAsync('SetSizeFromJsAsync', value, true);
                };

                document.addEventListener('pointermove', move, { passive: false });
                document.addEventListener('pointerup', end, { passive: false });
                document.addEventListener('pointercancel', end, { passive: false });
            };

            handle.addEventListener('pointerdown', onPointerDown);
            registrations.set(handleId, { handle: handle, onPointerDown: onPointerDown });
        },

        release: release
    };
})();
