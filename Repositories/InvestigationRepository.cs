using System.Collections.Generic;
using System.Linq;
using InvestigationApp.Data;
using InvestigationApp.Models;

namespace InvestigationApp.Repositories
{
    public class InvestigationRepository : IInvestigationRepository
    {
        private readonly CustomInvestigationDataContext _dataContext;

        public InvestigationRepository()
        {
            _dataContext = new CustomInvestigationDataContext();
        }

        public void Add(Investigations investigation)
        {
            _dataContext.Investigations.InsertOnSubmit(new Investigations
            {
                SubjectID = investigation.SubjectID,
                OffenseTypeID = investigation.OffenseTypeID,
                OffenseDate = investigation.OffenseDate,
                RewardAmount = investigation.RewardAmount,
                Description = investigation.Description,
                Status = investigation.Status
            });
            _dataContext.SubmitChanges();
        }

        public void Update(Investigations investigation)
        {
            var existing = _dataContext.Investigations.FirstOrDefault(i => i.InvestigationID == investigation.InvestigationID);
            if (existing != null)
            {
                existing.SubjectID = investigation.SubjectID;
                existing.OffenseTypeID = investigation.OffenseTypeID;
                existing.OffenseDate = investigation.OffenseDate;
                existing.RewardAmount = investigation.RewardAmount;
                existing.Description = investigation.Description;
                existing.Status = investigation.Status;
                _dataContext.SubmitChanges();
            }
        }

        public void Delete(int investigationId)
        {
            var investigation = _dataContext.Investigations.FirstOrDefault(i => i.InvestigationID == investigationId);
            if (investigation != null)
            {
                _dataContext.Investigations.DeleteOnSubmit(investigation);
                _dataContext.SubmitChanges();
            }
        }

        public List<Investigations> GetAll()
        {
            return _dataContext.Investigations.ToList();
        }
    }
}