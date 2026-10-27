/**
 * XForge Breakpoint Observer — ES module
 *
 * Watches viewport width via matchMedia and reports
 * the current Tailwind-compatible breakpoint to .NET.
 *
 * Breakpoints (matching Tailwind defaults):
 *   xs:  <640px
 *   sm:  >=640px
 *   md:  >=768px
 *   lg:  >=1024px
 *   xl:  >=1280px
 *   xxl: >=1536px
 */

const QUERIES = [
    { name: 'xxl', query: '(min-width: 1536px)' },
    { name: 'xl',  query: '(min-width: 1280px)' },
    { name: 'lg',  query: '(min-width: 1024px)' },
    { name: 'md',  query: '(min-width: 768px)' },
    { name: 'sm',  query: '(min-width: 640px)' },
    { name: 'xs',  query: '(max-width: 639px)' },
];

let dotNetRef = null;
let listeners = [];
let currentBreakpoint = 'xs';

function detect() {
    for (const { name, query } of QUERIES) {
        if (window.matchMedia(query).matches) {
            return name;
        }
    }
    return 'xs';
}

function handleChange() {
    const next = detect();
    if (next !== currentBreakpoint) {
        currentBreakpoint = next;
        if (dotNetRef) {
            dotNetRef.invokeMethodAsync('OnBreakpointChanged', next);
        }
    }
}

export function initialize(ref) {
    dotNetRef = ref;
    currentBreakpoint = detect();

    for (const { query } of QUERIES) {
        const mql = window.matchMedia(query);
        const handler = () => handleChange();
        mql.addEventListener('change', handler);
        listeners.push({ mql, handler });
    }

    // Report initial breakpoint
    if (dotNetRef) {
        dotNetRef.invokeMethodAsync('OnBreakpointChanged', currentBreakpoint);
    }
}

export function dispose() {
    for (const { mql, handler } of listeners) {
        mql.removeEventListener('change', handler);
    }
    listeners = [];
    dotNetRef = null;
}
