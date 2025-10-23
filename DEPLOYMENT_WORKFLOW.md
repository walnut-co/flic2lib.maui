# NuGet Deployment Workflow

This repository includes a GitHub Actions workflow to automatically build and deploy the flic2lib.Maui packages to NuGet.

## Setup Instructions

### 1. Configure NuGet API Key

1. Go to [NuGet.org](https://www.nuget.org) and sign in
2. Navigate to your profile and select "API Keys"
3. Create a new API key with "Push" permissions for your packages
4. In your GitHub repository, go to Settings > Secrets and variables > Actions
5. Add a new repository secret named `NUGET_API_KEY` with your NuGet API key as the value

### 2. Deployment Methods

#### Option A: Tag-based Deployment (Recommended)
```bash
# Create and push a version tag
git tag v1.0.7
git push origin v1.0.7
```

The workflow will automatically:
- Extract the version from the tag (removes the 'v' prefix)
- Update the nuspec files with the correct version
- Build the projects
- Create and deploy both packages to NuGet
- Create a GitHub release

#### Option B: Manual Deployment
1. Go to the "Actions" tab in your GitHub repository
2. Select "Deploy to NuGet" workflow
3. Click "Run workflow"
4. Enter the version number (e.g., `1.0.7`)
5. Click "Run workflow" button

### 3. Workflow Features

The workflow includes:
- ✅ **Automatic version extraction** from git tags or manual input
- ✅ **Dynamic version updating** in nuspec files
- ✅ **Multi-platform builds** (Android and iOS)
- ✅ **Package verification** before deployment
- ✅ **Sequential deployment** (Platforms first, then MAUI)
- ✅ **Artifact uploads** for backup
- ✅ **GitHub release creation** with changelog
- ✅ **Comprehensive error handling**

### 4. Package Dependencies

The workflow ensures proper dependency order:
1. **walnut.flic2lib.Platforms** is built and deployed first
2. **walnut.flic2lib.Maui** is deployed second (depends on Platforms)

### 5. Build Requirements

The workflow builds these components:
- `flic2lib.android` - Android bindings (.NET 9, API 29)
- `flic2lib.ios` - iOS bindings (.NET 9, iOS 13.0)
- `flic2lib.Maui` - MAUI library (depends on platform bindings)

### 6. Version Management

Versions are automatically synchronized across:
- `flic2lib.Platforms.simple.nuspec`
- `flic2lib.Maui.nuspec` (including dependency versions)

### 7. Testing Before Release

To test the workflow without deploying:
1. Comment out the deployment steps in the workflow
2. Run the workflow manually
3. Check the artifacts for generated packages

### 8. Troubleshooting

Common issues and solutions:

**Build Failures:**
- Ensure all required files exist in the expected paths
- Check that .NET 9 SDK is compatible with the binding projects

**Deployment Failures:**
- Verify the NUGET_API_KEY secret is correctly set
- Ensure the API key has push permissions
- Check if the version already exists on NuGet

**Missing Files:**
- The workflow verifies required DLL files exist before packaging
- Review the build output for any compilation errors

### 9. Package Information

**walnut.flic2lib.Platforms** contains:
- Android bindings with AAR file
- iOS bindings with xcframework
- Platform-specific native libraries

**walnut.flic2lib.Maui** contains:
- Cross-platform MAUI library
- Background event handling
- Depends on walnut.flic2lib.Platforms

### 10. Post-Deployment

After successful deployment:
- Packages are available on NuGet within minutes
- GitHub release is created with download links
- Build artifacts are stored for 30 days
- Package versions should be updated in consuming projects