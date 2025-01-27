using System.Data;
using Gacfox.Wealthome.Exceptions;
using Gacfox.Wealthome.Models;
using Gacfox.Wealthome.Models.Base;
using Gacfox.Wealthome.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EFCore DbContext服务
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// 注入DbConnection供Dapper使用
builder.Services.AddScoped<IDbConnection>(sp => sp.GetRequiredService<AppDbContext>().Database.GetDbConnection());

// 统一参数校验异常处理
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var firstError = context.ModelState
            .Where(kv => kv.Value?.Errors.Count > 0)
            .Select(kv => kv.Value?.Errors.FirstOrDefault()?.ErrorMessage)
            .FirstOrDefault();
        return new ObjectResult(ApiResult<object>.Failure("400", firstError ?? "请求参数异常"));
    };
});

// 数据保护服务
builder.Services.AddDataProtection();

// Cookie认证鉴权服务
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = ".AspNetCore.Auth";
    options.Cookie.HttpOnly = true;
    options.Events.OnRedirectToLogin = context =>
    {
        var apiResult = ApiResult<object>.Failure("401", "未登录");
        return context.Response.WriteAsJsonAsync(apiResult);
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        var apiResult = ApiResult<object>.Failure("403", "无权限");
        return context.Response.WriteAsJsonAsync(apiResult);
    };
});

// AutoMapper服务
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// 业务功能服务
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<TransferTypeService>();
builder.Services.AddScoped<TransferService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<StatService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // 开发环境启用Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 全局异常处理
app.UseExceptionHandler(applicationBuilder =>
{
    applicationBuilder.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionFeature != null)
        {
            var exception = exceptionFeature.Error;
            if (exception is BusinessException)
            {
                context.Response.StatusCode = 200;
                var apiResult = ApiResult<object>.Failure("401", exception.Message);
                await context.Response.WriteAsJsonAsync(apiResult);
            }
            else
            {
                context.Response.StatusCode = 200;
                var apiResult = ApiResult<object>.Failure("500", "服务端异常");
                await context.Response.WriteAsJsonAsync(apiResult);
            }
        }
    });
});

// 静态资源
app.UseStaticFiles();
app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api"), appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"));
    });
});

// 认证授权中间件
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();