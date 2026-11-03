// XForge Theme Service - JS interop
// Handles OS preference detection, theme persistence, and CSS injection
//
// NOTA: este arquivo é carregado como <script> clássico pelo App.razor (manual e
// sample). A forma anterior usava `export const`, que só funciona como módulo ES —
// ao carregar como script clássico o parser quebrava em "Unexpected token 'export'"
// e window.xfTheme nunca era definido, então toda chamada
// JSRuntime.InvokeAsync("xfTheme.*") falhava. Agora segue o mesmo padrão dos demais
// arquivos (xf-nav-state.js, xf-popover.js): IIFE expondo a API em window.
(function () {
    'use strict';

    var STORAGE_KEY = 'xf-theme-mode';
    var BRAND_STYLE_PREFIX = 'xf-brand-';
    var DARK_BRAND_STYLE_PREFIX = 'xf-dark-brand-';
    var mediaQuery = null;
    var dotNetRef = null;
    var listenerActive = false;

    function removeElementById(id) {
        var existing = document.getElementById(id);
        if (existing) {
            existing.remove();
        }
    }

    function handleMediaChange(event) {
        if (dotNetRef) {
            dotNetRef.invokeMethodAsync('OnSystemThemeChanged', event.matches);
        }
    }

    window.xfTheme = {
        getMode: function () {
            try {
                return localStorage.getItem(STORAGE_KEY);
            } catch (e) {
                return null;
            }
        },

        setMode: function (mode) {
            try {
                localStorage.setItem(STORAGE_KEY, mode);
            } catch (e) {
                // Storage not available
            }
        },

        startListening: function (dotNetReference) {
            if (listenerActive) {
                return;
            }
            dotNetRef = dotNetReference;
            mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
            mediaQuery.addEventListener('change', handleMediaChange);
            listenerActive = true;

            // Apply current OS preference
            handleMediaChange({ matches: mediaQuery.matches });
        },

        stopListening: function () {
            if (mediaQuery) {
                mediaQuery.removeEventListener('change', handleMediaChange);
            }
            listenerActive = false;
            dotNetRef = null;
        },

        prefersDark: function () {
            return window.matchMedia('(prefers-color-scheme: dark)').matches;
        },

        setThemeAttribute: function (theme) {
            document.documentElement.setAttribute('data-theme', theme);
        },

        getTheme: function () {
            // Espelha o helper inline do index.html do manual: data-theme do <html>
            // tem prioridade sobre localStorage, porque o script de boot já o aplicou.
            var attr = document.documentElement.getAttribute('data-theme');
            if (attr === 'dark' || attr === 'light') {
                return attr;
            }
            return this.getMode();
        },

        toggleTheme: function () {
            var current = document.documentElement.getAttribute('data-theme');
            var next = 'light';
            if (current === 'dark') {
                next = 'light';
            } else if (current === 'light') {
                next = 'dark';
            } else {
                next = this.prefersDark() ? 'light' : 'dark';
            }
            this.setMode(next);
            this.setThemeAttribute(next);
            return next;
        },

        // CSS Injection
        injectCss: function (styleId, css) {
            removeElementById(styleId);
            var style = document.createElement('style');
            style.id = styleId;
            style.textContent = css;
            document.head.appendChild(style);
        },

        removeCss: function (styleId) {
            removeElementById(styleId);
        },

        injectBrandCss: function (brandName, css) {
            this.injectCss(BRAND_STYLE_PREFIX + brandName, css);
        },

        injectDarkBrandCss: function (brandName, css) {
            this.injectCss(DARK_BRAND_STYLE_PREFIX + brandName, css);
        },

        removeBrandCss: function (brandName) {
            removeElementById(BRAND_STYLE_PREFIX + brandName);
            removeElementById(DARK_BRAND_STYLE_PREFIX + brandName);
        }
    };
})();
