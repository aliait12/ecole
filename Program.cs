using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Data.Entities;
using SchoolManagementSystem.Helpers;
using SchoolManagementSystem.Repositories;
using System.Text;

namespace SchoolManagementSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddFile("Logs/schoolmanagementsystem-{Date}.txt");

            builder.Services.AddControllersWithViews();

            // ✅ DbContext
            var connString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new Exception("❌ ConnectionString 'DefaultConnection' manquante dans appsettings.json");

            builder.Services.AddDbContext<SchoolDbContext>(options =>
                options.UseSqlServer(connString));

            // ✅ Identity
            builder.Services.AddIdentity<User, IdentityRole>(cfg =>
            {
                cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                cfg.SignIn.RequireConfirmedEmail = true;
                cfg.User.RequireUniqueEmail = true;

                cfg.Password.RequireDigit = false;
                cfg.Password.RequiredUniqueChars = 0;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequiredLength = 6;
            })
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<SchoolDbContext>();

            // ✅ JWT config safe (no null)
            var jwtKey = builder.Configuration["Tokens:Key"]
                ?? throw new Exception("❌ Tokens:Key manquante dans appsettings.json");

            var jwtIssuer = builder.Configuration["Tokens:Issuer"] ?? "SchoolManagementSystem";
            var jwtAudience = builder.Configuration["Tokens:Audience"] ?? "SchoolManagementSystemUsers";

            builder.Services.AddAuthentication()
                .AddCookie()
                .AddJwtBearer(cfg =>
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            // ✅ Repositories
            builder.Services.AddScoped<IAlertRepository, AlertRepository>();
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IGradeRepository, GradeRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<ISchoolClassRepository, SchoolClassRepository>();
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
            builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
            builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // ✅ Helpers
            builder.Services.AddScoped<IUserHelper, UserHelper>();
            builder.Services.AddTransient<IMailHelper, MailHelper>();
            builder.Services.AddScoped<IBlobHelper, BlobHelper>();
            builder.Services.AddScoped<IConverterHelper, ConverterHelper>();

            // ✅ Seeder
            builder.Services.AddTransient<SeedDb>();

            var app = builder.Build();

            // ✅ Seed with scope
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var seedDb = services.GetRequiredService<SeedDb>();
                await seedDb.SeedAsync();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
