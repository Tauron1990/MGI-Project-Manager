using System;

namespace Tauron.MgiManager.User.Shared.Dtos
{
    public class UserClaim
    {
        public string Data { get; set; }

        public int Id { get; set; }

        public UserClaim()
        {
            
        }

        public UserClaim(string data, int id)
        {
            Data = data;
            Id = id;
        }
    }
}