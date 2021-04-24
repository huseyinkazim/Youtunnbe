function init() {
    var vidDefer = document.getElementsByTagName('iframe');
    for (var i = 0; i < vidDefer.length; i++) {
        if (vidDefer[i].getAttribute('data-src')) {
            vidDefer[i].setAttribute('src', vidDefer[i].getAttribute('data-src'));
        }
    }


    if (link != "") {
        debugger;
        $("#link").val(link);
        $("form").submit();

    }

    debugger;
    //if (downloadMusicLink != "") {
    //    new QRCode(document.getElementById("qrcode"), downloadMusicLink);
    //}
}

window.onload = init;

