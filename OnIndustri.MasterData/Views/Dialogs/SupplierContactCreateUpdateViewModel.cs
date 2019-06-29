using OnIndustri.Data.Model.MasterData;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OnIndustri.MasterData.Views.Dialogs
{
    public class SupplierContactCreateUpdateViewModel : BindableBase, IInteractionRequestAware
    {
        public DelegateCommand ConfirmCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public InteractionRequest<INotification> NotificationRequest { get; set; }

        public SupplierContactCreateUpdateViewModel()
        {
            ConfirmCommand = new DelegateCommand(ConfirmCreate);
            CancelCommand = new DelegateCommand(CancelCreate);
            NotificationRequest = new InteractionRequest<INotification>();
        }

        private void CancelCreate()
        {
            _notification.NewContact = null;
            _notification.Confirmed = false;
            FinishInteraction?.Invoke();
        }

        private void ConfirmCreate()
        {
            if (string.IsNullOrWhiteSpace((Notification as SupplierContactCreateUpdateNotif).NewContact.Name))
            {
                //MessageBox.Show("联系人为空");
                NotificationRequest.Raise(new Notification { Content = "联系人姓名为空", Title = "错误提示" });
                return;
            }

            //_notification.NewContact = NewContact;
            _notification.Confirmed = true;
            FinishInteraction?.Invoke();
        }

        // IInteractionRequestAware
        public Action FinishInteraction { get; set; }
        private SupplierContactCreateUpdateNotif _notification;
        public INotification Notification
        {
            get { return _notification; }
            set { SetProperty(ref _notification, value as SupplierContactCreateUpdateNotif); }
        }
    }
}
