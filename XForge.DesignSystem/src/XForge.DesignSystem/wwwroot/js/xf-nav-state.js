// XForge Navigation State - localStorage persistence for brand selection
(function() {
    'use strict';

    var STORAGE_KEY_BRAND = 'xf-nav-brand';
    var STORAGE_KEY_BRAND_OLD = 'xf-brand'; // Legacy key from xf-theme.js

    function getBrand() {
        // Try new key first, fallback to legacy key
        var brand = localStorage.getItem(STORAGE_KEY_BRAND);
        if (!brand) {
            brand = localStorage.getItem(STORAGE_KEY_BRAND_OLD);
        }
        return brand || null;
    }

    function setBrand(brand) {
        if (!brand) {
            localStorage.removeItem(STORAGE_KEY_BRAND);
            return;
        }
        localStorage.setItem(STORAGE_KEY_BRAND, brand);
    }

    function clearBrand() {
        localStorage.removeItem(STORAGE_KEY_BRAND);
        localStorage.removeItem(STORAGE_KEY_BRAND_OLD);
    }

    window.xfNavState = {
        getBrand: getBrand,
        setBrand: setBrand,
        clearBrand: clearBrand
    };
})();