using URLShortener.Application.Interfaces;
using URLShortener.Infrastructure.Services;
using URLShortener.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

const string corsPolicyName = "custom";

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        // TODO: Test out in non-dev env - is this required?
        var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(';') ?? Array.Empty<string>();
        policy.WithOrigins(urls).WithMethods("GET").AllowAnyHeader();
    });
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IApplicationDatabaseContext, ApplicationDatabaseContext>();

builder.Services.AddTransient<IShortenedEntryRepository, ShortenedEntryRepository>();
builder.Services.AddTransient<IAliasService, AliasService>();
builder.Services.AddTransient<IShortenedEntryCreationService, ShortenedEntryCreationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();