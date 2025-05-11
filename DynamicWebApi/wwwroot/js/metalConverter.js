document.addEventListener("DOMContentLoaded", function () {
    $("#metal-convert-btn").click(function () {
        const amount = parseFloat($("#metal-amount").val());
        const from = $("#metal-from").val();
        const to = $("#metal-to").val();

        if (!amount || amount <= 0) {
            $("#metal-conversion-result").text("Lütfen geçerli bir miktar girin.");
            return;
        }

        if (from === to) {
            $("#metal-conversion-result").text("Aynı değerli madeni çeviremezsiniz.");
            return;
        }

        const fromData = window.metalRates[from];
        const toData = window.metalRates[to];

        if (!fromData || !toData || fromData.rate === 0 || toData.rate === 0) {
            $("#metal-conversion-result").text("Maden verisi eksik.");
            return;
        }

        const result = (amount * fromData.rate / fromData.unit) / (toData.rate / toData.unit);
        const formatted = new Intl.NumberFormat('tr-TR').format(result);

        $("#metal-conversion-result").text(`${amount} ${from} = ${formatted} ${to}`);
    });
});
