# Script to clear Events and EventRSVPs tables
# Usage: .\clear-events.ps1 -Token "your-jwt-token-here"

param(
    [Parameter(Mandatory=$true)]
    [string]$Token
)

$headers = @{
    "Authorization" = "Bearer $Token"
    "Content-Type" = "application/json"
}

$baseUrl = "http://localhost:5065/api"

Write-Host "Clearing Events and RSVPs..." -ForegroundColor Yellow

try {
    # Get all events
    $eventsResponse = Invoke-RestMethod -Uri "$baseUrl/events?includeUnpublished=true" -Method Get -Headers $headers
    
    if ($eventsResponse -and $eventsResponse.Length -gt 0) {
        Write-Host "Found $($eventsResponse.Length) events. Deleting..." -ForegroundColor Cyan
        
        foreach ($event in $eventsResponse) {
            try {
                Invoke-RestMethod -Uri "$baseUrl/events/$($event.id)" -Method Delete -Headers $headers
                Write-Host "  Deleted event: $($event.title)" -ForegroundColor Gray
            } catch {
                Write-Host "  Failed to delete event $($event.id): $($_.Exception.Message)" -ForegroundColor Red
            }
        }
        
        Write-Host "All events cleared successfully!" -ForegroundColor Green
    } else {
        Write-Host "No events found to delete." -ForegroundColor Yellow
    }
    
} catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    
    if ($statusCode -eq 401) {
        Write-Host "Error: Unauthorized. Please provide a valid admin token." -ForegroundColor Red
    } else {
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.ErrorDetails.Message) {
            Write-Host "Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
        }
    }
}

Write-Host ""
Write-Host "Now you can run: .\backend\seed-events.ps1 -Token `"your-token`"" -ForegroundColor Cyan
