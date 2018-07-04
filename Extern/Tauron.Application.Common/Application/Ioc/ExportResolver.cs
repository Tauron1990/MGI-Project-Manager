// The file InstanceResolver.cs is part of Tauron.Application.Common.
// 
// CoreEngine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CoreEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
//  along with Tauron.Application.Common If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstanceResolver.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The export resolver.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.BuildUp.Exports.DefaultExports;
using Tauron.Application.Ioc.Components;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The export resolver.</summary>
    [PublicAPI, Serializable]
    public sealed class ExportResolver
    {
        /// <summary>The assembly export provider.</summary>
        [Serializable]
        internal sealed class AssemblyExportProvider : ExportProvider, IEquatable<AssemblyExportProvider>
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="AssemblyExportProvider" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="AssemblyExportProvider" /> Klasse.
            ///     Initializes a new instance of the <see cref="AssemblyExportProvider" /> class.
            /// </summary>
            /// <param name="asm">
            ///     The asm.
            /// </param>
            public AssemblyExportProvider([NotNull] Assembly asm)
            {
                if (asm == null) throw new ArgumentNullException(nameof(asm));
                Asm = asm;
            }

            #endregion

            #region Public Properties

            /// <summary>Gets the technology.</summary>
            /// <value>The technology.</value>
            public override string Technology => AopConstants.DefaultExportFactoryName;

            #endregion

            #region Fields

            /// <summary>The asm.</summary>
            internal readonly Assembly Asm;

            /// <summary>The _provider.</summary>
            private TypeExportProvider _provider;

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     The equals.
            /// </summary>
            /// <param name="other">
            ///     The other.
            /// </param>
            /// <returns>
            ///     The <see cref="bool" />.
            /// </returns>
            public bool Equals(AssemblyExportProvider other)
            {
                if (ReferenceEquals(null, other)) return false;

                return ReferenceEquals(this, other) || Equals(Asm.FullName, other.Asm.FullName);
            }

            /// <summary>The ==.</summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            /// <returns></returns>
            public static bool operator ==(AssemblyExportProvider left, AssemblyExportProvider right)
            {
                return Equals(left, right);
            }

            /// <summary>The !=.</summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            /// <returns></returns>
            public static bool operator !=(AssemblyExportProvider left, AssemblyExportProvider right)
            {
                return !Equals(left, right);
            }

            public override IEnumerable<Tuple<IExport, int>> CreateExports(IExportFactory factory)
            {
                if (_provider == null) _provider = new TypeExportProvider(Asm.GetTypes());

                return _provider.CreateExports(factory);
            }

            /// <summary>
            ///     The equals.
            /// </summary>
            /// <param name="obj">
            ///     The obj.
            /// </param>
            /// <returns>
            ///     The <see cref="bool" />.
            /// </returns>
            public override bool Equals(object obj)
            {
                var prov = obj as AssemblyExportProvider;
                if (ReferenceEquals(prov, null)) return false;

                return Equals(prov);
            }

            /// <summary>The get hash code.</summary>
            /// <returns>
            ///     The <see cref="int" />.
            /// </returns>
            public override int GetHashCode()
            {
                return Asm != null ? Asm.GetHashCode() : 0;
            }

            #endregion
        }

        /// <summary>The path export provider.</summary>
        [Serializable]
        private sealed class PathExportProvider : ExportProvider, IDisposable
        {
            #region Fields

            /// <summary>The _discover changes.</summary>
            private readonly bool _discoverChanges;

            /// <summary>The _files.</summary>
            private readonly List<string> _files;

            /// <summary>The _option.</summary>
            private readonly SearchOption _option;

            /// <summary>The _path.</summary>
            private readonly string _path;

            /// <summary>The _searchpattern.</summary>
            private readonly string _searchpattern;

            /// <summary>The _factory.</summary>
            private IExportFactory _factory;

            /// <summary>The _providers.</summary>
            private List<AssemblyExportProvider> _providers;

            /// <summary>The _watcher.</summary>
            private FileSystemWatcher _watcher;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="PathExportProvider" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="PathExportProvider" /> Klasse.
            ///     Initializes a new instance of the <see cref="PathExportProvider" /> class.
            /// </summary>
            /// <param name="path">
            ///     The path.
            /// </param>
            /// <param name="searchpattern">
            ///     The searchpattern.
            /// </param>
            /// <param name="option">
            ///     The option.
            /// </param>
            /// <param name="discoverChanges">
            ///     The discover changes.
            /// </param>
            public PathExportProvider([NotNull] string path, [NotNull] string searchpattern, SearchOption option, bool discoverChanges)
            {
                _path            = path ?? throw new ArgumentNullException(nameof(path));
                _searchpattern   = searchpattern ?? throw new ArgumentNullException(nameof(searchpattern));
                _option          = option;
                _discoverChanges = discoverChanges;
                _files           = new List<string>(Directory.EnumerateFiles(path, searchpattern, option));
            }

            /// <summary>
            ///     Finalizes an instance of the <see cref="PathExportProvider" /> class.
            ///     Finalisiert eine Instanz der <see cref="PathExportProvider" /> Klasse.
            /// </summary>
            ~PathExportProvider()
            {
                Dispose();
            }

            #endregion

            #region Public Properties

            /// <summary>Gets a value indicating whether broadcast changes.</summary>
            /// <value>The broadcast changes.</value>
            public override bool BroadcastChanges => _discoverChanges;

            /// <summary>Gets the technology.</summary>
            /// <value>The technology.</value>
            public override string Technology => AopConstants.DefaultExportFactoryName;

            #endregion

            #region Public Methods and Operators

            /// <summary>The dispose.</summary>
            public void Dispose()
            {
                _watcher?.Dispose();

                GC.SuppressFinalize(this);
            }


            public override IEnumerable<Tuple<IExport, int>> CreateExports(IExportFactory factory)
            {
                _factory = factory;

                if (_providers == null)
                {
                    _providers = new List<AssemblyExportProvider>();
                    foreach (var file in _files)
                    {
                        AssemblyExportProvider exportProvider = null;
                        try
                        {
                            exportProvider = new AssemblyExportProvider(Assembly.LoadFile(file));
                        }
                        catch (FileLoadException)
                        {
                        }
                        catch (BadImageFormatException)
                        {
                        }

                        if (exportProvider == null) continue;

                        _providers.Add(exportProvider);
                    }
                }

                if (!_discoverChanges) return _providers.SelectMany(pro => pro.CreateExports(factory));

                _watcher = new FileSystemWatcher(_path, _searchpattern)
                           {
                               EnableRaisingEvents = true,
                               IncludeSubdirectories =
                                   _option
                                == SearchOption
                                       .AllDirectories
                           };
                _watcher.Created += Created;
                _watcher.Deleted += Deleted;

                return _providers.SelectMany(pro => pro.CreateExports(factory));
            }

            #endregion

            #region Methods

            /// <summary>
            ///     The created.
            /// </summary>
            /// <param name="sender">
            ///     The sender.
            /// </param>
            /// <param name="e">
            ///     The e.
            /// </param>
            private void Created([NotNull] object sender, [NotNull] FileSystemEventArgs e)
            {
                if (!Path.HasExtension(e.FullPath) || _providers == null) return;

                try
                {
                    var pro = new AssemblyExportProvider(Assembly.LoadFrom(e.FullPath));

                    if (_providers.Contains(pro)) return;

                    _providers.Add(pro);

                    OnExportsChanged(
                                     new ExportChangedEventArgs(
                                                                pro.CreateExports(_factory).SelectMany(exp => exp.Item1.ExportMetadata),
                                                                new ExportMetadata[0]));
                }
                catch (BadImageFormatException)
                {
                }
                catch (FileLoadException)
                {
                }
            }

            /// <summary>
            ///     The deleted.
            /// </summary>
            /// <param name="sender">
            ///     The sender.
            /// </param>
            /// <param name="e">
            ///     The e.
            /// </param>
            private void Deleted(object sender, FileSystemEventArgs e)
            {
                if (!Path.HasExtension(e.FullPath) || _providers == null) return;

                try
                {
                    var pro   = new AssemblyExportProvider(Assembly.LoadFrom(e.FullPath));
                    var index = _providers.IndexOf(pro);
                    if (index == -1) return;

                    pro = _providers[index];

                    _providers.RemoveAt(index);
                    OnExportsChanged(
                                     new ExportChangedEventArgs(
                                                                new ExportMetadata[0],
                                                                pro.CreateExports(_factory).SelectMany(exp => exp.Item1.ExportMetadata)));
                }
                catch (BadImageFormatException)
                {
                }
                catch (FileLoadException)
                {
                }
            }

            #endregion
        }

        /// <summary>The type export provider.</summary>
        [Serializable]
        private sealed class TypeExportProvider : ExportProvider
        {
            #region Public Properties

            /// <summary>Gets the technology.</summary>
            /// <value>The technology.</value>
            public override string Technology => AopConstants.DefaultExportFactoryName;

            #endregion

            #region Public Methods and Operators

            public override IEnumerable<Tuple<IExport, int>> CreateExports(IExportFactory factory)
            {
                var fac = (DefaultExportFactory) factory;

                if (_exports != null) return _exports;

                var exports = new List<Tuple<IExport, int>>(_types.Count());

                foreach (var type in _types)
                {
                    var currentLevel = _level;

                    var ex1 = fac.Create(type, ref currentLevel);
                    if (ex1 != null) exports.Add(Tuple.Create(ex1, currentLevel));

                    exports.AddRange(
                                     type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                                         .Select(methodInfo => fac.CreateMethodExport(methodInfo, ref currentLevel))
                                         .Where(ex2 => ex2 != null)
                                         .Select(exp => Tuple.Create(exp, currentLevel)));
                }

                _exports = exports.ToArray();
                return _exports;
            }

            #endregion

            #region Fields

            /// <summary>The _types.</summary>
            private readonly IEnumerable<Type> _types;

            /// <summary>The _exports.</summary>
            private Tuple<IExport, int>[] _exports;

            private int _level;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="TypeExportProvider" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="TypeExportProvider" /> Klasse.
            ///     Initializes a new instance of the <see cref="TypeExportProvider" /> class.
            /// </summary>
            /// <param name="types">
            ///     The types.
            /// </param>
            public TypeExportProvider([NotNull] IEnumerable<Type> types)
            {
                _types = types ?? throw new ArgumentNullException(nameof(types));
                _level = 0;
            }

            public TypeExportProvider([NotNull] IEnumerable<Type> types, int level)
            {
                _types = types ?? throw new ArgumentNullException(nameof(types));
                _level = level;
            }

            #endregion
        }

        #region Fields

        /// <summary>The _providers.</summary>
        private readonly List<ExportProvider> _providers;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExportResolver" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ExportResolver" /> Klasse.
        ///     Initializes a new instance of the <see cref="ExportResolver" /> class.
        /// </summary>
        public ExportResolver()
        {
            _providers = new List<ExportProvider>();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add assembly.
        /// </summary>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        public void AddAssembly([NotNull] Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            AddProvider(new AssemblyExportProvider(assembly));
        }

        /// <summary>
        ///     The add path.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="searchpattern">
        ///     The searchpattern.
        /// </param>
        /// <param name="option">
        ///     The option.
        /// </param>
        /// <param name="discoverChanges">
        ///     The discover changes.
        /// </param>
        public void AddPath([NotNull] string path, [NotNull] string searchpattern, SearchOption option, bool discoverChanges)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (searchpattern == null) throw new ArgumentNullException(nameof(searchpattern));
            if (path == string.Empty) path = AppDomain.CurrentDomain.BaseDirectory;

            path = path.GetFullPath();

            if (!path.ExisDirectory()) return;

            AddProvider(new PathExportProvider(path, searchpattern, option, discoverChanges));
        }

        /// <summary>
        ///     The add path.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        public void AddPath([NotNull] string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            AddPath(path, "*.dll");
        }

        /// <summary>
        ///     The add path.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="searchpattern">
        ///     The searchpattern.
        /// </param>
        public void AddPath([NotNull] string path, string searchpattern)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            AddPath(path, searchpattern, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        ///     The add path.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="searchpattern">
        ///     The searchpattern.
        /// </param>
        /// <param name="searchOption">
        ///     The search option.
        /// </param>
        public void AddPath([NotNull] string path, [NotNull] string searchpattern, SearchOption searchOption)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (searchpattern == null) throw new ArgumentNullException(nameof(searchpattern));
            AddPath(path, searchpattern, searchOption, false);
        }

        /// <summary>
        ///     The add provider.
        /// </summary>
        /// <param name="provider">
        ///     The provider.
        /// </param>
        public void AddProvider([NotNull] ExportProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            _providers.Add(provider);
        }

        /// <summary>
        ///     The add types.
        /// </summary>
        /// <param name="types">
        ///     The types.
        /// </param>
        public void AddTypes([NotNull] IEnumerable<Type> types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));
            AddProvider(new TypeExportProvider(types));
        }

        /// <summary>
        ///     The fill.
        /// </summary>
        /// <param name="componentRegistry">
        ///     The component registry.
        /// </param>
        /// <param name="exportRegistry">
        ///     The export registry.
        /// </param>
        /// <param name="exportProviderRegistry">
        ///     The export provider registry.
        /// </param>
        public void Fill(
            [NotNull] ComponentRegistry      componentRegistry,
            [NotNull] ExportRegistry         exportRegistry,
            [NotNull] ExportProviderRegistry exportProviderRegistry)
        {
            if (componentRegistry == null) throw new ArgumentNullException(nameof(componentRegistry));
            if (exportRegistry == null) throw new ArgumentNullException(nameof(exportRegistry));
            if (exportProviderRegistry == null) throw new ArgumentNullException(nameof(exportProviderRegistry));
            var factorys                                                                                         = new Dictionary<string, IExportFactory>();
            foreach (var factory in componentRegistry.GetAll<IExportFactory>()) factorys[factory.TechnologyName] = factory;

            foreach (var exportProvider in _providers)
            {
                foreach (var export in exportProvider.CreateExports(factorys[exportProvider.Technology]))
                {
                    exportRegistry.Register(export.Item1, export.Item2);
                }

                if (exportProvider.BroadcastChanges) exportProviderRegistry.Add(exportProvider);
            }
        }

        #endregion
    }
}