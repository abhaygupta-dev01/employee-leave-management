# Free Deployment Guide for Leave Management System

This guide will help you deploy your ASP.NET Core 8.0 Leave Management System application for free using various hosting platforms.

## 🚀 Free Hosting Options

### 1. **Railway** (Recommended - Easiest)
**Free Tier:** $5 credit/month (usually enough for small apps)

**Steps:**
1. Go to [railway.app](https://railway.app) and sign up with GitHub
2. Click "New Project" → "Deploy from GitHub repo"
3. Select your repository
4. Railway will auto-detect the Dockerfile
5. Add environment variables:
   - `ConnectionStrings__DefaultConnection` - Your database connection string
   - `EmailSettings__SmtpServer` - smtp.gmail.com
   - `EmailSettings__Port` - 587
   - `EmailSettings__SenderEmail` - Your email
   - `EmailSettings__SenderPassword` - Your app password
   - `EmailSettings__SenderName` - Leave Management System
6. Add a PostgreSQL database (Railway provides free PostgreSQL)
7. Update connection string to use PostgreSQL instead of SQL Server

**Database Migration:**
- Railway provides PostgreSQL for free
- You'll need to update your connection string format:
  ```
  Host=your-host;Database=your-db;Username=your-user;Password=your-password
  ```

---

### 2. **Render**
**Free Tier:** 750 hours/month (enough for 24/7 operation)

**Steps:**
1. Go to [render.com](https://render.com) and sign up
2. Click "New +" → "Web Service"
3. Connect your GitHub repository
4. Configure:
   - **Name:** leave-management-system
   - **Environment:** Docker
   - **Region:** Choose closest to you
   - **Branch:** main/master
5. Add environment variables (same as Railway above)
6. Add PostgreSQL database:
   - Click "New +" → "PostgreSQL"
   - Copy the connection string and use it in your app

---

### 3. **Azure App Service** (Free Tier)
**Free Tier:** F1 tier (limited resources, but free)

**Steps:**
1. Go to [portal.azure.com](https://portal.azure.com)
2. Create a new "App Service" (Web App)
3. Select:
   - **Runtime stack:** .NET 8
   - **Operating System:** Linux
   - **Pricing tier:** Free (F1)
4. Deploy using:
   - **GitHub Actions** (recommended)
   - **VS Code Azure Extension**
   - **Azure CLI**
5. Add configuration settings (environment variables) in Azure Portal
6. Add Azure SQL Database (has free tier for 12 months)

---

### 4. **Fly.io**
**Free Tier:** 3 shared-cpu VMs, 3GB persistent storage

**Steps:**
1. Install Fly CLI: `iwr https://fly.io/install.ps1 -useb | iex` (PowerShell)
2. Sign up: `fly auth signup`
3. Login: `fly auth login`
4. Initialize: `fly launch` (in your project directory)
5. Deploy: `fly deploy`
6. Set secrets: `fly secrets set KEY=value`

---

## 📋 Pre-Deployment Checklist

### 1. **Update Database Connection**
Your app uses SQL Server. For free hosting, consider:
- **PostgreSQL** (free on Railway, Render)
- **SQLite** (for very small apps, no setup needed)
- **Azure SQL** (free tier for 12 months)

### 2. **Environment Variables Setup**
Create these in your hosting platform:

```
ConnectionStrings__DefaultConnection=<your-database-connection-string>
EmailSettings__SmtpServer=smtp.gmail.com
EmailSettings__Port=587
EmailSettings__SenderEmail=your-email@gmail.com
EmailSettings__SenderPassword=your-app-password
EmailSettings__SenderName=Leave Management System
ASPNETCORE_ENVIRONMENT=Production
```

### 3. **Gmail App Password**
If using Gmail SMTP:
1. Enable 2-Factor Authentication on your Google account
2. Go to Google Account → Security → App passwords
3. Generate an app password for "Mail"
4. Use this password (not your regular Gmail password)

### 4. **Update Connection String Format**
For PostgreSQL (Railway/Render):
```
Host=hostname;Port=5432;Database=dbname;Username=user;Password=pass
```

For SQL Server (Azure):
```
Server=tcp:server.database.windows.net,1433;Database=dbname;User ID=user;Password=pass;Encrypt=True;TrustServerCertificate=False
```

---

## 🐳 Docker Deployment

If your platform supports Docker (Railway, Render, Fly.io):

1. **Build locally (optional test):**
   ```powershell
   docker build -t leave-management .
   docker run -p 8080:8080 leave-management
   ```

2. **Deploy:**
   - Push your code to GitHub
   - Connect your repository to the hosting platform
   - Platform will automatically build and deploy using Dockerfile

---

## 🔧 Quick Start: Railway (Recommended)

1. **Prepare your repository:**
   ```powershell
   git add .
   git commit -m "Add deployment files"
   git push origin main
   ```

2. **Deploy on Railway:**
   - Visit [railway.app](https://railway.app)
   - Click "New Project" → "Deploy from GitHub repo"
   - Select your repo
   - Railway auto-detects Dockerfile

3. **Add PostgreSQL:**
   - Click "New" → "Database" → "Add PostgreSQL"
   - Copy the connection string

4. **Set Environment Variables:**
   - Go to your service → Variables
   - Add all required variables
   - Use PostgreSQL connection string format

5. **Deploy:**
   - Railway will automatically deploy
   - Get your app URL from the service dashboard

---

## 📝 Database Migration Notes

If switching from SQL Server to PostgreSQL:
- Update `Microsoft.Data.SqlClient` to `Npgsql.EntityFrameworkCore.PostgreSQL`
- Update connection string format
- Test migrations locally first

---

## 🆘 Troubleshooting

### Application won't start:
- Check logs in your hosting platform
- Verify all environment variables are set
- Ensure database is accessible

### Database connection errors:
- Verify connection string format
- Check if database is running
- Ensure firewall allows connections

### Email not working:
- Verify Gmail app password is correct
- Check SMTP settings
- Ensure "Less secure app access" is enabled (if needed)

---

## 🎯 Recommended: Railway + PostgreSQL

**Why Railway?**
- ✅ Easiest setup
- ✅ Free PostgreSQL included
- ✅ Auto-deploys on git push
- ✅ Great for .NET apps
- ✅ $5 free credit/month

**Quick Deploy:**
1. Push code to GitHub
2. Connect Railway to GitHub
3. Add PostgreSQL database
4. Set environment variables
5. Deploy! 🚀

---

## 📚 Additional Resources

- [Railway Documentation](https://docs.railway.app)
- [Render Documentation](https://render.com/docs)
- [Azure App Service Docs](https://docs.microsoft.com/azure/app-service)
- [Fly.io Documentation](https://fly.io/docs)

---

**Need help?** Check the logs in your hosting platform's dashboard for detailed error messages.

