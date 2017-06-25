using System;
using System.Collections.Generic;
using System.Text;

namespace TriggerMe.Model
{
    public class Connection:ModelBase
    {
     
        public string ConnectionID { get; set; }
        public string UserAgent { get; set; }
        public bool Connected { get; set; }
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
    }
}
