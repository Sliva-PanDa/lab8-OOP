using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestigationApp.Data;
using InvestigationApp.Models;

namespace InvestigationApp.Repositories
{
    public class OffenseTypeRepository : IOffenseTypeRepository
    {
        private readonly CustomInvestigationDataContext _dataContext;

        public OffenseTypeRepository()
        {
            _dataContext = new CustomInvestigationDataContext();
        }

        public void Add(OffenseTypes offenseType)
        {
            _dataContext.OffenseTypes.InsertOnSubmit(new OffenseTypes { Name = offenseType.Name });
            _dataContext.SubmitChanges();
        }

        public void Update(OffenseTypes offenseType)
        {
            var existing = _dataContext.OffenseTypes.FirstOrDefault(o => o.OffenseTypeID == offenseType.OffenseTypeID);
            if (existing != null)
            {
                existing.Name = offenseType.Name;
                _dataContext.SubmitChanges();
            }
        }

        public void Delete(OffenseTypes offenseType)
        {
            var existing = _dataContext.OffenseTypes.FirstOrDefault(o => o.OffenseTypeID == offenseType.OffenseTypeID);
            if (existing != null)
            {
                _dataContext.OffenseTypes.DeleteOnSubmit(existing);
                _dataContext.SubmitChanges();
            }
        }

        public List<OffenseTypes> GetAll()
        {
            return _dataContext.OffenseTypes.ToList();
        }
    }
}