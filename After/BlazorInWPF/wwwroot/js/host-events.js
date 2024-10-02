function notifyWpfApp(hostEventName, content) {
    const blazorHostMessage = { blazorHostEventName: hostEventName, content: content }
    window.chrome.webview.postMessage(blazorHostMessage);
}