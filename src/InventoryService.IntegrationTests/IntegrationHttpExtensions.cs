using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;

namespace InventoryService.IntegrationTests;

public static class IntegrationHttpExtensions
{
    /// <summary>
    /// Read a typed object from an HTTP response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response payload/</typeparam>
    /// <param name="response">The response payload.</param>
    /// <returns>A task of the response payload type.</returns>
    public static async Task<TResponse?> Read<TResponse>(this HttpResponseMessage response) where TResponse : class
        => await response.Content.ReadFromJsonAsync<TResponse>();

    /// <summary>
    /// Assert a header is present and has a certain value.
    /// </summary>
    /// <param name="response">The HTTP response message</param>
    /// <param name="header">The expected header name.</param>
    /// <param name="value">the expected header value.</param>
    public static void AssertHeader(this HttpResponseMessage response, string header, string value)
        => response.Headers.GetValues(header).Any(x => x.Contains(value)).Should().BeTrue();

    /// <summary>
    /// Asserts that a problem details instance has a particular validation error message.
    /// </summary>
    /// <param name="problemDetails">The problem details.</param>
    /// <param name="errorMessage">The expected error message.</param>
    public static void AssertValidationErrorPresent(this ProblemDetails problemDetails, string propertyName, string errorMessage)
    {
        problemDetails.Extensions["errors"].Should().NotBeNull();
        var errors = (JsonElement)problemDetails.Extensions["errors"]!;

        var purchaseOrderErrors = errors
            .GetProperty(propertyName)
            .EnumerateArray()
            .Select(x => x.GetString())
            .ToList();

        problemDetails.Extensions["errors"]!.ToString().Should().Contain(errorMessage);
    }
}
