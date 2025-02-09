using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using registerAPI.Services;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); 

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<DatabaseService>();
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<TokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => 
{
    return Results.Content(
        content: """
        <!DOCTYPE html>
        <html>
        <head>
            <title>Madagascar Airlines API</title>
            <style>
                body { 
                    font-family: Arial, sans-serif; 
                    text-align: center;
                    padding: 2rem;
                    background-color: #f0f8ff;
                }
                .logo {
                    font-size: 2.5em;
                    color: #2c3e50;
                    margin-bottom: 1rem;
                }
                .endpoints {
                    text-align: left;
                    display: inline-block;
                    padding: 1rem;
                    background: white;
                    border-radius: 8px;
                    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                }
            </style>
        </head>
        <body>
            <div class="logo">✈️ Madagascar Airlines API</div>
            <div class="endpoints">
                <h3>Endpoints disponibles :</h3>
                <ul>
                    <li><strong>POST</strong> /api/auth/register</li>
                    <li><strong>GET</strong>  /api/auth/verify?token={token}</li>
                    <li><strong>POST</strong> /api/auth/login</li>
                </ul>
                <p>📧 Contact : <a href="mailto:contact@madagascar-airlines.mg">contact@madagascar-airlines.mg</a></p>
            </div>
        </body>
        </html>
        """,
        contentType: "text/html"
    );
});

app.MapGet("/db-test", (DatabaseService dbService) =>
{
    try
    {
        using (var connection = dbService.GetConnection())
        {
            connection.Open();
            return Results.Ok("Connexion à PostgreSQL réussie !");
        }
    }
    catch (PostgresException ex)
    {
        return Results.Problem($"Erreur PostgreSQL: {ex.Message}");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Erreur inattendue: {ex.Message}");
    }
});

app.MapControllers();

app.Run();