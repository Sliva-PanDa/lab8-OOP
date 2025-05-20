using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using lab6.Models;
using lab6.Repositories;
using System.Data.SqlClient;

namespace lab6
{
    public partial class MainWindow : Window
    {
        private readonly OffenseTypeRepository _offenseTypeRepository;
        private ObservableCollection<OffenseType> _offenseTypesList;
        private OffenseType _selectedOffenseType;

        private readonly SubjectRepository _subjectRepository;
        private ObservableCollection<Subject> _subjectsList;
        private Subject _selectedSubject;

        private readonly InvestigationRepository _investigationRepository;
        private ObservableCollection<Investigation> _investigationsList;
        private Investigation _selectedInvestigation;

        public MainWindow()
        {
            InitializeComponent();

            _offenseTypeRepository = new OffenseTypeRepository();
            _offenseTypesList = new ObservableCollection<OffenseType>();
            if (dgOffenseTypes != null) dgOffenseTypes.ItemsSource = _offenseTypesList;
            LoadOffenseTypes();

            _subjectRepository = new SubjectRepository();
            _subjectsList = new ObservableCollection<Subject>();
            if (dgSubjects != null) dgSubjects.ItemsSource = _subjectsList;
            LoadSubjects();

            _investigationRepository = new InvestigationRepository();
            _investigationsList = new ObservableCollection<Investigation>();
            if (dgInvestigations != null) dgInvestigations.ItemsSource = _investigationsList;

            if (cmbInvestigationSubject != null) cmbInvestigationSubject.ItemsSource = _subjectsList;
            if (cmbInvestigationOffenseType != null) cmbInvestigationOffenseType.ItemsSource = _offenseTypesList;

            LoadInvestigations();
        }

        private void LoadOffenseTypes()
        {
            try
            {
                _offenseTypesList.Clear();
                foreach (var type in _offenseTypeRepository.GetAll())
                {
                    _offenseTypesList.Add(type);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки видов правонарушений: {ex.Message}", "Ошибка загрузки данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgOffenseTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedOffenseType = dgOffenseTypes.SelectedItem as OffenseType;
            if (_selectedOffenseType != null)
            {
                txtOffenseTypeName.Text = _selectedOffenseType.Name;
                btnUpdateOffenseType.IsEnabled = true;
                btnDeleteOffenseType.IsEnabled = true;
            }
            else
            {
                ClearOffenseTypeForm();
            }
        }

        private void BtnAddOffenseType_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOffenseTypeName.Text))
            {
                MessageBox.Show("Название правонарушения не может быть пустым.", "Ввод данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtOffenseTypeName.Focus();
                return;
            }
            try
            {
                OffenseType newType = new OffenseType { Name = txtOffenseTypeName.Text.Trim() };
                _offenseTypeRepository.Add(newType);
                LoadOffenseTypes();
                ClearOffenseTypeForm();
                MessageBox.Show("Вид правонарушения успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (SqlException sqlEx) when (sqlEx.Number == 2627 || sqlEx.Number == 2601)
            {
                MessageBox.Show("Вид правонарушения с таким названием уже существует.", "Ошибка добавления", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления вида правонарушения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdateOffenseType_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOffenseType == null)
            {
                MessageBox.Show("Сначала выберите вид правонарушения для обновления.", "Обновление данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtOffenseTypeName.Text))
            {
                MessageBox.Show("Название правонарушения не может быть пустым.", "Обновление данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtOffenseTypeName.Focus();
                return;
            }
            try
            {
                _selectedOffenseType.Name = txtOffenseTypeName.Text.Trim();
                _offenseTypeRepository.Update(_selectedOffenseType);
                LoadOffenseTypes();
                ClearOffenseTypeForm();
                MessageBox.Show("Вид правонарушения успешно обновлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (SqlException sqlEx) when (sqlEx.Number == 2627 || sqlEx.Number == 2601)
            {
                MessageBox.Show("Вид правонарушения с таким названием уже существует (для другого ID).", "Ошибка обновления", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления вида правонарушения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteOffenseType_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOffenseType == null)
            {
                MessageBox.Show("Сначала выберите вид правонарушения для удаления.", "Удаление данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить '{_selectedOffenseType.Name}'?",
                                                     "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _offenseTypeRepository.Delete(_selectedOffenseType.OffenseTypeID);
                    LoadOffenseTypes();
                    ClearOffenseTypeForm();
                    MessageBox.Show("Вид правонарушения успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (SqlException sqlEx) when (sqlEx.Number == 547)
                {
                    MessageBox.Show("Невозможно удалить этот вид правонарушения, так как он используется в расследованиях.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления вида правонарушения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClearOffenseTypeForm_Click(object sender, RoutedEventArgs e)
        {
            ClearOffenseTypeForm();
        }

        private void ClearOffenseTypeForm()
        {
            if (txtOffenseTypeName != null) txtOffenseTypeName.Clear();
            _selectedOffenseType = null;
            if (dgOffenseTypes != null) dgOffenseTypes.SelectedItem = null;
            if (btnUpdateOffenseType != null) btnUpdateOffenseType.IsEnabled = false;
            if (btnDeleteOffenseType != null) btnDeleteOffenseType.IsEnabled = false;
            if (txtOffenseTypeName != null) txtOffenseTypeName.Focus();
        }


        private void LoadSubjects()
        {
            try
            {
                _subjectsList.Clear();
                foreach (var subject in _subjectRepository.GetAll())
                {
                    _subjectsList.Add(subject);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки субъектов: {ex.Message}", "Ошибка загрузки данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgSubjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedSubject = dgSubjects.SelectedItem as Subject;
            if (_selectedSubject != null)
            {
                if (txtSubjectLastName != null) txtSubjectLastName.Text = _selectedSubject.LastName;
                if (txtSubjectFirstName != null) txtSubjectFirstName.Text = _selectedSubject.FirstName;
                if (txtSubjectNotes != null) txtSubjectNotes.Text = _selectedSubject.Notes;
                if (btnUpdateSubject != null) btnUpdateSubject.IsEnabled = true;
                if (btnDeleteSubject != null) btnDeleteSubject.IsEnabled = true;
            }
            else
            {
                ClearSubjectForm();
            }
        }

        private void BtnAddSubject_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSubjectLastName.Text))
            {
                MessageBox.Show("Фамилия субъекта не может быть пустой.", "Ввод данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                if (txtSubjectLastName != null) txtSubjectLastName.Focus();
                return;
            }
            try
            {
                Subject newSubject = new Subject
                {
                    LastName = txtSubjectLastName.Text.Trim(),
                    FirstName = string.IsNullOrWhiteSpace(txtSubjectFirstName.Text) ? null : txtSubjectFirstName.Text.Trim(),
                    Notes = string.IsNullOrWhiteSpace(txtSubjectNotes.Text) ? null : txtSubjectNotes.Text.Trim()
                };
                _subjectRepository.Add(newSubject);
                LoadSubjects();
                ClearSubjectForm();
                MessageBox.Show("Субъект успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления субъекта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdateSubject_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSubject == null)
            {
                MessageBox.Show("Сначала выберите субъекта для обновления.", "Обновление данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtSubjectLastName.Text))
            {
                MessageBox.Show("Фамилия субъекта не может быть пустой.", "Обновление данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                if (txtSubjectLastName != null) txtSubjectLastName.Focus();
                return;
            }
            try
            {
                _selectedSubject.LastName = txtSubjectLastName.Text.Trim();
                _selectedSubject.FirstName = string.IsNullOrWhiteSpace(txtSubjectFirstName.Text) ? null : txtSubjectFirstName.Text.Trim();
                _selectedSubject.Notes = string.IsNullOrWhiteSpace(txtSubjectNotes.Text) ? null : txtSubjectNotes.Text.Trim();
                _subjectRepository.Update(_selectedSubject);
                LoadSubjects();
                ClearSubjectForm();
                MessageBox.Show("Данные субъекта успешно обновлены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления данных субъекта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteSubject_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSubject == null)
            {
                MessageBox.Show("Сначала выберите субъекта для удаления.", "Удаление данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить субъекта '{_selectedSubject.FullName}'? \nВНИМАНИЕ: Это также удалит все связанные с ним расследования!",
                                                     "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _subjectRepository.Delete(_selectedSubject.SubjectID);
                    LoadSubjects();
                    ClearSubjectForm();
                    MessageBox.Show("Субъект успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления субъекта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClearSubjectForm_Click(object sender, RoutedEventArgs e)
        {
            ClearSubjectForm();
        }

        private void ClearSubjectForm()
        {
            if (txtSubjectLastName != null) txtSubjectLastName.Clear();
            if (txtSubjectFirstName != null) txtSubjectFirstName.Clear();
            if (txtSubjectNotes != null) txtSubjectNotes.Clear();
            _selectedSubject = null;
            if (dgSubjects != null) dgSubjects.SelectedItem = null;
            if (btnUpdateSubject != null) btnUpdateSubject.IsEnabled = false;
            if (btnDeleteSubject != null) btnDeleteSubject.IsEnabled = false;
            if (txtSubjectLastName != null) txtSubjectLastName.Focus();
        }

        private void LoadInvestigations()
        {
            try
            {
                _investigationsList.Clear();
                foreach (var investigation in _investigationRepository.GetAll())
                {
                    _investigationsList.Add(investigation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки расследований: {ex.Message}", "Ошибка загрузки данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgInvestigations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedInvestigation = dgInvestigations.SelectedItem as Investigation;
            if (_selectedInvestigation != null)
            {
                if (cmbInvestigationSubject != null) cmbInvestigationSubject.SelectedValue = _selectedInvestigation.SubjectID;
                if (cmbInvestigationOffenseType != null) cmbInvestigationOffenseType.SelectedValue = _selectedInvestigation.OffenseTypeID;
                if (dpInvestigationOffenseDate != null) dpInvestigationOffenseDate.SelectedDate = _selectedInvestigation.OffenseDate;
                if (txtInvestigationRewardAmount != null) txtInvestigationRewardAmount.Text = _selectedInvestigation.RewardAmount.ToString("F2");
                if (txtInvestigationStatus != null) txtInvestigationStatus.Text = _selectedInvestigation.Status;
                if (txtInvestigationDescription != null) txtInvestigationDescription.Text = _selectedInvestigation.Description;
                if (btnUpdateInvestigation != null) btnUpdateInvestigation.IsEnabled = true;
                if (btnDeleteInvestigation != null) btnDeleteInvestigation.IsEnabled = true;
            }
            else
            {
                ClearInvestigationForm();
            }
        }

        private void BtnAddInvestigation_Click(object sender, RoutedEventArgs e)
        {
            if (cmbInvestigationSubject.SelectedItem == null)
            { MessageBox.Show("Выберите субъекта.", "Ввод данных", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (cmbInvestigationOffenseType.SelectedItem == null)
            { MessageBox.Show("Выберите вид правонарушения.", "Ввод данных", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (dpInvestigationOffenseDate.SelectedDate == null)
            { MessageBox.Show("Выберите дату правонарушения.", "Ввод данных", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (!decimal.TryParse(txtInvestigationRewardAmount.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal rewardAmount) || rewardAmount < 0)
            {
                MessageBox.Show("Введите корректную сумму вознаграждения (неотрицательное число, разделитель - точка).", "Ввод данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                if (txtInvestigationRewardAmount != null) txtInvestigationRewardAmount.Focus(); return;
            }
            try
            {
                Investigation newInvestigation = new Investigation
                {
                    SubjectID = (int)cmbInvestigationSubject.SelectedValue,
                    OffenseTypeID = (int)cmbInvestigationOffenseType.SelectedValue,
                    OffenseDate = dpInvestigationOffenseDate.SelectedDate.Value,
                    RewardAmount = rewardAmount,
                    Status = string.IsNullOrWhiteSpace(txtInvestigationStatus.Text) ? null : txtInvestigationStatus.Text.Trim(),
                    Description = string.IsNullOrWhiteSpace(txtInvestigationDescription.Text) ? null : txtInvestigationDescription.Text.Trim()
                };
                _investigationRepository.Add(newInvestigation);
                LoadInvestigations();
                ClearInvestigationForm();
                MessageBox.Show("Расследование успешно добавлено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Ошибка SQL при добавлении расследования: {sqlEx.Message} (Номер: {sqlEx.Number})", "Ошибка SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления расследования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdateInvestigation_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedInvestigation == null)
            { MessageBox.Show("Сначала выберите расследование для обновления.", "Обновление данных", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (cmbInvestigationSubject.SelectedItem == null)
            { MessageBox.Show("Выберите субъекта.", "Ввод данных", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (cmbInvestigationOffenseType.SelectedItem == null)
            { MessageBox.Show("Выберите вид правонарушения.", "Ввод данных", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (dpInvestigationOffenseDate.SelectedDate == null)
            { MessageBox.Show("Выберите дату правонарушения.", "Ввод данных", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (!decimal.TryParse(txtInvestigationRewardAmount.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal rewardAmount) || rewardAmount < 0)
            {
                MessageBox.Show("Введите корректную сумму вознаграждения (неотрицательное число, разделитель - точка).", "Ввод данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                if (txtInvestigationRewardAmount != null) txtInvestigationRewardAmount.Focus(); return;
            }
            try
            {
                _selectedInvestigation.SubjectID = (int)cmbInvestigationSubject.SelectedValue;
                _selectedInvestigation.OffenseTypeID = (int)cmbInvestigationOffenseType.SelectedValue;
                _selectedInvestigation.OffenseDate = dpInvestigationOffenseDate.SelectedDate.Value;
                _selectedInvestigation.RewardAmount = rewardAmount;
                _selectedInvestigation.Status = string.IsNullOrWhiteSpace(txtInvestigationStatus.Text) ? null : txtInvestigationStatus.Text.Trim();
                _selectedInvestigation.Description = string.IsNullOrWhiteSpace(txtInvestigationDescription.Text) ? null : txtInvestigationDescription.Text.Trim();
                _investigationRepository.Update(_selectedInvestigation);
                LoadInvestigations();
                ClearInvestigationForm();
                MessageBox.Show("Данные расследования успешно обновлены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Ошибка SQL при обновлении расследования: {sqlEx.Message} (Номер: {sqlEx.Number})", "Ошибка SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления данных расследования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteInvestigation_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedInvestigation == null)
            { MessageBox.Show("Сначала выберите расследование для удаления.", "Удаление данных", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить расследование ID: {_selectedInvestigation.InvestigationID}?",
                                                     "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _investigationRepository.Delete(_selectedInvestigation.InvestigationID);
                    LoadInvestigations();
                    ClearInvestigationForm();
                    MessageBox.Show("Расследование успешно удалено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления расследования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClearInvestigationForm_Click(object sender, RoutedEventArgs e)
        {
            ClearInvestigationForm();
        }

        private void ClearInvestigationForm()
        {
            if (cmbInvestigationSubject != null) cmbInvestigationSubject.SelectedItem = null;
            if (cmbInvestigationOffenseType != null) cmbInvestigationOffenseType.SelectedItem = null;
            if (dpInvestigationOffenseDate != null) dpInvestigationOffenseDate.SelectedDate = null;
            if (txtInvestigationRewardAmount != null) txtInvestigationRewardAmount.Clear();
            if (txtInvestigationStatus != null) txtInvestigationStatus.Clear();
            if (txtInvestigationDescription != null) txtInvestigationDescription.Clear();
            _selectedInvestigation = null;
            if (dgInvestigations != null) dgInvestigations.SelectedItem = null;
            if (btnUpdateInvestigation != null) btnUpdateInvestigation.IsEnabled = false;
            if (btnDeleteInvestigation != null) btnDeleteInvestigation.IsEnabled = false;
            if (cmbInvestigationSubject != null) cmbInvestigationSubject.Focus();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;
            string currentText = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength).Insert(textBox.CaretIndex, e.Text);

            if (char.IsControl(e.Text, 0))
            {
                e.Handled = false;
                return;
            }

            string separator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            if (e.Text == separator || e.Text == "." || e.Text == ",") 
            {
                if (textBox.Text.Contains(separator) || textBox.CaretIndex == 0)
                {
                    e.Handled = true;
                }
                else
                {
                }
            }
            else if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
            else if (textBox.Text.Contains(separator))
            {
                int decimalPointIndex = textBox.Text.IndexOf(separator);
                if (textBox.CaretIndex > decimalPointIndex && textBox.Text.Length - decimalPointIndex > 2)
                {
                    e.Handled = true;
                }
            }
        }
    }
}