using DatabaseControllerProvider;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormPortal.Core.Services
{
    public class FormEntryService : IModelService<FormEntry, int>
    {
        public Task CreateAsync(FormEntry input, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(FormEntry input, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public Task<FormEntry?> GetAsync(int identifier, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(FormEntry input, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(FormEntry input, FormEntry oldInputToCompare, IDbController dbController)
        {
            throw new NotImplementedException();
        }
    }
}
