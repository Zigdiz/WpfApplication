using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication
{
    public partial class Entities
    {
        public ObjectContext ObjectContext
        {
            get { return ((IObjectContextAdapter)this).ObjectContext; }
        }

        public bool HasUnsavedChanges()
        {
            ChangeTracker.DetectChanges();

            var objectContext = ObjectContext;

            return objectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added).Any()
                   || objectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Modified).Any()
                   || objectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Deleted).Any();
        }
    }
}
