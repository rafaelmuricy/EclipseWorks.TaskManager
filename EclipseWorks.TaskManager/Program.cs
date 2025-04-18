using EclipseWorks.TaskManager.Servico.Servico;

BaseDB.InicializarBD();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<BaseDB>();



var app = builder.Build();

//app.MapOpenApi("/swagger/{{documentName}}/swagger.json");
app.MapOpenApi();
app.UseSwaggerUI(option =>
{
    option.SwaggerEndpoint("/openapi/v1.json", "EclipseWorks.TaskManager V1");
    option.RoutePrefix = "";
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
