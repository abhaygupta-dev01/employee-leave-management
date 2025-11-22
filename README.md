# Leave Management System

An ASP.NET Core 8.0 MVC application for managing employee leave requests.

## Features

- Employee leave request management
- Admin dashboard for leave approval
- Email notifications
- Excel export functionality
- User authentication and authorization

## Tech Stack

- **Framework:** ASP.NET Core 8.0 MVC
- **Database:** SQL Server (can be migrated to PostgreSQL for free hosting)
- **Email:** MailKit (Gmail SMTP)
- **Export:** ClosedXML (Excel)

## Quick Start

### Prerequisites
- .NET 8.0 SDK
- SQL Server (or SQL Server Express)
- Visual Studio 2022 or VS Code

### Local Setup

1. Clone the repository
2. Update `appsettings.json` with your database connection string
3. Create the database and run migrations
4. Run the application:
   ```powershell
   dotnet run
   ```

## 🚀 Free Deployment

This project is ready for free deployment! See **[DEPLOYMENT.md](DEPLOYMENT.md)** for detailed instructions.

### Quick Deploy Options:

1. **Railway** (Recommended) - [railway.app](https://railway.app)
   - Free $5 credit/month
   - Auto-deploys from GitHub
   - Free PostgreSQL included

2. **Render** - [render.com](https://render.com)
   - 750 free hours/month
   - Free PostgreSQL available

3. **Azure App Service** - [portal.azure.com](https://portal.azure.com)
   - Free F1 tier available
   - Free Azure SQL for 12 months

4. **Fly.io** - [fly.io](https://fly.io)
   - 3 free VMs
   - Great for Docker deployments

### Deployment Files Included:
- ✅ `Dockerfile` - For containerized deployment
- ✅ `.dockerignore` - Optimized Docker builds
- ✅ `DEPLOYMENT.md` - Complete deployment guide

## Configuration

### Environment Variables (for production):

```
ConnectionStrings__DefaultConnection=<your-database-connection>
EmailSettings__SmtpServer=smtp.gmail.com
EmailSettings__Port=587
EmailSettings__SenderEmail=your-email@gmail.com
EmailSettings__SenderPassword=your-app-password
EmailSettings__SenderName=Leave Management System
```

## License

This project is open source and available for use.

## Support

For deployment help, see [DEPLOYMENT.md](DEPLOYMENT.md) or check the hosting platform's documentation.

