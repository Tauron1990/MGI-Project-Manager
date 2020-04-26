using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Akka.Copy.App.Helper;
using JetBrains.Annotations;

namespace Akka.Copy.App.Core
{
    [Serializable]
    public sealed class CopySettings : INotifyPropertyChanged
    {
        private string _base;
        private string _target;
        private string _entrysText;

        public string Base
        {
            get => _base;
            set
            {
                if (value == _base) return;
                _base = value;
                OnPropertyChanged();
            }
        }

        public string Target
        {
            get => _target;
            set
            {
                if (value == _target) return;
                _target = value;
                OnPropertyChanged();
            }
        }

        public string EntrysText
        {
            get => _entrysText;
            set
            {
                if (value == _entrysText) return;
                _entrysText = value;
                OnPropertyChanged();
                ParseLines();
            }
        }

        public UiObservableCollection<string> Entrys { get; } = new UiObservableCollection<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        public CopySettings() => Entrys.CollectionChanged += EntrysOnCollectionChanged;

        private void EntrysOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var entry in Entrys)
                builder.AppendLine(entry);

            EntrysText = builder.ToString();
        }

        private void ParseLines()
        {
            using (Entrys.BlockChangedMessages())
            {
                Entrys.Clear();

                Entrys.AddRange(EntrysText.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()));
            }
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void Load()
        {
            try
            {
                using var stream = new FileStream(GetFilePath(), FileMode.Open);
                var obj = (SaveObject) Formatter.Deserialize(stream);

                Entrys.Clear();
                Entrys.AddRange(obj.Entrys);
                Base = obj.Base;
                Target = obj.Target;
            }
            catch (Exception)
            {
                Target = string.Empty;
                Base = string.Empty;
                Entrys.Clear();
            }
        }

        public void Save()
        {
            using var stream = new FileStream(GetFilePath(), FileMode.Create);
            Formatter.Serialize(stream, new SaveObject
                                        {
                                            Target =  Target,
                                            Base =  Base,
                                            Entrys = new List<string>(Entrys)
                                        });
        }


        private static string GetFilePath() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, "last.json");
        private static readonly BinaryFormatter Formatter = new BinaryFormatter();


        [Serializable]
        private class SaveObject
        {
            public string Target { get; set; }
            public string Base { get; set; }

            public List<string> Entrys { get; set; }
        }
    }
}