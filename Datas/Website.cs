using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTraffic_Exchanger.Datas
{
    class Website
    {

        public Website(JToken JSONSTRING)
        {
            this.id = int.Parse(JSONSTRING.SelectToken("id").ToString());
            this.user_id = int.Parse(JSONSTRING.SelectToken("user_id").ToString());
            this.url = JSONSTRING.SelectToken("url").ToString();
            this.credits = int.Parse(JSONSTRING.SelectToken("credits").ToString());
            this.duration = int.Parse(JSONSTRING.SelectToken("duration").ToString());
            this.haslimit = int.Parse(JSONSTRING.SelectToken("haslimit").ToString());
            this.totalhits = int.Parse(JSONSTRING.SelectToken("totalhits").ToString());
            this.hits = int.Parse(JSONSTRING.SelectToken("hits").ToString());
            this.status = int.Parse(JSONSTRING.SelectToken("status").ToString());
        }

        public int id { get; set; }
        public int user_id { get; set; }
        public String url { get; set; }
        public int credits { get; set; }
        public int duration { get; set; }
        public int haslimit { get; set; }
        public int totalhits { get; set; }
        public int hits { get; set; }
        public int status { get; set; }


    }
}
