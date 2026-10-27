// xf-lottie.js — Lottie animation wrapper for XFLottie
// Uses lottie-web (https://github.com/airbnb/lottie-web) loaded from CDN
// ES module pattern: const obj = { methods }; export { obj };

const xfLottie = (() => {
    const CDN_URL = 'https://cdnjs.cloudflare.com/ajax/libs/lottie-web/5.12.2/lottie.min.js';
    let lottieLib = null;
    let loadPromise = null;

    /**
     * Loads lottie-web from CDN if not already loaded.
     * @returns {Promise<void>}
     */
    function ensureLottieLoaded() {
        if (lottieLib) return Promise.resolve();
        if (loadPromise) return loadPromise;

        loadPromise = new Promise((resolve, reject) => {
            const script = document.createElement('script');
            script.src = CDN_URL;
            script.async = true;
            script.onload = () => {
                lottieLib = window.lottie;
                resolve();
            };
            script.onerror = () => {
                loadPromise = null;
                reject(new Error('Failed to load lottie-web from CDN'));
            };
            document.head.appendChild(script);
        });

        return loadPromise;
    }

    /**
     * Creates a Lottie animation in the given container.
     * @param {HTMLElement} container - DOM element to render into
     * @param {string} path - URL to Lottie JSON
     * @param {boolean} autoplay - Auto-play on load
     * @param {boolean} loop - Loop animation
     * @param {number} speed - Playback speed
     * @param {number} direction - 1 = forward, -1 = backward
     * @param {string} renderer - 'svg', 'canvas', or 'html'
     * @param {object} dotNetRef - .NET object reference for callbacks
     * @returns {Promise<object>} Animation control handle
     */
    async function createAnimation(container, path, autoplay, loop, speed, direction, renderer, dotNetRef) {
        await ensureLottieLoaded();

        const anim = lottieLib.loadAnimation({
            container: container,
            renderer: renderer || 'svg',
            loop: loop,
            autoplay: autoplay,
            path: path,
        });

        anim.setSpeed(speed);
        anim.setDirection(direction);

        // Event handlers
        anim.addEventListener('complete', () => {
            try { dotNetRef.invokeMethodAsync('NotifyCompleted'); } catch {}
        });

        anim.addEventListener('loopComplete', () => {
            try { dotNetRef.invokeMethodAsync('NotifyLoopCompleted'); } catch {}
        });

        anim.addEventListener('DOMLoaded', () => {
            try {
                const totalFrames = anim.totalFrames || 0;
                const currentFrame = anim.currentFrame || 0;
                const frameRate = anim.frameRate || 30;
                dotNetRef.invokeMethodAsync('NotifyLoaded', totalFrames, currentFrame, frameRate);
            } catch {}
        });

        // Control handle
        return {
            play: () => anim.play(),
            pause: () => anim.pause(),
            stop: () => anim.stop(),
            goToAndStop: (frame, play) => {
                if (play) {
                    anim.goToAndPlay(frame, true);
                } else {
                    anim.goToAndStop(frame, true);
                }
            },
            goToAndPlay: (frame) => anim.goToAndPlay(frame, true),
            setSpeed: (spd) => anim.setSpeed(spd),
            setDirection: (dir) => anim.setDirection(dir),
            setLoop: (shouldLoop) => {
                anim.loop = shouldLoop;
            },
            destroy: () => {
                anim.destroy();
            }
        };
    }

    return { createAnimation };
})();

export { xfLottie };
