using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace InventoryService.IntegrationTests;

public static class IntegrationHttpExtensions
{
    public static async Task<TResponse?> Read<TResponse>(this HttpResponseMessage response) where TResponse : class
        => await response.Content.ReadFromJsonAsync<TResponse>();

    public static void AssertHeader(this HttpResponseMessage response, string header, string value)
        => response.Headers.GetValues(header).Contains(value);

    public static void AssertProblemDetails(this ProblemDetails problemDetails, string errorMessage)
    {
        problemDetails.Extensions["errors"].Should().NotBeNull();
        problemDetails.Extensions["errors"]!.ToString().Should().Contain(errorMessage);
    }
}
