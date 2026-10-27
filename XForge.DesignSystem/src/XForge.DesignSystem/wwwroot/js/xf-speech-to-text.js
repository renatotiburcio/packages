// xf-speech-to-text.js — Web Speech API wrapper for XFSpeechToText
// ES module pattern: const obj = { methods }; export { obj };

const xfSpeechToText = (() => {
    let recognition = null;

    function isSupported() {
        return 'webkitSpeechRecognition' in window || 'SpeechRecognition' in window;
    }

    function startRecognition(dotNetRef, language, continuous, interimResults) {
        if (!isSupported()) return false;

        const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
        recognition = new SpeechRecognition();

        recognition.lang = language || 'pt-BR';
        recognition.continuous = continuous || false;
        recognition.interimResults = interimResults || false;
        recognition.maxAlternatives = 1;

        recognition.onresult = (event) => {
            const last = event.results[event.results.length - 1];
            if (last.isFinal || interimResults) {
                const transcript = last[0].transcript;
                dotNetRef.invokeMethodAsync('OnSpeechResult', transcript);
            }
        };

        recognition.onend = () => {
            dotNetRef.invokeMethodAsync('OnSpeechEnd');
            recognition = null;
        };

        recognition.onerror = (event) => {
            dotNetRef.invokeMethodAsync('OnSpeechError', event.error || 'Unknown error');
            recognition = null;
        };

        try {
            recognition.start();
            return true;
        } catch (e) {
            return false;
        }
    }

    function stopRecognition() {
        if (recognition) {
            recognition.stop();
            recognition = null;
        }
    }

    async function checkMicAvailability() {
        if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
            return false;
        }
        try {
            const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
            stream.getTracks().forEach(t => t.stop());
            return true;
        } catch {
            return false;
        }
    }

    return { isSupported, startRecognition, stopRecognition, checkMicAvailability };
})();

export { xfSpeechToText };
