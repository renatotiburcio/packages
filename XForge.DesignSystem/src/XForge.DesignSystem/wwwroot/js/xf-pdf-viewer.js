// xf-pdf-viewer.js — PDF.js canvas rendering for XFPdfViewer
// ES module pattern: const obj = { methods }; export { obj };

const xfPdfViewer = (() => {
    const PDFJS_CDN = 'https://cdnjs.cloudflare.com/ajax/libs/pdf.js/3.11.174/pdf.min.js';
    const PDFJS_WORKER_CDN = 'https://cdnjs.cloudflare.com/ajax/libs/pdf.js/3.11.174/pdf.worker.min.js';
    const instances = {};
    let pdfJsLoaded = false;
    let pdfJsLoadPromise = null;

    function ensurePdfJs() {
        if (pdfJsLoaded) return Promise.resolve();
        if (pdfJsLoadPromise) return pdfJsLoadPromise;

        pdfJsLoadPromise = new Promise((resolve, reject) => {
            const script = document.createElement('script');
            script.src = PDFJS_CDN;
            script.async = true;
            script.onload = () => {
                if (window.pdfjsLib) {
                    window.pdfjsLib.GlobalWorkerOptions.workerSrc = PDFJS_WORKER_CDN;
                    pdfJsLoaded = true;
                    resolve();
                } else {
                    pdfJsLoadPromise = null;
                    reject(new Error('PDF.js loaded but pdfjsLib not found'));
                }
            };
            script.onerror = () => {
                pdfJsLoadPromise = null;
                reject(new Error('Failed to load PDF.js from CDN'));
            };
            document.head.appendChild(script);
        });

        return pdfJsLoadPromise;
    }

    async function initialize(containerId, canvasId, dotNetRef, source, pageNumber, scale) {
        await ensurePdfJs();

        const canvas = document.getElementById(canvasId);
        if (!canvas) return;

        const instance = {
            containerId,
            canvasId,
            dotNetRef,
            canvas,
            source,
            pdf: null,
            pageNumber: pageNumber || 1,
            totalPages: 0,
            scale: (scale || 100) / 100,
            pageRendering: false,
            pageNumberPending: null,
            destroyed: false,
            documentTitle: null,
        };

        instances[containerId] = instance;
        await loadDocument(instance, source);
    }

    async function loadDocument(instance, source) {
        if (!instance || instance.destroyed || !source) return;

        if (instance.pdf) {
            try { instance.pdf.destroy(); } catch { /* ignore */ }
            instance.pdf = null;
        }

        let loadSource = source;
        if (source.startsWith('data:application/pdf;base64,')) {
            const base64 = source.split(',')[1];
            const raw = atob(base64);
            const uint8 = new Uint8Array(raw.length);
            for (let i = 0; i < raw.length; i++) uint8[i] = raw.charCodeAt(i);
            loadSource = { data: uint8 };
        }

        try {
            const loadingTask = window.pdfjsLib.getDocument(loadSource);
            const pdf = await loadingTask.promise;

            if (instance.destroyed) {
                pdf.destroy();
                return;
            }

            instance.pdf = pdf;
            instance.totalPages = pdf.numPages;

            if (instance.pageNumber > instance.totalPages) {
                instance.pageNumber = instance.totalPages;
            }
            if (instance.pageNumber < 1) {
                instance.pageNumber = 1;
            }

            // Extract document title from metadata
            try {
                const metadata = await pdf.getMetadata();
                const infoTitle = metadata?.info?.Title;
                if (typeof infoTitle === 'string' && infoTitle.trim().length > 0) {
                    instance.documentTitle = infoTitle.trim();
                }
            } catch { /* ignore metadata errors */ }

            renderPage(instance, instance.pageNumber);

            if (instance.dotNetRef && !instance.destroyed) {
                instance.dotNetRef.invokeMethodAsync('NotifyPdfInitialized', {
                    pageNumber: instance.pageNumber,
                    totalPages: instance.totalPages,
                    scale: Math.round(instance.scale * 100),
                    title: instance.documentTitle || '',
                });
            }
        } catch (error) {
            if (!instance.destroyed) {
                console.error('XFPdfViewer: Failed to load PDF:', error);
            }
        }
    }

    function renderPage(instance, pageNumber) {
        if (!instance || instance.destroyed || !instance.pdf) return;

        instance.pageRendering = true;

        instance.pdf.getPage(pageNumber).then((page) => {
            const viewport = page.getViewport({ scale: instance.scale });
            const canvas = instance.canvas;
            const context = canvas.getContext('2d');
            canvas.height = viewport.height;
            canvas.width = viewport.width;

            const renderContext = {
                canvasContext: context,
                viewport: viewport,
            };

            const renderTask = page.render(renderContext);
            renderTask.promise.then(() => {
                if (instance.destroyed) return;

                instance.pageRendering = false;

                if (instance.pageNumberPending !== null) {
                    renderPage(instance, instance.pageNumberPending);
                    instance.pageNumberPending = null;
                }

                notifyChanged(instance);
            });
        });
    }

    function queueRenderPage(instance, pageNumber) {
        if (instance && pageNumber) {
            if (instance.pageRendering) {
                instance.pageNumberPending = pageNumber;
            } else {
                renderPage(instance, pageNumber);
            }
        }
    }

    function goToPage(containerId, pageNumber) {
        const instance = instances[containerId];
        if (!instance || !instance.pdf) return;

        if (pageNumber < 1 || pageNumber > instance.totalPages) return;

        instance.pageNumber = pageNumber;
        queueRenderPage(instance, pageNumber);
    }

    function setScale(containerId, scalePercent) {
        const instance = instances[containerId];
        if (!instance || !instance.pdf) return;

        instance.scale = scalePercent / 100;
        queueRenderPage(instance, instance.pageNumber);
    }

    function nextPage(containerId) {
        const instance = instances[containerId];
        if (!instance || !instance.pdf) return;

        if (instance.pageNumber >= instance.totalPages) return;

        instance.pageNumber++;
        queueRenderPage(instance, instance.pageNumber);
    }

    function prevPage(containerId) {
        const instance = instances[containerId];
        if (!instance || !instance.pdf) return;

        if (instance.pageNumber <= 1) return;

        instance.pageNumber--;
        queueRenderPage(instance, instance.pageNumber);
    }

    async function printPdf(containerId) {
        const instance = instances[containerId];
        if (!instance || !instance.pdf) return;

        const iframe = document.createElement('iframe');
        iframe.style.cssText = 'position:fixed;left:-9999px;top:-9999px;width:0;height:0;border:0;';
        document.body.appendChild(iframe);

        try {
            const doc = iframe.contentDocument || iframe.contentWindow.document;
            doc.open();
            doc.write('<html><head><title>Print</title></head><body></body></html>');
            doc.close();

            const container = doc.body;
            const pdf = instance.pdf;

            for (let i = 1; i <= pdf.numPages; i++) {
                const page = await pdf.getPage(i);
                const viewport = page.getViewport({ scale: 1.5 });
                const canvas = doc.createElement('canvas');
                canvas.width = viewport.width;
                canvas.height = viewport.height;
                canvas.style.cssText = 'display:block;margin:0 auto;page-break-after:always;';

                const ctx = canvas.getContext('2d');
                await page.render({ canvasContext: ctx, viewport }).promise;
                container.appendChild(canvas);
            }

            if (iframe.contentWindow) {
                iframe.contentWindow.focus();
                iframe.contentWindow.print();
            }
        } catch (error) {
            console.error('XFPdfViewer: Print failed:', error);
        } finally {
            setTimeout(() => {
                if (iframe.parentNode) {
                    iframe.parentNode.removeChild(iframe);
                }
            }, 1000);
        }
    }

    function downloadPdf(containerId, fileName) {
        const instance = instances[containerId];
        if (!instance) return;

        const resolvedName = fileName || instance.documentTitle || 'document.pdf';
        const finalName = resolvedName.toLowerCase().endsWith('.pdf')
            ? resolvedName
            : resolvedName + '.pdf';

        const link = document.createElement('a');
        link.href = instance.source;
        link.download = finalName;
        link.style.display = 'none';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }

    function toggleFullscreen(containerId) {
        const container = document.getElementById(containerId);
        if (!container) return;

        if (document.fullscreenElement) {
            document.exitFullscreen();
        } else {
            container.requestFullscreen().catch((err) => {
                console.error('XFPdfViewer: Fullscreen failed:', err);
            });
        }
    }

    function notifyChanged(instance) {
        if (!instance || instance.destroyed || !instance.dotNetRef) return;

        instance.dotNetRef.invokeMethodAsync('NotifyPdfChanged', {
            pageNumber: instance.pageNumber,
            totalPages: instance.totalPages,
            scale: Math.round(instance.scale * 100),
            title: instance.documentTitle || '',
        });
    }

    function destroy(containerId) {
        const instance = instances[containerId];
        if (!instance) return;

        instance.destroyed = true;

        if (instance.pdf) {
            try { instance.pdf.destroy(); } catch { /* ignore */ }
            instance.pdf = null;
        }

        delete instances[containerId];
    }

    return {
        initialize,
        destroy,
        goToPage,
        setScale,
        nextPage,
        prevPage,
        printPdf,
        downloadPdf,
        toggleFullscreen,
    };
})();

export { xfPdfViewer };
