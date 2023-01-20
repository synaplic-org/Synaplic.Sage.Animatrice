window.setupTrixEvent = () => {
    document.addEventListener("trix-change", (event) =>
        Trix.triggerEvent("change", { "onElement": event.target.inputElement }));
}