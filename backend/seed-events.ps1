# Script to seed Events table
# Usage: .\seed-events.ps1 -Token "your-jwt-token-here"

param(
    [Parameter(Mandatory=$true)]
    [string]$Token
)

$headers = @{
    "Authorization" = "Bearer $Token"
    "Content-Type" = "application/json"
}

$url = "http://localhost:5065/api/events/seed"

try {
    Write-Host "Seeding events..." -ForegroundColor Yellow
    $response = Invoke-RestMethod -Uri $url -Method Post -Headers $headers
    Write-Host "Success: $($response.message)" -ForegroundColor Green
} catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    
    if ($statusCode -eq 400) {
        try {
            $errorBody = $_.ErrorDetails.Message | ConvertFrom-Json
            Write-Host "Note: $($errorBody.message)" -ForegroundColor Yellow
        } catch {
            Write-Host "Error: Bad Request - $($_.Exception.Message)" -ForegroundColor Red
        }
    } elseif ($statusCode -eq 401) {
        Write-Host "Error: Unauthorized. Please provide a valid admin token." -ForegroundColor Red
    } else {
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.ErrorDetails.Message) {
            Write-Host "Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
        }
    }
}
