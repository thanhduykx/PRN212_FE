using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities.Enum
{
    public enum RentalOrderStatus
    {
        Pending,
        Confirmed,
        Renting,
        PaymentPending,
        Cancelled,
        Completed
    }
}
