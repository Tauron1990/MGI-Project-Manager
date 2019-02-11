using System.Collections.Generic;
using Tauron.Application.ProjectManager.Resources;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.AdminClient.Views
{
    public class ServerUserRight
    {
        public static readonly List<ServerUserRight> ServerUserRights = Create();

        private static List<ServerUserRight> Create()
        {
            List<ServerUserRight> rights = new List<ServerUserRight>
                                           {
                                               //new ServerUserRight(UserRights.Admin, AdminClientLabels.Text_UserRighs_Admin),
                                               new ServerUserRight(UserRights.Manager, AdminClientLabels.Text_UserRighs_Manager),
                                               new ServerUserRight(UserRights.Operater, AdminClientLabels.Text_UserRighs_Operator)
                                           };

            return rights;
        }

        public UserRights Right  { get; }
        public string     UIName { get; }

        public ServerUserRight(UserRights right, string uiName)
        {
            Right  = right;
            UIName = uiName;
        }
    }
}