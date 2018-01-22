#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Windows.Markup;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

#endregion

namespace Tauron.Application.Composition
{
    /// <summary>The assembly catalog.</summary>
    [PublicAPI]
    public class AssemblyCatalog : XamlCatalog
    {
        #region Public Properties

        /// <summary>Gets or sets the assembly name.</summary>
        /// <value>The assembly name.</value>
        [NotNull]
        public string AssemblyName { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     The fill container.
        /// </summary>
        /// <param name="container">
        ///     The container.
        /// </param>
        protected internal override void FillContainer(ExportResolver container)
        {
            try
            {
                var asm = string.IsNullOrWhiteSpace(AssemblyName)
                    ? Assembly.GetEntryAssembly()
                    : Assembly.Load(new AssemblyName(AssemblyName));

                container.AddAssembly(asm);
            }
            catch (Exception e)
            {
                CommonConstants.LogCommon(true, "Xaml Catalog: Invalid Assembly: {0}\n{1}", AssemblyName, e);
                throw;
            }
        }

        #endregion
    }

    /// <summary>The application catalog.</summary>
    [PublicAPI]
    public class ApplicationCatalog : XamlCatalog
    {
        #region Constants

        /// <summary>The plugin path.</summary>
        private const string PluginPath = "PlugIns";

        #endregion

        #region Methods

        /// <summary>
        ///     The fill container.
        /// </summary>
        /// <param name="container">
        ///     The container.
        /// </param>
        protected internal override void FillContainer(ExportResolver container)
        {
            container.AddPath(string.Empty);
            container.AddPath(PluginPath, "*.dll", SearchOption.AllDirectories);
            container.AddPath(string.Empty, "*.exe");
            container.AddPath(PluginPath, "*.exe", SearchOption.AllDirectories);
        }

        #endregion
    }

    /// <summary>The aggregate catalog.</summary>
    [ContentProperty("InnerCatalogs")]
    [DefaultProperty("InnerCatalogs")]
    [PublicAPI]
    public class AggregateCatalog : XamlCatalog
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AggregateCatalog" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="AggregateCatalog" /> Klasse.
        ///     Initializes a new instance of the <see cref="AggregateCatalog" /> class.
        /// </summary>
        public AggregateCatalog()
        {
            InnerCatalogs = new Collection<XamlCatalog>();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the inner catalogs.</summary>
        /// <value>The inner catalogs.</value>
        [NotNull]
        public Collection<XamlCatalog> InnerCatalogs { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     The fill container.
        /// </summary>
        /// <param name="container">
        ///     The container.
        /// </param>
        protected internal override void FillContainer(ExportResolver container)
        {
            foreach (var innerCatalog in InnerCatalogs) innerCatalog.FillContainer(container);
        }

        #endregion
    }

    /// <summary>The directory catalog.</summary>
    [PublicAPI]
    public class DirectoryCatalog : XamlCatalog
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DirectoryCatalog" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="DirectoryCatalog" /> Klasse.
        ///     Initializes a new instance of the <see cref="DirectoryCatalog" /> class.
        /// </summary>
        public DirectoryCatalog()
        {
            SearchPattern = "*.dll";
            SearchOption = SearchOption.AllDirectories;
            DiscoverChanges = false;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The fill container.
        /// </summary>
        /// <param name="container">
        ///     The container.
        /// </param>
        protected internal override void FillContainer(ExportResolver container)
        {
            container.AddPath(Path, SearchPattern, SearchOption, DiscoverChanges);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether discover changes.</summary>
        /// <value>The discover changes.</value>
        public bool DiscoverChanges { get; set; }

        /// <summary>Gets or sets the path.</summary>
        /// <value>The path.</value>
        [NotNull]
        public string Path { get; set; }

        /// <summary>Gets or sets the search option.</summary>
        /// <value>The search option.</value>
        public SearchOption SearchOption { get; set; }

        /// <summary>Gets or sets the search pattern.</summary>
        /// <value>The search pattern.</value>
        [NotNull]
        public string SearchPattern { get; set; }

        #endregion
    }

    /// <summary>The type catalog.</summary>
    [ContentProperty("Types")]
    [DefaultProperty("Types")]
    [PublicAPI]
    public class TypeCatalog : XamlCatalog
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeCatalog" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="TypeCatalog" /> Klasse.
        ///     Initializes a new instance of the <see cref="TypeCatalog" /> class.
        /// </summary>
        public TypeCatalog()
        {
            Types = new Collection<string>();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the types.</summary>
        /// <value>The types.</value>
        [NotNull]
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public Collection<string> Types { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     The fill container.
        /// </summary>
        /// <param name="container">
        ///     The container.
        /// </param>
        protected internal override void FillContainer(ExportResolver container)
        {
            var types = new List<Type>(Types.Count);

            foreach (var type in Types)
            {
                var tempType = Type.GetType(type);
                if (tempType == null)
                    CommonConstants.LogCommon(false, "Xaml Catalog: Type Not Found: {0}", type);
                else
                    types.Add(tempType);
            }

            container.AddTypes(types);
        }

        #endregion
    }
}