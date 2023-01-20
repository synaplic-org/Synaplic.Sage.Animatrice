 

## Add DB Migration
- in the package manager console (Nuget)
    EntityFrameworkCore\Add-Migration -Project "Infrastructure" -StartupProject "Server" -OutputDir  Migrations "00001"


## Getting Started with Docker in Windows :rocket:

- Install Docker on Windows via `https://docs.docker.com/docker-for-windows/install/`
- Open up Powershell on Windows and run the following
    - `cd c:\`
    - `dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\aspnetapp.pfx -p securePassword123`
    - `dotnet dev-certs https --trust`
    - Note - Make sure that you use the same password that has been configured in the `docker-compose.yml` file. By default, `securePassword123` is configured.
- 5005 & 5006 are the ports setup to run on Docker, so make sure that these ports are free. You could also change the ports in the `docker-compose.yml` and `Server\Dockerfile` files.
- Now navigate back to the root of the Uni Project on your local machine and run the following via terminal - `docker-compose -f 'docker-compose.yml' up --build`
- This will start pulling MSSQL Server Image from Docker Hub if you don't already have this image. It's around 500+ Mbs of download.
- Once that is done, dotnet SDKs and runtimes are downloaded, if not present already. That's almost 200+ more Mbs of download.
- PS If you find any issues while Docker installs the nuget packages, it is most likelt that your ssl certificates are not intalled properly. Apart from that I also added the `--disable-parallel` in the `Server\Dockerfile`to ensure network issues don't pop-up. You can remove this option to speed up the build process.
- That's almost everything. Once the containers are available, migrations are updated in the MSSQL DB, default data is seeded.
- Browse to https://localhost:5005/ to use your version of Uni !

