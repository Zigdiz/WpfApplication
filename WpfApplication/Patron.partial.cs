using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication
{
    public partial class Patron
    {
        public string FullName
        {
            get { return LastName + " " + FirstName; }
        }

        public string CurrentLoans
        {
            get { return string.Join(", ", Loans.Select(x => x.Book.Name)); }
        }

    }
}
