using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace WebTraffic_Exchanger.Datas
{
    class User
    {
        public User(JToken JSONSTRING)
        {
            this.id = int.Parse(JSONSTRING.SelectToken("id").ToString());
            this.name = JSONSTRING.SelectToken("name").ToString();
            this.email = JSONSTRING.SelectToken("email").ToString();
            this.username = JSONSTRING.SelectToken("username").ToString();
            this.credits = float.Parse(JSONSTRING.SelectToken("credits").ToString());
            this.slots = int.Parse(JSONSTRING.SelectToken("slots").ToString());

            this.userlevel = int.Parse(JSONSTRING.SelectToken("userlevel").ToString());
        }

        public int id { get; set; }
        public String name { get; set; }
        public String email { get; set; }
        public String username { get; set; }
        public float credits { get; set; }
        public int slots { get; set; }
    
        public int userlevel { get; set; }

    }
}
