FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY *.sln .
COPY WebApplicationtodolist/*.csproj WebApplicationtodolist/
RUN dotnet restore
COPY . .
WORKDIR /src/WebApplicationtodolist
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN if [ -f WebApplicationtodolist.dll ]; then mv WebApplicationtodolist.dll CoursePlatform.dll; fi
ENTRYPOINT ["dotnet", "CoursePlatform.dll"]
