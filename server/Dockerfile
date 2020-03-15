FROM microsoft/dotnet:2.2-sdk AS publish
WORKDIR /src
COPY . .
RUN dotnet publish src/Services/Identity/STP.Identity.API/STP.Identity.API.csproj -c Release -o /app/STP.Identity
RUN dotnet publish src/Services/Markets/STP.Markets.API/STP.Markets.API.csproj -c Release -o /app/STP.Markets
RUN dotnet publish src/Services/Datafeed/STP.Datafeed.API/STP.Datafeed.API.csproj -c Release -o /app/STP.Datafeed
RUN dotnet publish src/Services/Profile/STP.Profile.API/STP.Profile.API.csproj -c Release -o /app/STP.Profile
RUN dotnet publish src/Services/Realtime/STP.Realtime.API/STP.Realtime.API.csproj -c Release -o /app/STP.Realtime