using EmissionReportService;
using EmissionReportService.Service;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
// Configure Serilog for logging
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<IEmissionDataClient, EmissionDataClient>(client =>
{
    var baseUrl = builder.Configuration.GetValue<string>("EmissionDataRecordService:BaseUrl");
    client.BaseAddress = new Uri(baseUrl!);
});
builder.Services.AddScoped<IEmissionReportService, EmissionReportServices>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionMiddleware>();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
