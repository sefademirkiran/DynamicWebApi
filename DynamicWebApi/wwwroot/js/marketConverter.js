document.addEventListener("DOMContentLoaded", function () {
    $("#currency-convert-btn").click(function () {
        const amount = parseFloat($("#currency-amount").val());
        const from = $("#currency-from").val();
        const to = $("#currency-to").val();

        if (!amount || amount <= 0) {
            $("#currency-conversion-result").text("Lütfen geçerli bir miktar girin.");
            return;
        }

        if (from === to) {
            $("#currency-conversion-result").text("Aynı para birimi seçildi.");
            return;
        }

        const fromData = window.currencyRates[from];
        const toData = window.currencyRates[to];

        if (!fromData || !toData || fromData.rate === 0 || toData.rate === 0) {
            $("#currency-conversion-result").text("Kur bilgisi eksik.");
            return;
        }

        const result = (amount * fromData.rate / fromData.unit) / (toData.rate / toData.unit);
        const formatted = new Intl.NumberFormat('tr-TR', { style: 'currency', currency: to }).format(result);

        $("#currency-conversion-result").text(`${amount} ${from} = ${formatted}`);
    });
});
