# Migration script for Events schema changes
# This adds Slug to Events and changes EventRSVP.EventId from string to int

Write-Host "Creating migration for Events schema changes..." -ForegroundColor Yellow

cd backend\NewKenyaAPI

# Create migration
dotnet ef migrations add AddEventSlugAndFixRSVP

if ($LASTEXITCODE -eq 0) {
    Write-Host "Migration created successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "To apply the migration, run:" -ForegroundColor Cyan
    Write-Host "  dotnet ef database update" -ForegroundColor White
    Write-Host ""
    Write-Host "Note: You may need to clear existing Events and EventRSVPs data first." -ForegroundColor Yellow
} else {
    Write-Host "Migration failed!" -ForegroundColor Red
}

cd ..\..
