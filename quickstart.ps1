#!/usr/bin/env pwsh
# Quick Start Script for Chat Application
# This script helps you quickly start the chat application

Write-Host "================================" -ForegroundColor Cyan
Write-Host "  Chat Application - Quick Start" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Check if .NET 10 is installed
Write-Host "Checking .NET 10 installation..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version
Write-Host "✓ .NET Version: $dotnetVersion" -ForegroundColor Green

Write-Host ""
Write-Host "Choose an option:" -ForegroundColor Cyan
Write-Host "  1. Build & Run Server" -ForegroundColor White
Write-Host "  2. Build & Run Client" -ForegroundColor White
Write-Host "  3. Build Both" -ForegroundColor White
Write-Host "  4. Clean Build All" -ForegroundColor White
Write-Host "  5. Exit" -ForegroundColor White
Write-Host ""

$choice = Read-Host "Enter your choice (1-5)"

function BuildServer {
    Write-Host ""
    Write-Host "Building ChatServer..." -ForegroundColor Yellow
    dotnet build ChatServer\ChatServer.csproj
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Build successful!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Starting ChatServer..." -ForegroundColor Yellow
        Write-Host "Server will be available at: 127.0.0.1:9999" -ForegroundColor Cyan
        Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Gray
        dotnet run --project ChatServer\ChatServer.csproj
    } else {
        Write-Host "✗ Build failed!" -ForegroundColor Red
    }
}

function BuildClient {
    Write-Host ""
    Write-Host "Building ChatClient..." -ForegroundColor Yellow
    dotnet build ChatClient\ChatClient.csproj
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Build successful!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Starting ChatClient..." -ForegroundColor Yellow
        Write-Host "Make sure ChatServer is running first!" -ForegroundColor Cyan
        dotnet run --project ChatClient\ChatClient.csproj
    } else {
        Write-Host "✗ Build failed!" -ForegroundColor Red
    }
}

switch ($choice) {
    "1" {
        BuildServer
    }
    "2" {
        BuildClient
    }
    "3" {
        Write-Host ""
        Write-Host "Building all projects..." -ForegroundColor Yellow
        dotnet build
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Build successful!" -ForegroundColor Green
            Write-Host ""
            Write-Host "Now you can run:" -ForegroundColor Cyan
            Write-Host "  - Server: dotnet run --project ChatServer/ChatServer.csproj" -ForegroundColor White
            Write-Host "  - Client: dotnet run --project ChatClient/ChatClient.csproj" -ForegroundColor White
        } else {
            Write-Host "✗ Build failed!" -ForegroundColor Red
        }
    }
    "4" {
        Write-Host ""
        Write-Host "Cleaning all projects..." -ForegroundColor Yellow
        dotnet clean
        Write-Host "✓ Clean complete!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Building all projects..." -ForegroundColor Yellow
        dotnet build
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Build successful!" -ForegroundColor Green
        } else {
            Write-Host "✗ Build failed!" -ForegroundColor Red
        }
    }
    "5" {
        Write-Host "Exiting..." -ForegroundColor Gray
        exit
    }
    default {
        Write-Host "Invalid choice. Exiting..." -ForegroundColor Red
        exit
    }
}
