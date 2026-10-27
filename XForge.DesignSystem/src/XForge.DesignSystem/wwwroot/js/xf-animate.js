(function () {
  const observers = new Map();

  window.xfAnimate = {
    observe(id, dotNetRef, options) {
      const element = document.getElementById(id);

      if (!element || !("IntersectionObserver" in window)) {
        dotNetRef.invokeMethodAsync("SetViewportVisibleAsync", true);
        return;
      }

      this.release(id);

      const once = Boolean(options?.once);
      const mirror = Boolean(options?.mirror);
      const observer = new IntersectionObserver(
        entries => {
          for (const entry of entries) {
            if (entry.isIntersecting) {
              dotNetRef.invokeMethodAsync("SetViewportVisibleAsync", true);

              if (once) {
                observer.unobserve(element);
              }
            } else if (mirror && !once) {
              dotNetRef.invokeMethodAsync("SetViewportVisibleAsync", false);
            }
          }
        },
        {
          threshold: Number(options?.threshold ?? 0.12),
          rootMargin: options?.rootMargin || "0px 0px -10% 0px"
        }
      );

      observer.observe(element);
      observers.set(id, observer);
    },

    release(id) {
      const observer = observers.get(id);
      if (!observer) {
        return;
      }

      observer.disconnect();
      observers.delete(id);
    }
  };
})();
