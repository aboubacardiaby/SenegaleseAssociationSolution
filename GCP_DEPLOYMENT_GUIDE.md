# GCP Deployment Guide

This guide explains how to set up and deploy the Senegalese Association of Minnesota application to Google Cloud Platform using GitHub Actions.

## Prerequisites

- Google Cloud Platform account with billing enabled
- GitHub repository with the code
- GCP Project: `nimble-valve-479420-p2`
- Service Account: `svc-deploy-acct@nimble-valve-479420-p2.iam.gserviceaccount.com`
- Artifact Registry: `senregistry`

## GCP Setup

### 1. Enable Required APIs

Run these commands in Google Cloud Shell or locally with gcloud CLI:

```bash
gcloud config set project nimble-valve-479420-p2

# Enable required APIs
gcloud services enable \
  cloudbuild.googleapis.com \
  run.googleapis.com \
  artifactregistry.googleapis.com \
  containerregistry.googleapis.com \
  iam.googleapis.com
```

### 2. Create/Verify Artifact Registry

```bash
# Create the artifact registry if it doesn't exist
gcloud artifacts repositories create senregistry \
  --repository-format=docker \
  --location=us-central1 \
  --description="Docker repository for Senegalese Association app"

# Verify the registry exists
gcloud artifacts repositories list --location=us-central1
```

### 3. Configure Service Account Permissions

Grant necessary permissions to the service account:

```bash
# Service account email
SERVICE_ACCOUNT="svc-deploy-acct@nimble-valve-479420-p2.iam.gserviceaccount.com"

# Grant Cloud Run Admin role
gcloud projects add-iam-policy-binding nimble-valve-479420-p2 \
  --member="serviceAccount:${SERVICE_ACCOUNT}" \
  --role="roles/run.admin"

# Grant Artifact Registry Writer role
gcloud projects add-iam-policy-binding nimble-valve-479420-p2 \
  --member="serviceAccount:${SERVICE_ACCOUNT}" \
  --role="roles/artifactregistry.writer"

# Grant Service Account User role (needed to deploy as the service account)
gcloud projects add-iam-policy-binding nimble-valve-479420-p2 \
  --member="serviceAccount:${SERVICE_ACCOUNT}" \
  --role="roles/iam.serviceAccountUser"

# Grant Storage Admin role (for Cloud Build)
gcloud projects add-iam-policy-binding nimble-valve-479420-p2 \
  --member="serviceAccount:${SERVICE_ACCOUNT}" \
  --role="roles/storage.admin"
```

### 4. Create Service Account Key for GitHub

```bash
# Create a key file for the service account
gcloud iam service-accounts keys create key.json \
  --iam-account=svc-deploy-acct@nimble-valve-479420-p2.iam.gserviceaccount.com

# Display the key (copy this for GitHub secrets)
cat key.json
```

**⚠️ Important:** Keep this key secure and never commit it to your repository!

## GitHub Setup

### 1. Add GitHub Secret

1. Go to your GitHub repository
2. Navigate to **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Name: `GCP_SA_KEY`
5. Value: Paste the entire contents of the `key.json` file
6. Click **Add secret**

### 2. Configure Database Connection (if needed)

If your application uses a database, you need to:

#### Option A: Cloud SQL (Recommended for Production)

1. Create a Cloud SQL instance:
```bash
gcloud sql instances create sam-db \
  --database-version=SQLSERVER_2019_STANDARD \
  --tier=db-custom-2-7680 \
  --region=us-central1
```

2. Set the root password:
```bash
gcloud sql users set-password sqlserver \
  --instance=sam-db \
  --password=YOUR_SECURE_PASSWORD
```

3. Update the GitHub workflow to add Cloud SQL connection:
   - Add `--add-cloudsql-instances=nimble-valve-479420-p2:us-central1:sam-db` to the `gcloud run deploy` command
   - Add environment variable for connection string

#### Option B: External Database

Add the connection string as a GitHub secret and reference it in the workflow:

1. Create secret `DB_CONNECTION_STRING` in GitHub
2. Update the workflow to add: `--set-env-vars="ConnectionStrings__DefaultConnection=${{ secrets.DB_CONNECTION_STRING }}"`

### 3. Environment Variables

To add environment variables to your Cloud Run service, edit the workflow file and add them to the `--set-env-vars` flag:

```yaml
--set-env-vars="ASPNETCORE_ENVIRONMENT=Production,OTHER_VAR=value"
```

Or add them as secrets:

```yaml
--set-env-vars="ASPNETCORE_ENVIRONMENT=Production,API_KEY=${{ secrets.API_KEY }}"
```

## Deployment

### Automatic Deployment

The workflow automatically deploys when you push to `master` or `main` branch:

```bash
git add .
git commit -m "Deploy to GCP"
git push origin master
```

### Manual Deployment

You can manually trigger deployment from GitHub:

1. Go to **Actions** tab in your repository
2. Select **Deploy to Google Cloud Run** workflow
3. Click **Run workflow**
4. Choose the branch and click **Run workflow**

## Monitoring and Management

### View Logs

```bash
# View Cloud Run service logs
gcloud run services logs read senegalese-association --region=us-central1

# Follow logs in real-time
gcloud run services logs tail senegalese-association --region=us-central1
```

### Check Service Status

```bash
# Get service details
gcloud run services describe senegalese-association --region=us-central1

# List all Cloud Run services
gcloud run services list
```

### Update Configuration

To update service configuration without redeploying:

```bash
# Update memory allocation
gcloud run services update senegalese-association \
  --region=us-central1 \
  --memory=1Gi

# Update environment variables
gcloud run services update senegalese-association \
  --region=us-central1 \
  --set-env-vars="NEW_VAR=value"
```

## Workflow Configuration

The GitHub Actions workflow (`.github/workflows/deploy-to-gcp.yml`) includes:

- **Build**: Creates a Docker image from your .NET application
- **Push**: Uploads the image to Google Artifact Registry
- **Deploy**: Deploys the image to Cloud Run
- **Configuration**:
  - **Memory**: 512Mi (adjustable in workflow)
  - **CPU**: 1 (adjustable in workflow)
  - **Min instances**: 0 (scales to zero when not in use)
  - **Max instances**: 10 (adjustable based on needs)
  - **Port**: 8080 (configured in Dockerfile)
  - **Timeout**: 300 seconds

## Costs

### Cloud Run Pricing

- **Free tier**: 2 million requests/month, 360,000 GB-seconds/month
- **Pay-as-you-go**: Billed for actual usage
- With the current configuration (512Mi memory, min 0 instances), costs should be minimal for low-traffic sites

### Artifact Registry Pricing

- **Free tier**: 0.5 GB storage/month
- **Storage**: $0.10/GB/month
- **Network**: Egress charges may apply

## Troubleshooting

### Deployment Fails

1. **Check GitHub Actions logs**: Go to Actions tab and view the failed workflow
2. **Verify permissions**: Ensure service account has all required roles
3. **Check quotas**: Verify GCP project quotas aren't exceeded

### Service Not Accessible

1. **Check service URL**:
   ```bash
   gcloud run services describe senegalese-association --region=us-central1 --format="value(status.url)"
   ```

2. **Verify service is running**:
   ```bash
   gcloud run services describe senegalese-association --region=us-central1
   ```

3. **Check logs for errors**:
   ```bash
   gcloud run services logs read senegalese-association --region=us-central1 --limit=50
   ```

### Database Connection Issues

1. If using Cloud SQL, verify Cloud SQL connector is properly configured
2. Check connection string format in environment variables
3. Verify firewall rules allow Cloud Run to access your database

## Security Best Practices

1. **Never commit secrets** to your repository
2. **Rotate service account keys** regularly
3. **Use environment variables** for sensitive configuration
4. **Enable Cloud Armor** for DDoS protection if needed
5. **Set up Cloud Monitoring** alerts for unusual activity
6. **Use HTTPS only** (Cloud Run provides automatic HTTPS)
7. **Regular updates**: Keep dependencies up to date

## Custom Domain Setup

To use a custom domain:

1. **Map domain to Cloud Run**:
   ```bash
   gcloud run domain-mappings create \
     --service=senegalese-association \
     --domain=yourdomain.com \
     --region=us-central1
   ```

2. **Follow DNS instructions** provided by the command output

## Useful Commands

```bash
# Delete a Cloud Run service
gcloud run services delete senegalese-association --region=us-central1

# List all deployments
gcloud run revisions list --service=senegalese-association --region=us-central1

# Rollback to previous revision
gcloud run services update-traffic senegalese-association \
  --region=us-central1 \
  --to-revisions=REVISION_NAME=100

# View service metrics
gcloud monitoring dashboards list
```

## Support

For issues or questions:
- **GCP Documentation**: https://cloud.google.com/run/docs
- **GitHub Actions**: https://docs.github.com/actions
- **Cloud Run Pricing**: https://cloud.google.com/run/pricing
