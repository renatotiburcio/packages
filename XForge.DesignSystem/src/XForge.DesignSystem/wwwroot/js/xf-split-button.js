(function () {
    var registrations = new Map();

    function position(trigger, panel) {
        var rect = trigger.getBoundingClientRect();
        var panelRect = panel.getBoundingClientRect();
        var padding = 8;
        var top = rect.bottom + 8;
        var left = rect.right - panelRect.width;

        if (top + panelRect.height > window.innerHeight - padding) {
            top = Math.max(padding, rect.top - panelRect.height - 8);
        }

        left = Math.min(Math.max(padding, left), window.innerWidth - panelRect.width - padding);
        panel.style.top = top + 'px';
        panel.style.left = left + 'px';
    }

    function release(panelId) {
        var registration = registrations.get(panelId);
        if (!registration) {
            return;
        }

        document.removeEventListener('pointerdown', registration.onPointerDown, true);
        document.removeEventListener('keydown', registration.onKeyDown, true);
        window.removeEventListener('resize', registration.onReposition);
        window.removeEventListener('scroll', registration.onReposition, true);
        registrations.delete(panelId);
    }

    window.xfSplitButton = {
        register: function (triggerId, panelId, dotNetReference) {
            release(panelId);

            var trigger = document.getElementById(triggerId);
            var panel = document.getElementById(panelId);
            if (!trigger || !panel) {
                return;
            }

            var onReposition = function () {
                position(trigger, panel);
            };

            var onPointerDown = function (event) {
                if (panel.contains(event.target) || trigger.contains(event.target)) {
                    return;
                }

                dotNetReference.invokeMethodAsync('CloseFromJsAsync');
            };

            var onKeyDown = function (event) {
                if (event.key === 'Escape') {
                    dotNetReference.invokeMethodAsync('CloseFromJsAsync');
                }
            };

            position(trigger, panel);
            document.addEventListener('pointerdown', onPointerDown, true);
            document.addEventListener('keydown', onKeyDown, true);
            window.addEventListener('resize', onReposition);
            window.addEventListener('scroll', onReposition, true);

            registrations.set(panelId, {
                onPointerDown: onPointerDown,
                onKeyDown: onKeyDown,
                onReposition: onReposition
            });
        },

        release: release
    };
})();
