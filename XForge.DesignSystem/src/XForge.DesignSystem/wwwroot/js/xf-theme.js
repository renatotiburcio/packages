// XForge Theme Service - JS interop
// Handles OS preference detection, theme persistence, and CSS injection

const STORAGE_KEY = 'xf-theme-mode';
const BRAND_STYLE_PREFIX = 'xf-brand-';
const DARK_BRAND_STYLE_PREFIX = 'xf-dark-brand-';
let mediaQuery = null;
let dotNetRef = null;
let listenerActive = false;

export const xfTheme = {
    getMode() {
        try {
            return localStorage.getItem(STORAGE_KEY);
        } catch {
            return null;
        }
    },

    setMode(mode) {
        try {
            localStorage.setItem(STORAGE_KEY, mode);
        } catch {
            // Storage not available
        }
    },

    startListening(dotNetReference) {
        if (listenerActive) return;
        dotNetRef = dotNetReference;
        mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
        mediaQuery.addEventListener('change', handleMediaChange);
        listenerActive = true;

        // Apply current OS preference
        handleMediaChange({ matches: mediaQuery.matches });
    },

    stopListening() {
        if (mediaQuery) {
            mediaQuery.removeEventListener('change', handleMediaChange);
        }
        listenerActive = false;
        dotNetRef = null;
    },

    prefersDark() {
        return window.matchMedia('(prefers-color-scheme: dark)').matches;
    },

    setThemeAttribute(theme) {
        document.documentElement.setAttribute('data-theme', theme);
    },

    // CSS Injection
    injectCss(styleId, css) {
        removeElementById(styleId);
        const style = document.createElement('style');
        style.id = styleId;
        style.textContent = css;
        document.head.appendChild(style);
    },

    removeCss(styleId) {
        removeElementById(styleId);
    },

    injectBrandCss(brandName, css) {
        const id = BRAND_STYLE_PREFIX + brandName;
        injectCss(id, css);
    },

    injectDarkBrandCss(brandName, css) {
        const id = DARK_BRAND_STYLE_PREFIX + brandName;
        injectCss(id, css);
    },

    removeBrandCss(brandName) {
        removeElementById(BRAND_STYLE_PREFIX + brandName);
        removeElementById(DARK_BRAND_STYLE_PREFIX + brandName);
    }
};

function removeElementById(id) {
    const existing = document.getElementById(id);
    if (existing) existing.remove();
}

function handleMediaChange(event) {
    if (dotNetRef) {
        dotNetRef.invokeMethodAsync('OnSystemThemeChanged', event.matches);
    }
}
