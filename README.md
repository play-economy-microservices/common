# play.common
Common libraries used by Play Economy services.

## Create and publish package
```powershell
$version="1.0.5"
$owner="play-economy-microservices"
$gh_pat="[PAT HERE]"

dotnet pack src/Play.Common/ --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/play.common -o ../

dotnet nuget push ../packages/Play.Common.$version.nupkg --api-key $gh_pat --source "github"
```