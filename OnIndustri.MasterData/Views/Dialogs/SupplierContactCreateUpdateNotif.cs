using OnIndustri.Data.Model.MasterData;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnIndustri.MasterData.Views.Dialogs
{
    public class SupplierContactCreateUpdateNotif : Confirmation, ISupplierContactCreateUpdateNotif
    {
        public Contact NewContact { get; set; }

        public SupplierContactCreateUpdateNotif()
        {
            NewContact = new Contact();
        }

        public SupplierContactCreateUpdateNotif(Contact contact)
        {
            NewContact = new Contact()
            {
                Name = contact.Name,
                Position = contact.Position,
                Email = contact.Email,
                Phone = contact.Phone,
                MobilePhone = contact.MobilePhone,
                QQ = contact.QQ,
                WeChat = contact.WeChat,
            };
        }
    }
}
