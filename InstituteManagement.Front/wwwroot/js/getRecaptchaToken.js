// wwwroot/js/getRecaptchaToken.js
window.getRecaptchaToken = function () {
    return new Promise((resolve, reject) => {
        var siteKey = '6LdpWZcrAAAAAHve6fyRO5UIMDPXpZgHQesM0hM3';

        if (typeof grecaptcha !== 'undefined' && grecaptcha && grecaptcha.execute) {
            try {
                grecaptcha.ready(function () {
                    grecaptcha.execute(siteKey, { action: 'submit' })
                        .then(token => resolve(token))
                        .catch(err => reject(err));
                });
            } catch (e) {
                reject(e);
            }
        } else {
            reject("grecaptcha not loaded");
        }
    });
};
