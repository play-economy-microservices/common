# play.common

## Create and publish package
```powershell
$version="1.0.9"
$owner="play-economy-microservices"
$gh_pat="[PAT HERE]"

dotnet pack src/Play.Common/ --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/play.common -o ../packages

dotnet nuget push ../packages/Play.Common.$version.nupkg --api-key $gh_pat --source "github"
```
