#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using Tauron.Interop;

#endregion

namespace Tauron.Application.Implement
{
    /// <summary>
    ///     This class checks to make sure that only one instance of
    ///     this application is running at a time.
    /// </summary>
    /// <typeparam name="TApplication">
    /// </typeparam>
    /// <remarks>
    ///     Note: this class should be used with some caution, because it does no
    ///     security checking. For example, if one instance of an app that uses this class
    ///     is running as Administrator, any other instance, even if it is not
    ///     running as Administrator, can activate it with command line arguments.
    ///     For most apps, this will not be much of an issue.
    /// </remarks>
    [PublicAPI]
    public static class SingleInstance<TApplication>
        where TApplication : ISingleInstanceApp
    {
        /// <summary>
        ///     Remoting service class which is exposed by the server i.e the first instance and called by the second instance
        ///     to pass on the command line arguments to the first instance and cause it to activate itself.
        /// </summary>
        [DebuggerNonUserCode]
        private class IpcRemoteService : MarshalByRefObject
        {
            #region Public Methods and Operators

            /// <summary>
            ///     Remoting Object's ease expires after every 5 minutes by default. We need to override the InitializeLifetimeService
            ///     class
            ///     to ensure that lease never expires.
            /// </summary>
            /// <returns>Always null.</returns>
            public override object InitializeLifetimeService()
            {
                return null;
            }

            /// <summary>
            ///     Activates the first instance of the application.
            /// </summary>
            /// <param name="args">
            ///     List of arguments to pass to the first instance.
            /// </param>
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
            public void InvokeFirstInstance([NotNull] IList<string> args)
            {
                // Do an asynchronous call to ActivateFirstInstance function
                var thread = new Thread(ActivateFirstInstanceCallback);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start(args);
            }

            #endregion
        }

        #region Public Properties

        /// <summary>Gets list of command line arguments for the application.</summary>
        /// <value>The command line args.</value>
        [NotNull]
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static IList<string> CommandLineArgs => _commandLineArgs ?? throw new InvalidOperationException();

        #endregion

        public static void
            InitializeAsFirstInstance(Mutex mutex, string channelName, TApplication app)
        {
            _app                 = app;
            _singleInstanceMutex = mutex;
            CreateRemoteService(channelName);
        }

        #region Constants

        /// <summary>Suffix to the channel name.</summary>
        public const string ChannelNameSuffix = "SingeInstanceIPCChannel";

        /// <summary>String delimiter used in channel names.</summary>
        public const string Delimiter = ":";

        /// <summary>IPC protocol used (string).</summary>
        private const string IpcProtocol = "ipc://";

        /// <summary>Remote service name.</summary>
        private const string RemoteServiceName = "SingleInstanceApplicationService";

        #endregion

        #region Static Fields

        /// <summary>The _app.</summary>
        [CanBeNull]
        private static ISingleInstanceApp _app;

        /// <summary>IPC channel for communications.</summary>
        [CanBeNull]
        private static IpcServerChannel _channel;

        /// <summary>List of command line arguments for the application.</summary>
        [CanBeNull]
        private static IList<string> _commandLineArgs;

        /// <summary>Application mutex.</summary>
        [CanBeNull]
        private static Mutex _singleInstanceMutex;

        #endregion

        #region Public Methods and Operators

        /// <summary>Cleans up single-instance code, clearing shared resources, mutexes, etc.</summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static void Cleanup()
        {
            _app = null;
            if (_singleInstanceMutex != null)
            {
                _singleInstanceMutex.Close();
                _singleInstanceMutex = null;
            }

            if (_channel == null) return;

            ChannelServices.UnregisterChannel(_channel);
            _channel = null;
        }

        /// <summary>
        ///     Checks if the instance of the application attempting to start is the first instance.
        ///     If not, activates the first instance.
        /// </summary>
        /// <param name="uniqueName">
        ///     The unique Name.
        /// </param>
        /// <param name="application">
        ///     The application.
        /// </param>
        /// <returns>
        ///     True if this is the first instance of the application.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static bool InitializeAsFirstInstance([NotNull] string uniqueName, TApplication application)
        {
            if (string.IsNullOrWhiteSpace(uniqueName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(uniqueName));
            _commandLineArgs = GetCommandLineArgs(uniqueName);

            // Build unique application Id and the IPC channel name.
            var applicationIdentifier = uniqueName + Environment.UserName;

            var channelName = string.Concat(applicationIdentifier, Delimiter, ChannelNameSuffix);

            // Create mutex based on unique application Id to check if this is the first instance of the application.
            _singleInstanceMutex = new Mutex(true, applicationIdentifier, out var firstInstance);
            if (firstInstance)
            {
                CreateRemoteService(channelName);
                _app = application;
            }
            else
            {
                SignalFirstInstance(channelName, _commandLineArgs);
            }

            return firstInstance;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Activates the first instance of the application with arguments from a second instance.
        /// </summary>
        /// <param name="args">
        ///     List of arguments to supply the first instance of the application.
        /// </param>
        private static void ActivateFirstInstance([CanBeNull] IList<string> args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            // Set main window state and process command line args

            _app?.SignalExternalCommandLineArgs(args);
        }

        /// <summary>
        ///     Callback for activating first instance of the application.
        /// </summary>
        /// <param name="arg">
        ///     Callback argument.
        /// </param>
        private static void ActivateFirstInstanceCallback([CanBeNull] object arg)
        {
            // Get command line args to be passed to first instance
            var args = arg as IList<string>;
            ActivateFirstInstance(args);
        }

        /// <summary>
        ///     Creates a remote service for communication.
        /// </summary>
        /// <param name="channelName">
        ///     Application's IPC channel name.
        /// </param>
        public static void CreateRemoteService([NotNull] string channelName)
        {
            var serverProvider = new BinaryServerFormatterSinkProvider {TypeFilterLevel = TypeFilterLevel.Full};
            IDictionary props = new Dictionary<string, string>
                                {
                                    ["name"]                = channelName,
                                    ["portName"]            = channelName,
                                    ["exclusiveAddressUse"] = "false"
                                };

            // Create the IPC Server channel with the channel properties
            _channel = new IpcServerChannel(props, serverProvider);

            // Register the channel with the channel services
            ChannelServices.RegisterChannel(_channel, true);

            // Expose the remote service with the REMOTE_SERVICE_NAME
            var remoteService = new IpcRemoteService();
            RemotingServices.Marshal(remoteService, RemoteServiceName);
        }

        /// <summary>
        ///     Gets command line args - for ClickOnce deployed applications, command line args may not be passed directly, they
        ///     have to be retrieved.
        /// </summary>
        /// <param name="uniqueApplicationName">
        ///     The unique Application Name.
        /// </param>
        /// <returns>
        ///     List of command line arg strings.
        /// </returns>
        [NotNull]
        public static IList<string> GetCommandLineArgs([NotNull] string uniqueApplicationName)
        {
            if (string.IsNullOrWhiteSpace(uniqueApplicationName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(uniqueApplicationName));
            string[] args = null;
            if (AppDomain.CurrentDomain.ActivationContext == null) // The application was not clickonce deployed, get args from standard API's
            {
                args = Environment.GetCommandLineArgs();
            }
            else
            {
                // The application was clickonce deployed
                // Clickonce deployed apps cannot recieve traditional commandline arguments
                // As a workaround commandline arguments can be written to a shared location before
                // the app is launched and the app can obtain its commandline arguments from the
                // shared location
                var appFolderPath =
                    Path.Combine(
                                 Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                 uniqueApplicationName);

                var cmdLinePath = Path.Combine(appFolderPath, "cmdline.txt");
                if (File.Exists(cmdLinePath))
                    try
                    {
                        using (TextReader reader = new StreamReader(cmdLinePath, Encoding.Unicode)) args = NativeMethods.CommandLineToArgvW(reader.ReadToEnd());

                        File.Delete(cmdLinePath);
                    }
                    catch (IOException)
                    {
                    }
            }

            if (args == null) args = new string[] { };

            return new List<string>(args);
        }

        /// <summary>
        ///     Creates a client channel and obtains a reference to the remoting service exposed by the server -
        ///     in this case, the remoting service exposed by the first instance. Calls a function of the remoting service
        ///     class to pass on command line arguments from the second instance to the first and cause it to activate itself.
        /// </summary>
        /// <param name="channelName">
        ///     Application's IPC channel name.
        /// </param>
        /// <param name="args">
        ///     Command line arguments for the second instance, passed to the first instance to take appropriate action.
        /// </param>
        public static void SignalFirstInstance([NotNull] string channelName, [NotNull] IList<string> args)
        {
            var secondInstanceChannel = new IpcClientChannel();
            ChannelServices.RegisterChannel(secondInstanceChannel, true);

            var remotingServiceUrl = IpcProtocol + channelName + "/" + RemoteServiceName;

            // Obtain a reference to the remoting service exposed by the server i.e the first instance of the application
            var firstInstanceRemoteServiceReference =
                (IpcRemoteService) RemotingServices.Connect(typeof(IpcRemoteService), remotingServiceUrl);

            // Check that the remote service exists, in some cases the first instance may not yet have created one, in which case
            // the second instance should just exit
            firstInstanceRemoteServiceReference?.InvokeFirstInstance(args);
        }

        #endregion
    }
}