// wwwroot/js/auth.js 
(function () {
    // appAuth helper
    window.appAuth = {
        // internal base; can be configured via init() or window.__apiBase (see instructions)
        _apiBase: (window.__apiBase || window.API_BASE || window.location.origin).replace(/\/+$/, ''),

        /**
         * Optionally initialize with an explicit base URL (e.g. "https://localhost:5271")
         * You can call: await JS.InvokeVoidAsync("appAuth.init", "https://localhost:5271");
         */
        init: function (apiBase) {
            if (apiBase && typeof apiBase === "string") {
                this._apiBase = apiBase.replace(/\/+$/, '');
            }
        },

        // resolve a path or absolute URL to a full URL
        _toUrl: function (pathOrUrl) {
            if (!pathOrUrl) return this._apiBase;
            if (/^https?:\/\//i.test(pathOrUrl)) return pathOrUrl;
            if (pathOrUrl.startsWith('/')) return this._apiBase + pathOrUrl;
            return this._apiBase + '/' + pathOrUrl;
        },

        // Generic sign-in helper that returns { ok, status, body }
        signIn: async function (url, payload) {
            const full = this._toUrl(url);
            try {
                const res = await fetch(full, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload),
                    credentials: 'include' // IMPORTANT: include cookies in request and allow browser to store Set-Cookie
                });

                const txt = await res.text();
                let body;
                try { body = JSON.parse(txt); } catch { body = txt; }

                return { ok: res.ok, status: res.status, body: body };
            } catch (err) {
                return { ok: false, status: 0, error: err && err.message ? err.message : String(err) };
            }
        },

        // POST to a given API path (useful for logout). Returns { ok, status, body? }
        post: async function (url) {
            const full = this._toUrl(url);
            try {
                const res = await fetch(full, { method: 'POST', credentials: 'include' });
                let parsed = null;
                try { parsed = await res.json(); } catch { parsed = null; }
                return { ok: res.ok, status: res.status, body: parsed };
            } catch (err) {
                return { ok: false, status: 0, error: err && err.message ? err.message : String(err) };
            }
        },

        // whoami -> returns whatever the API returned (normalized keys)
        whoami: async function (url) {
            const full = this._toUrl(url);
            try {
                const res = await fetch(full, { credentials: 'include' });
                if (!res.ok) return { IsAuthenticated: false, Username: '', FullName: '' };

                const payload = await res.json();

                // Normalize common property names and include FullName normalization
                const isAuthenticated =
                    payload.IsAuthenticated ?? payload.isAuthenticated ?? payload.authenticated ?? false;
                const username =
                    payload.Username ?? payload.username ?? payload.name ?? payload.userName ?? '';
                const fullName =
                    payload.FullName ?? payload.fullName ?? payload.displayName ?? '';

                return {
                    IsAuthenticated: Boolean(isAuthenticated),
                    Username: String(username),
                    FullName: String(fullName)
                };
            } catch (err) {
                return { IsAuthenticated: false, Username: '', FullName: '' };
            }
        },

        get: async function (url) {
            const full = this._toUrl(url);
            try {
                const res = await fetch(full, { method: 'GET', credentials: 'include' });
                const txt = await res.text();
                let body;
                try { body = JSON.parse(txt); } catch { body = txt; }
                return { ok: res.ok, status: res.status, body };
            } catch (err) {
                return { ok: false, status: 0, error: err && err.message ? err.message : String(err) };
            }
        },

        put: async function (url, payload) {
            const full = this._toUrl(url);
            try {
                const res = await fetch(full, {
                    method: 'PUT',
                    headers: { 'Content-Type': 'application/json' },
                    credentials: 'include',
                    body: JSON.stringify(payload)
                });

                const txt = await res.text();
                let body;
                try { body = JSON.parse(txt); } catch { body = txt; }

                return { ok: res.ok, status: res.status, body };
            } catch (err) {
                return { ok: false, status: 0, error: err && err.message ? err.message : String(err) };
            }
        },

        // culture helpers (used by NavMenu earlier)
        setCulture: function (code) {
            try {
                var cookieValue = 'c=' + encodeURIComponent(code) + '|uic=' + encodeURIComponent(code);
                document.cookie = '.AspNetCore.Culture=' + cookieValue + ';path=/';
                location.reload();
            } catch (e) {
                console.warn("setCulture failed", e);
            }
        },

        getCulture: function () {
            try {
                var m = document.cookie.match(/\.AspNetCore\.Culture=([^;]+)/);
                if (m && m[1]) {
                    var val = decodeURIComponent(m[1]);
                    var match = val.match(/c=([^|]+)/);
                    if (match) return match[1];
                }
            } catch (e) {
                console.warn("getCulture failed", e);
            }
            return null;
        }
    };
})();
