export async function copyText(text) {
    if (navigator.clipboard && window.isSecureContext) {
        await navigator.clipboard.writeText(text ?? "");
        return true;
    }

    const textArea = document.createElement("textarea");
    textArea.value = text ?? "";
    textArea.setAttribute("readonly", "");
    textArea.style.position = "fixed";
    textArea.style.left = "-9999px";
    document.body.appendChild(textArea);
    textArea.select();

    try {
        return document.execCommand("copy");
    } finally {
        document.body.removeChild(textArea);
    }
}
