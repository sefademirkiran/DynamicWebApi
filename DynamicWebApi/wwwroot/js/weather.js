
$(document).ready(function () {
    $.get("/Weather/GetWeather", function (data) {
        if (!data || !data.main || !data.weather || !data.weather[0]) {
            $("#weather-box").html("Veri eksik veya hatalı.");
            return;
        }

        const city = data.name;
        const temp = Math.round(data.main.temp);
        const desc = data.weather[0].description;
        const icon = data.weather[0].icon;
        const iconUrl = `https://openweathermap.org/img/wn/${icon}@2x.png`;

        $("#weather-box").html(`
    <img src="${iconUrl}" alt="${desc}" style="vertical-align:middle;" />
    <strong>${city}</strong>: ${desc}, ${temp}°C
`);

    }).fail(function () {
        $("#weather-box").html("Hava durumu yüklenemedi.");
    });
});

