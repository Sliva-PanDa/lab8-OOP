using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using InvestigationApp.Repositories;
using InvestigationApp.Services;
using InvestigationApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace InvestigationApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IInvestigationRepository _investigationRepo;
        private readonly InvestigationService _investigationService;
        private Investigations _selectedInvestigation;
        private decimal _minRewardAmount;
        private int _selectedOffenseTypeId;
        private int _statusIndex = 0;

        public ObservableCollection<Investigations> Investigations { get; private set; }
        public ObservableCollection<Subjects> Subjects { get; private set; }
        public ObservableCollection<OffenseTypes> OffenseTypes { get; private set; }

        public int SelectedOffenseTypeId
        {
            get { return _selectedOffenseTypeId; }
            set
            {
                _selectedOffenseTypeId = value;
                OnPropertyChanged("SelectedOffenseTypeId");
            }
        }

        public Investigations SelectedInvestigation
        {
            get { return _selectedInvestigation; }
            set
            {
                _selectedInvestigation = value;
                OnPropertyChanged("SelectedInvestigation");
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string SearchText { get; set; }
        public decimal MinRewardAmount
        {
            get { return _minRewardAmount; }
            set
            {
                _minRewardAmount = value;
                OnPropertyChanged("MinRewardAmount");
            }
        }

        public Dictionary<string, int> StatusCounts { get; private set; }

        public RelayCommand LoadDataCommand { get; private set; }
        public RelayCommand AddInvestigationCommand { get; private set; }
        public RelayCommand DeleteInvestigationCommand { get; private set; }
        public RelayCommand UpdateInvestigationCommand { get; private set; }
        public RelayCommand SearchCommand { get; private set; }
        public RelayCommand FilterByRewardCommand { get; private set; }
        public RelayCommand FilterByOffenseTypeCommand { get; private set; }
        public RelayCommand ShowStatusCountsCommand { get; private set; }

        public MainViewModel()
        {
            _investigationRepo = new InvestigationRepository();
            _investigationService = new InvestigationService(_investigationRepo);

            Investigations = new ObservableCollection<Investigations>();
            Subjects = new ObservableCollection<Subjects>();
            OffenseTypes = new ObservableCollection<OffenseTypes>();
            StatusCounts = new Dictionary<string, int>();

            LoadDataCommand = new RelayCommand(LoadData);
            AddInvestigationCommand = new RelayCommand(AddInvestigation, CanAddInvestigation);
            DeleteInvestigationCommand = new RelayCommand(DeleteInvestigation, CanDeleteInvestigation);
            UpdateInvestigationCommand = new RelayCommand(UpdateInvestigation, CanUpdateInvestigation);
            SearchCommand = new RelayCommand(SearchInvestigations, CanSearch);
            FilterByRewardCommand = new RelayCommand(FilterByReward, CanFilterByReward);
            FilterByOffenseTypeCommand = new RelayCommand(FilterByOffenseType, CanFilterByOffenseType);
            ShowStatusCountsCommand = new RelayCommand(ShowStatusCounts);

            MinRewardAmount = 0m;
            SelectedOffenseTypeId = 0;
        }

        private void LoadData()
        {
            try
            {
                Investigations.Clear();
                var investigations = _investigationRepo.GetAll();
                foreach (var inv in investigations)
                {
                    string[] statuses = { "Open", "Closed", "InProgress", "Unknown" };
                    inv.Status = statuses[_statusIndex++ % 4];
                    Investigations.Add(inv);
                }
                Subjects.Clear();
                foreach (var sub in new SubjectRepository().GetAll()) Subjects.Add(sub);
                OffenseTypes.Clear();
                foreach (var off in new OffenseTypeRepository().GetAll()) OffenseTypes.Add(off);
                MessageBox.Show("Данные загружены.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        private void AddInvestigation()
        {
            try
            {
                var newInvestigation = new Investigations
                {
                    SubjectID = 1,
                    OffenseTypeID = 1,
                    OffenseDate = DateTime.Now,
                    RewardAmount = 1000.00m,
                    Description = "Новое расследование",
                    Status = "Open"
                };
                _investigationRepo.Add(newInvestigation);
                LoadData();
                MessageBox.Show("Расследование добавлено.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении: " + ex.Message);
            }
        }

        private bool CanAddInvestigation()
        {
            return true;
        }

        private void DeleteInvestigation()
        {
            if (SelectedInvestigation != null)
            {
                try
                {
                    _investigationRepo.Delete(SelectedInvestigation.InvestigationID);
                    LoadData();
                    MessageBox.Show("Расследование удалено.");
                    SelectedInvestigation = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Выберите расследование для удаления.");
            }
        }

        private bool CanDeleteInvestigation()
        {
            return SelectedInvestigation != null;
        }

        private void UpdateInvestigation()
        {
            if (SelectedInvestigation != null)
            {
                try
                {
                    _investigationRepo.Update(SelectedInvestigation);
                    LoadData();
                    MessageBox.Show("Расследование обновлено.");
                    SelectedInvestigation = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при обновлении: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Выберите расследование для обновления.");
            }
        }

        private bool CanUpdateInvestigation()
        {
            return SelectedInvestigation != null;
        }

        private void SearchInvestigations()
        {
            if (string.IsNullOrEmpty(SearchText)) return;
            try
            {
                var allInvestigations = _investigationRepo.GetAll();
                var filtered = allInvestigations
                    .Where(i => i.Description != null && i.Description.ToLower().Contains(SearchText.ToLower()))
                    .ToList();
                Investigations.Clear();
                foreach (var inv in filtered) Investigations.Add(inv);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при поиске: " + ex.Message);
            }
        }

        private bool CanSearch()
        {
            return !string.IsNullOrEmpty(SearchText);
        }

        private void FilterByReward()
        {
            try
            {
                var filtered = _investigationService.GetHighRewardInvestigations(MinRewardAmount);
                Investigations.Clear();
                foreach (var inv in filtered) Investigations.Add(inv);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при фильтрации по вознаграждению: " + ex.Message);
            }
        }

        private bool CanFilterByReward()
        {
            return MinRewardAmount >= 0;
        }

        private void FilterByOffenseType()
        {
            try
            {
                var filtered = _investigationService.GetInvestigationsByOffenseType(SelectedOffenseTypeId);
                Investigations.Clear();
                foreach (var inv in filtered) Investigations.Add(inv);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при фильтрации по типу нарушения: " + ex.Message);
            }
        }

        private bool CanFilterByOffenseType()
        {
            return SelectedOffenseTypeId > 0;
        }

        private void ShowStatusCounts()
        {
            try
            {
                StatusCounts = _investigationService.GetStatusCounts();
                string message = "Статистика статусов:\n";
                foreach (var pair in StatusCounts)
                {
                    message += pair.Key + ": " + pair.Value + "\n";
                }
                MessageBox.Show(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при отображении статистики: " + ex.Message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}