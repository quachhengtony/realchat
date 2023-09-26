# FROM mcr.microsoft.com/dotnet/sdk:6.0 as build

# WORKDIR /app

# COPY ./Realchat.Services.LLamaService.csproj .

# RUN dotnet restore

# COPY . .

# RUN dotnet publish -c Release -o ./published ./Realchat.Services.LLamaService.csproj

# FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime

# WORKDIR /app

# COPY --from=build /app/published .
# COPY --from=build /app/Files ./Files/

# ENTRYPOINT ["dotnet", "Realchat.Services.LLamaService.dll"]

FROM mcr.microsoft.com/dotnet/sdk:6.0 as build

WORKDIR /app

COPY ./Realchat.Services.LLamaService.csproj .

RUN dotnet restore

COPY . .

RUN dotnet publish -c Release ./Realchat.Services.LLamaService.csproj

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime

WORKDIR /app

COPY --from=build /app/bin/Release/net6.0/publish .
COPY --from=build /app/Files ./Files/

ENTRYPOINT ["dotnet", "Realchat.Services.LLamaService.dll"]