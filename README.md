https://docs.docker.com/samples/dotnetcore/

# Build and run the Docker image

### Open a command prompt and navigate to your project folder.

### Use the following commands to build and run your Docker image:


> docker build -t aspnetapp .
> 
> docker run -d -p 8080:80 --name myapp aspnetapp
