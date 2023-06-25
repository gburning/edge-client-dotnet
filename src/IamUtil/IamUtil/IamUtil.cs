
namespace Nabto.Edge.ClientIam;
using Nabto.Edge.ClientIam.Impl;


/**
 * <summary>
 * <para>This class simplifies interaction with the Nabto Edge Embedded SDK device's CoAP IAM endpoints.</para>
 *
 * <para>For instance, it is made simple to invoke the different pairing endpoints - just invoke a simple high level
 * pairing function to pair the client with the connected device and don't worry about CBOR encoding and decoding.</para>
 *
 * <para>Read more about the important concept of pairing in the <see href="https://docs.nabto.com/developer/guides/concepts/iam/pairing.html">Nabto Edge IAM Pairing</see> guide.</para>
 *
 * <para>All the most popular IAM device endpoints are wrapped to also allow management of the user profile on the device
 * (own or other users' if client is in admin role).</para>
 *
 * <para>Note that the device's IAM configuration must allow invocation of the different functions and the pairing modes must
 * be enabled at runtime. Read more about that in the <see href="https://docs.nabto.com/developer/guides/concepts/iam/intro.html">general Nabto Edge IAM intro</see>.</para>
 * </summary>
 */
public class IamUtil
{

    // Different ways to pair
    public static Task PairLocalInitialAsync(Nabto.Edge.Client.Connection connection)
    {
        return Pairing.PairLocalInitialAsync(connection);
    }

    /**
     * <summary>
     * <para>Perform <see href="https://docs.nabto.com/developer/guides/concepts/iam/pairing.html#open-local">Local Open pairing</see>, requesting the specified username.</para>
     *
     * <para>Local open pairing uses the trusted local network (LAN) pairing mechanism. No password is required for pairing and no
     * invitation is needed, anybody on the LAN can initiate pairing.</para>
     * </summary>
     *
     * <param name="connection">An established connection to the device this client should be paired with</param>
     * <param name="desiredUsername">Assign this username on the device if available (pairing fails with .USERNAME_EXISTS if not)</param>
     *
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`USERNAME_EXISTS` if desiredUsername is already in use on the device.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`INVALID_INPUT` if desiredUsername is <see href="https://docs.nabto.com/developer/api-reference/coap/iam/post-users.html#request">not valid</see>.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`BLOCKED_BY_DEVICE_CONFIGURATION` if the device configuration does not support local open pairing (the `IAM:PairingLocalOpen` action
     * is not set for the Unpaired role or the device does not support the pairing mode at all).</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`PAIRING_MODE_DISABLED` if the pairing mode is configured on the device but is disabled at runtime.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`IAM_NOT_SUPPORTED` if Nabto Edge IAM is not supported by the device.</exception>
     */
    public static Task PairLocalOpenAsync(Nabto.Edge.Client.Connection connection, string desiredUsername)
    {
        return Pairing.PairLocalOpenAsync(connection, desiredUsername);
    }

    /**
     * <summary>
     * <para>Perform <see href="https://docs.nabto.com/developer/guides/concepts/iam/pairing.html#open-password">Password Open pairing</see>, requesting the specified username and authenticating using the specified password.</para>
     *
     * <para>In this mode a device has set a password which can be used in the pairing process to grant a client access to the
     * device. The client can pair remotely to the device if necessary; it is not necessary to be on the same LAN.</para>
     * </summary>
     *
     * <param name="connection"> An established connection to the device this client should be paired with</param>
     * <param name="desiredUsername"> Assign this username on the device if available (pairing fails with `USERNAME_EXISTS` if not)</param>
     * <param name="password"> the common (not user-specific) password to allow pairing using Password Open pairing</param>
     *
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`USERNAME_EXISTS`</see> if desiredUsername is already in use on the device.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`AUTHENTICATION_ERROR`</see> if the open pairing password was invalid for the device.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`INVALID_INPUT`</see> if desiredUsername is not valid as per https://docs.nabto.com/developer/api-reference/coap/iam/post-users.html#request.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`INITIAL_USER_ALREADY_PAIRED`</see> if the initial user was already paired.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`BLOCKED_BY_DEVICE_CONFIGURATION`</see> if the device configuration does not support local open pairing (the `IAM:PairingPasswordOpen` action is not set for the Unpaired role or the device does not support the pairing mode at all).</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`TOO_MANY_WRONG_PASSWORD_ATTEMPTS`</see> if the client has attempted to authenticate too many times with a wrong password (try again after 10 seconds).</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`PAIRING_MODE_DISABLED`</see> if the pairing mode is configured on the device but is disabled at runtime.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`IAM_NOT_SUPPORTED`</see> if Nabto Edge IAM is not supported by the device.</exception>
     */
    public static Task PairPasswordOpenAsync(Nabto.Edge.Client.Connection connection, string desiredUsername, string password)
    {
        return Pairing.PairPasswordOpenAsync(connection, desiredUsername, password);
    }

   /**
     * <summary>
     * <para>Perform <see href="https://docs.nabto.com/developer/guides/concepts/iam/pairing.html#invite">Password Invite pairing</see>, authenticating with the specified username and password.</para>
     *
     * <para>In the Password invite pairing mode a user is required in the system to be able to pair: An existing user (or
     * the system autonomously) creates a username and password that is somehow passed to the new user (an invitation).</para>
     * </summary>
     *
     * <param name="connection"> An established connection to the device this client should be paired with</param>
     * <param name="username"> Username for the invited user</param>
     * <param name="password"> Password for the invited user</param>
     *
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`AUTHENTICATION_ERROR`</see> if authentication failed using the specified username/password combination for the device.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`BLOCKED_BY_DEVICE_CONFIGURATION`</see> if the device configuration does not support local open pairing (the `IAM:PairingPasswordInvite` action is not set for the Unpaired role or the device does not support the pairing mode at all).</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`PAIRING_MODE_DISABLED`</see> if the pairing mode is configured on the device but is disabled at runtime.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`TOO_MANY_WRONG_PASSWORD_ATTEMPTS`</see> if the client has attempted to authenticate too many times with a wrong password (try again after 10 seconds).</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`IAM_NOT_SUPPORTED`</see> if Nabto Edge IAM is not supported by the device.</exception>
     */
    public static Task PairPasswordInviteAsync(Nabto.Edge.Client.Connection connection, string username, string password)
    {
        return Pairing.PairPasswordInviteAsync(connection, username, password);
    }

    /**
     * <summary>Retrieve device information that typically does not need a paired user.</summary>
     * <param name="connection">An established connection to the device</param>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`BLOCKED_BY_DEVICE_CONFIGURATION` if the device configuration does not allow retrieving this list (the
     * `IAM:GetPairing` action is not set for the Unpaired role)</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`IAM_NOT_SUPPORTED`</see> if Nabto Edge IAM is not supported by the device.</exception>
     * <returns>A DeviceDetails object containing the device information.</returns>
     */
    public static Task<DeviceDetails> GetDeviceDetailsAsync(Nabto.Edge.Client.Connection connection)
    {
        return DeviceInfo.GetDeviceDetailsAsync(connection);
    }

    /**
     * <summary>Update the device's friendly name.</summary>
     * <param name="connection">An established connection to the device</param>
     * <param name="friendlyName">The friendly name to set</param>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`INVALID_INPUT`</see> if the input name is not valid.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`FORBIDDEN`</see> if the user is not allowed to perform this operation.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`IAM_NOT_SUPPORTED` if Nabto Edge IAM is not supported by the device.</exception>
     */
    public static Task UpdateDeviceFriendlyNameAsync(Nabto.Edge.Client.Connection connection, string friendlyName)
    {
        return DeviceInfo.UpdateDeviceFriendlyNameAsync(connection, friendlyName);
    }

    /**
     * <summary>Get the system IAM configuration from device. Note that this may differ from the IAM state for the current user, retrieved as part of <see cref="DeviceDetails"/>.</summary>
     * <param name="connection">An established connection to the device</param>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`FORBIDDEN`</see> if the user is not allowed to perform this operation.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`IAM_NOT_SUPPORTED` if Nabto Edge IAM is not supported by the device.</exception>
     * <returns>An IamSettings object.</returns>
     */
    public static Task<IamSettings> GetIamSettingsAsync(Nabto.Edge.Client.Connection connection)
    {
        return Nabto.Edge.ClientIam.Impl.IamSettings.GetIamSettingsAsync(connection);
    }

    /**
     * <summary>Enable/disable the password open pairing mode on the device.</summary>
     * <param name="connection">An established connection to the device</param>
     * <param name="enabled">Set to true if password open pairing mode should be enabled, false if it should be disabled.</param>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`FORBIDDEN`</see> if the user is not allowed to perform this operation.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`IAM_NOT_SUPPORTED` if Nabto Edge IAM is not supported by the device.</exception>
     */
    public static Task UpdateIamSettingsPasswordOpenPairingAsync(Nabto.Edge.Client.Connection connection, bool enabled)
    {
        return Nabto.Edge.ClientIam.Impl.IamSettings.UpdateIamSettingsPasswordOpenPairingAsync(connection, enabled);
    }

    /**
     * <summary>Enable/disable the password invite pairing mode on the device.</summary>
     * <param name="connection">An established connection to the device</param>
     * <param name="enabled">Set to true if password invite pairing mode should be enabled, false if it should be disabled.</param>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`FORBIDDEN`</see> if the user is not allowed to perform this operation.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`IAM_NOT_SUPPORTED` if Nabto Edge IAM is not supported by the device.</exception>
     */
    public static Task UpdateIamSettingsPasswordInvitePairingAsync(Nabto.Edge.Client.Connection connection, bool enabled)
    {
        return Nabto.Edge.ClientIam.Impl.IamSettings.UpdateIamSettingsPasswordInvitePairingAsync(connection, enabled);
    }

    /**
     * <summary>Enable/disable the local open pairing mode on the device.</summary>
     * <param name="connection">An established connection to the device</param>
     * <param name="enabled">Set to true if local open pairing mode should be enabled, false if it should be disabled.</param>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`FORBIDDEN`</see> if the user is not allowed to perform this operation.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`IAM_NOT_SUPPORTED` if Nabto Edge IAM is not supported by the device.</exception>
     */
    public static Task UpdateIamSettingsLocalOpenPairingAsync(Nabto.Edge.Client.Connection connection, bool enabled)
    {
        return Nabto.Edge.ClientIam.Impl.IamSettings.UpdateIamSettingsLocalOpenPairingAsync(connection, enabled);
    }

    /**
     * <summary>Get available <see href="https://docs.nabto.com/developer/guides/iam/intro.html">roles</see> on device.</summary>
     * <param name="connection">An established connection to the device</param>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`FORBIDDEN`</see> if the user is not allowed to perform this operation.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`IAM_NOT_SUPPORTED` if Nabto Edge IAM is not supported by the device.</exception>
     * <returns>A list of roles available on the device.</returns>
     */
    public static Task<List<string>> ListRolesAsync(Nabto.Edge.Client.Connection connection)
    {
        return DeviceInfo.ListRolesAsync(connection);
    }

    /**
     * <summary>Get available <see href="https://docs.nabto.com/developer/guides/push/intro.html">push notification categories</see> on device.</summary>
     * <param name="connection">An established connection to the device</param>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`FORBIDDEN`</see> if the user is not allowed to perform this operation.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`IAM_NOT_SUPPORTED` if Nabto Edge IAM is not supported by the device.</exception>
     * <returns>A list of notification categories available on the device.</returns>
     */
    public static Task<List<string>> ListNotificationCategoriesAsync(Nabto.Edge.Client.Connection connection)
    {
        return DeviceInfo.ListNotificationCategoriesAsync(connection);
    }

    /**
     * <summary>Get details about the user that has opened the current connection to the device.</summary>
     * <param name="connection">An established connection to the device</param>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`FORBIDDEN`</see> if the user is not allowed to perform this operation.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`IAM_NOT_SUPPORTED` if Nabto Edge IAM is not supported by the device.</exception>
     * <returns>An IamUser instance describing the current user.</returns>
     */
    public static Task<IamUser> GetCurrentUserAsync(Nabto.Edge.Client.Connection connection)
    {
        return User.GetCurrentUserAsync(connection);
    }


    /**
     * <summary>Get details about a specific user.</summary>
     * <param name="connection">An established connection to the device</param>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`USER_DOES_NOT_EXIST`</see> if the user does not exist on the device.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`FORBIDDEN`</see> if the user is not allowed to perform this operation.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`IAM_NOT_SUPPORTED` if Nabto Edge IAM is not supported by the device.</exception>
     * <returns>An IamUser instance describing the current user.</returns>
     */
    public static Task<IamUser> GetUserAsync(Nabto.Edge.Client.Connection connection, string username)
    {
        return User.GetUserAsync(connection, username);
    }

    // Update user settings
    public static Task UpdateUserDisplayNameAsync(Nabto.Edge.Client.Connection connection, string username, string displayName)
    {
        return UserSettings.UpdateUserDisplayNameAsync(connection, username, displayName);
    }

    public static Task UpdateUserFcmAsync(Nabto.Edge.Client.Connection connection, string username, string fcmProjectId, string fcmToken)
    {
        return UserSettings.UpdateUserFcmAsync(connection, username, fcmProjectId, fcmToken);
    }

    public static Task UpdateUserFingerprintAsync(Nabto.Edge.Client.Connection connection, string username, string fingerprint)
    {
        return UserSettings.UpdateUserFingerprintAsync(connection, username, fingerprint);
    }

    public static Task UpdateUserNotificationCategoriesAsync(Nabto.Edge.Client.Connection connection, string username, List<string> categories)
    {
        return UserSettings.UpdateUserNotificationCategoriesAsync(connection, username, categories);
    }

    public static Task UpdateUserPasswordAsync(Nabto.Edge.Client.Connection connection, string username, string password)
    {
        return UserSettings.UpdateUserPasswordAsync(connection, username, password);
    }

    public static Task UpdateUserRoleAsync(Nabto.Edge.Client.Connection connection, string username, string role)
    {
        return UserSettings.UpdateUserRoleAsync(connection, username, role);
    }

    public static Task UpdateUserSctAsync(Nabto.Edge.Client.Connection connection, string username, string sct)
    {
        return UserSettings.UpdateUserSctAsync(connection, username, sct);
    }

    public static Task UpdateUserUsernameAsync(Nabto.Edge.Client.Connection connection, string username, string newUsername)
    {
        return UserSettings.UpdateUserUsernameAsync(connection, username, newUsername);
    }

    public static Task DeleteUserAsync(Nabto.Edge.Client.Connection connection, string username)
    {
        return User.DeleteUserAsync(connection, username);
    }

    public static async Task CreateUserAsync(Nabto.Edge.Client.Connection connection, IamUser user)
    {
        await User.CreateUserAsync(connection, user);
    }
};
