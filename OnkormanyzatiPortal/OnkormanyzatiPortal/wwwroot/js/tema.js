(function () {
    var gyoker = document.documentElement;

    function tarolt(kulcs, alap) {
        try {
            return localStorage.getItem(kulcs) || alap;
        } catch (e) {
            return alap;
        }
    }

    function ment(kulcs, ertek) {
        try {
            localStorage.setItem(kulcs, ertek);
        } catch (e) {
        }
    }

    function temaBeallitas(tema) {
        gyoker.setAttribute("data-bs-theme", tema === "vilagos" ? "light" : "dark");

        if (tema === "kontraszt") {
            gyoker.setAttribute("data-kontraszt", "igen");
        } else {
            gyoker.removeAttribute("data-kontraszt");
        }
    }

    function betuBeallitas(meret) {
        if (meret === "nagy") {
            gyoker.setAttribute("data-betu", "nagy");
        } else {
            gyoker.removeAttribute("data-betu");
        }
    }

    temaBeallitas(tarolt("tema", "vilagos"));
    betuBeallitas(tarolt("betu", "normal"));

    document.addEventListener("DOMContentLoaded", function () {
        document.querySelectorAll("[data-tema]").forEach(function (gomb) {
            gomb.addEventListener("click", function () {
                var tema = gomb.getAttribute("data-tema");
                temaBeallitas(tema);
                ment("tema", tema);
            });
        });

        var betuGomb = document.querySelector("[data-betu-valto]");

        if (betuGomb) {
            betuGomb.addEventListener("click", function () {
                var uj = tarolt("betu", "normal") === "nagy" ? "normal" : "nagy";
                betuBeallitas(uj);
                ment("betu", uj);
            });
        }
    });
})();
