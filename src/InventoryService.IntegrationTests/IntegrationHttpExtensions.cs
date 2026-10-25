using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

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
        => response.Headers.GetValues(header).Contains(value);

    /// <summary>
    /// Asserts that a problem details instance has a particular error message.
    /// </summary>
    /// <param name="problemDetails">The problem details.</param>
    /// <param name="errorMessage">The expected error message.</param>
    public static void AssertProblemDetails(this ProblemDetails problemDetails, string errorMessage)
    {
        problemDetails.Extensions["errors"].Should().NotBeNull();
        problemDetails.Extensions["errors"]!.ToString().Should().Contain(errorMessage);
    }
}
