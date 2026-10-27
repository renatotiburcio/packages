// DomEventInterop.ts
// Centralized DOM event subscription bridge. Used by DomEventService.
export function subscribe(eventName: string, elementId: string, dotnetRef: any): void {
    const el = document.getElementById(elementId);
    if (!el) {
        console.warn(`[DomEventInterop] Element not found: ${elementId}`);
        return;
    }
    const handler = (e: Event) => {
        dotnetRef.invokeMethodAsync("OnDomEvent", eventName, serializeEvent(e));
    };
    (el as any).__domHandlers = (el as any).__domHandlers || {};
    (el as any).__domHandlers[eventName] = handler;
    el.addEventListener(eventName, handler);
}

export function unsubscribe(eventName: string, elementId: string): void {
    const el = document.getElementById(elementId);
    if (!el) return;
    const handlers = (el as any).__domHandlers || {};
    const handler = handlers[eventName];
    if (handler) {
        el.removeEventListener(eventName, handler);
        delete handlers[eventName];
    }
}

function serializeEvent(e: Event): string {
    return JSON.stringify({
        type: e.type,
        target: (e.target as HTMLElement)?.id || "",
        timeStamp: e.timeStamp,
    });
}