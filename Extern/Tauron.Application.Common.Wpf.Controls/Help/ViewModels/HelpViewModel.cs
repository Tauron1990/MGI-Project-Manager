#region

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml.Linq;
using JetBrains.Annotations;
using Tauron.Application.Controls;

#endregion

namespace Tauron.Application.Help.ViewModels
{
    /// <summary>The help topic.</summary>
    [PublicAPI]
    public sealed class HelpTopic : DependencyObject
    {
        #region Constructors and Destructors

        internal HelpTopic([NotNull] string name, [NotNull] string tocken)
        {
            Tocken = tocken;
            Name   = name;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The to string.</summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the name.</summary>
        [NotNull]
        public string Name { get; private set; }

        /// <summary>Gets the tocken.</summary>
        [CanBeNull]
        public string Tocken
        {
            get => (string) ToggleButtonList.GetTopic(this);

            private set
            {
                if (value != null) ToggleButtonList.SetTopic(this, value);
            }
        }

        #endregion
    }

    /// <summary>The help group.</summary>
    public sealed class HelpGroup : IHeaderProvider
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HelpGroup" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="HelpGroup" /> Klasse.
        /// </summary>
        /// <param name="titel">
        ///     The titel.
        /// </param>
        /// <param name="content">
        ///     The content.
        /// </param>
        /// <param name="id">
        ///     The id.
        /// </param>
        public HelpGroup([NotNull] string titel, [NotNull] string content, [NotNull] string id)
        {
            Id       = id;
            _titel   = titel;
            _content = content;
        }

        #endregion

        #region Fields

        private readonly string _content;

        private readonly string _titel;

        #endregion

        #region Public Properties

        /// <summary>Gets the content.</summary>
        [CanBeNull]
        public FlowDocument Content => XamlReader.Parse(_content) as FlowDocument;

        /// <summary>Gets the id.</summary>
        [NotNull]
        public string Id { get; }

        /// <summary>Gets the header.</summary>
        [NotNull]
        public object Header => _titel;

        #endregion
    }

    /// <summary>The help database.</summary>
    public class HelpDatabase
    {
        #region Fields

        private readonly XElement _database;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HelpDatabase" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="HelpDatabase" /> Klasse.
        /// </summary>
        /// <param name="fileName">
        ///     The file name.
        /// </param>
        public HelpDatabase([NotNull] string fileName)
        {
            if (fileName.ExisFile()) _database = XElement.Load(fileName);
            else if (fileName.ExisDirectory())
                foreach (var file in fileName.GetFiles())
                {
                    _database = new XElement("dataBase");

                    var ele = XElement.Load(file);
                    foreach (var element in ele.Elements()) _database.Add(element);
                }
            else _database = new XElement("help");
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the topics.</summary>
        [NotNull]
        public IEnumerable<HelpTopic> Topics => from ele in _database.Elements()
                                                select new HelpTopic(ele.Attribute("Name").Value, ele.Attribute("Tocken").Value);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The fill.
        /// </summary>
        /// <param name="groups">
        ///     The groups.
        /// </param>
        /// <param name="token">
        ///     The token.
        /// </param>
        public void Fill([NotNull] ICollection<HelpGroup> groups, [CanBeNull] string token)
        {
            var groupsContent =
                _database.Elements().FirstOrDefault(ele => ele.Attribute("Tocken").Value == token);
            if (groupsContent == null) return;

            foreach (var group in groupsContent.Elements())
            {
                groups.Add(
                           new HelpGroup(
                                         group.Attribute("Name").Value,
                                         group.Elements().First().ToString(),
                                         group.Attribute("Id").Value));
            }
        }

        #endregion
    }

    /// <summary>The help view model.</summary>
    [PublicAPI]
    public sealed class HelpViewModel : ObservableObject
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HelpViewModel" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="HelpViewModel" /> Klasse.
        /// </summary>
        /// <param name="helpfilePath">
        ///     The helpfile path.
        /// </param>
        public HelpViewModel([NotNull] string helpfilePath)
        {
            _groups   = new ObservableCollection<HelpGroup>();
            _database = new HelpDatabase(helpfilePath);

            Height = SystemParameters.MaximumWindowTrackHeight / 3 * 2;
            Width  = SystemParameters.MaximumWindowTrackWidth / 3 * 2;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The activate.
        /// </summary>
        /// <param name="topic">
        ///     The topic.
        /// </param>
        /// <param name="group">
        ///     The group.
        /// </param>
        public void Activate([CanBeNull] string topic, [CanBeNull] string group)
        {
            if (string.IsNullOrWhiteSpace(topic)) return;

            var htopic = HelpTopics.FirstOrDefault(top => top.Tocken == topic);
            ActiveTopic = htopic;
            if (ActiveTopic == null) return;

            if (string.IsNullOrWhiteSpace(group)) return;

            var hGroup = HelpGroups.FirstOrDefault(gr => gr.Id == group);
            ActiveGroup = hGroup;
        }

        #endregion

        #region Fields

        private readonly HelpDatabase _database;

        private readonly ObservableCollection<HelpGroup> _groups;

        private HelpGroup _activeGroup;

        private HelpTopic _activeTopic;

        private double _height;

        private double _width;

        #endregion

        #region Public Properties

        /// <summary>Gets the active group.</summary>
        [NotNull]
        public HelpGroup ActiveGroup
        {
            get => _activeGroup;

            private set
            {
                _activeGroup = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Gets the active topic.</summary>
        [NotNull]
        public HelpTopic ActiveTopic
        {
            get => _activeTopic;

            private set
            {
                _activeTopic = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Gets or sets the height.</summary>
        public double Height
        {
            get => _height;

            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Gets the help groups.</summary>
        [NotNull]
        public IEnumerable<HelpGroup> HelpGroups => _groups;

        /// <summary>Gets the help topics.</summary>
        [NotNull]
        public IEnumerable<HelpTopic> HelpTopics => _database.Topics;

        /// <summary>Gets or sets the width.</summary>
        public double Width
        {
            get => _width;

            set
            {
                _width = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        [EventTarget]
        private void NewTopic([NotNull] TopicChangedEvntArgs e)
        {
            _database.Fill(_groups, (string) e.Topic);
        }

        [EventTarget]
        private void TopicClear()
        {
            _groups.Clear();
        }

        #endregion
    }
}