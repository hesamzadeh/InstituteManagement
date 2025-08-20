// wwwroot/js/auth.js
window.appAuth = {
    signIn: async function (url, payload) {
        const res = await fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload),
            credentials: 'include'   // IMPORTANT: include cookies in request and allow browser to store Set-Cookie
        });
        const txt = await res.text();
        let body;
        try { body = JSON.parse(txt); } catch { body = txt; }
        return { ok: res.ok, status: res.status, body };
    },

    post: async function (url) {
        const res = await fetch(url, { method: 'POST', credentials: 'include' });
        return { ok: res.ok, status: res.status };
    },

    whoami: async function (url) {
        const res = await fetch(url, { credentials: 'include' });
        if (!res.ok) return { isAuthenticated: false, username: '' };
        const payload = await res.json();
        return payload;
    }
};
