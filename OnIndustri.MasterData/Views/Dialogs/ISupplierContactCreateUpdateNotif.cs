using OnIndustri.Data.Model.MasterData;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnIndustri.MasterData.Views.Dialogs
{
    public interface ISupplierContactCreateUpdateNotif : IConfirmation
    {
        Contact NewContact { get; set; }
    }
}
