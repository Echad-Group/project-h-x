# User Profile & Settings System

## Overview

Complete user profile and settings management system with a tabbed interface for profile information, security settings, and account management.

## Features

### Profile Management
- **Personal Information**: First name, last name, phone number
- **Profile Photo**: Upload, preview, and delete profile pictures
- **Bio**: Up to 200 character personal description
- **Location**: City, region, or address
- **Website**: Personal or professional URL
- **Social Media**: Twitter, Facebook, LinkedIn handles
- **Avatar**: Profile photo or automatically generated initials-based avatar

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
    
    // Profile fields
    public string? Bio { get; set; }  // Max 200 chars
    public string? Location { get; set; }  // Max 100 chars
    public string? Website { get; set; }  // Max 200 chars (URL)
    public string? Twitter { get; set; }  // Max 50 chars
    public string? Facebook { get; set; }  // Max 50 chars
    public string? LinkedIn { get; set; }  // Max 50 chars
    public string? ProfilePhotoUrl { get; set; }  // Max 500 chars (file path)
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
  "profilePhotoUrl": "/uploads/profile-photos/user-guid_123.jpg",
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

#### POST /api/userprofile/upload-photo
Upload a profile photo.

**Content-Type:** `multipart/form-data`

**Request Body:**
```
FormData with 'file' field containing the image file
```

**Validation:**
- File must be present
- Maximum file size: 5 MB
- Allowed formats: `.jpg`, `.jpeg`, `.png`, `.gif`, `.webp`

**Behavior:**
- Deletes old profile photo from disk if exists
- Generates unique filename: `{userId}_{Guid}.{extension}`
- Saves to `wwwroot/uploads/profile-photos/`
- Updates user's `ProfilePhotoUrl` in database
- Returns relative URL path

**Response:**
```json
{
  "photoUrl": "/uploads/profile-photos/user-guid_abc123.jpg"
}
```

**Error Responses:**
- 400: "No file uploaded"
- 400: "File size exceeds 5 MB limit"
- 400: "Invalid file type. Only JPG, PNG, GIF, and WEBP are allowed"

#### DELETE /api/userprofile/photo
Delete the current user's profile photo.

**Behavior:**
- Deletes photo file from disk if exists
- Sets `ProfilePhotoUrl` to `null` in database
- Returns success message

**Response:**
```json
{
  "message": "Photo deleted successfully"
}
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

**Header Section:**
- Profile photo display (circular, 32x32)
  - Shows uploaded photo if available
  - Falls back to initials avatar with color coding
- Photo upload on hover:
  - Hover overlay with camera emoji (📷)
  - Hidden file input triggered by clicking overlay
  - Delete button (X) when photo exists
- Loading state during upload/delete
- File validation before upload (client-side)

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

**Photo Upload Implementation:**
```javascript
// State
const [photoPreview, setPhotoPreview] = useState(null);
const [uploading, setUploading] = useState(false);

// Upload handler with validation
const handlePhotoUpload = async (e) => {
  const file = e.target.files[0];
  if (!file) return;

  // Client-side validation
  if (!file.type.startsWith('image/')) {
    toast.error('Please select an image file');
    return;
  }

  if (file.size > 5 * 1024 * 1024) {
    toast.error('File size must be less than 5MB');
    return;
  }

  setUploading(true);
  try {
    const result = await userProfileService.uploadProfilePhoto(file);
    setPhotoPreview(result.photoUrl);
    updateUser({ profilePhotoUrl: result.photoUrl });
    toast.success('Profile photo updated successfully');
  } catch (error) {
    toast.error('Failed to upload photo');
  } finally {
    setUploading(false);
  }
};

// Delete handler with confirmation
const handlePhotoDelete = async () => {
  if (!window.confirm('Are you sure you want to delete your profile photo?')) {
    return;
  }

  setUploading(true);
  try {
    await userProfileService.deleteProfilePhoto();
    setPhotoPreview(null);
    updateUser({ profilePhotoUrl: null });
    toast.success('Profile photo deleted successfully');
  } catch (error) {
    toast.error('Failed to delete photo');
  } finally {
    setUploading(false);
  }
};
```

### UI Features
- **Profile Photo**: Circular photo with hover upload overlay and delete button
  - 32x32 size in header
  - Hover shows camera icon for upload
  - Delete button (X) in bottom-right corner
  - Real-time preview after upload
  - Client-side validation (type, size)
- **Avatar**: Color-coded circular avatar with user initials (fallback when no photo)
- **Role Badges**: Green pills showing user roles
- **Tab Navigation**: Clean tab interface with icons
- **Form Validation**: Client-side validation before submission
- **Loading States**: Disabled buttons and spinners during operations
- **Error/Success Messages**: Clear feedback for all operations
- **Delete Modal**: Confirmation dialog to prevent accidental deletion
- **Character Counter**: Real-time bio character count

### Navbar Integration
Replaced plain text greeting with dropdown menu:
- User avatar with photo or initials
  - Displays profile photo if available (8x8 circular)
  - Falls back to initials avatar with color coding
  - Updates in real-time after photo upload (via Context)
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
  deleteAccount(password),
  uploadProfilePhoto(file),      // New
  deleteProfilePhoto()            // New
}
```

**Photo Upload Method:**
```javascript
uploadProfilePhoto: async (file) => {
  const formData = new FormData();
  formData.append('file', file);
  return api.post('/userprofile/upload-photo', formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  });
}
```

**Photo Delete Method:**
```javascript
deleteProfilePhoto: async () => {
  return api.delete('/userprofile/photo');
}
```

All methods use axios interceptor for automatic JWT token injection.

### AuthContext (`src/contexts/AuthContext.jsx`)
Added `updateUser` method for real-time profile updates:
```javascript
const updateUser = (userData) => {
  setUser(prevUser => ({ ...prevUser, ...userData }));
};
```

**Usage:**
- Called after photo upload/delete to sync navbar
- Allows partial user object updates without refetching entire profile
- Preserves existing user data while updating specific fields

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

### File Upload Security
- File type validation (only images: JPG, PNG, GIF, WEBP)
- File size limit enforcement (5 MB maximum)
- Unique filename generation to prevent conflicts
- Old file deletion when uploading new photo
- Static files served from protected wwwroot directory
- User-uploaded files excluded from version control (.gitignore)

## File Storage

### Directory Structure
```
backend/NewKenyaAPI/wwwroot/
  uploads/
    profile-photos/
      user-guid-1_abc123.jpg
      user-guid-2_def456.png
      ...
```

### File Naming Convention
- Pattern: `{userId}_{Guid}.{extension}`
- Example: `a1b2c3d4-e5f6-7890-abcd-ef1234567890_9f8e7d6c-5b4a-3210-fedc-ba9876543210.jpg`
- Ensures uniqueness and prevents filename collisions
- Associates files with specific users

### Static File Serving
- Middleware: `app.UseStaticFiles()` in `Program.cs`
- Base URL: `http://localhost:5065`
- File access: `http://localhost:5065/uploads/profile-photos/filename.jpg`
- Database stores relative path: `/uploads/profile-photos/filename.jpg`

### File Cleanup
- Old photo deleted automatically when uploading new one
- Physical file removed from disk using `File.Delete()`
- Handles missing files gracefully (file already deleted manually)

## Migration

### Created Migrations
```bash
dotnet ef migrations add AddUserProfileFields
dotnet ef migrations add AddProfilePhotoUrl
```

**Added Columns to AspNetUsers:**
- Bio (TEXT)
- Location (TEXT)
- Website (TEXT)
- Twitter (TEXT)
- Facebook (TEXT)
- LinkedIn (TEXT)
- ProfilePhotoUrl (TEXT) - New

### Apply Migration
```bash
dotnet ef database update
```

## User Flows

### Update Profile Flow
1. User clicks avatar dropdown → Profile & Settings
2. System loads current profile data (including photo URL)
3. User edits fields in Profile tab
4. User clicks "Save Changes"
5. System validates and updates
6. Success message shown
7. Profile refreshed with new data

### Upload Profile Photo Flow
1. User hovers over profile photo area in header
2. Camera overlay appears
3. User clicks camera icon
4. File picker opens
5. User selects image file
6. Client validates file type and size
7. If valid:
   - File uploaded via FormData to `/api/userprofile/upload-photo`
   - Server validates file again
   - Server deletes old photo (if exists)
   - Server saves new photo with unique name
   - Server updates `ProfilePhotoUrl` in database
   - Client receives photo URL
   - Client updates preview and navbar via Context
   - Success message shown
8. If invalid:
   - Error message shown (type or size issue)
   - Upload cancelled

### Delete Profile Photo Flow
1. User clicks delete button (X) on photo
2. Confirmation dialog appears
3. If confirmed:
   - DELETE request sent to `/api/userprofile/photo`
   - Server deletes file from disk
   - Server sets `ProfilePhotoUrl` to null
   - Client clears preview
   - Client updates navbar to show initials avatar
   - Success message shown
4. If cancelled:
   - No action taken

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

### Profile Photo Tests
- [ ] Upload JPG image successfully
- [ ] Upload PNG image successfully
- [ ] Upload GIF image successfully
- [ ] Upload WEBP image successfully
- [ ] Reject non-image files (e.g., PDF, TXT)
- [ ] Reject files over 5 MB
- [ ] Photo appears in header after upload
- [ ] Photo appears in navbar after upload
- [ ] Old photo deleted when uploading new one
- [ ] Delete button appears when photo exists
- [ ] Delete confirmation dialog works
- [ ] Photo removed from UI after deletion
- [ ] Photo file removed from disk after deletion
- [ ] Navbar reverts to initials avatar after deletion
- [ ] Upload disabled during processing
- [ ] Delete disabled during processing
- [ ] Error handling for network failures
- [ ] Error handling for server errors
- [ ] Loading states display correctly

## Future Enhancements

- [x] Profile picture upload (Completed)
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
- [ ] Profile photo cropping/editing
- [ ] Multiple profile photo formats (thumbnail, full size)
- [ ] Profile photo CDN integration for production

## Related Files

### Backend
- `Models/ApplicationUser.cs` - Extended user model with ProfilePhotoUrl
- `Models/DTOs/UserProfileDto.cs` - Profile DTOs with photo URL
- `Controllers/UserProfileController.cs` - Profile API with upload/delete endpoints
- `Migrations/*_AddUserProfileFields.cs` - Initial profile fields migration
- `Migrations/*_AddProfilePhotoUrl.cs` - Photo URL field migration
- `Program.cs` - Static files middleware configuration
- `wwwroot/uploads/profile-photos/` - Photo storage directory

### Frontend
- `pages/Profile.jsx` - Main profile page with photo upload UI
- `services/userProfileService.js` - API service with upload/delete methods
- `contexts/AuthContext.jsx` - Added updateUser for real-time sync
- `components/Navbar.jsx` - Updated with profile photo display
- `App.jsx` - Added profile route

## Troubleshooting

### Photo Upload Issues

**Problem:** Upload fails with 400 error
- **Solution:** Check file size (<5MB) and type (must be image)
- **Check:** Browser console for client-side validation errors

**Problem:** Photo doesn't appear after upload
- **Solution:** Verify static files middleware is enabled in Program.cs
- **Check:** Network tab shows correct photo URL returned from API
- **Check:** Photo file exists in wwwroot/uploads/profile-photos/

**Problem:** Old photo not deleted when uploading new one
- **Solution:** Ensure file path construction is correct in controller
- **Check:** Server logs for file deletion errors

**Problem:** Navbar doesn't update after upload
- **Solution:** Verify updateUser is called in Profile.jsx
- **Check:** AuthContext exports updateUser method
- **Check:** User object updates in React DevTools

### Photo Delete Issues

**Problem:** Delete fails silently
- **Solution:** Check server logs for file deletion errors
- **Check:** Verify ProfilePhotoUrl is set to null in database

**Problem:** Photo still visible after deletion
- **Solution:** Ensure setPhotoPreview(null) is called
- **Check:** updateUser called with profilePhotoUrl: null

### File Access Issues

**Problem:** 404 when accessing photo URL
- **Solution:** Verify UseStaticFiles() middleware is configured
- **Check:** Photo file exists in wwwroot/uploads/profile-photos/
- **Check:** URL path matches ProfilePhotoUrl in database

**Problem:** Photos lost after redeployment
- **Solution:** Move uploads folder outside wwwroot for production
- **Alternative:** Use external storage (Azure Blob, AWS S3)

## Notes

- All profile fields except FirstName and LastName are optional
- Bio limited to 200 characters for conciseness
- Social media fields store handles/usernames only, not full URLs
- Email changes trigger re-authentication for security
- Account deletion is immediate and cannot be undone
- Profile data excluded from public APIs for privacy
- **Profile photos stored locally in wwwroot/uploads/** (development)
- **Production should use external storage or persistent volume**
- **Uploaded files excluded from git via .gitignore**
- **Photo filenames use GUID to prevent conflicts**
- **Old photos automatically deleted to save disk space**
