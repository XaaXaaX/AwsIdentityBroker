set -e

trap 'echo "\"Restore\" command filed with exit code $?."' EXIT 1

dotnet restore $PROPJECTSLN

trap 'echo "\"Sonar Begin\" command filed with exit code $?."' EXIT 1

dotnet /sonar-scanner/SonarScanner.MSBuild.dll begin /k:$PROJECTKEY /n:$PROJECTNAME /d:sonar.host.url=$SONARURL /d:sonar.issuesReport.html.enable=true /d:sonar.login=$SONARLOGIN /d:sonar.password=$SONARPASSWORD /d:sonar.branch=$BRANCHNAME

trap 'echo "\"Build\" command filed with exit code $?."' EXIT 1

dotnet build $PROJECTSLN -c $CONFIGURATION /noconsolelogger /l:TeamCity.MSBuild.Logger.TeamCityMSBuildLogger,/TeamCityMSBuildLogger/msbuild15/TeamCity.MSBuild.Logger.dll

trap 'echo "\"Sonar End\" command filed with exit code $?."' EXIT 1

dotnet /sonar-scanner/SonarScanner.MSBuild.dll end /d:sonar.login=$SONARLOGIN /d:sonar.password=$SONARPASSWORD

dotnet test --logger teamcity /p:CollectCoverage=true /p:CoverletOutputFormat=teamcity /p:Include=$TESTASSEMBLIES

set +e
