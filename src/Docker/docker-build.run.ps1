# ==========================================================
# Clean Architecture Backend
# Docker Compose Startup Script
# ==========================================================

param(
    [Parameter(Mandatory = $false)]
    [string]$Environment = "development"
)

Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host " Starting Clean Architecture Backend" -ForegroundColor Cyan
Write-Host " Environment: $Environment" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Move to the script folder
Set-Location $PSScriptRoot

$ProjectName = $Environment
$EnvFile = ".env.$Environment"

if (!(Test-Path $EnvFile)) {
    Write-Host "Environment file '$EnvFile' not found!" -ForegroundColor Red
    exit 1
}

$ComposeFiles = @(
    "-f"
    "docker-compose.yml"
)

$ComposeArgs = @(
    "--env-file"
    $EnvFile
    "-p"
    $ProjectName
) + $ComposeFiles


Write-Host "Project name:     $ProjectName" -ForegroundColor Yellow
Write-Host "Environment file: $EnvFile" -ForegroundColor Yellow
Write-Host ""


Write-Host "Stopping existing containers..." -ForegroundColor Yellow
docker compose @ComposeArgs down

Write-Host ""
Write-Host "Building images..." -ForegroundColor Yellow
docker compose @ComposeArgs build

Write-Host ""
Write-Host "Starting containers..." -ForegroundColor Yellow
docker compose @ComposeArgs up -d

Write-Host ""
Write-Host "Running containers:" -ForegroundColor Green
docker compose @ComposeArgs ps


Write-Host ""
Write-Host "==========================================" -ForegroundColor Green
Write-Host " Backend started successfully!" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host ""

Write-Host "Environment: $Environment" -ForegroundColor Cyan
Write-Host "Project:     $ProjectName" -ForegroundColor Cyan
Write-Host ""


# Load ports from .env file
$EnvVariables = @{}

Get-Content $EnvFile | ForEach-Object {
    if ($_ -match "^(.*?)=(.*)$") {
        $EnvVariables[$matches[1]] = $matches[2]
    }
}


$WebApiPort = $EnvVariables["WEB_API_PORT"]
$DataProcessorPort = $EnvVariables["DATAPROCESSOR_API_PORT"]
$PgAdminPort = $EnvVariables["PGADMIN_PORT"]
$KibanaPort = $EnvVariables["KIBANA_PORT"]
$ElasticPort = $EnvVariables["ELASTICSEARCH_PORT"]
$RabbitMqPort = $EnvVariables["RABBITMQ_MANAGEMENT_PORT"]
$SeqPort = $EnvVariables["SEQ_PORT"]


Write-Host "Web API:            http://localhost:$WebApiPort/swagger" -ForegroundColor Cyan
Write-Host "DataProcessor API:  http://localhost:$DataProcessorPort/swagger" -ForegroundColor Cyan
Write-Host "PgAdmin:            http://localhost:$PgAdminPort" -ForegroundColor Cyan
Write-Host "Kibana:             http://localhost:$KibanaPort" -ForegroundColor Cyan
Write-Host "Elasticsearch:      http://localhost:$ElasticPort" -ForegroundColor Cyan
Write-Host "RabbitMQ:           http://localhost:$RabbitMqPort" -ForegroundColor Cyan
Write-Host "Seq:                http://localhost:$SeqPort" -ForegroundColor Cyan
Write-Host ""


# Open useful pages
# Start-Process "http://localhost:$WebApiPort/swagger"
# Start-Process "http://localhost:$DataProcessorPort/swagger"
# Start-Process "http://localhost:$PgAdminPort"
# Start-Process "http://localhost:$RabbitMqPort"
# Start-Process "http://localhost:$KibanaPort"
# Start-Process "http://localhost:$SeqPort"


Write-Host ""
Write-Host "Done." -ForegroundColor Green