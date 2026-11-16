document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll("form[data-piszkozat]").forEach(function (urlap) {
        var kulcs = "piszkozat:" + urlap.getAttribute("data-piszkozat");

        function mezok() {
            return Array.prototype.filter.call(urlap.elements, function (elem) {
                return elem.name && elem.type !== "file" && elem.type !== "hidden"
                    && elem.type !== "submit" && elem.type !== "button" && elem.type !== "password";
            });
        }

        function mentes() {
            var adat = {};

            mezok().forEach(function (elem) {
                adat[elem.name] = elem.type === "checkbox" ? elem.checked : elem.value;
            });

            try {
                localStorage.setItem(kulcs, JSON.stringify(adat));
            } catch (e) {
            }
        }

        var tarolt = null;

        try {
            tarolt = JSON.parse(localStorage.getItem(kulcs));
        } catch (e) {
        }

        if (tarolt) {
            mezok().forEach(function (elem) {
                if (!(elem.name in tarolt) || (elem.value && elem.type !== "checkbox")) {
                    return;
                }

                if (elem.type === "checkbox") {
                    elem.checked = tarolt[elem.name];
                } else {
                    elem.value = tarolt[elem.name];
                }
            });

            document.dispatchEvent(new Event("piszkozat-visszaallitva"));
        }

        urlap.addEventListener("input", mentes);
        urlap.addEventListener("change", mentes);

        urlap.addEventListener("submit", function () {
            try {
                localStorage.removeItem(kulcs);
            } catch (e) {
            }
        });
    });
});
