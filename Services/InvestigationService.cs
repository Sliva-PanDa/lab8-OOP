using System;
using System.Collections.Generic;
using System.Linq;
using InvestigationApp.Models;
using InvestigationApp.Repositories;

namespace InvestigationApp.Services
{
    public class InvestigationService
    {
        private readonly IInvestigationRepository _repository;

        public InvestigationService(IInvestigationRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            _repository = repository;
        }

        public List<Investigations> GetHighRewardInvestigations(decimal minAmount)
        {
            IEnumerable<Investigations> inv = _repository.GetAll().Where(i => i.RewardAmount >= minAmount);

            return inv.ToList();
        }

        public Dictionary<string, int> GetStatusCounts()
        {
            var counts = new Dictionary<string, int>();
            var investigations = _repository.GetAll();
            foreach (var inv in investigations)
            {
                string status = inv.Status ?? "Unknown";
                if (counts.ContainsKey(status))
                    counts[status]++;
                else
                    counts[status] = 1;
            }
            return counts;
        }

        public List<Investigations> GetInvestigationsByOffenseType(int offenseTypeId)
        {
            return _repository.GetAll().Where(i => i.OffenseTypeID == offenseTypeId).ToList();
        }
    }
}