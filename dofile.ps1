Target DockerUp -inherits DockerDown {
    docker compose up -d;
}

Target DockerDown {
    Push-Location docker;

    docker compose down -v --rmi all;
}

Target DotnetTest {
    Push-Location src;

    dotnet test;
}