using EnergyMix.Api.Clients;
using EnergyMix.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Konfiguracja CORS dla aplikacji React (Vite na porcie 5173)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IEnergyService, EnergyService>();

builder.Services.AddHttpClient<ICarbonIntensityClient, CarbonIntensityClient>(client =>
{
    client.BaseAddress = new Uri("https://api.carbonintensity.org.uk/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();