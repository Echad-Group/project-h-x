# Production Deployment Checklist

## ✅ Completed Items

### Push Notifications
- [x] Generate production VAPID keys using `npx web-push generate-vapid-keys`
- [x] Update `src/services/pushNotification.js` with production public key
- [x] Configure `appsettings.json` with VAPID keys
- [x] Install WebPush NuGet package (v1.0.12)
- [x] Implement actual push sending in `PushController.cs`
- [x] Install AspNetCoreRateLimit package (v5.0.0)
- [x] Configure rate limiting in `appsettings.json`
- [x] Add rate limiting middleware to `Program.cs`
- [x] Test build and verify no compilation errors

### Application Features
- [x] Enhanced volunteer registration form with 7 new fields
- [x] Push notification settings page at `/notification-settings`
- [x] Notification button in navbar with settings link
- [x] Service worker enhanced for push notifications
- [x] Full internationalization (English & Swahili)
- [x] JWT authentication with role-based authorization
- [x] Issues (policy platform) system with seed data
- [x] Events system with slug-based routing
- [x] Event RSVP functionality with duplicate prevention
- [x] Admin panel at `/admin` route
- [x] Volunteer organization system (Units, Teams, Assignments)
- [x] PowerShell seed scripts for Issues and Events

## 🔒 Security Checklist

### Before Deploying to Production

1. **VAPID Private Key Security**
   - [ ] Move VAPID private key to secure environment variables
   - [ ] Remove from `appsettings.json` before committing to public repo
   - [ ] Use Azure Key Vault, AWS Secrets Manager, or similar for production

2. **Database Security**
   - [ ] Use production database (not SQLite)
   - [ ] Configure connection string in environment variables
   - [ ] Enable SSL/TLS for database connections

3. **JWT Configuration**
   - [ ] Change JWT secret key to a strong random value (min 256 bits)
   - [ ] Store in environment variables or secrets manager
   - [ ] Configure appropriate token expiration times
   - [ ] Review and restrict role assignments (Admin, TeamLead, Moderator)

4. **Admin Access**
   - [ ] Review all users with Admin role
   - [ ] Implement admin invitation system
   - [ ] Add audit logging for admin actions
   - [ ] Secure admin panel routes

5. **CORS Configuration**
   - [ ] Update allowed origins to production domains only
   - [ ] Remove localhost origins from production build

6. **HTTPS**
   - [ ] Configure SSL certificate
   - [ ] Force HTTPS redirection
   - [ ] Update service worker to work with HTTPS

## 📊 Testing Checklist

### Push Notifications
- [ ] Test subscription flow in Chrome
- [ ] Test subscription flow in Firefox
- [ ] Test subscription flow in Edge
- [ ] Test Safari (iOS/macOS) with PWA installed
- [ ] Verify notifications appear correctly
- [ ] Test notification click actions
- [ ] Test unsubscribe flow
- [ ] Verify rate limiting works (attempt multiple rapid subscriptions)
- [ ] Test sending notification to multiple users
- [ ] Verify invalid subscriptions are marked inactive

### Volunteer Registration
- [ ] Test all form fields save correctly
- [ ] Test validation on required fields
- [ ] Verify location dropdown works
- [ ] Test multi-select for availability zones and skills
- [ ] Test hours per week slider
- [ ] Verify data appears correctly in database

### Events & RSVPs
- [ ] Test event listing page loads from API
- [ ] Test slug-based event URLs (e.g., `/events/town-hall-nairobi`)
- [ ] Test event detail page with all information
- [ ] Test RSVP form submission
- [ ] Verify duplicate RSVP prevention (same email for same event)
- [ ] Test RSVP count updates after submission
- [ ] Test guest count calculation
- [ ] Verify admin can view all RSVPs for an event

### Issues (Policy Platform)
- [ ] Test issues page loads all published issues
- [ ] Test individual issue detail pages
- [ ] Verify initiatives and questions display correctly
- [ ] Test clickable issue cards on home page
- [ ] Verify slug-based URLs work

### Authentication & Authorization
- [ ] Test user registration flow
- [ ] Test login with valid credentials
- [ ] Test login with invalid credentials
- [ ] Verify JWT token contains correct role claims
- [ ] Test admin-only endpoints reject non-admin users
- [ ] Test protected routes redirect to login
- [ ] Verify token refresh works before expiration

## 🚀 Deployment Steps

1. **Backend Deployment**
   ```bash
   # Build the backend
   cd backend/NewKenyaAPI
   dotnet publish -c Release -o ./publish
   
   # Deploy to server (example for Azure)
   # az webapp deploy --name <app-name> --resource-group <group-name> --src-path ./publish
   ```

2. **Frontend Deployment**
   ```bash
   # Build the frontend
   npm run build
   
   # Deploy dist folder to static hosting
   # (Netlify, Vercel, Azure Static Web Apps, etc.)
   ```

3. **Environment Configuration**
   - Set environment variables on hosting platform
   - Configure custom domain
   - Set up SSL certificate
   - Update service worker scope if needed

4. **Database Migration**
   ```bash
   # On production server
   dotnet ef database update
   ```

5. **Seed Initial Data**
   ```bash
   # Get admin JWT token first (via login)
   # Then seed issues and events
   .\backend\seed-issues.ps1 -Token "your-admin-jwt"
   .\backend\seed-events.ps1 -Token "your-admin-jwt"
   ```

## 📈 Post-Deployment

- [ ] Monitor error logs for first 24 hours
- [ ] Check push notification success/failure rates
- [ ] Monitor rate limiting hits
- [ ] Verify analytics tracking works
- [ ] Test from different devices and browsers
- [ ] Monitor database performance
- [ ] Set up alerts for errors
- [ ] Document any issues found

## 🔧 Performance Optimization

- [ ] Enable response compression
- [ ] Configure CDN for static assets
- [ ] Optimize image sizes
- [ ] Enable browser caching
- [ ] Minify and bundle frontend assets (done by Vite)
- [ ] Consider implementing notification queuing for bulk sends
- [ ] Set up database indexes (already done)
- [ ] Configure connection pooling

## 📝 Documentation

- [ ] Update README.md with production URLs
- [ ] Document environment variables needed
- [ ] Create user guide for notification settings
- [ ] Document API endpoints for admin use
- [ ] Create admin user guide for Events/Issues management
- [ ] Document authentication flow and JWT token usage
- [ ] Create runbook for common issues
- [ ] Document backup/restore procedures
- [ ] Document seed script usage

## Current Status

**Last Updated:** December 18, 2025

**Implementation Status:** Core features including authentication, Issues, Events, and admin panel implemented and tested in development

**Ready for Production:** Pending (complete security checklist items above)

**Recent Additions:**
- JWT authentication with role-based authorization
- Issues (policy platform) system with full CRUD
- Events system with slug-based routing and RSVP functionality
- Admin panel for content management
- Seed scripts for Issues and Events
- DTO pattern for API responses to prevent circular references

**Next Steps:**
1. Review and complete security checklist
2. Move secrets to secure storage
3. Deploy to staging environment for testing
4. Complete testing checklist
5. Deploy to production
