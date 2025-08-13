using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RERPAPI.IRepo;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Repo;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserPermissionService, UserPermissionService>();
builder.Services.AddScoped<RTCContext>();

//Config connect database
builder.Services.AddDbContext<RTCContext>(o => o.UseSqlServer(Config.ConnectionString));

builder.Services.AddMvc().AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null);


//Config CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCors", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});





// Load JWT settings
var jwtSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSection);
var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings();

builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                        NameClaimType = "sub" // Để Middleware lấy đúng UserID
                    };
                });
builder.Services.AddAuthentication();


builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true; // Chuyển tất cả URL thành chữ thường
});


builder.Services.Configure<ModulaConfig>(builder.Configuration.GetSection("ModulaConfig"));

// Nếu bạn muốn inject trực tiếp:
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<ModulaConfig>>().Value);

builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<ModulaConfig>();

    return new PersistentTcpClientService(
        config.IpAddress,
        config.Port,
        connectTimeoutMs: 3000,
        sendTimeoutMs: 3000,
        receiveTimeoutMs: 3000,
        maxReconnectAttempts: 3,
        reconnectDelayMs: 1000
    );
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseRouting();
app.UseAuthorization();
app.UseAuthorization();
app.UseMiddleware<DynamicAuthorizationMiddleware>();

app.MapControllers();

app.UseCors("MyCors");

//Config static file
app.UseStaticFiles();
List<PathStaticFile> staticFiles = builder.Configuration.GetSection("PathStaticFiles").Get<List<PathStaticFile>>() ?? new List<PathStaticFile>();
foreach (var item in staticFiles)
{
    app.UseStaticFiles(new StaticFileOptions()
    {
        FileProvider = new PhysicalFileProvider(item.PathFull),
        RequestPath = new PathString($"/api/{item.PathName.Trim().ToLower()}")
    });
}

app.Run();
