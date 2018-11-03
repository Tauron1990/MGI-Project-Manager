#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Tauron.Application.Ioc;
using Tauron.Interop;

#endregion

namespace Tauron.Application.Implement
{
    // [DebuggerNonUserCode]

    /// <summary>The tauron enviroment.</summary>
    [PublicAPI]
    [Export(typeof(ITauronEnviroment))]
    public class TauronEnviroment : ITauronEnviroment
    {
        #region Constants

        /// <summary>The app repository.</summary>
        public static string AppRepository = "Tauron";

        #endregion

        #region Fields

        /// <summary>The _default path.</summary>
        private string _defaultPath;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the default profile path.</summary>
        /// <value>The default profile path.</value>
        public string DefaultProfilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_defaultPath)) _defaultPath = LocalApplicationData;

                _defaultPath.CreateDirectoryIfNotExis();

                return _defaultPath;
            }

            set => _defaultPath = value;
        }

        /// <summary>Gets the local application data.</summary>
        /// <value>The local application data.</value>
        public string LocalApplicationData => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).CombinePath(AppRepository);

        /// <summary>Gets the local application temp folder.</summary>
        /// <value>The local application temp folder.</value>
        public string LocalApplicationTempFolder => LocalApplicationData.CombinePath("Temp");

        /// <summary>Gets the local download folder.</summary>
        /// <value>The local download folder.</value>
        public string LocalDownloadFolder => SearchForFolder(KnownFolder.Downloads);

        #endregion

        #region Public Methods and Operators

        public IEnumerable<string> GetProfiles(string application)
        {
            return
                DefaultProfilePath.CombinePath(application)
                    .EnumerateDirectorys()
                    .Select(ent => ent.Split('\\').Last());
        }

        /// <summary>
        ///     The search for folder.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string SearchForFolder(Guid id)
        {
            IntPtr pPath;
            if (NativeMethods.SHGetKnownFolderPath(id, 0, IntPtr.Zero, out pPath) == 0)
            {
                var s = Marshal.PtrToStringUni(pPath);
                Marshal.FreeCoTaskMem(pPath);
                return s;
            }

            return null;
        }

        #endregion
    }
}