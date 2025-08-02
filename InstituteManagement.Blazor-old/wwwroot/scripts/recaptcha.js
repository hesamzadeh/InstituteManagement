window.executeRecaptcha = async function () {
    return await grecaptcha.execute('6LdpWZcrAAAAAHve6fyRO5UIMDPXpZgHQesM0hM3', { action: 'submit' });
};
