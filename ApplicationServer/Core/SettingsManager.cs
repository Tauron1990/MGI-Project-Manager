using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.ProjectManager.ApplicationServer.Data;
using Tauron.Application.ProjectManager.ApplicationServer.Data.PrivateEntitys;

namespace Tauron.Application.ProjectManager.ApplicationServer.Core
{
    [PublicAPI]
    public class SettingsManager
    {
        private static readonly Dictionary<string, (OptionEntity Entity, object Value)> Settings = Initialize();

        private static Dictionary<string, (OptionEntity Entity, object Value)> Initialize()
        {
            var dic = new Dictionary<string, (OptionEntity Entity, object Value)>();

            using (var db = new DatabaseImpl())
                foreach (var optionEntity in db.Options)
                    dic.Add(optionEntity.Id, (optionEntity, null));

            return dic;
        }

        public static readonly SettingsManager Default = new SettingsManager();

        private static TType GetValue<TType>(string name, TType defaultValue)
        {
            if (!Settings.TryGetValue(name, out var option))
                return defaultValue;

            if (string.IsNullOrWhiteSpace(option.Entity.Content))
                return defaultValue;

            object value = option.Value;
            if (value != null) return (TType) value;

            value = Convert.ChangeType(option.Entity.Content, typeof(TType));
            Settings[name] = (option.Entity, value);

            return (TType) value;
        }

        private static void SetValue(string name, object value)
        {
            if (Settings.TryGetValue(name, out var option))
            {
                if (option.Value == value)
                    return;

                using (var db = new DatabaseImpl())
                {
                    var entity = option.Entity;
                    entity.Content = value.ToString();

                    db.Update(entity);
                    db.SaveChanges();

                    Settings[name] = (entity, value);
                }
            }
            else
            {
                OptionEntity entity = new OptionEntity { Content = value.ToString()};

                using (var db = new DatabaseImpl())
                {
                    db.Add(entity);
                    db.SaveChanges();
                }

                Settings[name] = (entity, value);
            }
        }

        public int IterationTime
        {
            get => GetValue("IterationTime", 3);
            set => SetValue("IterationTime", value);
        }

        public int SetupTime
        {
            get => GetValue("SetupTime", 20);
            set => SetValue("SetupTime", value);
        }

        public double PefectDifference
        {
            get => GetValue("PefectDifference", 0.01d);
            set => SetValue("PefectDifference", value);
        }

        public double NearCornerDifference
        {
            get => GetValue("NearCornerDifference", 0.3d);
            set => SetValue("NearCornerDifference", value);
        }

        public long EntityExpire
        {
            get => GetValue("EntityExpire", 1576800000000000L);
            set => SetValue("EntityExpire", value);
        }

        public int AmoutMismatch
        {
            get => GetValue("AmoutMismatch", 2000);
            set => SetValue("AmoutMismatch", value);
        }
    }
}