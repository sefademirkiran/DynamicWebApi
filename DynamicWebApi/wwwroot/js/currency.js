$(document).ready(function () {
    $("#convert-btn").click(function () {
        const amount = $("#amount").val();
        const from = $("#from-currency").val();
        const to = $("#to-currency").val();

        if (from === to) {
            $("#result-text").text("Aynı para birimini çeviremezsin.");
            return;
        }

        const url = `https://api.frankfurter.app/latest?amount=${amount}&from=${from}&to=${to}`;

        $.get(url, function (data) {
            const result = data.rates[to];
            $("#result-text").text(`${amount} ${from} = ${result.toFixed(2)} ${to}`);
        }).fail(function () {
            $("#result-text").text("Döviz bilgisi alınamadı.");
        });
    });
});
