let quillPromise;
const editors = new Map();

function loadQuill() {
  if (window.Quill) {
    return Promise.resolve(window.Quill);
  }

  if (!quillPromise) {
    quillPromise = new Promise((resolve, reject) => {
      // Load Quill CSS
      if (!document.querySelector('link[href*="quill"]')) {
        const link = document.createElement("link");
        link.rel = "stylesheet";
        link.href = "/_content/XForge.DesignSystem/vendor/quill/quill.snow.css";
        document.head.appendChild(link);
      }

      // Load Quill JS
      const script = document.createElement("script");
      script.src = "/_content/XForge.DesignSystem/vendor/quill/quill.js";
      script.async = true;
      script.onload = () => resolve(window.Quill);
      script.onerror = reject;
      document.head.appendChild(script);
    });
  }

  return quillPromise;
}

export async function initialize(containerId, dotNetReference, options) {
  release(containerId);

  const Quill = await loadQuill();
  const container = document.getElementById(containerId);
  if (!container) return;

  const theme = options?.theme || "snow";
  const readOnly = options?.readOnly || false;
  const placeholder = options?.placeholder || "";

  const quill = new Quill(container, {
    theme,
    placeholder,
    readOnly,
    modules: {
      toolbar: options?.toolbarId
        ? `#${options.toolbarId}`
        : [
            [{ header: [1, 2, 3, false] }],
            ["bold", "italic", "underline", "strike"],
            [{ color: [] }, { background: [] }],
            [{ align: [] }],
            [{ list: "ordered" }, { list: "bullet" }],
            ["blockquote", "code-block"],
            ["link", "image"],
            ["clean"]
          ]
    }
  });

  const state = {
    quill,
    dotNetReference,
    container,
    ignoreChange: false
  };

  quill.on("text-change", () => {
    if (state.ignoreChange) return;
    const html = container.querySelector(".ql-editor")?.innerHTML || "";
    const text = quill.getText() || "";
    dotNetReference.invokeMethodAsync("OnContentChangedFromJs", html, text.trim());
  });

  editors.set(containerId, state);

  // Set initial content if provided
  if (options?.initialHtml) {
    state.ignoreChange = true;
    container.querySelector(".ql-editor").innerHTML = options.initialHtml;
    state.ignoreChange = false;
  }
}

export function setHtml(containerId, html) {
  const state = editors.get(containerId);
  if (!state) return;
  state.ignoreChange = true;
  const editor = state.container.querySelector(".ql-editor");
  if (editor) editor.innerHTML = html || "";
  state.ignoreChange = false;
}

export function getHtml(containerId) {
  const state = editors.get(containerId);
  if (!state) return "";
  return state.container.querySelector(".ql-editor")?.innerHTML || "";
}

export function getText(containerId) {
  const state = editors.get(containerId);
  if (!state) return "";
  return state.quill?.getText()?.trim() || "";
}

export function setReadOnly(containerId, readOnly) {
  const state = editors.get(containerId);
  if (!state) return;
  state.quill?.enable(!readOnly);
}

export function focus(containerId) {
  const state = editors.get(containerId);
  if (!state) return;
  state.quill?.focus();
}

export function clear(containerId) {
  const state = editors.get(containerId);
  if (!state) return;
  state.ignoreChange = true;
  state.quill?.setText("");
  state.ignoreChange = false;
}

export function release(containerId) {
  const state = editors.get(containerId);
  if (!state) return;
  // Quill does not have a built-in destroy; remove DOM and clear references
  editors.delete(containerId);
}
