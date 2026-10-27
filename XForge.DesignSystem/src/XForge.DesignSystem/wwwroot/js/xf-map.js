// xf-map.js — Leaflet map wrapper for XFMap
// Uses Leaflet (https://leafletjs.com/) loaded from CDN
// ES module pattern: const obj = { methods }; export { obj };

const xfMap = (() => {
    const LEAFLET_CSS_URL = 'https://unpkg.com/leaflet@1.9.4/dist/leaflet.css';
    const LEAFLET_JS_URL = 'https://unpkg.com/leaflet@1.9.4/dist/leaflet.js';
    let leafletLoaded = false;
    let loadPromise = null;
    const instances = {};

    /**
     * Loads Leaflet JS + CSS from CDN if not already loaded.
     * @returns {Promise<void>}
     */
    function ensureLeaflet() {
        if (leafletLoaded) return Promise.resolve();
        if (loadPromise) return loadPromise;

        loadPromise = new Promise((resolve, reject) => {
            // Load CSS
            if (!document.querySelector(`link[href="${LEAFLET_CSS_URL}"]`)) {
                const link = document.createElement('link');
                link.rel = 'stylesheet';
                link.href = LEAFLET_CSS_URL;
                link.integrity = 'sha256-p4NxAoJBhIIN+hmNHrzRCf9tD/miZyoHS5obTRR9BMY=';
                link.crossOrigin = '';
                document.head.appendChild(link);
            }

            // Load JS
            if (window.L) {
                leafletLoaded = true;
                resolve();
                return;
            }

            const script = document.createElement('script');
            script.src = LEAFLET_JS_URL;
            script.integrity = 'sha256-20nQCchB9co0qIjJZRGuk2/Z9VM+kNiyxNV1lvTlZBo=';
            script.crossOrigin = '';
            script.async = true;
            script.onload = () => {
                leafletLoaded = true;
                resolve();
            };
            script.onerror = () => {
                loadPromise = null;
                reject(new Error('Failed to load Leaflet from CDN'));
            };
            document.head.appendChild(script);
        });

        return loadPromise;
    }

    /**
     * Initializes a Leaflet map.
     * @param {HTMLElement} container - DOM element to render into
     * @param {object} dotNetRef - .NET object reference for callbacks
     * @param {number} lat - Initial center latitude
     * @param {number} lng - Initial center longitude
     * @param {number} zoom - Initial zoom level
     * @param {boolean} interactive - Whether the map is interactive
     * @param {boolean} showZoom - Show zoom control
     * @param {boolean} showAttribution - Show attribution control
     * @param {number} minZoom - Minimum zoom level
     * @param {number} maxZoom - Maximum zoom level
     * @returns {Promise<void>}
     */
    async function initialize(container, dotNetRef, lat, lng, zoom, interactive, showZoom, showAttribution, minZoom, maxZoom) {
        await ensureLeaflet();

        const mapId = container.id || container.getAttribute('data-map-id');
        if (instances[mapId]) {
            destroy(container);
        }

        const options = {
            center: [lat, lng],
            zoom: zoom,
            dragging: interactive,
            touchZoom: interactive,
            scrollWheelZoom: interactive,
            doubleClickZoom: interactive,
            boxZoom: interactive,
            keyboard: interactive,
            zoomControl: showZoom,
            attributionControl: showAttribution,
        };

        if (minZoom != null) options.minZoom = minZoom;
        if (maxZoom != null) options.maxZoom = maxZoom;

        const map = L.map(container, options);

        instances[mapId] = {
            map: map,
            dotNetRef: dotNetRef,
            layers: [],
            programmaticViewChange: false,
        };

        // Map events → .NET callbacks
        map.on('click', (e) => {
            try {
                dotNetRef.invokeMethodAsync('OnMapClickJS', e.latlng.lat, e.latlng.lng, e.containerPoint.x, e.containerPoint.y);
            } catch (_) { /* disposed */ }
        });

        map.on('dblclick', (e) => {
            try {
                dotNetRef.invokeMethodAsync('OnMapDoubleClickJS', e.latlng.lat, e.latlng.lng, e.containerPoint.x, e.containerPoint.y);
            } catch (_) { /* disposed */ }
        });

        map.on('contextmenu', (e) => {
            try {
                dotNetRef.invokeMethodAsync('OnMapContextMenuJS', e.latlng.lat, e.latlng.lng, e.containerPoint.x, e.containerPoint.y);
            } catch (_) { /* disposed */ }
        });

        map.on('moveend', (e) => {
            if (instances[mapId]) {
                const source = instances[mapId].programmaticViewChange ? 'Programmatic' : 'User';
                instances[mapId].programmaticViewChange = false;
                const center = map.getCenter();
                const bounds = map.getBounds();
                try {
                    dotNetRef.invokeMethodAsync('OnViewChangedJS',
                        center.lat, center.lng, map.getZoom(),
                        bounds.getSouth(), bounds.getWest(), bounds.getNorth(), bounds.getEast(),
                        source);
                } catch (_) { /* disposed */ }
            }
        });

        map.on('zoomend', () => {
            if (instances[mapId]) {
                const center = map.getCenter();
                try {
                    dotNetRef.invokeMethodAsync('OnViewChangedJS',
                        center.lat, center.lng, map.getZoom(),
                        0, 0, 0, 0,
                        'Zoom');
                } catch (_) { /* disposed */ }
            }
        });

        // Notify .NET that the map is ready
        try {
            dotNetRef.invokeMethodAsync('OnMapReadyJS');
        } catch (_) { /* disposed */ }
    }

    /**
     * Adds a tile layer to the map.
     * @param {HTMLElement} container
     * @param {string} source - XYZ tile URL template
     * @param {string} attribution - Attribution text
     * @param {number} tileSize - Tile size in pixels
     * @param {number|null} minZoom
     * @param {number|null} maxZoom
     * @param {string[]} subdomains
     * @returns {number} Layer index
     */
    function addTileLayer(container, source, attribution, tileSize, minZoom, maxZoom, subdomains) {
        const inst = getInst(container);
        if (!inst) return -1;

        const options = { attribution: attribution || '' };
        if (tileSize) options.tileSize = tileSize;
        if (minZoom != null) options.minZoom = minZoom;
        if (maxZoom != null) options.maxZoom = maxZoom;
        if (subdomains && subdomains.length) options.subdomains = subdomains;

        const layer = L.tileLayer(source, options);
        layer.addTo(inst.map);
        inst.layers.push(layer);
        return inst.layers.length - 1;
    }

    /**
     * Adds a marker to the map.
     * @param {HTMLElement} container
     * @param {string} markerId
     * @param {number} lat
     * @param {number} lng
     * @param {string|null} title
     * @param {string|null} popupText
     * @param {boolean} draggable
     * @param {object|null} icon - { url, size: [w, h], anchor: [x, y], popupAnchor: [x, y] }
     * @returns {number} Layer index
     */
    function addMarker(container, markerId, lat, lng, title, popupText, draggable, icon) {
        const inst = getInst(container);
        if (!inst) return -1;

        const options = { draggable: draggable || false };
        if (title) options.title = title;

        if (icon && icon.url) {
            const iconOpts = {};
            if (icon.size) iconOpts.iconSize = icon.size;
            if (icon.anchor) iconOpts.iconAnchor = icon.anchor;
            if (icon.popupAnchor) iconOpts.popupAnchor = icon.popupAnchor;
            options.icon = L.icon({ iconUrl: icon.url, ...iconOpts });
        }

        const marker = L.marker([lat, lng], options);
        if (popupText) marker.bindPopup(popupText);
        marker.addTo(inst.map);

        marker._xfMarkerId = markerId;

        marker.on('click', () => {
            try {
                inst.dotNetRef.invokeMethodAsync('OnMarkerClickJS', markerId, lat, lng);
            } catch (_) { /* disposed */ }
        });

        if (draggable) {
            marker.on('dragend', () => {
                const pos = marker.getLatLng();
                try {
                    inst.dotNetRef.invokeMethodAsync('OnMarkerDragEndJS', markerId, pos.lat, pos.lng);
                } catch (_) { /* disposed */ }
            });
        }

        inst.layers.push(marker);
        return inst.layers.length - 1;
    }

    /**
     * Adds a circle to the map.
     * @param {HTMLElement} container
     * @param {number} lat
     * @param {number} lng
     * @param {number} radius - in meters
     * @param {object} style - { strokeColor, strokeOpacity, strokeWidth, strokeDashArray, fillColor, fillOpacity }
     * @returns {number} Layer index
     */
    function addCircle(container, lat, lng, radius, style) {
        const inst = getInst(container);
        if (!inst) return -1;

        const circle = L.circle([lat, lng], {
            radius: radius,
            color: style.strokeColor || '#3388ff',
            opacity: style.strokeOpacity ?? 1,
            weight: style.strokeWidth ?? 3,
            dashArray: style.strokeDashArray || null,
            fillColor: style.fillColor || '#3388ff',
            fillOpacity: style.fillOpacity ?? 0.2,
        });

        circle.addTo(inst.map);
        inst.layers.push(circle);
        return inst.layers.length - 1;
    }

    /**
     * Adds a polyline to the map.
     * @param {HTMLElement} container
     * @param {Array} paths - Array of [lat, lng] pairs
     * @param {object} style
     * @returns {number} Layer index
     */
    function addPolyline(container, paths, style) {
        const inst = getInst(container);
        if (!inst) return -1;

        const polyline = L.polyline(paths, {
            color: style.strokeColor || '#3388ff',
            opacity: style.strokeOpacity ?? 1,
            weight: style.strokeWidth ?? 3,
            dashArray: style.strokeDashArray || null,
        });

        polyline.addTo(inst.map);
        inst.layers.push(polyline);
        return inst.layers.length - 1;
    }

    /**
     * Adds a polygon to the map.
     * @param {HTMLElement} container
     * @param {Array} rings - Array of rings (each ring is array of [lat, lng])
     * @param {object} style
     * @returns {number} Layer index
     */
    function addPolygon(container, rings, style) {
        const inst = getInst(container);
        if (!inst) return -1;

        const polygon = L.polygon(rings, {
            color: style.strokeColor || '#3388ff',
            opacity: style.strokeOpacity ?? 1,
            weight: style.strokeWidth ?? 3,
            dashArray: style.strokeDashArray || null,
            fillColor: style.fillColor || '#3388ff',
            fillOpacity: style.fillOpacity ?? 0.2,
        });

        polygon.addTo(inst.map);
        inst.layers.push(polygon);
        return inst.layers.length - 1;
    }

    /**
     * Removes a layer by index.
     * @param {HTMLElement} container
     * @param {number} layerIndex
     */
    function removeLayer(container, layerIndex) {
        const inst = getInst(container);
        if (!inst) return;
        const layer = inst.layers[layerIndex];
        if (layer) {
            inst.map.removeLayer(layer);
            inst.layers[layerIndex] = null;
        }
    }

    /**
     * Removes all layers from the map.
     * @param {HTMLElement} container
     */
    function clearLayers(container) {
        const inst = getInst(container);
        if (!inst) return;
        for (const layer of inst.layers) {
            if (layer) inst.map.removeLayer(layer);
        }
        inst.layers = [];
    }

    /**
     * Sets the map view.
     * @param {HTMLElement} container
     * @param {number} lat
     * @param {number} lng
     * @param {number} zoom
     * @param {boolean} animated
     */
    function setView(container, lat, lng, zoom, animated) {
        const inst = getInst(container);
        if (!inst) return;
        inst.programmaticViewChange = true;
        inst.map.setView([lat, lng], zoom, { animate: animated, duration: animated ? 0.25 : 0 });
    }

    /**
     * Pans to a location.
     * @param {HTMLElement} container
     * @param {number} lat
     * @param {number} lng
     * @param {boolean} animated
     */
    function panTo(container, lat, lng, animated) {
        const inst = getInst(container);
        if (!inst) return;
        inst.programmaticViewChange = true;
        inst.map.panTo([lat, lng], { animate: animated });
    }

    /**
     * Zooms in.
     * @param {HTMLElement} container
     */
    function zoomIn(container) {
        const inst = getInst(container);
        if (!inst) return;
        inst.programmaticViewChange = true;
        inst.map.zoomIn();
    }

    /**
     * Zooms out.
     * @param {HTMLElement} container
     */
    function zoomOut(container) {
        const inst = getInst(container);
        if (!inst) return;
        inst.programmaticViewChange = true;
        inst.map.zoomOut();
    }

    /**
     * Fits the map to given bounds.
     * @param {HTMLElement} container
     * @param {number} southLat
     * @param {number} westLng
     * @param {number} northLat
     * @param {number} eastLng
     * @param {boolean} animated
     * @param {number[]|null} padding - [x, y] in pixels
     */
    function fitBounds(container, southLat, westLng, northLat, eastLng, animated, padding) {
        const inst = getInst(container);
        if (!inst) return;
        inst.programmaticViewChange = true;
        const options = { animate: animated };
        if (padding) options.padding = padding;
        inst.map.fitBounds([[southLat, westLng], [northLat, eastLng]], options);
    }

    /**
     * Gets the current map view.
     * @param {HTMLElement} container
     * @returns {{ lat: number, lng: number, zoom: number, south: number, west: number, north: number, east: number }}
     */
    function getView(container) {
        const inst = getInst(container);
        if (!inst) return null;
        const center = inst.map.getCenter();
        const bounds = inst.map.getBounds();
        return {
            lat: center.lat,
            lng: center.lng,
            zoom: inst.map.getZoom(),
            south: bounds.getSouth(),
            west: bounds.getWest(),
            north: bounds.getNorth(),
            east: bounds.getEast(),
        };
    }

    /**
     * Invalidates the map size (call after container resize).
     * @param {HTMLElement} container
     */
    function invalidateSize(container) {
        const inst = getInst(container);
        if (!inst) return;
        inst.map.invalidateSize();
    }

    /**
     * Destroys the map instance and cleans up.
     * @param {HTMLElement} container
     */
    function destroy(container) {
        const mapId = container.id || container.getAttribute('data-map-id');
        const inst = instances[mapId];
        if (!inst) return;

        // Remove all layers
        for (const layer of inst.layers) {
            if (layer) {
                try { inst.map.removeLayer(layer); } catch (_) { }
            }
        }

        // Remove event listeners and destroy map
        try {
            inst.map.off();
            inst.map.remove();
        } catch (_) { }

        delete instances[mapId];
    }

    function getInst(container) {
        const mapId = container.id || container.getAttribute('data-map-id');
        return instances[mapId] || null;
    }

    return {
        initialize,
        addTileLayer,
        addMarker,
        addCircle,
        addPolyline,
        addPolygon,
        removeLayer,
        clearLayers,
        setView,
        panTo,
        zoomIn,
        zoomOut,
        fitBounds,
        getView,
        invalidateSize,
        destroy,
    };
})();

export { xfMap };
