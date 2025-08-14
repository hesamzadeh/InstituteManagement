// wwwroot/js/site.js
(function () {
    // convert Persian/Arabic digits to western
    function toWesternDigits(s) {
        if (!s) return s;
        return s.replace(/[\u06F0-\u06F9]/g, function (d) { return String.fromCharCode('0'.charCodeAt(0) + d.charCodeAt(0) - 0x06F0); })
            .replace(/[\u0660-\u0669]/g, function (d) { return String.fromCharCode('0'.charCodeAt(0) + d.charCodeAt(0) - 0x0660); });
    }

    // produce ISO yyyy-MM-dd string from persian-date input or unix timestamp or string
    function toIsoGregorian(value) {
        try {
            if (value == null) return "";

            // if numeric unix ms
            if (typeof value === 'number' || /^\d+$/.test(String(value))) {
                var pd = new persianDate(Number(value));
                // convert to gregorian calendar and format ISO
                var g = pd.toCalendar('gregorian');
                return g.format('YYYY-MM-DD');
            }

            // object with unix property
            if (typeof value === 'object' && value !== null) {
                if (value.unix) {
                    var pd2 = new persianDate(Number(value.unix));
                    return pd2.toCalendar('gregorian').format('YYYY-MM-DD');
                }
                // fallback to string
                value = String(value);
            }

            // string: normalize digits then try persian-date parser then fallback to regex parse
            var s = toWesternDigits(String(value).trim());
            if (!s) return "";

            // if string is already ISO-like yyyy-MM-dd or yyyy/MM/dd
            var isoMatch = s.match(/^(\d{4})[\/\-](\d{1,2})[\/\-](\d{1,2})$/);
            if (isoMatch) {
                var y = Number(isoMatch[1]), m = Number(isoMatch[2]), d = Number(isoMatch[3]);
                var dt = new Date(y, m - 1, d);
                return dt.getFullYear().toString().padStart(4, '0') + '-' + (dt.getMonth() + 1).toString().padStart(2, '0') + '-' + dt.getDate().toString().padStart(2, '0');
            }

            // try persianDate parser (handles many formats)
            try {
                var pd3 = new persianDate(s);
                return pd3.toCalendar('gregorian').format('YYYY-MM-DD');
            } catch (e) {
                console.warn("persian-date parse failed:", s, e);
            }
        } catch (e) {
            console.error("toIsoGregorian error:", e, value);
        }
        return "";
    }

    window.initPersianDatePicker = function (id) {
        try {
            var $input = $('#' + id);
            if (!$input || !$input.length) return;

            if ($input.data('pdp-initialized')) return;
            $input.data('pdp-initialized', true);

            $input.persianDatepicker({
                format: 'YYYY/MM/DD', // UI format displayed inside picker (plugin controls this)
                autoClose: true,
                onSelect: function (selected) {
                    console.log("persianDatepicker.onSelect raw:", selected);
                    var iso = toIsoGregorian(selected);
                    if (!iso) {
                        // fallback: read input and convert
                        var raw = toWesternDigits($input.val());
                        iso = toIsoGregorian(raw);
                    }
                    if (iso) {
                        console.log("persianDatepicker -> setting iso:", iso);
                        // Use ISO for input value so Blazor parses easily
                        $input.val(iso);
                        // dispatch input event to notify Blazor binding
                        $input[0].dispatchEvent(new Event('input', { bubbles: true }));
                    } else {
                        console.warn("Could not convert selected date to ISO:", selected);
                    }
                }
            });

            // sure-fire fallback: when input loses focus or changes, convert its visible value and notify Blazor
            $input.on('change blur', function () {
                var raw = toWesternDigits($input.val());
                console.log("input change/blur value:", raw);
                var iso = toIsoGregorian(raw);
                if (iso) {
                    if ($input.val() !== iso) {
                        $input.val(iso);
                    }
                    $input[0].dispatchEvent(new Event('input', { bubbles: true }));
                }
            });

            // initial value: if empty, set today's ISO date and notify once
            if (!$input.val()) {
                var t = new Date();
                var isoToday = t.getFullYear().toString().padStart(4, '0') + '-' + (t.getMonth() + 1).toString().padStart(2, '0') + '-' + t.getDate().toString().padStart(2, '0');
                $input.val(isoToday);
                $input[0].dispatchEvent(new Event('input', { bubbles: true }));
            }
        } catch (e) {
            console.error("initPersianDatePicker error:", e);
        }
    };
})();
