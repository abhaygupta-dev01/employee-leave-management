# Quick Start: Deploy Your App in 5 Minutes

## Step 1: Push to GitHub (if not done)

1. Open terminal in your project folder
2. Run these commands:

```powershell
git init
git add .
git commit -m "Add deployment files"
git branch -M main
git remote add origin https://github.com/YOUR_USERNAME/YOUR_REPO_NAME.git
git push -u origin main
```

**Don't have a GitHub repo yet?**
- Go to [github.com](https://github.com) and create a new repository
- Copy the repository URL
- Use it in the `git remote add origin` command above

---

## Step 2: Deploy on Railway (Easiest Option)

### Option A: Quick Deploy (Recommended)

1. **Go to [railway.app](https://railway.app)**
2. **Sign up** with your GitHub account
3. **Click "New Project"**
4. **Select "Deploy from GitHub repo"**
5. **Choose your repository** (LeaveManagementSystem)
6. **Railway will automatically:**
   - Detect your Dockerfile
   - Start building your app
   - Give you a URL

### Option B: Add Database

1. In Railway dashboard, click **"New"** → **"Database"** → **"Add PostgreSQL"**
2. Railway will create a free PostgreSQL database
3. Copy the connection string (it will look like: `postgresql://user:pass@host:port/dbname`)

### Option C: Set Environment Variables

1. Go to your service → **"Variables"** tab
2. Click **"New Variable"** and add these:

```
ConnectionStrings__DefaultConnection=Host=your-host;Port=5432;Database=your-db;Username=your-user;Password=your-password
EmailSettings__SmtpServer=smtp.gmail.com
EmailSettings__Port=587
EmailSettings__SenderEmail=your-email@gmail.com
EmailSettings__SenderPassword=your-gmail-app-password
EmailSettings__SenderName=Leave Management System
```

**For Gmail App Password:**
1. Go to [Google Account Security](https://myaccount.google.com/security)
2. Enable 2-Factor Authentication
3. Go to "App passwords"
4. Generate password for "Mail"
5. Use that 16-character password

---

## Step 3: Your App is Live! 🎉

Railway will give you a URL like: `https://your-app.railway.app`

Click on it to see your deployed app!

---

## Alternative: Deploy on Render

1. Go to [render.com](https://render.com)
2. Sign up with GitHub
3. Click **"New +"** → **"Web Service"**
4. Connect your GitHub repo
5. Select **"Docker"** as environment
6. Add environment variables (same as above)
7. Click **"Create Web Service"**

---

## Troubleshooting

**App won't start?**
- Check the logs in Railway/Render dashboard
- Make sure all environment variables are set
- Verify database connection string is correct

**Database errors?**
- Make sure you added a PostgreSQL database
- Check the connection string format
- Ensure database is running

**Need help?**
- Check `DEPLOYMENT.md` for detailed instructions
- Check `ENVIRONMENT_VARIABLES.md` for environment variable setup

