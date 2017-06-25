using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriggerMe.Model;
using TriggerMe.Models;
using WebApplicationBasic.Data;
using Z.EntityFramework.Plus;

namespace TriggerMe.DAL
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationDbContext _context;
        // use repository base for now since we don't have custom logic 
        public IRepositoryBase<Client> Clients { get; set; }
        public IRepositoryBase<Connection> Connections { get; set; }
        public IRepositoryBase<TriggerMessage> Triggers { get; set; }
        private AppTenant tenant;
        private bool _tenancyActive = true;
        public UnitOfWork(ApplicationDbContext context,AppTenant tenant)
        {
           
            this.tenant = tenant;
            _context = context;

            Initialize();
        }
        private void Initialize()
        {
            Clients = new RepositoryBase<Client>(_context, tenant,_tenancyActive);
            Connections = new RepositoryBase<Connection>(_context, tenant, _tenancyActive);
            Triggers = new RepositoryBase<TriggerMessage>(_context, tenant, _tenancyActive);
        }
        
        public void SetTenancy(bool isActive)
        {
            _tenancyActive = isActive;
            Initialize();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public int Persist()
        {
            return _context.SaveChanges();
        }

        public async Task<int> PersistAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
