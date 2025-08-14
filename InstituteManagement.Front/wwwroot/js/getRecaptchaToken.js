window.getRecaptchaToken = function () {
    return new Promise((resolve, reject) => {
        if (typeof grecaptcha !== 'undefined') {
            grecaptcha.ready(function () {
                grecaptcha.execute('6LdpWZcrAAAAAHve6fyRO5UIMDPXpZgHQesM0hM3', { action: 'submit' })
                    .then(resolve)
                    .catch(reject);
            });
        } else {
            reject("grecaptcha not loaded");
        }
    });
};