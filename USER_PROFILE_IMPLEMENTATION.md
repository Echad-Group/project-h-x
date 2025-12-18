# User Profile & Settings System

## Overview

Complete user profile and settings management system with a tabbed interface for profile information, security settings, and account management.

## Features

### Profile Management
- **Personal Information**: First name, last name, phone number
- **Bio**: Up to 200 character personal description
- **Location**: City, region, or address
- **Website**: Personal or professional URL
- **Social Media**: Twitter, Facebook, LinkedIn handles
- **Avatar**: Automatically generated initials-based avatar

### Security
- **Password Management**: Change password with current password verification
- **Account Info**: View member since date, last login, email verification status
- **Secure Updates**: All password-protected operations require confirmation

### Account Settings
- **Email Update**: Change email address with password confirmation
- **Account Deletion**: Permanently delete account with password verification
- **Role Display**: View assigned roles (Admin, Volunteer, etc.)

## Database Schema

### ApplicationUser Model (Extended)
```csharp
public class ApplicationUser : IdentityUser
{
    // Existing fields
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    
    // New profile fields
    public string? Bio { get; set; }  // Max 200 chars
    public string? Location { get; set; }  // Max 100 chars
    public string? Website { get; set; }  // Max 200 chars (URL)
    public string? Twitter { get; set; }  // Max 50 chars
    public string? Facebook { get; set; }  // Max 50 chars
    public string? LinkedIn { get; set; }  // Max 50 chars
}
```

## API Endpoints

### UserProfile Controller (`/api/userprofile`)

All endpoints require authentication (`[Authorize]` attribute).

#### GET /api/userprofile
Get current user's complete profile information.

**Response:**
```json
{
  "id": "user-guid",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+254712345678",
  "bio": "Passionate about community service",
  "location": "Nairobi, Kenya",
  "website": "https://johndoe.com",
  "twitter": "@johndoe",
  "facebook": "johndoe",
  "linkedIn": "johndoe",
  "createdAt": "2024-01-01T00:00:00Z",
  "lastLoginAt": "2024-12-18T04:52:00Z",
  "emailConfirmed": true,
  "phoneNumberConfirmed": false,
  "roles": ["User"]
}
```

#### PUT /api/userprofile
Update profile information.

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+254712345678",
  "bio": "Updated bio",
  "location": "Nairobi",
  "website": "https://example.com",
  "twitter": "@username",
  "facebook": "username",
  "linkedIn": "username"
}
```

**Validation:**
- FirstName: Required, max 50 chars
- LastName: Required, max 50 chars
- PhoneNumber: Optional, valid phone format, max 20 chars
- Bio: Optional, max 200 chars
- Location: Optional, max 100 chars
- Website: Optional, valid URL, max 200 chars
- Social media: Optional, max 50 chars each

#### PUT /api/userprofile/password
Change user password.

**Request Body:**
```json
{
  "currentPassword": "OldPassword123!",
  "newPassword": "NewPassword123!",
  "confirmPassword": "NewPassword123!"
}
```

**Validation:**
- CurrentPassword: Required
- NewPassword: Required, min 6 characters
- ConfirmPassword: Required, must match NewPassword

#### PUT /api/userprofile/email
Update email address.

**Request Body:**
```json
{
  "newEmail": "newemail@example.com",
  "password": "CurrentPassword123!"
}
```

**Security:**
- Requires current password verification
- Checks email availability
- Updates both email and username
- Requires re-login after change

#### DELETE /api/userprofile
Delete user account permanently.

**Request Body:** (JSON string)
```json
"password123"
```

**Security:**
- Requires password confirmation
- Permanent deletion (cannot be undone)
- All user data is removed

## Frontend Implementation

### Route
- `/profile` - Protected route (requires authentication)

### Components

#### Profile Page (`src/pages/Profile.jsx`)
Main profile management interface with three tabs:

**Tab 1: Profile Information**
- Personal details form
- Bio textarea with character counter
- Location and website inputs
- Social media handles (Twitter, Facebook, LinkedIn)
- Save changes button

**Tab 2: Security**
- Change password form with 3 fields
- Account information display (member since, last login, verification status)
- Password strength indicators (future enhancement)

**Tab 3: Account Settings**
- Update email form with password confirmation
- Danger zone with account deletion
- Confirmation modal for deletion

### UI Features
- **Avatar**: Color-coded circular avatar with user initials
- **Role Badges**: Green pills showing user roles
- **Tab Navigation**: Clean tab interface with icons
- **Form Validation**: Client-side validation before submission
- **Loading States**: Disabled buttons and spinners during operations
- **Error/Success Messages**: Clear feedback for all operations
- **Delete Modal**: Confirmation dialog to prevent accidental deletion
- **Character Counter**: Real-time bio character count

### Navbar Integration
Replaced plain text greeting with dropdown menu:
- User avatar with initials
- Name display
- Dropdown on hover with:
  - Profile & Settings link
  - Logout button with icon

## Service Layer

### userProfileService (`src/services/userProfileService.js`)
```javascript
{
  getProfile(),
  updateProfile(profileData),
  changePassword(passwordData),
  updateEmail(emailData),
  deleteAccount(password)
}
```

All methods use axios interceptor for automatic JWT token injection.

## Security Considerations

### Authentication
- All endpoints require valid JWT token
- Token automatically attached via axios interceptor
- 401 responses trigger automatic logout and redirect

### Authorization
- Users can only access their own profile
- User ID extracted from JWT claims (`ClaimTypes.NameIdentifier`)
- No cross-user profile access

### Password Protection
- Email changes require password verification
- Account deletion requires password verification
- Password changes validate current password first
- Minimum 6 characters for new passwords

### Data Validation
- Server-side validation using DataAnnotations
- Client-side validation in forms
- URL validation for website field
- Phone number format validation
- Email format validation

## Migration

### Created Migration
```bash
dotnet ef migrations add AddUserProfileFields
```

**Added Columns to AspNetUsers:**
- Bio (TEXT)
- Location (TEXT)
- Website (TEXT)
- Twitter (TEXT)
- Facebook (TEXT)
- LinkedIn (TEXT)

### Apply Migration
```bash
dotnet ef database update
```

## User Flows

### Update Profile Flow
1. User clicks avatar dropdown → Profile & Settings
2. System loads current profile data
3. User edits fields in Profile tab
4. User clicks "Save Changes"
5. System validates and updates
6. Success message shown
7. Profile refreshed with new data

### Change Password Flow
1. User navigates to Security tab
2. User enters current password
3. User enters new password twice
4. User clicks "Update Password"
5. System validates passwords match
6. System verifies current password
7. Password updated, success message shown

### Change Email Flow
1. User navigates to Account Settings tab
2. User enters new email
3. User enters current password for confirmation
4. User clicks "Update Email"
5. System validates email not in use
6. System verifies password
7. Email updated
8. User automatically logged out
9. User must log in with new email

### Delete Account Flow
1. User navigates to Account Settings tab
2. User clicks "Delete Account" button
3. Modal opens with warning
4. User enters password to confirm
5. User clicks "Delete Account" in modal
6. System verifies password
7. Account deleted permanently
8. User logged out
9. Redirect to home page

## Testing Checklist

- [ ] Load profile displays all fields correctly
- [ ] Update profile saves changes
- [ ] Profile validation works (required fields, character limits)
- [ ] Change password with correct current password works
- [ ] Change password fails with incorrect current password
- [ ] New passwords must match
- [ ] Email update with correct password works
- [ ] Email update fails if email already exists
- [ ] Email update requires re-login
- [ ] Account deletion requires password
- [ ] Account deletion is permanent
- [ ] User logged out after deletion
- [ ] Avatar displays correct initials
- [ ] Role badges display correctly
- [ ] Dropdown menu works on hover
- [ ] All tabs switch properly
- [ ] Form validation works client-side
- [ ] Error messages display correctly
- [ ] Success messages display correctly
- [ ] Character counter for bio works
- [ ] Social media fields save optional values
- [ ] Website URL validation works

## Future Enhancements

- [ ] Profile picture upload
- [ ] Email verification flow
- [ ] Phone number verification with SMS
- [ ] Two-factor authentication
- [ ] Activity log (login history)
- [ ] Connected devices management
- [ ] Privacy settings (profile visibility)
- [ ] Export user data (GDPR compliance)
- [ ] Password strength meter
- [ ] Social media OAuth integration
- [ ] Notification preferences integration
- [ ] Data export before account deletion
- [ ] Account recovery/reactivation period
- [ ] Profile completeness indicator
- [ ] Custom avatar colors

## Related Files

### Backend
- `Models/ApplicationUser.cs` - Extended user model
- `Models/DTOs/UserProfileDto.cs` - Profile DTOs
- `Controllers/UserProfileController.cs` - Profile API
- `Migrations/*_AddUserProfileFields.cs` - Database migration

### Frontend
- `pages/Profile.jsx` - Main profile page
- `services/userProfileService.js` - API service
- `components/Navbar.jsx` - Updated with profile dropdown
- `App.jsx` - Added profile route

## Notes

- All profile fields except FirstName and LastName are optional
- Bio limited to 200 characters for conciseness
- Social media fields store handles/usernames only, not full URLs
- Email changes trigger re-authentication for security
- Account deletion is immediate and cannot be undone
- Profile data excluded from public APIs for privacy
