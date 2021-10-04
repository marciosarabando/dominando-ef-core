using System;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DominandoEntityFramework.Interceptadores
{
    public class InterceptadorPersistencia : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            Console.WriteLine(eventData.Context.ChangeTracker.DebugView.LongView);

            return base.SavingChanges(eventData, result);
        }
    }
}