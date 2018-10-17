FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 62624
EXPOSE 44356

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY PiSprinkler.AspNet/PiSprinkler.AspNet.csproj PiSprinkler.AspNet/
RUN dotnet restore PiSprinkler.AspNet/PiSprinkler.AspNet.csproj
COPY . .
WORKDIR /src/PiSprinkler.AspNet
RUN dotnet build PiSprinkler.AspNet.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish PiSprinkler.AspNet.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PiSprinkler.AspNet.dll"]
