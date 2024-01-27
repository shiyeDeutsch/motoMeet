using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using motoMeet;
using motoMeet.Manager;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MotoMeetDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConfigDB")));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMailingService, MailingService>();
builder.Services.AddScoped<IEmailService,EmailService>();
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<RouteManager>();
builder.Services.AddScoped<AuthManager>();

// string secretKey = builder.Configuration["JwtSettings:SecretKey"]; 
// Console.Write (secretKey);
// var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)); // Create SymmetricSecurityKey

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {

//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = securityKey,
//            ValidateIssuer = false,
//            ValidateAudience = false
//        };
//    });

   var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"] );
    var securityKey = new SymmetricSecurityKey(key);

   builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                   ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256  }
            };
        });

    // Add other services...












// builder.Services.AddAuthentication(x =>
//           {
//               x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//               x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//           })
//           .AddJwtBearer(x =>
//           {
//               x.RequireHttpsMetadata = false;
//               x.SaveToken = true;
//               x.TokenValidationParameters = new TokenValidationParameters
//               {
//                   ValidateIssuerSigningKey = true,
//                   // IssuerSigningKey = new SymmetricSecurityKey(),
//                   IssuerSigningKey = new SymmetricSecurityKey("88e513d1608fcf998c56fcdc2621d017f2c30182208980b86a72b7ad6752f592" as byte),

//                   ValidateIssuer = false,
//                   ValidateAudience = false
//               };
//           });




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //   app.UseSwagger();
    // app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();  // add this

app.UseAuthentication(); // this should be before UseAuthorization

app.UseAuthorization();

app.MapControllers();

app.Run();
