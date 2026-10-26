using Interview.Application.Models;
using Xunit;

namespace Interview.API.UnitTests.Application;

public sealed class PagedResultTests
{
    [Theory]
    [InlineData(0, 10, 0)]
    [InlineData(1, 10, 1)]
    [InlineData(19, 10, 2)]
    [InlineData(20, 10, 2)]
    [InlineData(21, 10, 3)]
    public void TotalPages_RoundsUpToIncludePartialPage(
        int totalItems,
        int pageSize,
        int expectedTotalPages)
    {
        var result = new PagedResult<object>([], 1, pageSize, totalItems);

        Assert.Equal(expectedTotalPages, result.TotalPages);
    }
}
