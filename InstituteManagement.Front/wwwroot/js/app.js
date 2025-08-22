window.app = window.app || {};

app.setCulture = function (code) {
    // set AspNet Core culture cookie (common pattern)
    var cookieValue = 'c=' + encodeURIComponent(code) + '|uic=' + encodeURIComponent(code);
    document.cookie = '.AspNetCore.Culture=' + cookieValue + ';path=/';
    location.reload();
};

app.getCulture = function () {
    // try read cookie quickly (best-effort)
    var m = document.cookie.match(/\.AspNetCore\.Culture=([^;]+)/);
    if (m && m[1]) {
        var val = decodeURIComponent(m[1]);
        var match = val.match(/c=([^|]+)/);
        if (match) return match[1];
    }
    return null;
};