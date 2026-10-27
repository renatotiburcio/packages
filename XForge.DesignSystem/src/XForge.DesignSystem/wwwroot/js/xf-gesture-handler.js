/**
 * xf-gesture-handler.js
 * Touch/pointer gesture recognition for XFGestureHandler.
 * Detects: tap, double-tap, long-press, swipe (4 directions), pinch (in/out), rotate.
 */

const instances = new Map();

function initialize(element, dotNetRef, swipeThreshold, swipeVelocityThreshold,
    longPressDuration, doubleTapWindow, enableSwipe, enablePinch, enableLongPress,
    enableDoubleTap, enableRotate) {

    const state = {
        element,
        dotNetRef,
        swipeThreshold,
        swipeVelocityThreshold,
        longPressDuration,
        doubleTapWindow,
        enableSwipe,
        enablePinch,
        enableLongPress,
        enableDoubleTap,
        enableRotate,
        pointers: new Map(),
        startTime: 0,
        startX: 0,
        startY: 0,
        lastTapTime: 0,
        longPressTimer: null,
        isTracking: false,
        initialPinchDistance: 0,
        initialPinchAngle: 0
    };

    const onPointerDown = (e) => handlePointerDown(state, e);
    const onPointerMove = (e) => handlePointerMove(state, e);
    const onPointerUp = (e) => handlePointerUp(state, e);
    const onPointerCancel = (e) => handlePointerCancel(state, e);

    element.addEventListener('pointerdown', onPointerDown, { passive: false });
    element.addEventListener('pointermove', onPointerMove, { passive: false });
    element.addEventListener('pointerup', onPointerUp, { passive: false });
    element.addEventListener('pointercancel', onPointerCancel, { passive: false });
    element.addEventListener('pointerleave', onPointerCancel, { passive: false });

    state.handlers = { onPointerDown, onPointerMove, onPointerUp, onPointerCancel };
    instances.set(element, state);
}

function handlePointerDown(state, e) {
    state.element.setPointerCapture(e.pointerId);
    state.pointers.set(e.pointerId, { x: e.clientX, y: e.clientY, time: Date.now() });

    if (state.pointers.size === 1) {
        state.startTime = Date.now();
        state.startX = e.clientX;
        state.startY = e.clientY;
        state.isTracking = true;

        state.dotNetRef.invokeMethodAsync('OnGestureStateChanged', 'Tracking');

        if (state.enableLongPress) {
            state.longPressTimer = setTimeout(() => {
                if (state.isTracking && state.pointers.size === 1) {
                    const dx = e.clientX - state.startX;
                    const dy = e.clientY - state.startY;
                    const dist = Math.sqrt(dx * dx + dy * dy);
                    if (dist < state.swipeThreshold) {
                        fireGesture(state, 'LongPress', state.startX, state.startY,
                            e.clientX, e.clientY, dx, dy, dist, 0, 0, 1, 0,
                            Date.now() - state.startTime, 1);
                    }
                }
            }, state.longPressDuration);
        }
    } else if (state.pointers.size === 2 && state.enablePinch) {
        const pts = Array.from(state.pointers.values());
        state.initialPinchDistance = getDistance(pts[0], pts[1]);
        state.initialPinchAngle = getAngle(pts[0], pts[1]);
    }
}

function handlePointerMove(state, e) {
    if (!state.isTracking) return;
    state.pointers.set(e.pointerId, { x: e.clientX, y: e.clientY, time: Date.now() });

    if (state.pointers.size === 2 && state.enablePinch) {
        const pts = Array.from(state.pointers.values());
        const currentDist = getDistance(pts[0], pts[1]);
        const scale = state.initialPinchDistance > 0 ? currentDist / state.initialPinchDistance : 1;
        const currentAngle = getAngle(pts[0], pts[1]);
        const rotation = currentAngle - state.initialPinchAngle;

        if (Math.abs(scale - 1) > 0.02) {
            const midX = (pts[0].x + pts[1].x) / 2;
            const midY = (pts[0].y + pts[1].y) / 2;
            fireGesture(state, scale < 1 ? 'PinchIn' : 'PinchOut',
                state.startX, state.startY, midX, midY, 0, 0, 0, 0, 0,
                scale, rotation, Date.now() - state.startTime, 2);
        }
    }
}

function handlePointerUp(state, e) {
    if (!state.isTracking) return;

    clearTimeout(state.longPressTimer);
    state.pointers.delete(e.pointerId);

    if (state.pointers.size > 0) return;

    state.isTracking = false;
    const duration = Date.now() - state.startTime;
    const dx = e.clientX - state.startX;
    const dy = e.clientY - state.startY;
    const dist = Math.sqrt(dx * dx + dy * dy);
    const velocityX = duration > 0 ? Math.abs(dx) / duration : 0;
    const velocityY = duration > 0 ? Math.abs(dy) / duration : 0;

    state.dotNetRef.invokeMethodAsync('OnGestureStateChanged', 'Idle');

    // Detect swipe
    if (state.enableSwipe && dist >= state.swipeThreshold) {
        const maxVel = Math.max(velocityX, velocityY);
        if (maxVel >= state.swipeVelocityThreshold) {
            let type;
            if (Math.abs(dx) > Math.abs(dy)) {
                type = dx > 0 ? 'SwipeRight' : 'SwipeLeft';
            } else {
                type = dy > 0 ? 'SwipeDown' : 'SwipeUp';
            }
            fireGesture(state, type, state.startX, state.startY,
                e.clientX, e.clientY, dx, dy, dist, velocityX, velocityY,
                1, 0, duration, 1);
            return;
        }
    }

    // Detect tap / double-tap
    if (dist < state.swipeThreshold && duration < 300) {
        const now = Date.now();
        if (state.enableDoubleTap && (now - state.lastTapTime) < state.doubleTapWindow) {
            fireGesture(state, 'DoubleTap', state.startX, state.startY,
                e.clientX, e.clientY, dx, dy, dist, 0, 0, 1, 0, duration, 1);
            state.lastTapTime = 0;
        } else {
            fireGesture(state, 'Tap', state.startX, state.startY,
                e.clientX, e.clientY, dx, dy, dist, 0, 0, 1, 0, duration, 1);
            state.lastTapTime = now;
        }
    }
}

function handlePointerCancel(state, e) {
    clearTimeout(state.longPressTimer);
    state.pointers.delete(e.pointerId);
    if (state.pointers.size === 0) {
        state.isTracking = false;
        state.dotNetRef.invokeMethodAsync('OnGestureStateChanged', 'Cancelled');
        state.dotNetRef.invokeMethodAsync('OnGestureStateChanged', 'Idle');
    }
}

function fireGesture(state, type, startX, startY, currentX, currentY,
    deltaX, deltaY, distance, velocityX, velocityY, scale, rotation,
    durationMs, pointerCount) {
    state.dotNetRef.invokeMethodAsync('OnGestureDetected',
        type, startX, startY, currentX, currentY,
        deltaX, deltaY, distance, velocityX, velocityY,
        scale, rotation, durationMs, pointerCount);
}

function getDistance(p1, p2) {
    const dx = p2.x - p1.x;
    const dy = p2.y - p1.y;
    return Math.sqrt(dx * dx + dy * dy);
}

function getAngle(p1, p2) {
    return Math.atan2(p2.y - p1.y, p2.x - p1.x) * 180 / Math.PI;
}

function dispose(element) {
    const state = instances.get(element);
    if (!state) return;

    clearTimeout(state.longPressTimer);
    element.removeEventListener('pointerdown', state.handlers.onPointerDown);
    element.removeEventListener('pointermove', state.handlers.onPointerMove);
    element.removeEventListener('pointerup', state.handlers.onPointerUp);
    element.removeEventListener('pointercancel', state.handlers.onPointerCancel);
    element.removeEventListener('pointerleave', state.handlers.onPointerCancel);
    instances.delete(element);
}

const xfGestureHandler = { initialize, dispose };
export { xfGestureHandler };
