using FluentAssertions;
using InventoryService.Infrastructure.Auth;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace InventoryService.UnitTests.Infrastructure.Auth;

public class ComputeHashTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Compute_HashesAsExpected(
        IOptions<HashOptions> options,
        string value)
    {
        // Arrange
        var sut = new ComputeHash(options);

        // Act
        var result = sut.Compute(value);

        // Assert
        var saltedValye = $"{value}{options.Value.Salt}";

        var bytes = Encoding.UTF8.GetBytes(value);

        var hash = SHA256.HashData(bytes);

        var expected = Convert.ToHexString(hash);

        result.Should().Be(expected);
    }
}
