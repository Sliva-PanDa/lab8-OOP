using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestigationApp.Models;

namespace InvestigationApp.Repositories
{
  public interface IInvestigationRepository
    {
        void Add(Investigations investigation);
        void Update(Investigations investigation);
        void Delete(int investigationId);
        List<Investigations> GetAll();
    }
}
