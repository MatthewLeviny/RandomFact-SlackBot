FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
ARG VERSION="1.0.0.0"

#Copy all the content from the root folder to the ./app folder (including folder structure)
COPY . ./app

#Set workdir to the project folder (so all the commands from now on will automatically run inside the workdir folder)
WORKDIR /app/RandomFact-Slack.Api
RUN dotnet restore --ignore-failed-sources
RUN dotnet test --verbosity=detailed --results-directory /TestResults/ --logger "trx;LogFileName=test_results.xml" ../RandomFact-Slack.Test/RandomFact-Slack.Test.csproj
RUN dotnet publish -c release /p:Version=$VERSION -o ../out

WORKDIR /app
COPY --from=builder /app/out .
COPY --from=builder /TestResults /TestResults
EXPOSE 80

ENTRYPOINT ["dotnet", "RandomFact-Slack.Api.dll"]