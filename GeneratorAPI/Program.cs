var builder = WebApplication.CreateBuilder(args);

// 加入 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// 啟用 CORS
app.UseCors("AllowAll");

app.UseRouting();
app.MapControllers();
app.Run();
