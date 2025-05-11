document.addEventListener("DOMContentLoaded", function () {
    $("#crypto-convert-btn").click(function () {
        const amount = parseFloat($("#crypto-amount").val());
        const from = $("#crypto-from-currency").val();
        const to = $("#crypto-to-currency").val();

        if (!amount || amount <= 0) {
            $("#crypto-result").text("Lütfen geçerli bir miktar girin.");
            return;
        }

        if (from === to) {
            $("#crypto-result").text("Aynı para birimi seçildi.");
            return;
        }

        const fromData = window.cryptoRates[from];
        const toData = window.cryptoRates[to];


        if (!fromData || !toData || fromData.rate === 0 || toData.rate === 0) {
            $("#crypto-result").text("Kur bilgisi eksik.");
            return;
        }

        const result = (amount * fromData.rate / fromData.unit) / (toData.rate / toData.unit);

        const formatted = new Intl.NumberFormat('tr-TR', {
            style: 'currency',
            currency: to
        }).format(result);

        $("#crypto-result").text(`${amount} ${from} = ${formatted}`);
    });
});
