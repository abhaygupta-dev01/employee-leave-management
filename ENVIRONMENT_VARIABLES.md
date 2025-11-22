# Environment Variables Guide

For production deployment, use environment variables instead of hardcoding values in `appsettings.json`.

## Required Environment Variables

Set these in your hosting platform's dashboard:

### Database Connection
```
ConnectionStrings__DefaultConnection=<your-database-connection-string>
```

**Examples:**
- **PostgreSQL (Railway/Render):** `Host=hostname;Port=5432;Database=dbname;Username=user;Password=pass`
- **SQL Server (Azure):** `Server=tcp:server.database.windows.net,1433;Database=dbname;User ID=user;Password=pass;Encrypt=True`
- **SQLite:** `Data Source=leavemanagement.db`

### Email Settings
```
EmailSettings__SmtpServer=smtp.gmail.com
EmailSettings__Port=587
EmailSettings__SenderEmail=your-email@gmail.com
EmailSettings__SenderPassword=your-app-password
EmailSettings__SenderName=Leave Management System
```

### ASP.NET Core Settings
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:8080
```

## How to Set Environment Variables

### Railway
1. Go to your service → Variables tab
2. Click "New Variable"
3. Add each variable with the exact name and value

### Render
1. Go to your service → Environment
2. Add each variable in the "Environment Variables" section

### Azure App Service
1. Go to Configuration → Application settings
2. Add each variable as a new application setting

### Fly.io
```powershell
fly secrets set ConnectionStrings__DefaultConnection="your-connection-string"
fly secrets set EmailSettings__SmtpServer="smtp.gmail.com"
fly secrets set EmailSettings__Port="587"
fly secrets set EmailSettings__SenderEmail="your-email@gmail.com"
fly secrets set EmailSettings__SenderPassword="your-app-password"
fly secrets set EmailSettings__SenderName="Leave Management System"
```

## Gmail App Password Setup

1. Enable 2-Factor Authentication on your Google account
2. Go to [Google Account Security](https://myaccount.google.com/security)
3. Click "App passwords" (under "2-Step Verification")
4. Select "Mail" and "Other (Custom name)"
5. Enter "Leave Management System"
6. Copy the generated 16-character password
7. Use this password in `EmailSettings__SenderPassword`

## Security Notes

- ⚠️ Never commit sensitive data to Git
- ✅ Always use environment variables in production
- ✅ Keep `appsettings.Development.json` in `.gitignore`
- ✅ Use app passwords, not your regular Gmail password

