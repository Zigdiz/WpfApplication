using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;

namespace WpfApplication.ViewModels
{
    public class PatronViewModel
    {
        private Entities context;

        public virtual IMessageBoxService MessageBoxService
        {
            get { return null; }
        }

        public virtual ObservableCollection<Patron> Patrons { get; set; }
        public virtual Patron SelectedPatron { get; set; }

        public virtual string FirstNameSearchCriteria { get; set; }
        public virtual string LastNameSearchCriteria { get; set; }
        public virtual string AddressSearchCriteria { get; set; }
        public virtual bool HasExpiredLoans { get; set; }

        public PatronViewModel()
        {
            if (this.IsInDesignMode())
                return;

            context = new Entities();
            Patrons = new ObservableCollection<Patron>();
        }

        public bool CanSave()
        {
            return context.HasUnsavedChanges();
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Search()
        {
            if (CanSave())
            {
                var result = MessageBoxService.Show("You have unsaved data. Do you want to save?", "Information",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);

                if(result == MessageBoxResult.Cancel)
                    return;

                if(result == MessageBoxResult.Yes)
                    Save();
            }

            context = new Entities();
            var patronQuery = context.Patrons.AsQueryable();

            if (!string.IsNullOrEmpty(FirstNameSearchCriteria))
            {
                patronQuery = patronQuery.Where(x => x.FirstName.Contains(FirstNameSearchCriteria));
            }

            if (!string.IsNullOrEmpty(LastNameSearchCriteria))
            {
                patronQuery = patronQuery.Where(x => x.LastName.Contains(LastNameSearchCriteria));
            }

            if (!string.IsNullOrEmpty(AddressSearchCriteria))
            {
                patronQuery = patronQuery.Where(x => x.Address.Contains(AddressSearchCriteria));
            }

            if (HasExpiredLoans)
            {
                patronQuery = patronQuery.Where(x => x.Loans.Any(s => s.DueDate <= DateTime.Today));
            }

            Patrons = new ObservableCollection<Patron>(patronQuery);
        }

        public void NewPatron()
        {
            var newPatron = context.Patrons.Create();
            newPatron.Id = Guid.NewGuid();
            context.Patrons.Add(newPatron);
            Patrons.Add(newPatron);
        }

        public bool CanDeletePatron()
        {
            return SelectedPatron != null;
        }

        public void DeletePatron()
        {
            var patronToRemove = SelectedPatron;

            if (patronToRemove.Loans.Any())
            {
                foreach (var selectedPatronLoan in patronToRemove.Loans.ToList())
                {
                    context.Loans.Remove(selectedPatronLoan);
                }
            }

            context.Patrons.Remove(patronToRemove);
            Patrons.Remove(patronToRemove);
        }
    }
}
