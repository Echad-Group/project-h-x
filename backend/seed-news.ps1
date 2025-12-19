# Script to seed News Articles
# Usage: .\seed-news.ps1 -Token "your-jwt-token-here"

param(
    [Parameter(Mandatory=$true)]
    [string]$Token
)

$headers = @{
    "Authorization" = "Bearer $Token"
    "Content-Type" = "application/json"
}

$baseUrl = "http://localhost:5065/api/news"

# Sample articles data
$articles = @(
    @{
        title = "Youth Empowerment Initiative Launches Across Kenya"
        excerpt = "New Kenya Campaign announces comprehensive youth agenda focusing on employment, education, and entrepreneurship opportunities for young Kenyans."
        content = @"
The New Kenya Campaign has unveiled an ambitious youth empowerment initiative aimed at addressing the challenges facing young Kenyans. This comprehensive program focuses on three key pillars: employment creation, educational reform, and entrepreneurship support.

Our youth agenda recognizes that Kenya's greatest asset is its young population. We are committed to creating 1 million new job opportunities over the next four years through strategic investments in technology, agriculture, and manufacturing sectors.

The educational reform component will introduce practical skills training into the curriculum, ensuring that graduates are job-ready and equipped with relevant 21st-century skills. We will also establish innovation hubs in every county to foster creativity and technological advancement.

For aspiring entrepreneurs, we are launching the Youth Enterprise Fund with an initial capital of 10 billion shillings, providing low-interest loans and business mentorship to young Kenyans with innovative business ideas.

Together, we can build a Kenya where every young person has the opportunity to thrive and contribute to our nation's prosperity.
"@
        category = "policy"
        tags = @("youth", "employment", "education", "entrepreneurship")
        author = "Campaign Team"
        publishedDate = "2025-10-15T10:00:00Z"
        featuredImageUrl = "https://images.unsplash.com/photo-1523240795612-9a054b0db644?w=800"
        imageUrls = @(
            "https://images.unsplash.com/photo-1523240795612-9a054b0db644?w=1200",
            "https://images.unsplash.com/photo-1522202176988-66273c2fd55f?w=1200"
        )
        status = "published"
        isFeatured = $true
        readTimeMinutes = 4
    },
    @{
        title = "Comprehensive Healthcare Reform Plan Unveiled"
        excerpt = "Ambitious plan to strengthen primary healthcare, expand insurance coverage, and improve medical infrastructure nationwide."
        content = @"
Access to quality healthcare is a fundamental right of every Kenyan citizen. Today, we present our comprehensive healthcare reform plan that will transform the health sector and ensure that no Kenyan is left behind.

Our plan focuses on three main areas: strengthening primary healthcare at the grassroots level, expanding the National Health Insurance coverage to achieve universal health coverage, and modernizing medical infrastructure across the country.

We will construct 500 new health centers in underserved areas and upgrade existing facilities with modern equipment and technology. Every Kenyan will have access to quality primary healthcare within 5 kilometers of their home.

The enhanced NHIF will cover all Kenyans, with government subsidies for the most vulnerable. We will also recruit 20,000 additional healthcare workers to address the chronic shortage of medical personnel.

Preventive care will be prioritized through community health programs, maternal and child health initiatives, and disease prevention campaigns. A healthy nation is a prosperous nation.
"@
        category = "policy"
        tags = @("healthcare", "NHIF", "hospitals", "reform")
        author = "Policy Team"
        publishedDate = "2025-10-10T09:00:00Z"
        featuredImageUrl = "https://images.unsplash.com/photo-1576091160399-112ba8d25d1d?w=800"
        imageUrls = @(
            "https://images.unsplash.com/photo-1576091160399-112ba8d25d1d?w=1200"
        )
        status = "published"
        isFeatured = $false
        readTimeMinutes = 5
    },
    @{
        title = "Successful Town Hall Meeting in Nakuru"
        excerpt = "Thousands gather in Nakuru as campaign team presents vision for economic transformation and engages with local communities."
        content = @"
The New Kenya Campaign held a highly successful town hall meeting in Nakuru yesterday, attracting over 5,000 enthusiastic attendees. The event provided a platform for direct engagement between our campaign team and the residents of the Rift Valley region.

Key topics discussed included agricultural modernization, infrastructure development, and tourism promotion. Farmers expressed their concerns about access to markets and inputs, while youth raised questions about employment opportunities.

Our team presented detailed plans for the agricultural sector, including the introduction of subsidized fertilizers, improved extension services, and the establishment of county-level aggregation centers to connect farmers directly with markets.

The meeting demonstrated the strong grassroots support for our campaign and the desire for transformative change. We remain committed to listening to the people and incorporating their feedback into our policy formulation process.

Similar town hall meetings will be held in all 47 counties to ensure every Kenyan has a voice in shaping our nation's future.
"@
        category = "events"
        tags = @("townhall", "Nakuru", "agriculture", "engagement")
        author = "Field Team"
        publishedDate = "2025-09-28T14:30:00Z"
        featuredImageUrl = "https://images.unsplash.com/photo-1540910419892-4a36d2c3266c?w=800"
        imageUrls = @(
            "https://images.unsplash.com/photo-1540910419892-4a36d2c3266c?w=1200",
            "https://images.unsplash.com/photo-1515187029135-18ee286d815b?w=1200",
            "https://images.unsplash.com/photo-1431540015161-0bf868a2d407?w=1200"
        )
        status = "published"
        isFeatured = $false
        readTimeMinutes = 3
    },
    @{
        title = "Massive Rally in Kisii Draws Record Crowds"
        excerpt = "Campaign momentum builds as thousands fill Kisii stadium to show support for progressive policies and inclusive governance."
        content = @"
The New Kenya Campaign momentum continues to build across the nation, with yesterday's rally in Kisii demonstrating unprecedented public support. The Gusii Stadium was filled to capacity with over 15,000 supporters, while thousands more gathered outside.

The rally showcased our commitment to regional development and inclusivity. We presented our plans for improving infrastructure in Nyanza region, including the construction of new roads, expansion of electricity access, and modernization of public services.

Special emphasis was placed on tea and coffee farming, which are the backbone of the local economy. Our administration will ensure fair prices for farmers, improved processing facilities, and direct access to international markets without exploitative middlemen.

The enthusiastic response from the people of Kisii sends a clear message: Kenyans are ready for change, ready for progress, and ready for a government that works for everyone.

We thank the people of Kisii for their overwhelming support and promise to continue working tirelessly to earn the trust of all Kenyans.
"@
        category = "events"
        tags = @("rally", "Kisii", "support", "campaign")
        author = "Field Team"
        publishedDate = "2025-09-20T16:00:00Z"
        featuredImageUrl = "https://images.unsplash.com/photo-1529107386315-e1a2ed48a620?w=800"
        imageUrls = @(
            "https://images.unsplash.com/photo-1529107386315-e1a2ed48a620?w=1200",
            "https://images.unsplash.com/photo-1557804506-669a67965ba0?w=1200"
        )
        status = "published"
        isFeatured = $false
        readTimeMinutes = 3
    },
    @{
        title = "Revolutionary Education Plan to Transform Learning"
        excerpt = "Detailed education reform strategy announced, featuring curriculum modernization, teacher training, and infrastructure development."
        content = @"
Education is the foundation of national development, and our comprehensive education reform plan will revolutionize learning in Kenya. We are committed to providing every Kenyan child with access to quality education that prepares them for the modern world.

Our plan includes a complete overhaul of the curriculum to emphasize critical thinking, creativity, and practical skills. We will integrate technology into every classroom, providing digital learning tools and internet connectivity to all schools.

Teacher welfare is paramount. We will increase teachers' salaries by 30%, provide continuous professional development opportunities, and improve working conditions. Happy and motivated teachers create successful students.

Infrastructure development is also critical. We will construct 1,000 new classrooms, renovate dilapidated structures, and ensure every school has adequate learning facilities including libraries, laboratories, and sports equipment.

Free primary and secondary education will be genuinely free, with the government providing textbooks, uniforms, and meals to all students. No Kenyan child will be denied education due to financial constraints.

Our goal is to make Kenya's education system a model for Africa, producing graduates who can compete globally and drive our nation's development.
"@
        category = "policy"
        tags = @("education", "schools", "teachers", "curriculum")
        author = "Policy Team"
        publishedDate = "2025-09-15T11:00:00Z"
        featuredImageUrl = "https://images.unsplash.com/photo-1503676260728-1c00da094a0b?w=800"
        imageUrls = @(
            "https://images.unsplash.com/photo-1503676260728-1c00da094a0b?w=1200"
        )
        status = "published"
        isFeatured = $true
        readTimeMinutes = 5
    },
    @{
        title = "Rural Connectivity Initiative to Bridge Digital Divide"
        excerpt = "Ambitious plan to connect every village to high-speed internet and expand mobile network coverage to underserved areas."
        content = @"
In the 21st century, access to the internet is no longer a luxury—it is a necessity. Our Rural Connectivity Initiative will ensure that every Kenyan, regardless of their location, has access to reliable high-speed internet.

We will partner with telecommunications companies and leverage emerging technologies like satellite internet to connect the last mile. Every sub-location will have at least one digital hub providing free internet access and computer training.

The benefits are immense: farmers will access market information in real-time, students can utilize online educational resources, businesses can reach global markets, and healthcare workers can use telemedicine to consult with specialists.

We will also establish e-government services centers in every ward, allowing citizens to access government services online without traveling to major towns. This will reduce costs, save time, and improve service delivery.

The digital divide between urban and rural Kenya will be eliminated, creating equal opportunities for all and accelerating our journey to becoming a digital economy.
"@
        category = "policy"
        tags = @("technology", "internet", "rural", "connectivity")
        author = "Policy Team"
        publishedDate = "2025-08-30T10:30:00Z"
        featuredImageUrl = "https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=800"
        imageUrls = @(
            "https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=1200",
            "https://images.unsplash.com/photo-1488590528505-98d2b5aba04b?w=1200"
        )
        status = "published"
        isFeatured = $false
        readTimeMinutes = 4
    },
    @{
        title = "Mombasa Port Expansion Project Announced"
        excerpt = "Major infrastructure project to double port capacity, create jobs, and position Kenya as East Africa's logistics hub."
        content = @"
The Mombasa Port is the gateway to East and Central Africa, handling cargo for six landlocked countries. Our administration will undertake a massive expansion project to double the port's capacity and modernize operations.

This KSh 500 billion project will include construction of new berths, acquisition of modern cargo handling equipment, and development of a state-of-the-art container terminal. We will also deepen the harbor to accommodate larger vessels.

The expansion will create over 50,000 direct jobs and thousands more in related industries. We will prioritize local employment and provide training programs to ensure Kenyans have the skills needed for these opportunities.

To complement the port expansion, we will upgrade the Standard Gauge Railway and expand the Northern Corridor road network, ensuring efficient cargo movement to the hinterland.

This project will solidify Kenya's position as East Africa's premier logistics and trade hub, attracting foreign investment and boosting economic growth. A world-class port for a world-class nation.
"@
        category = "press"
        tags = @("infrastructure", "Mombasa", "port", "economy")
        author = "Press Office"
        publishedDate = "2025-08-22T09:45:00Z"
        featuredImageUrl = "https://images.unsplash.com/photo-1578575437130-527eed3abbec?w=800"
        imageUrls = @(
            "https://images.unsplash.com/photo-1578575437130-527eed3abbec?w=1200"
        )
        status = "published"
        isFeatured = $false
        readTimeMinutes = 4
    },
    @{
        title = "Agricultural Subsidy Program to Support Farmers"
        excerpt = "Government announces comprehensive subsidy program covering fertilizers, seeds, and equipment to boost food security."
        content = @"
Agriculture is the backbone of Kenya's economy, employing over 70% of rural population. Our administration recognizes the critical role of farmers and is launching a comprehensive subsidy program to support agricultural production.

The program will provide subsidized fertilizers at 50% of market price, certified seeds for major crops, and access to modern farming equipment through county-based mechanization centers. Small-scale farmers will benefit most from these interventions.

We are allocating KSh 30 billion annually to this program, benefiting over 5 million farming households. Registration will be done through farmer cooperatives and agricultural extension officers to ensure transparency and efficiency.

Additionally, we will establish a Price Stabilization Fund to guarantee minimum prices for agricultural produce, protecting farmers from market exploitation. Cold storage facilities will be built in every county to reduce post-harvest losses.

The program includes livestock farmers, with subsidized animal feeds, veterinary services, and artificial insemination programs to improve breed quality and productivity.

Food security is national security. By supporting our farmers, we ensure that every Kenyan has access to affordable, nutritious food while creating prosperity in rural areas.
"@
        category = "policy"
        tags = @("agriculture", "subsidies", "farming", "food security")
        author = "Policy Team"
        publishedDate = "2025-08-10T08:00:00Z"
        featuredImageUrl = "https://images.unsplash.com/photo-1625246333195-78d9c38ad449?w=800"
        imageUrls = @(
            "https://images.unsplash.com/photo-1625246333195-78d9c38ad449?w=1200",
            "https://images.unsplash.com/photo-1464226184884-fa280b87c399?w=1200"
        )
        status = "published"
        isFeatured = $false
        readTimeMinutes = 4
    },
    @{
        title = "Women's Economic Empowerment Fund Launched"
        excerpt = "Special fund established to provide loans, training, and mentorship to women entrepreneurs across Kenya."
        content = @"
Gender equality and women's empowerment are not just moral imperatives—they are economic necessities. Today, we launch the Women's Economic Empowerment Fund with an initial capital of KSh 15 billion.

This fund will provide low-interest loans to women entrepreneurs, with no collateral required for loans up to KSh 500,000. We will also establish women's business incubation centers in every county, offering training, mentorship, and market linkages.

Special provisions have been made for women in marginalized communities, single mothers, and young women entrepreneurs. The fund will support businesses in all sectors including agriculture, manufacturing, services, and technology.

Beyond financing, we will ensure that at least 40% of all government procurement goes to women-owned businesses. This will guarantee market access and enable women entrepreneurs to grow and scale their enterprises.

We will also address social and cultural barriers that limit women's economic participation through awareness campaigns and legal reforms. Women's empowerment is Kenya's empowerment.

Together with our sisters, mothers, and daughters, we will build an inclusive economy where everyone has equal opportunities to succeed and prosper.
"@
        category = "community"
        tags = @("women", "empowerment", "entrepreneurship", "equality")
        author = "Community Team"
        publishedDate = "2025-07-28T12:00:00Z"
        featuredImageUrl = "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=800"
        imageUrls = @(
            "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=1200"
        )
        status = "published"
        isFeatured = $false
        readTimeMinutes = 4
    }
)

Write-Host "Starting to seed news articles..." -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Yellow

$successCount = 0
$failCount = 0

foreach ($article in $articles) {
    try {
        Write-Host "Creating article: $($article.title)" -ForegroundColor Cyan
        
        $body = $article | ConvertTo-Json -Depth 10
        $response = Invoke-RestMethod -Uri $baseUrl -Method Post -Headers $headers -Body $body
        
        Write-Host "  ✓ Successfully created article with slug: $($response.slug)" -ForegroundColor Green
        $successCount++
        
    } catch {
        Write-Host "  ✗ Failed to create article" -ForegroundColor Red
        
        if ($_.Exception.Response) {
            $statusCode = $_.Exception.Response.StatusCode.value__
            
            if ($statusCode -eq 401) {
                Write-Host "    Error: Unauthorized. Please provide a valid admin token." -ForegroundColor Red
                break
            } elseif ($statusCode -eq 400) {
                try {
                    $errorBody = $_.ErrorDetails.Message | ConvertFrom-Json
                    Write-Host "    Error: $($errorBody.message)" -ForegroundColor Red
                } catch {
                    Write-Host "    Error: Bad Request - $($_.Exception.Message)" -ForegroundColor Red
                }
            } else {
                Write-Host "    Error: $($_.Exception.Message)" -ForegroundColor Red
            }
        } else {
            Write-Host "    Error: $($_.Exception.Message)" -ForegroundColor Red
        }
        
        $failCount++
    }
    
    Start-Sleep -Milliseconds 500
}

Write-Host "----------------------------------------" -ForegroundColor Yellow
Write-Host "Seeding completed!" -ForegroundColor Yellow
Write-Host "Success: $successCount articles" -ForegroundColor Green
Write-Host "Failed: $failCount articles" -ForegroundColor $(if ($failCount -gt 0) { "Red" } else { "Yellow" })
