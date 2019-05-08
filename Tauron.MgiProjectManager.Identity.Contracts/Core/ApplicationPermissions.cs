using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using Tauron.MgiProjectManager.Identity.Resources;

namespace Tauron.MgiProjectManager.Identity.Core
{
    [PublicAPI]
    public static class ApplicationPermissions
    {
        public static readonly ReadOnlyCollection<ApplicationPermission> AllPermissions;


        public const string UsersPermissionGroupName = nameof(IdentRes.ApplicationPermission_ViewUsers_GroupName);
        public static readonly ApplicationPermission ViewUsers = new ApplicationPermission(nameof(IdentRes.ApplicationPermissons_ViewUsers), "users.view", UsersPermissionGroupName, nameof(IdentRes.ApplicationPermissions_ViewUsers_Description));
        public static readonly ApplicationPermission ManageUsers = new ApplicationPermission(nameof(IdentRes.ApplicationPermissons_ManageUser), "users.manage", UsersPermissionGroupName, nameof(IdentRes.ApplicationPermission_ManageUser_Description));

        public const string RolesPermissionGroupName = nameof(IdentRes.ApplicationPermission_RolesPermission_GroupName);
        public static readonly ApplicationPermission ViewRoles = new ApplicationPermission(nameof(IdentRes.ApplicationPermission_ViewRoles), "roles.view", RolesPermissionGroupName, nameof(IdentRes.ApplicationPermission_ViewRoles_Description));
        public static readonly ApplicationPermission ManageRoles = new ApplicationPermission(nameof(IdentRes.ApplicationPermission_ManageRoles), "roles.manage", RolesPermissionGroupName, nameof(IdentRes.ApplicationPermission_ManageRoles_Description));
        public static readonly ApplicationPermission AssignRoles = new ApplicationPermission(nameof(IdentRes.ApplicationPermission_AssignRole), "roles.assign", RolesPermissionGroupName, nameof(IdentRes.ApplicationPermission_AssignRole_Description));

        public const string FilesPermissionGroupName = nameof(IdentRes.ApplicationPermission_Files_GroupName);
        public static readonly ApplicationPermission UploadFiles = new ApplicationPermission(nameof(IdentRes.ApplicationPermission_Files_UploadFiles), "files.upload", FilesPermissionGroupName, nameof(IdentRes.ApplicationPermission_Files_UploadFiles_Description));
        public static readonly ApplicationPermission DownloadFiles = new ApplicationPermission(nameof(IdentRes.ApplicationPermission_Files_DownloadFiles), "files.download", FilesPermissionGroupName, nameof(IdentRes.ApplicationPermission_Files_DownloadFiles_Description));

        static ApplicationPermissions()
        {
            List<ApplicationPermission> allPermissions = new List<ApplicationPermission>()
            {
                ViewUsers,
                ManageUsers,

                ViewRoles,
                ManageRoles,
                AssignRoles,

                UploadFiles,
                DownloadFiles
            };

            AllPermissions = allPermissions.AsReadOnly();
        }

        public static ApplicationPermission GetPermissionByName(string permissionName) 
            => AllPermissions.SingleOrDefault(p => p.Name == permissionName);

        public static ApplicationPermission GetPermissionByValue(string permissionValue) 
            => AllPermissions.SingleOrDefault(p => p.Value == permissionValue);

        public static string[] GetAllPermissionValues() 
            => AllPermissions.Select(p => p.Value).ToArray();

        public static string[] GetAdministrativePermissionValues() 
            => new string[] { ManageUsers, ManageRoles, AssignRoles };
    }


    [PublicAPI]
    public class ApplicationPermission
    {
        public string NameKey { get; set; }
        public string GroupNameKey { get; set; }
        public string DescriptionKey { get; set; }

        public ApplicationPermission()
        { }

        public ApplicationPermission(string name, string value, string groupName, string description)
        {
            Value = value;
            NameKey = name;
            GroupNameKey = groupName;
            DescriptionKey = description;
        }



        public string Name => IdentRes.ResourceManager.GetString(NameKey);
        public string Value { get; }
        public string GroupName => IdentRes.ResourceManager.GetString(GroupNameKey);
        public string Description => IdentRes.ResourceManager.GetString(DescriptionKey);


        public override string ToString()
        {
            return Value;
        }


        public static implicit operator string(ApplicationPermission permission)
        {
            return permission.Value;
        }
    }
}
