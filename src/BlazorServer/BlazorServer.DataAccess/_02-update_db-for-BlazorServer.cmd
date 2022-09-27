dotnet tool update --global dotnet-ef --version 6.0.9
dotnet build
dotnet ef --startup-project ../BlazorServer.App/ database update --context ApplicationDbContext
dotnet ef --startup-project ../BlazorServer.App/ database update --context Parbad.Storage.EntityFrameworkCore.Context.ParbadDataContext 
pause