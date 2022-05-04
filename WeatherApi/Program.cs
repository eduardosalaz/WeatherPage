using System.Net.Http.Headers;
using System.Text.Json;
using WeatherAPI;

var builder = WebApplication.CreateBuilder(args);
var apiCorsPolicy = "ApiCorsPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: apiCorsPolicy,
        builder =>
        {
            builder.WithOrigins("http://localhost:4200", "http://localhost:64477")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            //.WithMethods("OPTIONS", "GET");
        });
});

var weatherApiKey = builder.Configuration["OpenWeather:ApiKey"];

var httpClient = new HttpClient();

async Task<WeatherData?> GetCurrentWeather(string lat, string lon, string key, bool metric){
    string? units;
    if(metric)
    {
        units = "metric";
    }
    else
    {
        units = "imperial";
    }
    httpClient.DefaultRequestHeaders.Accept.Clear();
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    httpClient.DefaultRequestHeaders.Add("User-Agent", "OpenWeather API caller");
    var streamTask = httpClient.GetStreamAsync(
        $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={key}&units={units}");
    var weatherData = await JsonSerializer.DeserializeAsync<WeatherData>(await streamTask);
    return weatherData;
}
/*
async Task GetCoordsFromCity(string cityName, string key, string countryCode = ""){
    httpClient.DefaultRequestHeaders.Accept.Clear();
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json;charset=utf-8"));
    httpClient.DefaultRequestHeaders.Add("User-Agent", "OpenWeather API caller");
    var streamTask = httpClient.GetStringAsync(
        $"https://api.openweathermap.org/geo/1.0/direct?q={cityName},{countryCode}&limit=1&appid={key}");
    //var coordinates = await JsonSerializer.DeserializeAsync<Coordinates>(await streamTask);
    var stringTsk = await streamTask;
    Console.WriteLine(stringTsk);
}
*/

/*
async Task<WeatherData?> GetWeatherForecast(String lat, String lon, String key, bool metric){
    string? units;
    if(metric)
    {
        units = "metric";
    }
    else
    {
        units = "imperial";
    }
    httpClient.DefaultRequestHeaders.Accept.Clear();
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    httpClient.DefaultRequestHeaders.Add("User-Agent", "OpenWeather API caller");
    var streamTask = httpClient.GetStreamAsync(
        $"https://api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&appid={key}&units={units}");
    var weatherData = await JsonSerializer.DeserializeAsync<WeatherData>(await streamTask);
    return weatherData;
}
*/

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/weather_data/{lat}/{lon}/{metric}", async (string lat, string lon, bool metric) => 
    await GetCurrentWeather(lat, lon, weatherApiKey, metric));

//app.MapGet("/geolocation/{cityName}/{countryCode?}", async (string cityName, string countryCode) => 
    // await GetCoordsFromCity(cityName, weatherApiKey, countryCode));

void ConfigureServices(IServiceCollection services)
{
    services.AddCors(); // Make sure you call this previous to AddMvc
}

app.UseCors(apiCorsPolicy);
app.Run();

