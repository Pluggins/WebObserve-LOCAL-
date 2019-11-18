using Observer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Observer.Models
{
    public class PlayersModel : _CommonAttribute
    {
        public string Id { get; set; }
        public UsersModel User { get; set; }
        public string IGN { get; set; }
        public int Status { get; set; }
        public string PIN { get; set; }

        public PlayersModel()
        {
            Id = Guid.NewGuid().ToString();
            Status = 0;
            PIN = EncryptionAlgorithmServiceManager.GetRandomDigit(6);
        }
    }
}