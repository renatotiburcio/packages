let echartsPromise;
const charts = new Map();
const observers = new Map();
const frameRequests = new Map();

function isPlainObject(value) {
  return value && typeof value === "object" && !Array.isArray(value);
}

function mergeOptions(target, source) {
  if (!isPlainObject(source)) {
    return target;
  }

  Object.keys(source).forEach(key => {
    const value = source[key];
    if (isPlainObject(value) && isPlainObject(target[key])) {
      mergeOptions(target[key], value);
    } else {
      target[key] = value;
    }
  });

  return target;
}

function resolveOptions(options) {
  if (!options?.xfBaseOptions) {
    return options;
  }

  return mergeOptions(options.xfBaseOptions, options.xfAdditionalOptions);
}

function loadECharts() {
  if (window.echarts) {
    return Promise.resolve(window.echarts);
  }

  if (!echartsPromise) {
    echartsPromise = new Promise((resolve, reject) => {
      const script = document.createElement("script");
      script.src = "/_content/XForge.DesignSystem/vendor/echarts/echarts.min.js";
      script.async = true;
      script.onload = () => resolve(window.echarts);
      script.onerror = reject;
      document.head.appendChild(script);
    });
  }

  return echartsPromise;
}

export async function render(element, options, dotNetReference) {
  if (!element) {
    return;
  }

  const echarts = await loadECharts();
  const existing = charts.get(element);
  const chart = existing || echarts.init(element, null, { renderer: "canvas" });
  charts.set(element, chart);

  chart.off("click");
  chart.off("legendselectchanged");
  chart.setOption(resolveOptions(options), true);
  chart.on("click", params => {
    const index = params?.data?.xfIndex;
    if (Number.isInteger(index) && dotNetReference) {
      dotNetReference.invokeMethodAsync("SelectPointFromChart", index);
    }
  });
  chart.on("legendselectchanged", params => {
    const name = params?.name;
    if (typeof name === "string" && dotNetReference) {
      const selected = params?.selected?.[name] !== false;
      dotNetReference.invokeMethodAsync("LegendChangedFromChart", name, selected);
    }
  });

  observeResize(element, chart);
  chart.resize();
}

export function resize(element) {
  charts.get(element)?.resize();
}

export function dispose(element) {
  const observer = observers.get(element);
  if (observer) {
    observer.disconnect();
    observers.delete(element);
  }

  const frame = frameRequests.get(element);
  if (frame) {
    cancelAnimationFrame(frame);
    frameRequests.delete(element);
  }

  const chart = charts.get(element);
  if (chart) {
    chart.dispose();
    charts.delete(element);
  }
}

function observeResize(element, chart) {
  if (!element || observers.has(element)) {
    return;
  }

  const resize = () => {
    const existingFrame = frameRequests.get(element);
    if (existingFrame) {
      cancelAnimationFrame(existingFrame);
    }

    const nextFrame = requestAnimationFrame(() => {
      frameRequests.delete(element);
      chart.resize();
    });

    frameRequests.set(element, nextFrame);
  };

  const observer = new ResizeObserver(resize);
  observer.observe(element);
  observers.set(element, observer);

  window.addEventListener("resize", resize, { passive: true });
  const disconnect = observer.disconnect.bind(observer);
  observer.disconnect = () => {
    window.removeEventListener("resize", resize);
    disconnect();
  };
}
