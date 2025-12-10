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
   - [ ] Change JWT secret key to a strong random value
   - [ ] Store in environment variables or secrets manager
   - [ ] Configure appropriate token expiration times

4. **CORS Configuration**
   - [ ] Update allowed origins to production domains only
   - [ ] Remove localhost origins from production build

5. **HTTPS**
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
- [ ] Create runbook for common issues
- [ ] Document backup/restore procedures

## Current Status

**Last Updated:** December 10, 2025

**Implementation Status:** All core features implemented and tested in development

**Ready for Production:** Yes (after completing security checklist items above)

**Next Steps:**
1. Review and complete security checklist
2. Move secrets to secure storage
3. Deploy to staging environment for testing
4. Complete testing checklist
5. Deploy to production
