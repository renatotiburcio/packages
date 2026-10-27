// XFSchedulerAdvanced JavaScript Interop

export const scheduler = {
    // Update visual feedback during drag
    updateDragVisual(dotNetRef, clientX, clientY) {
        // This can be used for more sophisticated drag feedback
        // For now, the Blazor component handles the visual updates
    },

    // Download file for export
    downloadFile(filename, content) {
        const blob = new Blob([content], { type: 'application/json' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    },

    // Get element position relative to container
    getElementPosition(element, container) {
        const elementRect = element.getBoundingClientRect();
        const containerRect = container.getBoundingClientRect();
        return {
            x: elementRect.left - containerRect.left,
            y: elementRect.top - containerRect.top,
            width: elementRect.width,
            height: elementRect.height
        };
    },

    // Smooth scroll to element
    scrollToElement(container, element) {
        const containerRect = container.getBoundingClientRect();
        const elementRect = element.getBoundingClientRect();

        if (elementRect.top < containerRect.top || elementRect.bottom > containerRect.bottom) {
            element.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
        }
    },

    // Initialize drag and drop with pointer events
    initDragDrop(element, dotNetRef) {
        let isDragging = false;
        let startX, startY;

        element.addEventListener('pointerdown', (e) => {
            isDragging = true;
            startX = e.clientX;
            startY = e.clientY;
            element.setPointerCapture(e.pointerId);
        });

        element.addEventListener('pointermove', (e) => {
            if (!isDragging) return;
            // The Blazor component handles the actual movement
        });

        element.addEventListener('pointerup', (e) => {
            isDragging = false;
        });
    }
};
