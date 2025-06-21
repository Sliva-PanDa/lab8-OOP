using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestigationApp.Models;

namespace InvestigationApp.Repositories
{
    public interface IOffenseTypeRepository
    {
        void Add(OffenseTypes offenseType);
        void Update(OffenseTypes offenseType);
        void Delete(OffenseTypes offenseType);
        List<OffenseTypes> GetAll();
    }
}