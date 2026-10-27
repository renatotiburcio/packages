(function () {
    const popovers = new Map();

    function clamp(value, min, max) {
        return Math.min(Math.max(value, min), max);
    }

    function opposite(placement) {
        if (placement === 'top') return 'bottom';
        if (placement === 'bottom') return 'top';
        if (placement === 'left') return 'right';
        return 'left';
    }

    function hasRoom(anchor, panel, placement, offset, padding) {
        if (placement === 'top') return anchor.top - panel.height - offset >= padding;
        if (placement === 'bottom') return anchor.bottom + panel.height + offset <= window.innerHeight - padding;
        if (placement === 'left') return anchor.left - panel.width - offset >= padding;
        return anchor.right + panel.width + offset <= window.innerWidth - padding;
    }

    function compute(anchor, panel, options) {
        let placement = options.placement || 'bottom';
        const offset = options.offset ?? 8;
        const padding = options.collisionPadding ?? 12;

        if (options.flip && !hasRoom(anchor, panel, placement, offset, padding)) {
            const next = opposite(placement);
            if (hasRoom(anchor, panel, next, offset, padding)) {
                placement = next;
            }
        }

        const alignment = options.alignment || 'center';
        let left = anchor.left + (anchor.width - panel.width) / 2;
        let top = anchor.bottom + offset;

        if (placement === 'top') {
            top = anchor.top - panel.height - offset;
        } else if (placement === 'left') {
            left = anchor.left - panel.width - offset;
            top = anchor.top + (anchor.height - panel.height) / 2;
        } else if (placement === 'right') {
            left = anchor.right + offset;
            top = anchor.top + (anchor.height - panel.height) / 2;
        }

        if (placement === 'top' || placement === 'bottom') {
            if (alignment === 'start') {
                left = anchor.left;
            } else if (alignment === 'end') {
                left = anchor.right - panel.width;
            }
        } else if (alignment === 'start') {
            top = anchor.top;
        } else if (alignment === 'end') {
            top = anchor.bottom - panel.height;
        }

        left = clamp(left, padding, window.innerWidth - panel.width - padding);
        top = clamp(top, padding, window.innerHeight - panel.height - padding);

        return { left, top, placement };
    }

    function position(anchorId, panelId, options) {
        const anchor = document.getElementById(anchorId);
        const panel = document.getElementById(panelId);
        if (!anchor || !panel) return;

        const anchorRect = anchor.getBoundingClientRect();
        const panelRect = panel.getBoundingClientRect();
        const next = compute(anchorRect, panelRect, options);

        panel.style.left = `${next.left}px`;
        panel.style.top = `${next.top}px`;
        panel.dataset.placement = next.placement;
    }

    window.xfPopover = {
        register(anchorId, panelId, dotnet, options) {
            this.release(panelId);
            position(anchorId, panelId, options);

            const onPointerDown = event => {
                if (!options.closeOnOutsideClick) return;
                const anchor = document.getElementById(anchorId);
                const panel = document.getElementById(panelId);
                if (!anchor || !panel) return;
                if (!anchor.contains(event.target) && !panel.contains(event.target)) {
                    dotnet.invokeMethodAsync('CloseFromJsAsync');
                }
            };

            const onKeyDown = event => {
                if (options.closeOnEscape && event.key === 'Escape') {
                    dotnet.invokeMethodAsync('CloseFromJsAsync');
                }
            };

            const onReposition = () => position(anchorId, panelId, options);

            document.addEventListener('pointerdown', onPointerDown, true);
            document.addEventListener('keydown', onKeyDown, true);
            window.addEventListener('resize', onReposition);
            window.addEventListener('scroll', onReposition, true);

            popovers.set(panelId, { onPointerDown, onKeyDown, onReposition });
        },

        release(panelId) {
            const popover = popovers.get(panelId);
            if (!popover) return;

            document.removeEventListener('pointerdown', popover.onPointerDown, true);
            document.removeEventListener('keydown', popover.onKeyDown, true);
            window.removeEventListener('resize', popover.onReposition);
            window.removeEventListener('scroll', popover.onReposition, true);
            popovers.delete(panelId);
        }
    };
})();
