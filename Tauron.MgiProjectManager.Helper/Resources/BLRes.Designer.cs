﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tauron.MgiProjectManager.Resources {
    using System;
    
    
    /// <summary>
    ///   Eine stark typisierte Ressourcenklasse zum Suchen von lokalisierten Zeichenfolgen usw.
    /// </summary>
    // Diese Klasse wurde von der StronglyTypedResourceBuilder automatisch generiert
    // -Klasse über ein Tool wie ResGen oder Visual Studio automatisch generiert.
    // Um einen Member hinzuzufügen oder zu entfernen, bearbeiten Sie die .ResX-Datei und führen dann ResGen
    // mit der /str-Option erneut aus, oder Sie erstellen Ihr VS-Projekt neu.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class BLRes {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal BLRes() {
        }
        
        /// <summary>
        ///   Gibt die zwischengespeicherte ResourceManager-Instanz zurück, die von dieser Klasse verwendet wird.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Tauron.MgiProjectManager.Resources.BLRes", typeof(BLRes).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Überschreibt die CurrentUICulture-Eigenschaft des aktuellen Threads für alle
        ///   Ressourcenzuordnungen, die diese stark typisierte Ressourcenklasse verwenden.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die This file type is not supported. {0} ähnelt.
        /// </summary>
        public static string Api_FilesController_DisallowedExtension {
            get {
                return ResourceManager.GetString("Api_FilesController_DisallowedExtension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die The operation was not found. ähnelt.
        /// </summary>
        public static string Api_FilesController_NoOperationFound {
            get {
                return ResourceManager.GetString("Api_FilesController_NoOperationFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Operation is not compatible. ähnelt.
        /// </summary>
        public static string Api_FilesController_NotCompatible {
            get {
                return ResourceManager.GetString("Api_FilesController_NotCompatible", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die {0} file(s) / {1} bytes uploaded successfully! ähnelt.
        /// </summary>
        public static string Api_FilesController_SucessMessage {
            get {
                return ResourceManager.GetString("Api_FilesController_SucessMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die The job name do not match the requirements. ähnelt.
        /// </summary>
        public static string LinkingFileOperation_Incompatible_JobName {
            get {
                return ResourceManager.GetString("LinkingFileOperation_Incompatible_JobName", resourceCulture);
            }
        }
    }
}
