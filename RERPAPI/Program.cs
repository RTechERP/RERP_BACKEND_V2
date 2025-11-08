using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http.Features;
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
using static Microsoft.IO.RecyclableMemoryStreamManager;

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
Config.ConnectionString = builder.Configuration.GetValue<string>("ConnectionString") ?? "";
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



//Config FormOption

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
});

builder.Services.Configure<FormOptions>(opt =>
{
    // Kích thước tối đa mỗi phần form (field/file) 
    opt.MultipartBodyLengthLimit = Int32.MaxValue;

    // Nếu file < 1 MB thì vẫn buffer hết trong RAM trước khi viết ra
    opt.MemoryBufferThreshold = 1 * 1024 * 1024;
    // (Tuỳ chọn) nếu có rất nhiều fields, tăng số fields tối đa
    opt.ValueCountLimit = 1000;

    // (Tuỳ chọn) tăng độ dài tối đa tên key/value nếu cần
    opt.ValueLengthLimit = 64 * 1024;
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

app.UseStaticFiles();
app.UseRouting();
app.UseCors("MyCors");
app.UseAuthorization();
//app.UseAuthentication();
app.UseMiddleware<DynamicAuthorizationMiddleware>();

app.MapControllers();


//Config static file

app.Use(async (context, next) =>
{
    context.Request.Path = context.Request.Path.Value?.ToLower();
    await next();
});


app.UseStaticFiles();
List<PathStaticFile> staticFiles = builder.Configuration.GetSection("PathStaticFiles").Get<List<PathStaticFile>>() ?? new List<PathStaticFile>();
foreach (var item in staticFiles)
{
    app.UseStaticFiles(new StaticFileOptions()
    {
        FileProvider = new PhysicalFileProvider($@"\\192.168.1.190\Software"),
        RequestPath = new PathString($"/api/share/{item.PathName.Trim().ToLower()}")
    });


    //app.UseDirectoryBrowser(new DirectoryBrowserOptions
    //{
    //    FileProvider = new PhysicalFileProvider(item.PathFull),
    //    RequestPath = new PathString($"/api/share/{item.PathName.Trim().ToLower()}")
    //});
}

app.Run();