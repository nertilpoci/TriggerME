using System;
using System.Collections.Generic;
using System.Text;

namespace TriggerMe.Model
{
 public   class TriggerMessage:ModelBase
    {
        public string Name { get;set;}
        public string Description { get; set; }
        public string Secret { get; set; }
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
    }
}
