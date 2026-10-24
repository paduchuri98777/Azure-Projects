using ITServiceDesk.API.Data;
var builder=WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddCors(o=>o.AddPolicy("Frontend",p=>p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddSingleton<SqlConnectionFactory>();
var app=builder.Build();
app.UseCors("Frontend");
app.MapControllers();
app.Run();