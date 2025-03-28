/*
 * Copyright (c) 2024 PlayEveryWare
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

#if !EOS_DISABLE

namespace PlayEveryWare.EpicOnlineServices
{
    // This compile conditional is here so that when EOS is disabled, nothing is
    // referenced in the Epic namespace.
#if !EOS_DISABLE
    using Epic.OnlineServices.IntegratedPlatform;
    using Epic.OnlineServices.Auth;
    using Epic.OnlineServices.Platform;
    using Epic.OnlineServices.UI;
#endif
    using Common;
    using Common.Extensions;
    using Newtonsoft.Json;
    using System;

#if !EXTERNAL_TO_UNITY
    using UnityEngine;
#endif

    using Utility;

    /// <summary>
    /// Represents a set of configuration data for use by the EOS Plugin for
    /// Unity on a specific platform.
    /// </summary>
    public abstract class PlatformConfig : Config
    {
        private const PlatformManager.Platform OVERLAY_COMPATIBLE_PLATFORMS = ~(PlatformManager.Platform.Android |
                                                                   PlatformManager.Platform.iOS |
                                                                   PlatformManager.Platform.macOS |
                                                                   PlatformManager.Platform.Linux);

        /// <summary>
        /// The platform that the set of configuration data is to be applied on.
        /// </summary>
        [JsonIgnore]
        public PlatformManager.Platform Platform { get; }

        /// <summary>
        /// Any overriding values that should replace the central EOSConfig. On
        /// a platform-by-platform basis values can be overridden by setting
        /// these values. At runtime, these values will replace the ones on the
        /// central/main EOSConfig. This behavior is currently incomplete in
        /// it's implementation, but the intent is documented here for the sake
        /// of clarity and context.
        /// </summary>
        [JsonProperty] // Allow deserialization
        [JsonIgnore]   // Disallow serialization
        [Obsolete]     // Mark as obsolete so that warnings are generated
                       // whenever this is utilized (in the code that does the
                       // migration those warnings will be suppressed).
        public EOSConfig overrideValues;

        #region Deployment

        [ConfigField("Deployment", ConfigFieldType.Deployment, "Select the deployment to use.", 1)]
        public Deployment deployment;

#if !EOS_DISABLE
        [ConfigField("Client Credentials", ConfigFieldType.ClientCredentials, "Select client credentials to use.", 1)]
        public EOSClientCredentials clientCredentials;
#endif

        [ConfigField("Is Server", ConfigFieldType.Flag, "Check this if your game is a dedicated game server.", 1)]
        public bool isServer;

#endregion

        #region Flags

        // This conditional is here because if EOS_DISABLE is defined, then
        // the field member types will not be available.
#if !EOS_DISABLE
        /// <summary>
        /// Flags; used to initialize the EOS platform.
        /// </summary>
        [ConfigField("Platform Flags",
            ConfigFieldType.Enum,
            "Platform option flags",
            2, "https://dev.epicgames.com/docs/epic-online-services/eos-get-started/working-with-the-eos-sdk/eos-overlay-overview#eos-platform-flags-for-the-eos-overlay")]
        [JsonConverter(typeof(ListOfStringsToPlatformFlags))]
        public WrappedPlatformFlags platformOptionsFlags;

        /// <summary>
        /// Flags; used to set user auth when logging in.
        /// </summary>
        [ConfigField("Auth Scope Flags",
            ConfigFieldType.Enum,
            "Platform option flags",
            2, "https://dev.epicgames.com/docs/api-ref/enums/eos-e-auth-scope-flags?lang=en-US")]
        [JsonConverter(typeof(ListOfStringsToAuthScopeFlags))]
        public AuthScopeFlags authScopeOptionsFlags;

        /// <summary>
        /// Used to store integrated platform management flags.
        /// </summary>
        [ConfigField("Integrated Platform Management Flags", 
            ConfigFieldType.Enum, "Integrated Platform Management " +
                                  "Flags for platform specific options.",
            2, "https://dev.epicgames.com/docs/api-ref/enums/eos-e-integrated-platform-management-flags")]
        [JsonConverter(typeof(ListOfStringsToIntegratedPlatformManagementFlags))]
        public IntegratedPlatformManagementFlags integratedPlatformManagementFlags;

        // This property exists to maintain backwards-compatibility with 
        // previous versions of the config json structures.
        [JsonProperty] // Mark it so that it gets read
        [JsonIgnore] // Ignore so that it does not get written
        [Obsolete("This property is deprecated. Use the property integratedPlatformManagementFlags instead.")]
        [JsonConverter(typeof(ListOfStringsToIntegratedPlatformManagementFlags))]
        public IntegratedPlatformManagementFlags flags
        {
            get
            {
                return integratedPlatformManagementFlags;
            }
            set
            {
                integratedPlatformManagementFlags = value;
            }
        }
    
#endif

        #endregion

        #region Thread Affinity & Various Time Budgets

        /// <summary>
        /// Tick Budget; used to define the maximum amount of execution time the
        /// EOS SDK can use each frame.
        /// </summary>
        [ConfigField("Tick Budget (ms)",
            ConfigFieldType.Uint,
            "Used to define the maximum amount of execution time the " +
            "EOS SDK can use each frame.",
            3)]
        public uint tickBudgetInMilliseconds;

        /// <summary>
        /// TaskNetworkTimeoutSeconds; used to define the maximum number of
        /// seconds the EOS SDK will allow network calls to run before failing
        /// with EOS_TimedOut. This plugin treats any value that is less than or
        /// equal to zero as using the default value for the EOS SDK, which is
        /// 30 seconds.
        ///
        /// This value is only used when the <see cref="NetworkStatus"/> is not
        /// <see cref="NetworkStatus.Online"/>.
        /// <seealso cref="PlatformInterface.GetNetworkStatus"/>
        /// </summary>
        [ConfigField("Network Timeout Seconds",
            ConfigFieldType.Double,
            "Indicates the maximum number of seconds that (before " +
            "first coming online) the EOS SDK will allow network calls to " +
            "run before failing with EOS_TimedOut. This value does not apply " +
            "after the EOS SDK has been initialized.",
            3)]
        public double taskNetworkTimeoutSeconds;

        // This compile conditional is here so that when EOS is disabled, nothing is
        // referenced in the Epic namespace.
#if !EOS_DISABLE
        [ConfigField("Thread Affinity Options", 
            ConfigFieldType.WrappedInitializeThreadAffinity, 
            "Defines the thread affinity for threads started by the " +
            "EOS SDK. Leave values at zero to use default platform settings.",
            3, "https://dev.epicgames.com/docs/api-ref/structs/eos-initialize-thread-affinity")]
        public WrappedInitializeThreadAffinity threadAffinity;
#endif
#endregion

        #region Overlay Options

        /// <summary>
        /// Always Send Input to Overlay &lt;/c&gt;If true, the plugin will
        /// always send input to the overlay from the C# side to native, and
        /// handle showing the overlay. This doesn't always mean input makes it
        /// to the EOS SDK.
        /// </summary>
        [ConfigField(OVERLAY_COMPATIBLE_PLATFORMS,
            "Always Send Input to Overlay",
            ConfigFieldType.Flag,
            "If true, the plugin will always send input to the " +
            "overlay from the C# side to native, and handle showing the " +
            "overlay. This doesn't always mean input makes it to the EOS SDK.",
            4)]
        public bool alwaysSendInputToOverlay;

        /// <summary>
        /// Initial Button Delay.
        /// </summary>
        [ConfigField(OVERLAY_COMPATIBLE_PLATFORMS,
            "Initial Button Delay", ConfigFieldType.Float,
            "Initial Button Delay (if not set, whatever the default " +
            "is will be used).", 4)]
        [JsonConverter(typeof(StringToTypeConverter<float>))]
        public float initialButtonDelayForOverlay;

        /// <summary>
        /// Repeat button delay for overlay.
        /// </summary>
        [ConfigField(OVERLAY_COMPATIBLE_PLATFORMS,
            "Repeat Button Delay", ConfigFieldType.Float,
            "Repeat button delay for the overlay. If not set, " +
            "whatever the default is will be used.", 4)]
        [JsonConverter(typeof(StringToTypeConverter<float>))]
        public float repeatButtonDelayForOverlay;

        // This compile conditional is here so that when EOS is disabled, in the
        // Epic namespace is referenced.
#if !EOS_DISABLE
        /// <summary>
        /// When this combination of buttons is pressed on a controller, the
        /// social overlay will toggle on.
        /// Default to <see cref="InputStateButtonFlags.SpecialLeft"/>, and will
        /// use that value if this configuration field is null, empty, or contains
        /// only <see cref="InputStateButtonFlags.None"/>.
        /// </summary>
        [JsonConverter(typeof(ListOfStringsToInputStateButtonFlags))]
        [ConfigField(
            OVERLAY_COMPATIBLE_PLATFORMS,
            "Default Activate Overlay Button",
            ConfigFieldType.Enum,
            "Users can press the button's associated with this value " +
            "to activate the Epic Social Overlay. Not all combinations are " +
            "valid; the SDK will log an error at the start of runtime if an " +
            "invalid combination is selected.", 4)]
        public InputStateButtonFlags toggleFriendsButtonCombination = InputStateButtonFlags.SpecialLeft;

#endif

        #endregion

        /// <summary>
        /// Create a PlatformConfig by defining the platform it pertains to.
        /// </summary>
        /// <param name="platform">
        /// The platform to apply the config values on.
        /// </param>
        protected PlatformConfig(PlatformManager.Platform platform) :
            base(PlatformManager.GetConfigFileName(platform))
        {
            Platform = platform;
        }

        protected override void OnReadCompleted()
        {
            base.OnReadCompleted();

            // If the deployment and client credentials are complete, there is
            // nothing to do.
            if (deployment.IsComplete && clientCredentials is { IsComplete: true })
            {
                return;
            }

            ProductConfig productConfig = Get<ProductConfig>();
            bool valuesImported = false;

            if (!deployment.IsComplete && 
                productConfig.Environments.TryGetFirstDefinedNamedDeployment(out Named<Deployment> namedDeployment))
            {
                deployment = namedDeployment.Value;
                Debug.Log($"Platform {Platform} has no defined deployment, " +
                          $"so one was selected: {namedDeployment}.");
                valuesImported = true;
            }

            if (clientCredentials is not { IsComplete: true } &&
                productConfig.TryGetFirstCompleteNamedClientCredentials(
                    out Named<EOSClientCredentials> namedCredentials))
            {
                clientCredentials = namedCredentials.Value;
                Debug.Log($"Platform {Platform} has no defined client " +
                          $"credentials, so one was selected: " +
                          $"{namedCredentials}.");
                valuesImported = true;
            }

            // This compile conditional is here because writing configs to disk
            // is only allowed within the context of the unity editor.
#if UNITY_EDITOR
            if (valuesImported)
            {
                Write();
            }
#endif
            // If thread affinity is null then instantiate it.
            threadAffinity ??= new();
        }

        #region Logic for Migrating Override Values from Previous Structure

        // This warning is suppressed because while EOSConfig is marked as 
        // obsolete - it is important that it remain and be used within this
        // section of code so that things can be properly migrated.
#pragma warning disable CS0618 // Type or member is obsolete
#if !EOS_DISABLE

        protected sealed class NonOverrideableConfigValues : Config
        {
            public string deploymentID;
            public string clientID;
            public uint tickBudgetInMilliseconds;
            public double taskNetworkTimeoutSeconds;

            [JsonConverter(typeof(ListOfStringsToPlatformFlags))]
            public WrappedPlatformFlags platformOptionsFlags;

            [JsonConverter(typeof(ListOfStringsToAuthScopeFlags))]
            public AuthScopeFlags authScopeOptionsFlags;

            [JsonConverter(typeof(ListOfStringsToIntegratedPlatformManagementFlags))]
            public IntegratedPlatformManagementFlags integratedPlatformManagementFlags;

            public bool alwaysSendInputToOverlay;

            static NonOverrideableConfigValues()
            {
                RegisterFactory(() => new NonOverrideableConfigValues());
            }

            internal NonOverrideableConfigValues() : base("EpicOnlineServicesConfig.json") { }
        }

        internal sealed class OverrideableConfigValues : Config
        {
            [JsonConverter(typeof(ListOfStringsToPlatformFlags))]
            public WrappedPlatformFlags platformOptionsFlags;

            [JsonConverter(typeof(StringToTypeConverter<float>))]
            public float? initialButtonDelayForOverlay;

            [JsonConverter(typeof(StringToTypeConverter<float>))]
            public float? repeatButtonDelayForOverlay;

            [JsonConverter(typeof(StringToTypeConverter<ulong>))]
            public ulong? ThreadAffinity_networkWork;

            [JsonConverter(typeof(StringToTypeConverter<ulong>))]
            public ulong? ThreadAffinity_storageIO;

            [JsonConverter(typeof(StringToTypeConverter<ulong>))]
            public ulong? ThreadAffinity_webSocketIO;

            [JsonConverter(typeof(StringToTypeConverter<ulong>))]
            public ulong? ThreadAffinity_P2PIO;

            [JsonConverter(typeof(StringToTypeConverter<ulong>))]
            public ulong? ThreadAffinity_HTTPRequestIO;

            [JsonConverter(typeof(StringToTypeConverter<ulong>))]
            public ulong? ThreadAffinity_RTCIO;

            static OverrideableConfigValues()
            {
                RegisterFactory(() => new OverrideableConfigValues());
            }

            internal OverrideableConfigValues() : base("EpicOnlineServicesConfig.json") { }
        }

        private static TK SelectValue<TK>(TK overrideValuesFromFieldMember, TK mainConfigValue)
        {
            // If the value in the overrides is not default, then it takes
            // precedent
            return !overrideValuesFromFieldMember.Equals(default) ? overrideValuesFromFieldMember : mainConfigValue;
        }


        private void MigrateButtonDelays(EOSConfig overrideValuesFromFieldMember, OverrideableConfigValues mainOverrideableConfig)

        {
            // Import the values for initial button delay and repeat button
            // delay
            initialButtonDelayForOverlay = SelectValue(
                overrideValuesFromFieldMember.initialButtonDelayForOverlay ?? 0,
                mainOverrideableConfig.initialButtonDelayForOverlay ?? 0);

            repeatButtonDelayForOverlay = SelectValue(
                overrideValuesFromFieldMember.repeatButtonDelayForOverlay ?? 0,
                mainOverrideableConfig.repeatButtonDelayForOverlay ?? 0);
        }

        private void MigrateThreadAffinity(EOSConfig overrideValuesFromFieldMember, OverrideableConfigValues mainOverrideableConfig)
        {
            threadAffinity ??= new();

            // Import the values for thread initialization
            threadAffinity.NetworkWork = SelectValue(
                overrideValuesFromFieldMember.ThreadAffinity_networkWork,
                mainOverrideableConfig.ThreadAffinity_networkWork) ?? 0;

            threadAffinity.StorageIo = SelectValue(
                overrideValuesFromFieldMember.ThreadAffinity_storageIO,
                mainOverrideableConfig.ThreadAffinity_storageIO) ?? 0;

            threadAffinity.WebSocketIo = SelectValue(
                overrideValuesFromFieldMember.ThreadAffinity_webSocketIO,
                mainOverrideableConfig.ThreadAffinity_webSocketIO) ?? 0;

            threadAffinity.P2PIo = SelectValue(
                overrideValuesFromFieldMember.ThreadAffinity_P2PIO,
                mainOverrideableConfig.ThreadAffinity_P2PIO) ?? 0;

            threadAffinity.HttpRequestIo = SelectValue(
                overrideValuesFromFieldMember.ThreadAffinity_HTTPRequestIO,
                mainOverrideableConfig.ThreadAffinity_HTTPRequestIO) ?? 0;

            threadAffinity.RTCIo = SelectValue(
                overrideValuesFromFieldMember.ThreadAffinity_RTCIO,
                mainOverrideableConfig.ThreadAffinity_RTCIO) ?? 0;
        }

        private void MigrateOverrideableConfigValues(EOSConfig overrideValuesFromFieldMember,
            OverrideableConfigValues mainOverrideableConfig)
        {
            // Import the values for platform option flags.
            platformOptionsFlags |= overrideValuesFromFieldMember.platformOptionsFlags;

            MigrateButtonDelays(overrideValuesFromFieldMember, mainOverrideableConfig);
            MigrateThreadAffinity(overrideValuesFromFieldMember, mainOverrideableConfig);
        }

        protected virtual void MigrateNonOverrideableConfigValues(EOSConfig overrideValuesFromFieldMember,
            NonOverrideableConfigValues mainNonOverrideableConfig)
        {
            authScopeOptionsFlags = mainNonOverrideableConfig.authScopeOptionsFlags;

            // Because by default EOSManager used to define the auth scope flags to default to
            // the following, when migrating from the old configuration to the new configuration,
            // set these auth scope options flags explicitly so that they are reflected in both
            // the functionality and the user interface that displays the configuration
            authScopeOptionsFlags |= AuthScopeFlags.BasicProfile;
            authScopeOptionsFlags |= AuthScopeFlags.FriendsList;
            authScopeOptionsFlags |= AuthScopeFlags.Presence;

            tickBudgetInMilliseconds = mainNonOverrideableConfig.tickBudgetInMilliseconds;
            taskNetworkTimeoutSeconds = mainNonOverrideableConfig.taskNetworkTimeoutSeconds;
            alwaysSendInputToOverlay = mainNonOverrideableConfig.alwaysSendInputToOverlay;

            MigratePlatformFlags(overrideValuesFromFieldMember, mainNonOverrideableConfig);

            // If there are no Integrated Platform Management Flags in the original config, apply a set of default per-platform IMPFs
            if ((int)mainNonOverrideableConfig.integratedPlatformManagementFlags == 0 || mainNonOverrideableConfig.integratedPlatformManagementFlags == IntegratedPlatformManagementFlags.Disabled)
            {
                integratedPlatformManagementFlags = GetDefaultIntegratedPlatformManagementFlags();
            }
            else
            {
                integratedPlatformManagementFlags = mainNonOverrideableConfig.integratedPlatformManagementFlags;
            }

            ProductConfig productConfig = Get<ProductConfig>();
            string compDeploymentString = mainNonOverrideableConfig.deploymentID?.ToLower();

            // If the compDeploymentString is explicitly set, then seek that out and use it
            if (!string.IsNullOrEmpty(compDeploymentString))
            {
                Deployment? foundDeployment = null;

                foreach (Named<Deployment> dep in productConfig.Environments.Deployments)
                {
                    if (!compDeploymentString.Equals(dep.Value.DeploymentId.ToString("N").ToLowerInvariant()))
                    {
                        continue;
                    }

                    foundDeployment = dep.Value;
                    break;
                }

                // If we didn't find a matching deployment, log about it
                if (foundDeployment.HasValue)
                {
                    deployment = foundDeployment.Value;
                }
                else
                {
                    Debug.LogWarning($"The previous config explicitly identified "
                        + $"'{compDeploymentString}' as the Deployment GUID, but "
                        + "could not find a deployment with that id in the config. "
                        + "You must set the Deployment in the EOS Configuration window.");
                }
            }
            else if (productConfig.Environments.Deployments.Count == 1)
            {
                // If compDeploymentString wasn't explicitly set, and there was exactly
                // one defined deployment, then intuitively use that.

                deployment = productConfig.Environments.Deployments[0].Value;
                Debug.Log($"The previous config did not explicitly define a " +
                    $"deployment. There was one defined deployment, automatically " +
                    $"selecting deployment name '{productConfig.Environments.Deployments[0].Name}' " +
                    $"with id '{deployment.DeploymentId}'.");
            }
            else
            {
                // If there are multiple deployments, we can't auto select one.
                // The user will have to set one directly.
                Debug.LogWarning($"The previous config did not explicitly define " +
                    $"a deployment for this platform, and there are more than " +
                    $"one available deployments. You must set the Deployment in" +
                    $"the EOS Configuration window for this platform.");
            }

            string compClientString = mainNonOverrideableConfig.clientID?.ToLowerInvariant();
            // If the compClientString is explicitly set, then seek that out and use it
            if (!string.IsNullOrEmpty(compClientString))
            {
                EOSClientCredentials foundCredentials = null;

                foreach (Named<EOSClientCredentials> creds in productConfig.Clients)
                {
                    if (!compClientString.Equals(creds.Value.ClientId.ToLowerInvariant()))
                    {
                        continue;
                    }

                    foundCredentials = creds.Value;
                    break;
                }

                // If we didn't find a matching deployment, log about it
                if (foundCredentials != null)
                {
                    clientCredentials = foundCredentials;
                }
                else
                {
                    Debug.LogWarning($"The previous config explicitly identified "
                        + $"'{compClientString}' as the Client ID, but "
                        + "could not find client credentials with that id in the config. "
                        + "You must set the Client Credentials in the EOS Configuration window.");
                }
            }
            else if (productConfig.Clients.Count == 1)
            {
                // If compClientString wasn't explicitly set, and there was exactly
                // one defined client credential, then intuitively use that.

                clientCredentials = productConfig.Clients[0].Value;
                Debug.Log($"The previous config did not explicitly define " +
                    $"client credentials. There was one defined client credential, automatically " +
                    $"selecting client credential name '{productConfig.Clients[0].Name}' " +
                    $"with id '{clientCredentials.ClientId}'.");
            }
            else
            {
                // If there are multiple deployments, we can't auto select one.
                // The user will have to set one directly.
                Debug.LogWarning($"The previous config did not explicitly define " +
                    $"client credentials for this platform, and there are more than " +
                    $"one available client credentials. You must set the " +
                    $"Client Credentials in the EOS Configuration window for this platform.");
            }
        }

        protected virtual void MigratePlatformFlags(EOSConfig overrideValuesFromFieldMember,
            NonOverrideableConfigValues mainNonOverrideableConfig)
        {
            // The best (perhaps only) way to meaningfully migrate values from
            // the overrideValues field member and the main config is to combine
            // them, then filter them to exclude any platform flags that are 
            // incompatible with the platform for this Config. THIS is the 
            // primary reason it is necessary to warn the user and ask them to 
            // double check the values after migration.
            WrappedPlatformFlags combinedPlatformFlags = mainNonOverrideableConfig.platformOptionsFlags;

            if (overrideValuesFromFieldMember != null)
            {
                combinedPlatformFlags |= overrideValuesFromFieldMember.platformOptionsFlags;
            }

            WrappedPlatformFlags migratedPlatformFlags = WrappedPlatformFlags.None;
            foreach (WrappedPlatformFlags flag in EnumUtility<WrappedPlatformFlags>.GetEnumerator(combinedPlatformFlags))
            {
                switch (flag)
                {
                    case WrappedPlatformFlags.None:
                    case WrappedPlatformFlags.LoadingInEditor:
                    case WrappedPlatformFlags.DisableOverlay:
                    case WrappedPlatformFlags.DisableSocialOverlay:
                    case WrappedPlatformFlags.Reserved1:
                        migratedPlatformFlags |= flag;
                        break;
                    case WrappedPlatformFlags.WindowsEnableOverlayD3D9:
                    case WrappedPlatformFlags.WindowsEnableOverlayD3D10:
                    case WrappedPlatformFlags.WindowsEnableOverlayOpengl:
                        if (Platform == PlatformManager.Platform.Windows)
                        {
                            migratedPlatformFlags |= flag;
                        }
                        break;
                    case WrappedPlatformFlags.ConsoleEnableOverlayAutomaticUnloading:
                        if (Platform == PlatformManager.Platform.Console)
                        {
                            migratedPlatformFlags |= flag;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            platformOptionsFlags = migratedPlatformFlags;
        }


        protected override void MigrateConfig()
        {
            // The following code takes any values that used to exist within the
            // overrideValues field member, and places them into the
            // appropriate new field members. It also takes values from the 
            // main EOSConfig - if the values are not defined in the
            // overrideValues.

            // Do nothing if the values have already been moved, or if
            // overrideValues is null.
#pragma warning disable CS0612 // Type or member is obsolete
            if (null != overrideValues)
            {
                // This config represents the set of values that previously were 
                // overrideable from the editor window. These values should take
                // priority over the main config if they are not default values.
                OverrideableConfigValues mainOverrideableConfigValues = Get<OverrideableConfigValues>();
#pragma warning disable CS0612 // Type or member is obsolete
                MigrateOverrideableConfigValues(overrideValues, mainOverrideableConfigValues);
#pragma warning restore CS0612 // Type or member is obsolete
            }
#pragma warning restore CS0612 // Type or member is obsolete

            // This config represents the set of values that were not
            // overrideable from the editor window. The migrated values should
            // favor these set of values.
            NonOverrideableConfigValues mainNonOverrideableConfigValuesThatCouldNotBeOverridden =
                Get<NonOverrideableConfigValues>();
#pragma warning disable CS0612 // Type or member is obsolete
            MigrateNonOverrideableConfigValues(overrideValues,
                mainNonOverrideableConfigValuesThatCouldNotBeOverridden);
#pragma warning restore CS0612 // Type or member is obsolete

            // Notify the user of the migration, encourage them to double check
            // that migration was successful.
            Debug.LogWarning(
                $"Configuration values for {GetType().Name} have been " +
                "migrated. Please double check your configuration in EOS " +
                "Plugin -> EOS Configuration to make sure that the " +
                "migration was successful.");
        }

        public virtual IntegratedPlatformManagementFlags GetDefaultIntegratedPlatformManagementFlags()
        {
            return IntegratedPlatformManagementFlags.Disabled;
        }


#endif

#pragma warning restore CS0618 // Type or member is obsolete

        #endregion
    }
}

#endif