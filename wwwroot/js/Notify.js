document.addEventListener("DOMContentLoaded", function () {
    if (Notification.permission !== "granted")
        Notification.requestPermission();
});
function notifyMe(from) {
    if (!Notification) {
        alert("Desktop notifications not available in your browser");
        return;
    }

    if (Notification.permission !== "granted")
        Notification.requestPermission();
    else {
        const notification = new Notification("New Message", {
            icon:
                "./wwwroot/favicon-16x16.png",
            body: "You have a new Message From " + from,
        });
        notification.onclick = function () {
            window.focus();
            this.close();
        };
    }
}