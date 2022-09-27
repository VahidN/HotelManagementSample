For /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set mydate=%%c_%%a_%%b)
For /f "tokens=1-2 delims=/:" %%a in ("%TIME: =0%") do (set mytime=%%a%%b)
dotnet tool update --global dotnet-ef --version 6.0.9
dotnet build
dotnet ef migrations --startup-project ../../BlazorWasm/BlazorWasm.WebApi/ add V%mydate%_%mytime% --context ApplicationDbContext
dotnet ef migrations --startup-project ../../BlazorWasm/BlazorWasm.WebApi/ add P%mydate%_%mytime% --context Parbad.Storage.EntityFrameworkCore.Context.ParbadDataContext 
pause