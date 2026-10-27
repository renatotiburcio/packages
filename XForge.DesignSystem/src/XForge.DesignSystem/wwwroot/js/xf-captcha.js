// xf-captcha.js — Google reCAPTCHA v2 wrapper for XFCaptcha
// Loads reCAPTCHA script from Google and manages the widget lifecycle
// ES module pattern: direct named exports for Blazor JS interop

const RECAPTCHA_SCRIPT_URL = 'https://www.google.com/recaptcha/api.js?render=explicit';
let scriptLoaded = false;
let scriptLoading = false;
let loadPromise = null;
const widgetMap = new WeakMap();

/**
 * Loads the reCAPTCHA script if not already loaded.
 * @returns {Promise<void>}
 */
function ensureScriptLoaded() {
    if (scriptLoaded) return Promise.resolve();
    if (loadPromise) return loadPromise;

    loadPromise = new Promise((resolve, reject) => {
        if (window.grecaptcha && window.grecaptcha.render) {
            scriptLoaded = true;
            resolve();
            return;
        }

        const script = document.createElement('script');
        script.src = RECAPTCHA_SCRIPT_URL;
        script.async = true;
        script.defer = true;
        script.onload = () => {
            const checkReady = () => {
                if (window.grecaptcha && window.grecaptcha.render) {
                    scriptLoaded = true;
                    scriptLoading = false;
                    resolve();
                } else {
                    setTimeout(checkReady, 100);
                }
            };
            checkReady();
        };
        script.onerror = () => {
            loadPromise = null;
            scriptLoading = false;
            reject(new Error('Failed to load reCAPTCHA script'));
        };
        scriptLoading = true;
        document.head.appendChild(script);
    });

    return loadPromise;
}

/**
 * Renders a reCAPTCHA widget in the given container.
 * @param {HTMLElement} container - DOM element to render into
 * @param {string} siteKey - reCAPTCHA site key
 * @param {string} theme - 'light' or 'dark'
 * @param {string} size - 'normal' or 'compact'
 * @param {string} badge - 'bottomright', 'bottomleft', or 'inline'
 * @param {string|null} language - Language code (e.g., 'pt-BR')
 * @param {object} dotNetRef - .NET object reference for callbacks
 */
async function renderCaptcha(container, siteKey, theme, size, badge, language, dotNetRef) {
    await ensureScriptLoaded();

    const params = {
        sitekey: siteKey,
        theme: theme || 'light',
        size: size || 'normal',
        callback: (response) => {
            try { dotNetRef.invokeMethodAsync('OnSuccessHandler', response); } catch {}
        },
        'expired-callback': () => {
            try { dotNetRef.invokeMethodAsync('OnExpiredHandler'); } catch {}
        },
        'error-callback': () => {
            try { dotNetRef.invokeMethodAsync('OnExpiredHandler'); } catch {}
        }
    };

    if (language) {
        window.___grecaptcha_cfg = window.___grecaptcha_cfg || {};
        window.___grecaptcha_cfg.hl = language;
    }

    const widgetId = window.grecaptcha.render(container, params);
    widgetMap.set(container, widgetId);
}

/**
 * Resets the reCAPTCHA widget.
 * @param {HTMLElement} container
 */
function resetCaptcha(container) {
    const widgetId = widgetMap.get(container);
    if (widgetId !== undefined && window.grecaptcha) {
        try { window.grecaptcha.reset(widgetId); } catch {}
    }
}

/**
 * Executes invisible reCAPTCHA.
 * @param {HTMLElement} container
 */
function executeCaptcha(container) {
    const widgetId = widgetMap.get(container);
    if (widgetId !== undefined && window.grecaptcha) {
        try { window.grecaptcha.execute(widgetId); } catch {}
    }
}

/**
 * Destroys the reCAPTCHA widget.
 * @param {HTMLElement} container
 */
function destroyCaptcha(container) {
    widgetMap.delete(container);
}

export { renderCaptcha, resetCaptcha, executeCaptcha, destroyCaptcha };
