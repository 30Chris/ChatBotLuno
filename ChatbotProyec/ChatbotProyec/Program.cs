using ChatbotProyec;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache(); // Cache en memoria
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Expira en 30 min
    options.Cookie.HttpOnly = true; // Seguridad
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor(); // Permite inyectar IHttpContextAccessor

builder.Services.AddSingleton<Chatbot>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseRouting();

app.UseSession(); // Habilitar el middleware de sesión

app.UseAuthorization();

app.MapControllers();

app.Run();
