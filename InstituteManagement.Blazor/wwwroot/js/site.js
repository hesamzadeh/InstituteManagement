window.initDatePicker = (selector, locale) => {
    flatpickr(selector, {
        dateFormat: "Y-m-d",
        locale: locale === "fa" ? flatpickr.l10ns.fa : "default",
        onChange: function (selectedDates, dateStr) {
            document.querySelector(selector).value = dateStr;
        }
    });
};
