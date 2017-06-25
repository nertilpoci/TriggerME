using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriggerMe.Model;

namespace TriggerMe.DAL
{
    public interface IUnitOfWork
    {
         IRepositoryBase<Client> Clients { get; }
        IRepositoryBase<Connection> Connections { get; }
        IRepositoryBase<TriggerMessage> Triggers { get; }
        void SetTenancy(bool isActive);
        int Persist();
        Task<int> PersistAsync();
    }
}
