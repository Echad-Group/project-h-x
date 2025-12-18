# Seed Campaign Team Members
# This script adds sample campaign team members for testing
# Usage: .\backend\seed-campaign-team.ps1 -Token "your-jwt-token"

param(
    [Parameter(Mandatory=$true)]
    [string]$Token
)

$apiUrl = "http://localhost:5065/api"
$headers = @{
    "Authorization" = "Bearer $Token"
    "Content-Type" = "application/json"
}

$teamMembers = @(
    @{
        name = "Sarah Kimani"
        role = "Campaign Manager"
        email = "sarah.kimani@newkenya.org"
        phone = "+254 712 345 678"
        bio = "Experienced political strategist with 10+ years in grassroots organizing. Led successful campaigns across multiple counties in Kenya."
        responsibilities = "Overall campaign strategy, team coordination, stakeholder management"
        photoUrl = "https://randomuser.me/api/portraits/women/44.jpg"
        twitterHandle = "@SarahKimani"
        linkedInUrl = "https://linkedin.com/in/sarahkimani"
        displayOrder = 1
    },
    @{
        name = "John Omondi"
        role = "Communications Director"
        email = "john.omondi@newkenya.org"
        phone = "+254 723 456 789"
        bio = "Former journalist with expertise in political communications. Previously worked with major media houses covering national politics."
        responsibilities = "Media relations, press releases, social media strategy, spokesperson duties"
        photoUrl = "https://randomuser.me/api/portraits/men/32.jpg"
        twitterHandle = "@JohnOmondi"
        facebookUrl = "https://facebook.com/johnomondi"
        displayOrder = 2
    },
    @{
        name = "Mary Wanjiru"
        role = "Field Operations Manager"
        email = "mary.wanjiru@newkenya.org"
        phone = "+254 734 567 890"
        bio = "Community organizer specializing in rural mobilization. Track record of building strong volunteer networks across Kenya."
        responsibilities = "Ground operations, volunteer coordination, regional campaigns"
        photoUrl = "https://randomuser.me/api/portraits/women/68.jpg"
        linkedInUrl = "https://linkedin.com/in/marywanjiru"
        displayOrder = 3
    },
    @{
        name = "David Mutua"
        role = "Digital Strategy Lead"
        email = "david.mutua@newkenya.org"
        phone = "+254 745 678 901"
        bio = "Tech entrepreneur turned political campaigner. Expert in digital organizing and online engagement."
        responsibilities = "Digital campaigns, tech infrastructure, data analytics"
        photoUrl = "https://randomuser.me/api/portraits/men/75.jpg"
        twitterHandle = "@DavidMutua"
        linkedInUrl = "https://linkedin.com/in/davidmutua"
        displayOrder = 4
    },
    @{
        name = "Grace Akinyi"
        role = "Youth Engagement Coordinator"
        email = "grace.akinyi@newkenya.org"
        phone = "+254 756 789 012"
        bio = "Youth activist with deep connections to student movements and young professionals networks across Kenya."
        responsibilities = "Youth outreach, university campus organizing, youth policy development"
        photoUrl = "https://randomuser.me/api/portraits/women/90.jpg"
        twitterHandle = "@GraceAkinyi"
        facebookUrl = "https://facebook.com/graceakinyi"
        displayOrder = 5
    }
)

Write-Host "Seeding Campaign Team Members..." -ForegroundColor Green
Write-Host ""

$created = 0
$failed = 0

foreach ($member in $teamMembers) {
    try {
        $body = $member | ConvertTo-Json
        $response = Invoke-RestMethod -Uri "$apiUrl/campaignteam" -Method Post -Headers $headers -Body $body
        Write-Host "✓ Created: $($member.name) - $($member.role)" -ForegroundColor Green
        $created++
    }
    catch {
        Write-Host "✗ Failed to create: $($member.name)" -ForegroundColor Red
        Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
        $failed++
    }
}

Write-Host ""
Write-Host "Seeding complete!" -ForegroundColor Cyan
Write-Host "Created: $created" -ForegroundColor Green
Write-Host "Failed: $failed" -ForegroundColor Red
