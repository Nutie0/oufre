using Microsoft.AspNetCore.Mvc;
using Npgsql;
using registerAPI.Models;
using registerAPI.Services;

namespace registerAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly DatabaseService _db;
    private readonly TokenService _tokenService;
    private readonly EmailService _emailService;

    public AuthController(DatabaseService db, TokenService tokenService, EmailService emailService)
    {
        _db = db;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthRequest request)
    {
        using var connection = _db.GetConnection();
        await connection.OpenAsync();

        var checkCmd = new NpgsqlCommand(
            "SELECT COUNT(*) FROM users WHERE email = @email",
            connection);
        checkCmd.Parameters.AddWithValue("@email", request.Email);

        if ((long)await checkCmd.ExecuteScalarAsync() > 0)
            return Conflict("Email already registered");

        var (verificationToken, pin) = _tokenService.GenerateTokenWithPin();

        var insertCmd = new NpgsqlCommand(
            @"INSERT INTO users (email, username, password_hash, verification_token) 
        VALUES (@email, @username, @password, @token)",
            connection);

        insertCmd.Parameters.AddWithValue("@email", request.Email);
        insertCmd.Parameters.AddWithValue("@username", request.Email.Split('@')[0]);
        insertCmd.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.HashPassword(request.Password));
        insertCmd.Parameters.AddWithValue("@token", verificationToken);

        await insertCmd.ExecuteNonQueryAsync();

        await _emailService.SendVerificationEmail(request.Email, verificationToken, pin);

        return Ok(new
        {
            Message = "Registration successful. Please check your email.",
            // Pin = pin 
        });
    }
    [HttpGet("verify")]
    public async Task<IActionResult> VerifyAccount(
       [FromQuery] string token,
       [FromQuery] string pin = null)
    {
        using var connection = _db.GetConnection();
        await connection.OpenAsync();

        if (!string.IsNullOrEmpty(pin) && !_tokenService.ValidatePin(token, pin))
            return BadRequest("Invalid PIN");

        var updateCmd = new NpgsqlCommand(
            @"UPDATE users 
        SET is_verified = TRUE,
            verification_token = NULL
        WHERE verification_token = @token",
            connection);

        updateCmd.Parameters.AddWithValue("@token", token);

        var affectedRows = await updateCmd.ExecuteNonQueryAsync();

        return affectedRows > 0
            ? Ok("Account verified successfully!")
            : BadRequest("Invalid token or PIN");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        using var connection = _db.GetConnection();
        await connection.OpenAsync();

        var cmd = new NpgsqlCommand(
            "SELECT password_hash, is_verified FROM users WHERE email = @email",
            connection);
        cmd.Parameters.AddWithValue("@email", request.Email);

        using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return Unauthorized("Invalid credentials");

        var passwordHash = reader.GetString(0);
        var isVerified = reader.GetBoolean(1);

        if (!BCrypt.Net.BCrypt.Verify(request.Password, passwordHash))
            return Unauthorized("Invalid credentials");

        return isVerified
            ? Ok("Login successful")
            : BadRequest("Email not verified");
    }
}