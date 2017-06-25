using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TriggerMe.Model
{
   public class Client:ModelBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid Identifier { get; set; }
        public bool IsOnline { get; set; }

        public virtual HashSet<TriggerMessage> TriggerMessages { get; set; } = new HashSet<TriggerMessage>();
        public virtual HashSet<Connection> Connections { get; set; } = new HashSet<Connection>();
       

    }
}
