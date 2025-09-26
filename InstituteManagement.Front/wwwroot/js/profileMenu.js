window.profileMenu = {
    setTheme: function (mode) {
        try {
            localStorage.setItem('theme', mode);
            if (mode === 'dark') document.documentElement.classList.add('dark');
            else document.documentElement.classList.remove('dark');
        } catch (e) { console.warn(e); }
    },
    getSavedTheme: function () {
        try {
            var t = localStorage.getItem('theme');
            return t === 'dark';
        } catch (e) { return false; }
    }
};
