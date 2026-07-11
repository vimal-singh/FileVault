using FileVault.Application.Features.Files.Commands.UploadFile;
using FileVault.Application.Features.Files.Queries.GetFileById;
using FileVault.Domain.Repositories;
using FileVault.Infrastructure.Persistence;
using FileVault.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FileVaultDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<UploadFileCommandHandler>();
builder.Services.AddScoped<GetFileByIdQueryHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
