using System;
using System.Collections.Generic;
using Tauron.Application.ProjectManager.Resources;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.AdminClient.Views
{
    public class ServerUser : ObservableObject
    {
        private readonly Action<UserRights, ServerUser> _userRightsChanged;
        private          ServerUserRight    _currentRight;
        private bool _block;
        private ServerUserRight _old;

        public string User { get; }

        public bool IsOk { get; }

        public List<ServerUserRight> ServerUserRights { get; set; }

        public ServerUserRight CurrentRight
        {
            get => _currentRight;
            set
            {
                _old = _currentRight;
                SetProperty(ref _currentRight, value, UserRightsChanged);
            }
        }

        private void UserRightsChanged()
        {
            if(_block) return;;
            _userRightsChanged(CurrentRight.Right, this);
        }

        public ServerUser()
            : this("Test", UserRights.Admin, (r, u) => { })
        {
            
        }

        public ServerUser(string user, UserRights rights, Action<UserRights, ServerUser> userRightsChanged)
        {
            _userRightsChanged = userRightsChanged;
            User               = user;

            if (rights != UserRights.Error)
            {
                ServerUserRights = ServerUserRight.ServerUserRights;
                _currentRight    = ServerUserRights.Find(r => r.Right == rights);
                IsOk = true;
            }
            else
            {
                ServerUserRights = new List<ServerUserRight> { new ServerUserRight(UserRights.Error, AdminClientLabels.Text_UserRighs_Error) };
                _currentRight = ServerUserRights[0];
            }
        }

        public void Revert()
        {
            _block = true;
            CurrentRight = _old;
            _block = false;
        }
    }
}