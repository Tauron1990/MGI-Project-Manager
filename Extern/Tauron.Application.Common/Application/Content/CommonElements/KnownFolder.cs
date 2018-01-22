#region

using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The known folder.</summary>
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "OEM")]
    [PublicAPI]
    public static class KnownFolder
    {
        #region Static Fields

        /// <summary>The add new programs.</summary>
        public static readonly Guid AddNewPrograms = new Guid("de61d971-5ebc-4f02-a3a9-6c82895e5c04");

        /// <summary>The admin tools.</summary>
        public static readonly Guid AdminTools = new Guid("724EF170-A42D-4FEF-9F26-B60E846FBA4F");

        /// <summary>The app updates.</summary>
        public static readonly Guid AppUpdates = new Guid("a305ce99-f527-492b-8b1a-7e76fa98d6e4");

        /// <summary>The cd burning.</summary>
        public static readonly Guid CDBurning = new Guid("9E52AB10-F80D-49DF-ACB8-4330F5687855");

        /// <summary>The change remove programs.</summary>
        public static readonly Guid ChangeRemovePrograms = new Guid("df7266ac-9274-4867-8d55-3bd661de872d");

        /// <summary>The common admin tools.</summary>
        public static readonly Guid CommonAdminTools = new Guid("D0384E7D-BAC3-4797-8F14-CBA229B392B5");

        /// <summary>The common oem links.</summary>
        public static readonly Guid CommonOemLinks = new Guid("C1BAE2D0-10DF-4334-BEDD-7AA20B227A9D");

        /// <summary>The common programs.</summary>
        public static readonly Guid CommonPrograms = new Guid("0139D44E-6AFE-49F2-8690-3DAFCAE6FFB8");

        /// <summary>The common start menu.</summary>
        public static readonly Guid CommonStartMenu = new Guid("A4115719-D62E-491D-AA7C-E74B8BE3B067");

        /// <summary>The common startup.</summary>
        public static readonly Guid CommonStartup = new Guid("82A5EA35-D9CD-47C5-9629-E15D2F714E6E");

        /// <summary>The common templates.</summary>
        public static readonly Guid CommonTemplates = new Guid("B94237E7-57AC-4347-9151-B08C6C32D1F7");

        /// <summary>The computer folder.</summary>
        public static readonly Guid ComputerFolder = new Guid("0AC0837C-BBF8-452A-850D-79D08E667CA7");

        /// <summary>The conflict folder.</summary>
        public static readonly Guid ConflictFolder = new Guid("4bfefb45-347d-4006-a5be-ac0cb0567192");

        /// <summary>The connections folder.</summary>
        public static readonly Guid ConnectionsFolder = new Guid("6F0CD92B-2E97-45D1-88FF-B0D186B8DEDD");

        /// <summary>The contacts.</summary>
        public static readonly Guid Contacts = new Guid("56784854-C6CB-462b-8169-88E350ACB882");

        /// <summary>The control panel folder.</summary>
        public static readonly Guid ControlPanelFolder = new Guid("82A74AEB-AEB4-465C-A014-D097EE346D63");

        /// <summary>The cookies.</summary>
        public static readonly Guid Cookies = new Guid("2B0F765D-C0E9-4171-908E-08A611B84FF6");

        /// <summary>The desktop.</summary>
        public static readonly Guid Desktop = new Guid("B4BFCC3A-DB2C-424C-B029-7FE99A87C641");

        /// <summary>The documents.</summary>
        public static readonly Guid Documents = new Guid("FDD39AD0-238F-46AF-ADB4-6C85480369C7");

        /// <summary>The downloads.</summary>
        public static readonly Guid Downloads = new Guid("374DE290-123F-4565-9164-39C4925E467B");

        /// <summary>The favorites.</summary>
        public static readonly Guid Favorites = new Guid("1777F761-68AD-4D8A-87BD-30B759FA33DD");

        /// <summary>The fonts.</summary>
        public static readonly Guid Fonts = new Guid("FD228CB7-AE11-4AE3-864C-16F3910AB8FE");

        /// <summary>The game tasks.</summary>
        public static readonly Guid GameTasks = new Guid("054FAE61-4DD8-4787-80B6-090220C4B700");

        /// <summary>The games.</summary>
        public static readonly Guid Games = new Guid("CAC52C1A-B53D-4edc-92D7-6B2E8AC19434");

        /// <summary>The history.</summary>
        public static readonly Guid History = new Guid("D9DC8A3B-B784-432E-A781-5A1130A75963");

        /// <summary>The internet cache.</summary>
        public static readonly Guid InternetCache = new Guid("352481E8-33BE-4251-BA85-6007CAEDCF9D");

        /// <summary>The internet folder.</summary>
        public static readonly Guid InternetFolder = new Guid("4D9F7874-4E0C-4904-967B-40B0D20C3E4B");

        /// <summary>The links.</summary>
        public static readonly Guid Links = new Guid("bfb9d5e0-c6a9-404c-b2b2-ae6db6af4968");

        /// <summary>The local app data.</summary>
        public static readonly Guid LocalAppData = new Guid("F1B32785-6FBA-4FCF-9D55-7B8E7F157091");

        /// <summary>The local app data low.</summary>
        public static readonly Guid LocalAppDataLow = new Guid("A520A1A4-1780-4FF6-BD18-167343C5AF16");

        /// <summary>The localized resources dir.</summary>
        public static readonly Guid LocalizedResourcesDir = new Guid("2A00375E-224C-49DE-B8D1-440DF7EF3DDC");

        /// <summary>The music.</summary>
        public static readonly Guid Music = new Guid("4BD8D571-6D19-48D3-BE97-422220080E43");

        /// <summary>The net hood.</summary>
        public static readonly Guid NetHood = new Guid("C5ABBF53-E17F-4121-8900-86626FC2C973");

        /// <summary>The network folder.</summary>
        public static readonly Guid NetworkFolder = new Guid("D20BEEC4-5CA8-4905-AE3B-BF251EA09B53");

        /// <summary>The original images.</summary>
        public static readonly Guid OriginalImages = new Guid("2C36C0AA-5812-4b87-BFD0-4CD0DFB19B39");

        /// <summary>The photo albums.</summary>
        public static readonly Guid PhotoAlbums = new Guid("69D2CF90-FC33-4FB7-9A0C-EBB0F0FCB43C");

        /// <summary>The pictures.</summary>
        public static readonly Guid Pictures = new Guid("33E28130-4E1E-4676-835A-98395C3BC3BB");

        /// <summary>The playlists.</summary>
        public static readonly Guid Playlists = new Guid("DE92C1C7-837F-4F69-A3BB-86E631204A23");

        /// <summary>The print hood.</summary>
        public static readonly Guid PrintHood = new Guid("9274BD8D-CFD1-41C3-B35E-B13F55A758F4");

        /// <summary>The printers folder.</summary>
        public static readonly Guid PrintersFolder = new Guid("76FC4E2D-D6AD-4519-A663-37BD56068185");

        /// <summary>The profile.</summary>
        public static readonly Guid Profile = new Guid("5E6C858F-0E22-4760-9AFE-EA3317B67173");

        /// <summary>The program data.</summary>
        public static readonly Guid ProgramData = new Guid("62AB5D82-FDC1-4DC3-A9DD-070D1D495D97");

        /// <summary>The program files.</summary>
        public static readonly Guid ProgramFiles = new Guid("905e63b6-c1bf-494e-b29c-65b732d3d21a");

        /// <summary>The program files common.</summary>
        public static readonly Guid ProgramFilesCommon = new Guid("F7F1ED05-9F6D-47A2-AAAE-29D317C6F066");

        /// <summary>The program files common x 64.</summary>
        public static readonly Guid ProgramFilesCommonX64 = new Guid("6365D5A7-0F0D-45E5-87F6-0DA56B6A4F7D");

        /// <summary>The program files common x 86.</summary>
        public static readonly Guid ProgramFilesCommonX86 = new Guid("DE974D24-D9C6-4D3E-BF91-F4455120B917");

        /// <summary>The program files x 64.</summary>
        public static readonly Guid ProgramFilesX64 = new Guid("6D809377-6AF0-444b-8957-A3773F02200E");

        /// <summary>The program files x 86.</summary>
        public static readonly Guid ProgramFilesX86 = new Guid("7C5A40EF-A0FB-4BFC-874A-C0F2E0B9FA8E");

        /// <summary>The programs.</summary>
        public static readonly Guid Programs = new Guid("A77F5D77-2E2B-44C3-A6A2-ABA601054A51");

        /// <summary>The public.</summary>
        public static readonly Guid Public = new Guid("DFDF76A2-C82A-4D63-906A-5644AC457385");

        /// <summary>The public desktop.</summary>
        public static readonly Guid PublicDesktop = new Guid("C4AA340D-F20F-4863-AFEF-F87EF2E6BA25");

        /// <summary>The public documents.</summary>
        public static readonly Guid PublicDocuments = new Guid("ED4824AF-DCE4-45A8-81E2-FC7965083634");

        /// <summary>The public downloads.</summary>
        public static readonly Guid PublicDownloads = new Guid("3D644C9B-1FB8-4f30-9B45-F670235F79C0");

        /// <summary>The public game tasks.</summary>
        public static readonly Guid PublicGameTasks = new Guid("DEBF2536-E1A8-4c59-B6A2-414586476AEA");

        /// <summary>The public music.</summary>
        public static readonly Guid PublicMusic = new Guid("3214FAB5-9757-4298-BB61-92A9DEAA44FF");

        /// <summary>The public pictures.</summary>
        public static readonly Guid PublicPictures = new Guid("B6EBFB86-6907-413C-9AF7-4FC2ABF07CC5");

        /// <summary>The public videos.</summary>
        public static readonly Guid PublicVideos = new Guid("2400183A-6185-49FB-A2D8-4A392A602BA3");

        /// <summary>The quick launch.</summary>
        public static readonly Guid QuickLaunch = new Guid("52a4f021-7b75-48a9-9f6b-4b87a210bc8f");

        /// <summary>The recent.</summary>
        public static readonly Guid Recent = new Guid("AE50C081-EBD2-438A-8655-8A092E34987A");

        /// <summary>The recycle bin folder.</summary>
        public static readonly Guid RecycleBinFolder = new Guid("B7534046-3ECB-4C18-BE4E-64CD4CB7D6AC");

        /// <summary>The resource dir.</summary>
        public static readonly Guid ResourceDir = new Guid("8AD10C31-2ADB-4296-A8F7-E4701232C972");

        /// <summary>The roaming app data.</summary>
        public static readonly Guid RoamingAppData = new Guid("3EB685DB-65F9-4CF6-A03A-E3EF65729F3D");

        /// <summary>The sample music.</summary>
        public static readonly Guid SampleMusic = new Guid("B250C668-F57D-4EE1-A63C-290EE7D1AA1F");

        /// <summary>The sample pictures.</summary>
        public static readonly Guid SamplePictures = new Guid("C4900540-2379-4C75-844B-64E6FAF8716B");

        /// <summary>The sample playlists.</summary>
        public static readonly Guid SamplePlaylists = new Guid("15CA69B3-30EE-49C1-ACE1-6B5EC372AFB5");

        /// <summary>The sample videos.</summary>
        public static readonly Guid SampleVideos = new Guid("859EAD94-2E85-48AD-A71A-0969CB56A6CD");

        /// <summary>The saved games.</summary>
        public static readonly Guid SavedGames = new Guid("4C5C32FF-BB9D-43b0-B5B4-2D72E54EAAA4");

        /// <summary>The saved searches.</summary>
        public static readonly Guid SavedSearches = new Guid("7d1d3a04-debb-4115-95cf-2f29da2920da");

        /// <summary>The search csc.</summary>
        public static readonly Guid SearchCsc = new Guid("ee32e446-31ca-4aba-814f-a5ebd2fd6d5e");

        /// <summary>The search home.</summary>
        public static readonly Guid SearchHome = new Guid("190337d1-b8ca-4121-a639-6d472d16972a");

        /// <summary>The search mapi.</summary>
        public static readonly Guid SearchMapi = new Guid("98ec0e18-2098-4d44-8644-66979315a281");

        /// <summary>The send to.</summary>
        public static readonly Guid SendTo = new Guid("8983036C-27C0-404B-8F08-102D10DCFD74");

        /// <summary>The sidebar default parts.</summary>
        public static readonly Guid SidebarDefaultParts = new Guid("7B396E54-9EC5-4300-BE0A-2482EBAE1A26");

        /// <summary>The sidebar parts.</summary>
        public static readonly Guid SidebarParts = new Guid("A75D362E-50FC-4fb7-AC2C-A8BEAA314493");

        /// <summary>The start menu.</summary>
        public static readonly Guid StartMenu = new Guid("625B53C3-AB48-4EC1-BA1F-A1EF4146FC19");

        /// <summary>The startup.</summary>
        public static readonly Guid Startup = new Guid("B97D20BB-F46A-4C97-BA10-5E3608430854");

        /// <summary>The sync manager folder.</summary>
        public static readonly Guid SyncManagerFolder = new Guid("43668BF8-C14E-49B2-97C9-747784D784B7");

        /// <summary>The sync results folder.</summary>
        public static readonly Guid SyncResultsFolder = new Guid("289a9a43-be44-4057-a41b-587a76d7e7f9");

        /// <summary>The sync setup folder.</summary>
        public static readonly Guid SyncSetupFolder = new Guid("0F214138-B1D3-4a90-BBA9-27CBC0C5389A");

        /// <summary>The system.</summary>
        public static readonly Guid System = new Guid("1AC14E77-02E7-4E5D-B744-2EB1AE5198B7");

        /// <summary>The system x 86.</summary>
        public static readonly Guid SystemX86 = new Guid("D65231B0-B2F1-4857-A4CE-A8E7C6EA7D27");

        /// <summary>The templates.</summary>
        public static readonly Guid Templates = new Guid("A63293E8-664E-48DB-A079-DF759E0509F7");

        /// <summary>The tree properties.</summary>
        public static readonly Guid TreeProperties = new Guid("5b3749ad-b49f-49c1-83eb-15370fbd4882");

        /// <summary>The user profiles.</summary>
        public static readonly Guid UserProfiles = new Guid("0762D272-C50A-4BB0-A382-697DCD729B80");

        /// <summary>The users files.</summary>
        public static readonly Guid UsersFiles = new Guid("f3ce0f7c-4901-4acc-8648-d5d44b04ef8f");

        /// <summary>The videos.</summary>
        public static readonly Guid Videos = new Guid("18989B1D-99B5-455B-841C-AB7C74E4DDFC");

        /// <summary>The windows.</summary>
        public static readonly Guid Windows = new Guid("F38BF404-1D43-42F2-9305-67DE0B28FC23");

        #endregion
    }
}