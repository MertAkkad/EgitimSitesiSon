# Deploying EgitimSitesi to Render

This guide provides instructions for deploying your EgitimSitesi application to Render using PostgreSQL.

## Prerequisites

1. A Render account: Sign up at [render.com](https://render.com) if you don't have an account
2. Cloudinary account: You should already have this set up

## Deployment Steps

### 1. Set Up Render Blueprint

The easiest way to deploy is using the `render.yaml` file included in this repository. This file defines both the web service and PostgreSQL database.

1. Log in to your Render dashboard
2. Click "New" and select "Blueprint"
3. Connect your GitHub repository
4. Render will automatically detect the `render.yaml` file
5. Review the resources that will be created and click "Apply"

### 2. Configure Environment Variables

After deployment is initiated, you'll need to set up Cloudinary environment variables:

1. In your Render dashboard, navigate to your newly created web service
2. Go to the "Environment" tab
3. Add the following environment variables with your Cloudinary credentials:
   - `CLOUDINARY_CLOUD_NAME`
   - `CLOUDINARY_API_KEY`
   - `CLOUDINARY_API_SECRET`
4. Click "Save Changes"

### 3. Trigger Initial Database Migration

When your app first starts, the database migration will run automatically, creating all necessary tables based on your models. No seed data will be added as requested.

### 4. Access Your Application

Once deployment is complete (it may take a few minutes), you can access your application at the URL provided by Render.

## Troubleshooting

- **Database Connection Issues**: Verify that the PostgreSQL connection string is correctly using the environment variables.
- **Migration Errors**: Check the logs in the Render dashboard for any database migration errors.
- **Cloudinary Issues**: Ensure your Cloudinary environment variables are correctly set.

## Important Notes

- This deployment uses an empty PostgreSQL database with no seed data, as requested.
- The database will be initialized with tables based on your model structure only.
- All Cloudinary functionality should continue to work as before, using the environment variables.
 