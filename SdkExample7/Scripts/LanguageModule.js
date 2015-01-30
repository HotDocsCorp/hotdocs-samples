(function ($) {
    // Register the new language module with the HotDocs locale infrastructure.
    var Locales = $.Locales = (typeof $.Locales === "undefined") ? {} : $.Locales;
    // Add a new language module definition to the array.
    Locales["de-DE"] = {
        name: "German (Germany)",
        dateOrder: "DMY",
        months: ["Januar", "Februar", "März", "April", "Mai", "Juni", "Juli", "August", "September", "Oktober", "November", "Dezember"],
        monthsShort: ["Jan", "Feb", "Mär", "Apr", "Mai", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dez"],
        days: ["Sonntag", "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag", "Samstag"],
        daysShort: ["So", "Mo", "Di", "Mi", "Do", "Fr", "Sa"],
        calWeekBegin: 1,
        calWeekend: [0, 6],
        zeroFormats: ["Null"],
        numSeps: [",", "."],
        strings: { ui_yes: "Ja", ui_no: "Nein" },
        SpellNum: function (value, strZero, bOrdinal) { }
    };
    // Either use the existing array or create a new one if it is not yet defined in the global scope.
    window.HOTDOC$ = $;
}(typeof HOTDOC$ === "undefined" ? {} : HOTDOC$));