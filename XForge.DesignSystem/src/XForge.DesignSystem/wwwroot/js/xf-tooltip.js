// XForge Tooltip - Portal to body with proper positioning
// Styles are handled by CSS (.xf-tooltip-popup--theme classes)

(function() {
    'use strict';

    const Z_INDEX = 9999;
    const ANIMATION_DURATION = 150;

    // Only positioning logic here - styles come from CSS

    function getOrCreateTooltip(trigger, tooltipPopupId) {
        let tooltip = document.getElementById(tooltipPopupId);

        if (!tooltip) {
            tooltip = document.createElement('div');
            tooltip.id = tooltipPopupId;
            tooltip.className = 'xf-tooltip-popup';
            tooltip.setAttribute('role', 'tooltip');
            tooltip.style.cssText = `
                position: fixed;
                z-index: ${Z_INDEX};
                pointer-events: none;
                display: none;
                opacity: 0;
            `;
            document.body.appendChild(tooltip);
        }

        return tooltip;
    }

    function getOptions(trigger) {
        return {
            id: trigger.getAttribute('data-tooltip-id') || ('xf-tooltip-' + trigger.id),
            delay: parseInt(trigger.getAttribute('data-tooltip-delay')) || 200,
            hideDelay: parseInt(trigger.getAttribute('data-tooltip-hide-delay')) || 100,
            text: trigger.getAttribute('data-tooltip-text') || '',
            position: trigger.getAttribute('data-tooltip-position') || 'right',
            theme: trigger.getAttribute('data-tooltip-theme') || 'dark',
            trigger: trigger.getAttribute('data-tooltip-trigger') || 'hover-focus',
            maxWidth: trigger.getAttribute('data-tooltip-max-width') || '',
            multiline: trigger.getAttribute('data-tooltip-multiline') === 'true',
            showArrow: trigger.getAttribute('data-tooltip-arrow') !== 'false',
            disabled: trigger.getAttribute('data-tooltip-disabled') === 'true'
        };
    }

    function applyTooltipContent(tooltip, options) {
        tooltip.className = 'xf-tooltip-popup xf-tooltip-popup--' + options.theme + ' xf-tooltip-popup--' + options.position;
        if (!options.showArrow) {
            tooltip.className += ' xf-tooltip-popup--no-arrow';
        }
        if (options.multiline) {
            tooltip.className += ' xf-tooltip-popup--multiline';
        }
        tooltip.textContent = options.text;
        tooltip.style.maxWidth = options.maxWidth || '';

        // Apply brand color to primary theme tooltips
        if (options.theme === 'primary') {
            var primaryColor = getComputedStyle(document.documentElement).getPropertyValue('--color-primary').trim() || '#6366f1';
            tooltip.style.backgroundColor = primaryColor;
        }
    }

    function showTooltip(trigger, immediate) {
        const options = getOptions(trigger);
        if (!options.text || options.disabled) return;

        const tooltip = getOrCreateTooltip(trigger, options.id);
        applyTooltipContent(tooltip, options);
        tooltip.style.display = 'block';
        positionTooltip(tooltip, trigger, options.position);

        window.clearTimeout(trigger.__xfTooltipHideTimer);
        window.clearTimeout(trigger.__xfTooltipShowTimer);

        const show = () => requestAnimationFrame(() => {
            tooltip.style.opacity = '1';
        });

        if (immediate) {
            show();
        } else {
            trigger.__xfTooltipShowTimer = window.setTimeout(show, options.delay);
        }
    }

    function hideTooltip(trigger, immediate) {
        const options = getOptions(trigger);
        const tooltip = document.getElementById(options.id);
        if (!tooltip) return;

        window.clearTimeout(trigger.__xfTooltipShowTimer);
        window.clearTimeout(trigger.__xfTooltipHideTimer);

        const hide = () => {
            tooltip.style.opacity = '0';
            window.setTimeout(() => {
                tooltip.style.display = 'none';
            }, ANIMATION_DURATION);
        };

        if (immediate) {
            hide();
        } else {
            trigger.__xfTooltipHideTimer = window.setTimeout(hide, options.hideDelay);
        }
    }

    function positionTooltip(tooltip, trigger, position) {
        const triggerRect = trigger.getBoundingClientRect();
        const tooltipRect = tooltip.getBoundingClientRect();

        let left, top;

        switch (position) {
            case 'top':
                left = triggerRect.left + triggerRect.width / 2 - tooltipRect.width / 2;
                top = triggerRect.top - tooltipRect.height - 10;
                break;
            case 'bottom':
                left = triggerRect.left + triggerRect.width / 2 - tooltipRect.width / 2;
                top = triggerRect.bottom + 10;
                break;
            case 'left':
                left = triggerRect.left - tooltipRect.width - 10;
                top = triggerRect.top + triggerRect.height / 2 - tooltipRect.height / 2;
                break;
            case 'right':
            default:
                left = triggerRect.right + 10;
                top = triggerRect.top + triggerRect.height / 2 - tooltipRect.height / 2;
                break;
        }

        // Keep within viewport
        const padding = 8;
        left = Math.max(padding, Math.min(left, window.innerWidth - tooltipRect.width - padding));
        top = Math.max(padding, Math.min(top, window.innerHeight - tooltipRect.height - padding));

        tooltip.style.left = left + 'px';
        tooltip.style.top = top + 'px';
    }

    window.xfTooltip = {
        init: function(trigger, tooltipId) {
            if (!trigger) return;
            if (trigger.__xfTooltipCleanup) {
                trigger.__xfTooltipCleanup();
            }
            if (!trigger.id) {
                trigger.id = 'xf-tt-' + Math.random().toString(36).substr(2, 9);
            }
            trigger.setAttribute('data-tooltip-id', tooltipId || ('xf-tooltip-' + trigger.id));

            const listeners = [];
            const add = (event, handler) => {
                trigger.addEventListener(event, handler);
                listeners.push([event, handler]);
            };
            const options = getOptions(trigger);

            if (options.trigger === 'hover' || options.trigger === 'hover-focus') {
                add('mouseenter', () => showTooltip(trigger, false));
                add('mouseleave', () => hideTooltip(trigger, false));
            }

            if (options.trigger === 'focus' || options.trigger === 'hover-focus') {
                add('focus', () => showTooltip(trigger, false));
                add('blur', () => hideTooltip(trigger, true));
            }

            if (options.trigger === 'click') {
                add('click', () => {
                    const tooltip = document.getElementById(getOptions(trigger).id);
                    if (tooltip && tooltip.style.display === 'block' && tooltip.style.opacity === '1') {
                        hideTooltip(trigger, true);
                    } else {
                        showTooltip(trigger, true);
                    }
                });
            }

            trigger.__xfTooltipCleanup = () => {
                listeners.forEach(([event, handler]) => trigger.removeEventListener(event, handler));
                window.clearTimeout(trigger.__xfTooltipShowTimer);
                window.clearTimeout(trigger.__xfTooltipHideTimer);
            };
        },

        update: function(trigger) {
            if (!trigger) return;
            const options = getOptions(trigger);
            const tooltip = document.getElementById(options.id);
            if (tooltip && tooltip.style.display === 'block') {
                applyTooltipContent(tooltip, options);
                positionTooltip(tooltip, trigger, options.position);
            }
        },

        show: function(trigger) {
            showTooltip(trigger, true);
        },

        hide: function(trigger) {
            hideTooltip(trigger, true);
        },

        dispose: function(trigger) {
            if (trigger && trigger.__xfTooltipCleanup) {
                trigger.__xfTooltipCleanup();
            }
            const tooltipId = trigger.getAttribute('data-tooltip-id') || ('xf-tooltip-' + trigger.id);
            const tooltip = document.getElementById(tooltipId);
            if (tooltip) {
                tooltip.remove();
            }
        },

        // Position rail active indicator based on actual item position
        positionRailIndicator: function(indicatorId, activeItemSelector, railContainer) {
            const indicator = document.getElementById(indicatorId);
            const activeItem = railContainer.querySelector(activeItemSelector);

            if (!indicator || !activeItem) return;

            const railRect = railContainer.getBoundingClientRect();
            const itemRect = activeItem.getBoundingClientRect();

            // Calculate position relative to rail container
            const top = itemRect.top - railRect.top + (itemRect.height / 2) - (indicator.offsetHeight / 2);

            indicator.style.top = top + 'px';
        },

// Update all rail indicators in a container
        updateRailIndicators: function(containerSelector) {
            const container = typeof containerSelector === 'string'
                ? document.querySelector(containerSelector)
                : containerSelector;

            if (!container) {
                console.log('[xfTooltip] Container não encontrado:', containerSelector);
                // Try to find any rail-items container
                container = document.querySelector('.rail-items');
            }

            if (!container) {
                console.log('[xfTooltip] Container rail-items não encontrado');
                return;
            }

            const indicators = container.querySelectorAll('[data-rail-indicator]');
            const items = container.querySelectorAll('[data-rail-item]');

            console.log('[xfTooltip] Indicadores:', indicators.length, 'Itens:', items.length);

            indicators.forEach((indicator, index) => {
                if (items[index]) {
                    const itemRect = items[index].getBoundingClientRect();
                    const containerRect = container.getBoundingClientRect();
                    const top = itemRect.top - containerRect.top + (itemRect.height / 2) - (indicator.offsetHeight / 2);
                    indicator.style.top = top + 'px';
                    console.log('[xfTooltip] Indicador', index, 'posicionado em top:', top);
                }
            });
        }
    };
})();
