window.xfSelect = {
  _registry: new Map(),

  register: function (selectId, panelId, dotNetRef) {
    var select = document.getElementById(selectId);
    var panel = document.getElementById(panelId);
    if (!select || !panel) return;

    this._release(panelId);

    function onPointerDown(e) {
      if (panel.contains(e.target) || select.contains(e.target)) return;
      dotNetRef.invokeMethodAsync('CloseFromJsAsync');
    }

    function onKeyDown(e) {
      if (e.key === 'Escape') dotNetRef.invokeMethodAsync('CloseFromJsAsync');
      if (e.key === 'Tab') dotNetRef.invokeMethodAsync('CloseFromJsAsync');
    }

    function onResize() { position(select, panel); }

    document.addEventListener('pointerdown', onPointerDown, true);
    document.addEventListener('keydown', onKeyDown, true);
    window.addEventListener('resize', onResize);
    window.addEventListener('scroll', onResize, true);

    this._registry.set(panelId, { onPointerDown: onPointerDown, onKeyDown: onKeyDown, onResize: onResize });

    // Initial position after DOM is ready
    requestAnimationFrame(function () { position(select, panel); });
  },

  _release: function (panelId) {
    var reg = this._registry.get(panelId);
    if (!reg) return;
    document.removeEventListener('pointerdown', reg.onPointerDown, true);
    document.removeEventListener('keydown', reg.onKeyDown, true);
    window.removeEventListener('resize', reg.onResize);
    window.removeEventListener('scroll', reg.onResize, true);
    this._registry.delete(panelId);
  },

  release: function (panelId) {
    this._release(panelId);
  },

  position: function (panelId) {
    var panel = document.getElementById(panelId);
    if (!panel) return;
    var select = panel.parentElement;
    if (!select) return;
    position(select, panel);
  }
};

function position(trigger, panel) {
  var btn = trigger.querySelector('button') || trigger;
  var rect = btn.getBoundingClientRect();
  var padding = 12;
  var gap = 4;

  // Width: match button width, but never narrower than 160px
  var minDropdownWidth = 160;
  var finalWidth = Math.max(rect.width, minDropdownWidth);

  // Cap at viewport width
  var maxWidth = window.innerWidth - (padding * 2);
  if (finalWidth > maxWidth) finalWidth = maxWidth;

  // Measure content height for positioning
  var contentHeight = panel.scrollHeight || 200;

  // Vertical: prefer below, flip upward if no room
  var top = rect.bottom + gap;
  if (top + contentHeight > window.innerHeight - padding) {
    var topUp = rect.top - contentHeight - gap;
    if (topUp >= padding) {
      top = topUp;
    } else {
      // Cap height
      var maxH = window.innerHeight - top - padding;
      if (maxH < 100) maxH = 100;
      panel.style.maxHeight = maxH + 'px';
    }
  }

  // Horizontal: align left, slide left if overflows right edge
  var left = rect.left;
  if (left + finalWidth > window.innerWidth - padding) {
    left = window.innerWidth - finalWidth - padding;
  }
  left = Math.max(padding, left);

  // Apply
  panel.style.position = 'fixed';
  panel.style.top = top + 'px';
  panel.style.left = left + 'px';
  panel.style.width = finalWidth + 'px';
  panel.style.visibility = '';
  panel.style.maxHeight = panel.style.maxHeight || '';
}


// === Blazor bridge functions ===
window.xfSelectRegister = function(selectId, panelId, dotNetRef) {
  window.xfSelect.register(selectId, panelId, dotNetRef);
};

window.xfSelectRelease = function(panelId) {
  window.xfSelect.release(panelId);
};