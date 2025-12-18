# Campaign Team Profile System Implementation

## Overview
The Campaign Team Profile system allows administrators to manage core campaign team member profiles and displays them publicly on the `/team` page. This system was implemented to replace the hardcoded team member data with a fully dynamic, database-driven solution.

**Implementation Date:** December 18, 2025

## Database Schema

### CampaignTeamMembers Table
```sql
CREATE TABLE "CampaignTeamMembers" (
    "Id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Role" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "Phone" TEXT NULL,
    "Bio" TEXT NOT NULL,
    "Responsibilities" TEXT NULL,
    "PhotoUrl" TEXT NULL,
    "TwitterHandle" TEXT NULL,
    "LinkedInUrl" TEXT NULL,
    "FacebookUrl" TEXT NULL,
    "DisplayOrder" INTEGER NOT NULL DEFAULT 0,
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "JoinedDate" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL
);
```

### Migration
- **Migration Name:** `20251218074907_AddCampaignTeamMembers`
- **Location:** `backend/NewKenyaAPI/Migrations/`

## Backend Implementation

### 1. Model (`Models/CampaignTeamMember.cs`)
```csharp
public class CampaignTeamMember
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Role { get; set; }
    
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; }
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    [Required]
    [MaxLength(1000)]
    public string Bio { get; set; }
    
    [MaxLength(500)]
    public string? Responsibilities { get; set; }
    
    [MaxLength(500)]
    public string? PhotoUrl { get; set; }
    
    // Social Media Links
    [MaxLength(200)]
    public string? TwitterHandle { get; set; }
    
    [MaxLength(200)]
    public string? LinkedInUrl { get; set; }
    
    [MaxLength(200)]
    public string? FacebookUrl { get; set; }
    
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime JoinedDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

### 2. Controller (`Controllers/CampaignTeamController.cs`)

**Base Route:** `/api/campaignteam`

#### Endpoints

##### Public Endpoints
- **GET `/api/campaignteam`**
  - Description: Get all active campaign team members
  - Authorization: None (public)
  - Response: Array of CampaignTeamMember objects
  - Ordering: By DisplayOrder (ascending), then by Name
  
- **GET `/api/campaignteam/{id}`**
  - Description: Get a single team member by ID
  - Authorization: None (public)
  - Response: CampaignTeamMember object or 404

##### Admin-Only Endpoints
- **POST `/api/campaignteam`**
  - Description: Create a new team member
  - Authorization: Admin role required
  - Request Body: CampaignTeamMember object (without Id)
  - Response: Created CampaignTeamMember with 201 status

- **PUT `/api/campaignteam/{id}`**
  - Description: Update an existing team member
  - Authorization: Admin role required
  - Request Body: Complete CampaignTeamMember object
  - Response: 204 No Content on success

- **DELETE `/api/campaignteam/{id}`**
  - Description: Delete a team member
  - Authorization: Admin role required
  - Response: 204 No Content on success

- **PUT `/api/campaignteam/{id}/reorder`**
  - Description: Update the display order of a member
  - Authorization: Admin role required
  - Request Body: Integer (new order value)
  - Response: 204 No Content on success

### 3. Database Context
Added to `ApplicationDbContext.cs`:
```csharp
public DbSet<CampaignTeamMember> CampaignTeamMembers { get; set; }
```

Configuration in `OnModelCreating`:
```csharp
modelBuilder.Entity<CampaignTeamMember>().ToTable("CampaignTeamMembers");
```

## Frontend Implementation

### 1. Service Layer (`src/services/campaignTeamService.js`)
```javascript
const campaignTeamService = {
  async getMembers() { ... },           // GET all members
  async getMember(id) { ... },          // GET single member
  async createMember(memberData) { ... }, // POST (Admin)
  async updateMember(id, memberData) { ... }, // PUT (Admin)
  async deleteMember(id) { ... },       // DELETE (Admin)
  async reorderMember(id, newOrder) { ... }  // PUT reorder (Admin)
};
```

### 2. Admin Component (`src/components/admin/AdminCampaignTeam.jsx`)

**Features:**
- Grid display of all team members with profile cards
- Statistics dashboard (Total Members, Leadership, Staff)
- Add new member modal with comprehensive form
- Edit member modal (pre-populated with existing data)
- Delete member with confirmation
- Real-time data updates after all operations
- Loading states and error handling
- Profile photos or initials fallback
- Social media link display

**Form Fields:**
- Name* (required)
- Role/Position* (required)
- Email* (required)
- Phone (optional)
- Bio* (required, textarea)
- Key Responsibilities (optional, textarea)
- Photo URL (optional)
- Twitter Handle (optional)
- LinkedIn URL (optional)
- Facebook URL (optional)
- Display Order (number input)

**Member Card Display:**
- Profile photo or initials avatar
- Name and role
- Bio snippet
- Contact info (email, phone)
- Social media icons
- Responsibilities section
- Join date
- Edit and Delete buttons

### 3. Public Team Page (`src/pages/Team.jsx`)

**Features:**
- Hero section with title and description
- Responsive grid layout (1 col mobile → 2 col tablet → 3 col desktop)
- Member cards with enhanced design
- Profile photos with fallback to gradient avatars
- Role badges with styling
- Bio display
- Contact information (email, phone)
- Social media links
- Key responsibilities section
- Join date display
- Loading states with spinner
- Error handling with retry message
- Empty state when no members exist

**Design Elements:**
- Gradient backgrounds for avatars (Kenyan flag colors)
- Hover effects with scale animation
- Card shadows on hover
- Responsive typography
- Accessible color contrast
- Clean, modern layout

## Seed Script

**Location:** `backend/seed-campaign-team.ps1`

**Usage:**
```powershell
.\backend\seed-campaign-team.ps1 -Token "your-admin-jwt-token"
```

**Sample Data Created:**
1. **Sarah Kimani** - Campaign Manager
2. **John Omondi** - Communications Director
3. **Mary Wanjiru** - Field Operations Manager
4. **David Mwangi** - Digital Strategy Lead
5. **Grace Achieng** - Policy Research Director

Each member includes:
- Complete profile information
- Professional bio
- Contact details
- Key responsibilities
- Social media links (Twitter, LinkedIn)
- Display order for proper positioning

## Routes & Navigation

### Public Routes
- **`/team`** - Public-facing team page
  - Accessible to all visitors
  - Displays active team members only
  - Ordered by DisplayOrder field

### Admin Routes
- **`/admin`** → Campaign Team Profile tab
  - Requires Admin role
  - Full CRUD operations
  - Real-time data management

### Navigation
- Navbar → "Team" link (public access)
- Admin Panel → "Campaign Team Profile" tab (admin only)

## Security Considerations

1. **Authorization**
   - All write operations (POST, PUT, DELETE) require Admin role
   - Public read access for displaying team members
   - JWT token validation on protected endpoints

2. **Data Validation**
   - Required fields enforced at model level
   - Email format validation
   - Maximum length constraints on all fields
   - Input sanitization on frontend

3. **Privacy**
   - Only active members (IsActive = true) displayed publicly
   - Phone numbers optional (can be hidden)
   - Email addresses displayed but not scraped (rendered in components)

## Testing Checklist

### Admin Functionality
- [ ] Create new team member with all fields
- [ ] Create member with minimum required fields only
- [ ] Edit existing member information
- [ ] Delete team member with confirmation
- [ ] Verify display order changes member position
- [ ] Test form validation (required fields, email format)
- [ ] Test social media links format correctly
- [ ] Verify photo URL displays or falls back to initials
- [ ] Test statistics update after adding/removing members

### Public Display
- [ ] Team page displays all active members
- [ ] Members appear in correct display order
- [ ] Profile photos load correctly
- [ ] Initials avatar generates for members without photos
- [ ] Contact links work (mailto:, tel:)
- [ ] Social media links open in new tab
- [ ] Responsive design works on mobile/tablet/desktop
- [ ] Loading state displays during API call
- [ ] Error state displays on API failure
- [ ] Empty state shows when no members exist

### API Endpoints
- [ ] GET /api/campaignteam returns all active members
- [ ] GET /api/campaignteam/{id} returns single member
- [ ] POST requires authentication and Admin role
- [ ] PUT requires authentication and Admin role
- [ ] DELETE requires authentication and Admin role
- [ ] Non-admin users get 403 Forbidden on protected endpoints

## Future Enhancements

### Potential Features
1. **Photo Upload**: Integrate file upload for profile photos instead of URLs
2. **Rich Text Bio**: Support markdown or rich text formatting in bio field
3. **Department Grouping**: Add department/team categories
4. **Featured Members**: Highlight specific members on homepage
5. **Member Testimonials**: Add quotes or testimonials section
6. **Email Integration**: Send automatic emails on member addition
7. **Activity Log**: Track who made changes and when
8. **Bulk Import**: CSV import for multiple team members
9. **Draft Mode**: Save member profiles as drafts before publishing
10. **SEO Optimization**: Individual member profile pages with meta tags

### Technical Improvements
1. Image optimization and CDN integration
2. Caching strategy for frequently accessed data
3. Rate limiting on public endpoints
4. Search and filter functionality in admin panel
5. Pagination for large team rosters
6. Export functionality (PDF, CSV)
7. Version history for member profiles
8. Automated testing suite

## Related Documentation
- [User Profile Implementation](./USER_PROFILE_IMPLEMENTATION.md)
- [Volunteer Organization System](./VOLUNTEER_ORGANIZATION_SYSTEM.md)
- [Production Checklist](./PRODUCTION_CHECKLIST.md)
- [API Testing Guide](./backend/API_TESTING.md)

## Troubleshooting

### Common Issues

**Issue:** Team members not displaying on public page
- Check: Are members marked as `IsActive = true`?
- Check: Is the API running and accessible?
- Check: Browser console for API errors

**Issue:** Admin can't create new members
- Check: Is user authenticated with Admin role?
- Check: JWT token hasn't expired
- Check: All required fields filled in form

**Issue:** Profile photos not loading
- Check: Photo URL is accessible and valid
- Check: URL starts with http:// or https://
- Check: CORS settings allow loading external images
- Fallback: Initials avatar should display automatically

**Issue:** Display order not working
- Check: DisplayOrder values are unique integers
- Check: Lower numbers appear first (0, 1, 2, etc.)
- Refresh page after updating order

## Support & Maintenance

For issues or questions:
1. Check this documentation first
2. Review console logs (browser and server)
3. Test API endpoints with tools like Postman
4. Verify database entries directly if needed
5. Check related documentation files

**Last Updated:** December 18, 2025
