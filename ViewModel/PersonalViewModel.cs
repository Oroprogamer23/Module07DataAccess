using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Module07DataAccess.Model;
using Module07DataAccess.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.VisualBasic;

namespace Module07DataAccess.ViewModel
{
    public class PersonalViewModel : INotifyPropertyChanged
    {
        private readonly PersonalService _personalService;
        public ObservableCollection<Personal> PersonalList { get; set; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        private Personal _selectedPersonal;
        public Personal SelectedPersonal
        {
            get => _selectedPersonal;
            set
            {
                _selectedPersonal = value;
                if (_selectedPersonal != null)
                {
                    NewPersonalname = _selectedPersonal.name;
                    NewPersonaladdress = _selectedPersonal.address;
                    NewPersonalemail = _selectedPersonal.email;
                    NewPersonalphone = _selectedPersonal.phone;
                    isPersonSelected = true;
                }
                else 
                {
                    isPersonSelected = false;
                }
                OnPropertyChanged();
            }
        }

        private bool _isPersonSelected;

        public bool isPersonSelected
        {
            get =>_isPersonSelected;
            set
            {
                _isPersonSelected = value;
                OnPropertyChanged();
            }
        }
        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        //New Personal entry for name, address, email, phone
        private string _newPersonalname;
        public string NewPersonalname
        {
            get => _newPersonalname;
            set
            {
                _newPersonalname = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonaladdress;
        public string NewPersonaladdress
        {
            get => _newPersonaladdress;
            set
            {
                _newPersonaladdress = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalemail;
        public string NewPersonalemail
        {
            get => _newPersonalemail;
            set
            {
                _newPersonalemail = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalphone;
        public string NewPersonalphone
        {
            get => _newPersonalphone;
            set
            {
                _newPersonalphone = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadDataCommand { get; }
        public ICommand AddPersonalCommand { get; }
        public ICommand SelectedPersonCommand { get; }
        public ICommand DeletePersonCommand { get; }

        public PersonalViewModel()
        {
            _personalService = new PersonalService();
            PersonalList = new ObservableCollection<Personal>();
            LoadDataCommand = new Command(async () => await LoadData());
            AddPersonalCommand = new Command(async () => await AddPerson());
            SelectedPersonCommand = new Command<Personal>(person => SelectedPersonal = person);
            DeletePersonCommand = new Command(async () => await DeletePersonal(), () => SelectedPersonal != null);

            LoadData();
        }

        public async Task LoadData()
        {
            if (_isBusy) return;
            IsBusy = true;
            StatusMessage = "Loading personal data...";
            try
            {
                var personals = await _personalService.GetAllPersonalsAsync();
                PersonalList.Clear();
                foreach (var personal in personals)
                {
                    PersonalList.Add(personal);
                }
                StatusMessage = "data loaded successfully!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load data: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddPerson()
        {
            if (IsBusy 
                || string.IsNullOrWhiteSpace(NewPersonalname) || string.IsNullOrWhiteSpace(NewPersonaladdress) || string.IsNullOrWhiteSpace(NewPersonalemail) || string.IsNullOrWhiteSpace(NewPersonalphone))
            {
                StatusMessage = "Please fill in all fields before addings";
                return;
            }
            IsBusy = true;
            StatusMessage = "Adding new person...";
            try
            {
                var newPerson = new Personal
                {
                    name = NewPersonalname,
                    address = NewPersonaladdress,
                    email = NewPersonalemail,
                    phone = NewPersonalphone
                };
                var isSuccess = await _personalService.AddPersonalAsync(newPerson);
                if (isSuccess)
                {
                    NewPersonalname = string.Empty;
                    NewPersonaladdress = string.Empty;
                    NewPersonalemail = string.Empty;
                    NewPersonalphone = string.Empty;
                    StatusMessage = "New employee added successfully";
                }
                else
                {
                    StatusMessage = "Failed to add the new employee";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed adding person: {ex.Message}";
            }
            finally 
            {  
                IsBusy = false;
                await LoadData();
            }
        }

        private async Task DeletePersonal()
        {
            if (SelectedPersonal != null) return;
            var answer = await Application.Current.MainPage.DisplayAlert
                ("Confirm Delete", $"Are you sure you want to delete {SelectedPersonal.name}?", "Yes", "No");

            if (!answer) return;

            IsBusy = true;
            StatusMessage = "Deleting person..";

            try
            {
                var success = await _personalService.DeletePersonalAsync(SelectedPersonal.id);
                StatusMessage = success ? "Person deleted successfully!" : "Failed to delete person";

                if (success)
                {
                    PersonalList.Remove(SelectedPersonal);
                    SelectedPersonal = null;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting person: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
