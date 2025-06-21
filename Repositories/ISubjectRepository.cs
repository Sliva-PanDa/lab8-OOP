using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestigationApp.Models;

namespace InvestigationApp.Repositories
{
    public interface ISubjectRepository
    {
        void Add(Subjects subject);
        void Update(Subjects subject);
        void Delete(Subjects subject);
        List<Subjects> GetAll();
    }
}