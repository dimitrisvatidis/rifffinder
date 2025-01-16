using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using rifffinder.Data;
using rifffinder.Models;
using rifffinder.Repositories;
using rifffinder.Services;
using System.Text;

namespace rifffinder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<BandRepository, BandRepository>();
            builder.Services.AddScoped<MusicianRepository, MusicianRepository>();
            builder.Services.AddScoped<PostingRepository, PostingRepository>();
            builder.Services.AddScoped<RequestRepository, RequestRepository>();

            builder.Services.AddScoped<BandService, BandService>();
            builder.Services.AddScoped<PostingService, PostingService>();
            builder.Services.AddScoped<RequestService, RequestService>();
            builder.Services.AddScoped<LoginService, LoginService>();
            builder.Services.AddScoped<MusicianService, MusicianService>();

            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSecret"])),
                        ValidIssuer = "rifffinder",
                        ValidAudience = "rifffinderAudience"
                    };
                });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (args.Contains("--seed"))
            {
                SeedData(app);
                return;
            }

            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapControllers();

            app.Run();
        }

        private static void SeedData(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Database.Migrate();

            if (!context.Musicians.Any())
            {
                context.Musicians.AddRange(
                    new Musician { Name = "John", Surname = "Doe", Email = "john.doe@example.com", Instrument = "Guitar", Password = BCrypt.Net.BCrypt.HashPassword("password") },
                    new Musician { Name = "Jane", Surname = "Smith", Email = "jane.smith@example.com", Instrument = "Drums", Password = BCrypt.Net.BCrypt.HashPassword("password") }
                );
            }

            if (!context.Bands.Any())
            {
                context.Bands.AddRange(
                    new Band { Name = "The Rockers", Genre = "Rock", Bio = "We rock the stage!" },
                    new Band { Name = "Jazz Masters", Genre = "Jazz", Bio = "Smooth and classy jazz tunes." }
                );
            }

            if (!context.Postings.Any())
            {
                context.Postings.AddRange(
                    new Posting { Title = "Looking for a Drummer", Text = "We need a skilled drummer for our upcoming gigs.", InstrumentWanted = "Drums", BandId = 1, Status = PostingStatus.Open },
                    new Posting { Title = "Guitarist Needed", Text = "Join our jazz band as a guitarist.", InstrumentWanted = "Guitar", BandId = 2, Status = PostingStatus.Open }
                );
            }

            context.SaveChanges();
            Console.WriteLine("Database has been seeded successfully!");
        }
    }
}
