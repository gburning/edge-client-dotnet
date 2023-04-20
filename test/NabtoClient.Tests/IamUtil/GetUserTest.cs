using Xunit;

namespace Nabto.Edge.Client.Tests;

public class GetUserTest
{

    [Fact]
    public async Task GetCurrentUser()
    {
        var iamConnection = await IamConnection.Create();

        var user = await IamUtil.GetCurrentUserAsync(iamConnection.Connection);
        Assert.Equal(user.Username, iamConnection.Username);
        Assert.NotNull(user.Role);
        Assert.NotNull(user.Sct);
        Assert.Equal(user.Fingerprint, iamConnection.Connection.GetClientFingerprint());
        Assert.Equal(user.NotificationCategories.Count, 0);
    }

    // TODO
    [Fact]
    public async Task SetGetNotificationCategories()
    {
        var iamConnection = await IamConnection.Create();

        await IamUtil.UpdateUserNotificationCategoriesAsync(iamConnection.Connection, iamConnection.Username, new List<string>{});

        var user2 = await IamUtil.GetCurrentUserAsync(iamConnection.Connection);

        Assert.Equal(user2.NotificationCategories.Count, 0);
    }

    [Fact]
    public async Task SetUserDisplayName() { 
        var iamConnection = await IamConnection.Create();

        var newDisplayName = "foo";
        await IamUtil.UpdateUserDisplayNameAsync(iamConnection.Connection, iamConnection.Username, newDisplayName);

        var user2 = await IamUtil.GetCurrentUserAsync(iamConnection.Connection);
        Assert.Equal(user2.DisplayName, newDisplayName);
    }

    [Fact]
    public async Task ListRoles() { 
        var iamConnection = await IamConnection.Create();

        var roles = await IamUtil.ListRolesAsync(iamConnection.Connection);
        Assert.True(roles.Count > 0);
    }

    [Fact]
    public async Task ListNotificationCategories() { 
        var iamConnection = await IamConnection.Create();

        var categories = await IamUtil.ListNotificationCategoriesAsync(iamConnection.Connection);
        Assert.Equal(categories.Count, 0);
    }



}