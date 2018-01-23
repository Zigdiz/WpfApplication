using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;

namespace WpfApplication.ViewModels
{
    public class BookViewModel
    {
        private Entities context;

        public virtual IMessageBoxService MessageBoxService
        {
            get { return null; }
        }

        public virtual ObservableCollection<Book> Books { get; set; }
        public virtual Book SelectedBook { get; set; }

        public virtual string BookNameSearchCriteria { get; set; }

        public virtual bool IsLoanedSearchCriteria { get; set; }

        public BookViewModel()
        {
            if (this.IsInDesignMode())
                return;

            context = new Entities();
            Books = new ObservableCollection<Book>();
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

            var bookQuery = context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(BookNameSearchCriteria))
            {
                bookQuery = bookQuery.Where(x => x.Name.Contains(BookNameSearchCriteria));
            }

            if (IsLoanedSearchCriteria)
            {
                bookQuery = bookQuery.Where(x => x.Loans.Any());
            }

            Books = new ObservableCollection<Book>(bookQuery);
        }

        public void NewBook()
        {
            var newBook = context.Books.Create();
            newBook.Id = Guid.NewGuid();

            Books.Add(newBook);
            context.Books.Add(newBook);
        }

        public bool CanDeleteBook()
        {
            return SelectedBook != null;
        }

        public void DeleteBook()
        {
            var bookToRemove = SelectedBook;

            if (bookToRemove.Loans.Any())
            {
                foreach (var loan in bookToRemove.Loans.ToList())
                {
                    context.Loans.Remove(loan);
                }
            }

            Books.Remove(bookToRemove);
            context.Books.Remove(bookToRemove);
        }

        public bool CanSave()
        {
            return context.HasUnsavedChanges();
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
