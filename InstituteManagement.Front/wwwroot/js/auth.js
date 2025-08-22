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
            // if absolute url already
            if (/^https?:\/\//i.test(pathOrUrl)) return pathOrUrl;
            // if path begins with '/', append to base
            if (pathOrUrl.startsWith('/')) return this._apiBase + pathOrUrl;
            // otherwise treat as relative path
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

                return { ok: res.ok, status: res.status, body };
            } catch (err) {
                return { ok: false, status: 0, error: err && err.message ? err.message : String(err) };
            }
        },

        // POST to a given API path (useful for logout). Returns { ok, status, body? }
        post: async function (url) {
            const full = this._toUrl(url);
            try {
                const res = await fetch(full, { method: 'POST', credentials: 'include' });
                // try to parse response body if any
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
                if (!res.ok) return { IsAuthenticated: false, Username: '' };

                const payload = await res.json();

                // Normalize common property names (some endpoints return camelCase, some PascalCase)
                const isAuthenticated =
                    payload.IsAuthenticated ?? payload.isAuthenticated ?? payload.isAuthenticated ?? payload.authenticated ?? false;
                const username =
                    payload.Username ?? payload.username ?? payload.name ?? payload.userName ?? '';

                return { IsAuthenticated: Boolean(isAuthenticated), Username: String(username) };
            } catch (err) {
                return { IsAuthenticated: false, Username: '' };
            }
        }
    };
})();
