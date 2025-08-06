// wwwroot/js/blazorCulture.js
window.setLanguageCookie = function (lang) {
    const value = `.AspNetCore.Culture=c=${lang}|uic=${lang}`;
    const expires = new Date(Date.now() + 365 * 24 * 60 * 60 * 1000).toUTCString();
    document.cookie = `.AspNetCore.Culture=${value}; path=/; expires=${expires}`;
};
