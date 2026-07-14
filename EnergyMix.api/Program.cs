using EnergyMix.Api.Clients;
using EnergyMix.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        // Allow requests from the React app running on localhost:5173 and the deployed frontend on Render
        policy.WithOrigins("http://localhost:5173", "https://energy-mix-frontend-5379.onrender.com")
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
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.MapControllers();

app.Run();
