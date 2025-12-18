# Issues System Implementation

## Overview

The Issues system provides a comprehensive policy platform where the campaign can present policy positions, initiatives, and answer common questions. It includes full CRUD operations via API and public-facing pages.

## Database Schema

### Issue Model
```csharp
public class Issue
{
    public int Id { get; set; }
    public string Slug { get; set; }  // SEO-friendly URL
    public string Title { get; set; }
    public string Summary { get; set; }
    public string? Icon { get; set; }  // Optional emoji or icon
    public string? Color { get; set; }  // Optional theme color
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<IssueInitiative> Initiatives { get; set; }
    public ICollection<IssueQuestion> Questions { get; set; }
}
```

### IssueInitiative Model
```csharp
public class IssueInitiative
{
    public int Id { get; set; }
    public int IssueId { get; set; }  // Foreign key
    public string Title { get; set; }
    public string Description { get; set; }
    public int DisplayOrder { get; set; }
    
    public Issue Issue { get; set; }
}
```

### IssueQuestion Model
```csharp
public class IssueQuestion
{
    public int Id { get; set; }
    public int IssueId { get; set; }  // Foreign key
    public string Question { get; set; }
    public string Answer { get; set; }
    public int DisplayOrder { get; set; }
    
    public Issue Issue { get; set; }
}
```

### Key Design Decisions

1. **Nested Resources**: Initiatives and Questions are child resources of Issues
2. **Display Order**: Each entity has DisplayOrder for flexible sorting
3. **Cascade Delete**: Deleting an Issue removes all its Initiatives and Questions
4. **Optional Fields**: Icon and Color are nullable for flexibility
5. **Slug-based Routing**: SEO-friendly URLs like `/issues/economic-growth`
6. **Published Flag**: Allows draft mode before publishing

## API Endpoints

### Issues Controller (`/api/issues`)

#### Public Endpoints
- `GET /api/issues` - Get all published issues (sorted by DisplayOrder)
- `GET /api/issues/{id}` - Get issue by ID with initiatives and questions
- `GET /api/issues/slug/{slug}` - Get issue by slug

#### Admin-Only Endpoints (Requires JWT with Admin role)
- `POST /api/issues` - Create new issue with nested initiatives and questions
- `PUT /api/issues/{id}` - Update issue
- `DELETE /api/issues/{id}` - Delete issue (cascade deletes children)
- `POST /api/issues/seed` - Seed 6 sample policy issues

## Seeded Sample Issues

The seed endpoint creates the following policy areas:

1. **Economic Growth & Job Creation**
   - 4 initiatives: Business support, job training, export promotion, digital economy
   - 2 questions: Small business support, youth unemployment

2. **Healthcare for All**
   - 4 initiatives: Universal healthcare, maternal health, mental health, medical infrastructure
   - 1 question: Healthcare access

3. **Education Reform**
   - 4 initiatives: Teacher training, school infrastructure, technical education, higher education
   - 2 questions: Quality improvement, higher education fees

4. **Security & Justice**
   - 4 initiatives: Police reform, community policing, judicial efficiency, rehabilitation
   - 1 question: Community safety

5. **Infrastructure Development**
   - 4 initiatives: Road expansion, public transport, rural electrification, water access
   - 2 questions: Rural infrastructure, public transport

6. **Agriculture & Food Security**
   - 4 initiatives: Farmer support, irrigation, market access, research & technology
   - 1 question: Small-scale farmer support

## Frontend Implementation

### Route Structure
```
/issues              → Issues.jsx (list all issues)
/issues/:slug        → (Future: individual issue detail page)
```

### Home Page Integration
**IssuesPreview Component** (`src/sections/IssuesPreview.jsx`):
- Displays first 3 issues on home page
- Cards are fully clickable (navigate to `/issues`)
- Green header color (`text-[var(--kenya-green)]`)
- Falls back to hardcoded data if API unavailable

### Issues Page (`src/pages/Issues.jsx`)

**Features:**
- Fetches from `/api/issues`
- Displays all published issues
- Shows initiatives as bullet points
- Shows questions in expandable sections
- Loading state with spinner
- Falls back to hardcoded data if backend unavailable

**Data Structure Handling:**
```javascript
// API format (from backend)
{
  title: "Economic Growth",
  summary: "...",
  initiatives: [{ title, description }],
  questions: [{ question, answer }]
}

// Fallback format (hardcoded)
{
  title: "Economic Growth",
  desc: "...",
  details: ["point 1", "point 2"],
  faqs: [{ q, a }]
}
```

**Hooks Fix:**
- All hooks (`useState`, `useEffect`, `useMeta`, `useTranslation`) must be called before any conditional returns
- `fallbackIssues` moved outside component to avoid recreation on each render
- Single `useEffect` for loading data and updating meta tags

## Issues Encountered & Solutions

### Issue 1: React Rules of Hooks Violation
**Problem:** `useEffect` called after conditional return statement  
**Error:** "React has detected a change in the order of Hooks"  
**Solution:** Moved all hooks to top of component before any returns  
**Files Modified:** Issues.jsx

### Issue 2: API Format vs Hardcoded Format Mismatch
**Problem:** Backend returns `summary`, `initiatives`, `questions` but hardcoded uses `desc`, `details`, `faqs`  
**Solution:** Component checks which format and adapts rendering accordingly  
**Files Modified:** Issues.jsx

### Issue 3: Circular References in JSON
**Problem:** Not encountered yet due to simple structure, but could occur with future relations  
**Prevention:** Return DTOs from controller if Issue model gets more navigation properties

## Seed Script Usage

```powershell
# Requires admin JWT token
.\backend\seed-issues.ps1 -Token "eyJhbGciOiJIUzI1NiIsInR5cC..."
```

**What it does:**
1. POSTs to `/api/issues/seed`
2. Creates 6 policy issues with:
   - Title, slug, summary, icon, color
   - 4 initiatives each
   - 1-2 questions each
3. Sets display order and published status

**Note:** Script checks for existing issues and returns a message if they already exist.

## Testing Checklist

- [ ] Issues page loads from API
- [ ] All issues displayed with proper formatting
- [ ] Initiatives rendered as bullet list
- [ ] Questions expandable/collapsible
- [ ] Home page shows first 3 issues
- [ ] Issue cards on home page clickable (navigate to `/issues`)
- [ ] Header color is green on home page cards
- [ ] Loading spinner shows while fetching
- [ ] Falls back to hardcoded data when API unavailable
- [ ] Unpublished issues hidden from public
- [ ] Admin can create/update/delete issues via API
- [ ] Nested initiatives and questions save correctly
- [ ] Display order affects rendering sequence

## Migration Commands

```bash
# Issues table was created in earlier migrations
# No additional migrations needed unless schema changes
cd backend/NewKenyaAPI
dotnet ef database update
```

## Admin UI (Future Enhancement)

Currently, Issues are managed via API only. Planned admin UI features:

- [ ] List all issues (published and draft)
- [ ] Create new issue form
- [ ] Edit issue with initiative/question management
- [ ] Drag-and-drop reordering (DisplayOrder)
- [ ] Rich text editor for descriptions
- [ ] Image upload for issue icons
- [ ] Preview before publishing
- [ ] Bulk operations (publish/unpublish)

## UX Improvements

### Home Page Changes
1. **Removed "Learn more →" link** - Entire card is now clickable
2. **Changed header color** - From red to green to match branding
3. **Click behavior** - Cards navigate to `/issues` page instead of individual issue

### Issues Page
- Clean, scannable layout
- Clear visual hierarchy
- Expandable FAQ sections
- Responsive design for mobile

## Future Enhancements

- [ ] Individual issue detail pages (`/issues/:slug`)
- [ ] Search and filtering on Issues page
- [ ] Related issues/cross-linking
- [ ] Progress tracking for initiatives
- [ ] User comments/feedback on issues
- [ ] Social sharing for individual issues
- [ ] Print-friendly view
- [ ] PDF export of policy positions
- [ ] Issue tags/categories
- [ ] Timeline view for implementation plans

## Code Quality Notes

### Hooks Pattern
```javascript
// ✅ CORRECT: All hooks at top
function Issues() {
  const [issues, setIssues] = useState([]);
  const [loading, setLoading] = useState(true);
  const { updateMeta } = useMeta();
  const { t } = useTranslation();
  
  useEffect(() => {
    // load data
  }, [issues, updateMeta]);
  
  if (loading) return <Spinner />;
  // ... rest of component
}

// ❌ INCORRECT: Hook after conditional return
function Issues() {
  const [issues, setIssues] = useState([]);
  
  if (!issues) return null;  // ❌ Returns before all hooks
  
  useEffect(() => { ... }, []);  // ❌ Hook after return
}
```

### Fallback Data Pattern
```javascript
// ✅ Move fallback data outside component
const fallbackIssues = [ /* static data */ ];

function Issues() {
  // Component uses fallbackIssues without recreating it
}
```

## References

- API Testing Documentation: `backend/API_TESTING.md`
- Database Models: `backend/NewKenyaAPI/Models/`
- Controller: `backend/NewKenyaAPI/Controllers/IssuesController.cs`
- Frontend Page: `src/pages/Issues.jsx`
- Home Section: `src/sections/IssuesPreview.jsx`
- Seed Script: `backend/seed-issues.ps1`
