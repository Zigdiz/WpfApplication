using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using WpfApplication.Helpers;

namespace WpfApplication.ViewModels
{
    public class LoanViewModel
    {
        private Entities context;

        public virtual IMessageBoxService MessageBoxService
        {
            get { return null; }
        }

        public virtual ObservableCollection<Loan> Loans { get; set; }
        public virtual Loan SelectedLoan { get; set; }

        public virtual List<object> PatronSearchCriteria { get; set; }
        public virtual List<object> BookSearchCriteria { get; set; }
        public virtual bool IsExpired { get; set; }

        public virtual List<Patron> Patrons { get; set; }
        public virtual List<Book> Books { get; set; }

        public LoanViewModel()
        {
            if (this.IsInDesignMode())
                return;

            context = new Entities();
            Loans = new ObservableCollection<Loan>();
            Patrons = context.Patrons.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).AsNoTracking().ToList();
            Books = context.Books.OrderBy(x => x.Name).AsNoTracking().ToList();
        }

        public void Search()
        {
            if (CanSave())
            {
                var result = MessageBoxService.Show("You have unsaved data. Do you want to save?", "Information",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);

                if (result == MessageBoxResult.Cancel)
                    return;

                if (result == MessageBoxResult.Yes)
                    Save();
            }

            context = new Entities();

            var loanQuery = context.Loans.AsQueryable();

            if (PatronSearchCriteria != null && PatronSearchCriteria.Any())
            {
                loanQuery = loanQuery.WherePropertyIn(x => x.PatronId, PatronSearchCriteria);
            }

            if (BookSearchCriteria != null && BookSearchCriteria.Any())
            {
                loanQuery = loanQuery.WherePropertyIn(x => x.BookId, BookSearchCriteria);
            }

            if (IsExpired)
            {
                loanQuery = loanQuery.Where(x => x.DueDate <= DateTime.Today);
            }

            Loans = new ObservableCollection<Loan>(loanQuery);
        }

        public void NewLoan()
        {
            var newLoan = context.Loans.Create();
            newLoan.Id = Guid.NewGuid();
            newLoan.LoanDate = DateTime.Today;
            newLoan.DueDate = DateTime.Today.AddMonths(1);

            Loans.Add(newLoan);
            context.Loans.Add(newLoan);
        }

        public bool CanDeleteLoan()
        {
            return SelectedLoan != null;
        }

        public void DeleteBook()
        {
            var loanToRemove = SelectedLoan;
            Loans.Remove(loanToRemove);
            context.Loans.Remove(SelectedLoan);
        }

        public bool CanSave()
        {
            return context.HasUnsavedChanges();
        }

        public void Save()
        {
            var invalidLoans = Loans.Where(x => x.PatronId == null || x.BookId == null);

            if (invalidLoans.Any())
            {
                MessageBoxService.Show("Not all loans have patron or book assigned to them and save can't be done",
                    "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            context.SaveChanges();
        }
    }
}
