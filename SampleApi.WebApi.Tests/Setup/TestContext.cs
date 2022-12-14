using Docker.DotNet;
using Docker.DotNet.Models;
using System.Runtime.InteropServices;
using Xunit;

namespace SampleApi.WebApi.Tests.Setup
{
    public class TestContext : IAsyncLifetime
    {
        private readonly DockerClient _dockerClient;
        private const string ContainerImageUri = "amazon/dynamodb-local:latest";
        private string? _containerId;
        public TestContext()
        {
            var uri = new Uri("npipe://./pipe/docker_engine");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                uri = new Uri("unix:///var/run/docker.sock");
            _dockerClient = new DockerClientConfiguration(uri).CreateClient();
        }
        public async Task InitializeAsync()
        {
         // Will run directly after the class constructor 
            await PullImage();
            await StartContainer();
            //setup the table 
            await new TestDataSetup().CreateTable();
        }
        private async Task PullImage()
        {
            await _dockerClient.Images
                .CreateImageAsync(new ImagesCreateParameters
                {
                    FromImage = ContainerImageUri,
                    Tag = "latest"
                },
                    new AuthConfig(),
                    new Progress<JSONMessage>());
        }

        private async Task StartContainer()
        {
            var response = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = ContainerImageUri,
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    {
                        "8000", default
                    }
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        {"8000", new List<PortBinding> {new PortBinding {HostPort = "8000"}}}
                    },
                    PublishAllPorts = true
                }
            });

            _containerId = response.ID;

            await _dockerClient.Containers.StartContainerAsync(_containerId, null);
        }
        public async Task DisposeAsync()
        {
            if (_containerId != null)
            {
                await _dockerClient.Containers.KillContainerAsync(_containerId, new ContainerKillParameters());
            }
        }
    }
}
