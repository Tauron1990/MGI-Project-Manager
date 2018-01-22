#region

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Markup;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

#endregion

namespace Tauron.Application.Composition
{
    /// <summary>The catalog collection.</summary>
    [ContentProperty("Catalogs")]
    [DefaultProperty("Catalogs")]
    [PublicAPI]
    public sealed class CatalogCollection
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CatalogCollection" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="CatalogCollection" /> Klasse.
        ///     Initializes a new instance of the <see cref="CatalogCollection" /> class.
        /// </summary>
        public CatalogCollection()
        {
            Catalogs = new Collection<XamlCatalog>();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the catalogs.</summary>
        /// <value>The catalogs.</value>
        [NotNull]
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public Collection<XamlCatalog> Catalogs { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The fill catalag.
        /// </summary>
        /// <param name="container">
        ///     The container.
        /// </param>
        public void FillCatalag([NotNull] ExportResolver container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            foreach (var xamlCatalog in Catalogs) xamlCatalog.FillContainer(container);
        }

        #endregion

        #region Fields

        #endregion

        #region Methods

        #endregion
    }
}