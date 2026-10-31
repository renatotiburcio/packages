export function scrollMainToTop(mainId) {
    const prefersReducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    const behavior = prefersReducedMotion ? "auto" : "smooth";
    const main = document.getElementById(mainId);

    if (main && typeof main.scrollTo === "function") {
        main.scrollTo({ top: 0, left: 0, behavior });

        if (typeof main.focus === "function") {
            main.focus({ preventScroll: true });
        }

        return;
    }

    window.scrollTo({ top: 0, left: 0, behavior });
}
