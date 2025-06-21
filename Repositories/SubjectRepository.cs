using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestigationApp.Data;
using InvestigationApp.Models;

namespace InvestigationApp.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly CustomInvestigationDataContext _dataContext;

        public SubjectRepository()
        {
            _dataContext = new CustomInvestigationDataContext();
        }

        public void Add(Subjects subject)
        {
            _dataContext.Subjects.InsertOnSubmit(new Subjects
            {
                LastName = subject.LastName,
                FirstName = subject.FirstName,
                Notes = subject.Notes
            });
            _dataContext.SubmitChanges();
        }

        public void Update(Subjects subject)
        {
            var existing = _dataContext.Subjects.FirstOrDefault(s => s.SubjectID == subject.SubjectID);
            if (existing != null)
            {
                existing.LastName = subject.LastName;
                existing.FirstName = subject.FirstName;
                existing.Notes = subject.Notes;
                _dataContext.SubmitChanges();
            }
        }

        public void Delete(Subjects subject)
        {
            var existing = _dataContext.Subjects.FirstOrDefault(s => s.SubjectID == subject.SubjectID);
            if (existing != null)
            {
                _dataContext.Subjects.DeleteOnSubmit(existing);
                _dataContext.SubmitChanges();
            }
        }

        public List<Subjects> GetAll()
        {
            return _dataContext.Subjects.ToList();
        }
    }
}