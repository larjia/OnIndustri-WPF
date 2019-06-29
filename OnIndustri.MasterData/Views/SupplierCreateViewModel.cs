using Microsoft.EntityFrameworkCore;
using OnIndustri.Data.Database;
using OnIndustri.Data.Model.MasterData;
using OnIndustri.Infrastructure;
using OnIndustri.Infrastructure.Helpers;
using OnIndustri.MasterData.Views.Dialogs;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OnIndustri.MasterData.Views
{
    public class SupplierCreateViewModel : ViewModelBase, INavigationAware
    {
        private string _title = "新增供应商";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private Supplier _newSupplier;
        public Supplier NewSupplier
        {
            get { return _newSupplier; }
            set { SetProperty(ref _newSupplier, value); }
        }

        private bool _isEdit;
        public bool IsEdit
        {
            get { return _isEdit; }
            set { SetProperty(ref _isEdit, value); }
        }

        // Notifications
        public InteractionRequest<INotification> NotificationRequest { get; set; }
        public InteractionRequest<IConfirmation> ConfirmationRequest { get; set; }

        // Commands
        public DelegateCommand SaveCommand { get; private set; }
        // Commands - Contact Create
        public InteractionRequest<ISupplierContactCreateUpdateNotif> SupplierContactCreateRequest { get; set; }
        public DelegateCommand SupplierContactCreateCommand { get; private set; }

        // Commands - Contact Update
        public InteractionRequest<ISupplierContactCreateUpdateNotif> SupplierContactUpdateRequest { get; set; }
        public DelegateCommand<object> SupplierContactUpdateCommand { get; private set; }

        // Commands - Contact Delete
        public DelegateCommand<object> SupplierContactDeleteCommand { get; private set; }

        public SupplierCreateViewModel()
        {
            NewSupplier = new Supplier
            {
                InternalType = "Supplier",
                Contacts = new ObservableCollection<Contact>(),
            };

            SaveCommand = new DelegateCommand(Save);

            // Notifications
            NotificationRequest = new InteractionRequest<INotification>();
            ConfirmationRequest = new InteractionRequest<IConfirmation>();

            // Contact Create
            SupplierContactCreateRequest = new InteractionRequest<ISupplierContactCreateUpdateNotif>();
            SupplierContactCreateCommand = new DelegateCommand(RaiseSupplierContactCreate);

            // Contact Update
            SupplierContactUpdateRequest = new InteractionRequest<ISupplierContactCreateUpdateNotif>();
            SupplierContactUpdateCommand = new DelegateCommand<object>(RaiseSupplierContactUpdate);

            // Contact Delete
            SupplierContactDeleteCommand = new DelegateCommand<object>(RaiseSupplierContactDelete);
        }

        private void RaiseSupplierContactCreate()
        {
            SupplierContactCreateRequest.Raise(new SupplierContactCreateUpdateNotif { Title = "新增联系人" }, r =>
            {
                if (r.Confirmed && r.NewContact != null)
                {
                    NewSupplier.Contacts.Add(r.NewContact);
                }
            });
        }

        private void RaiseSupplierContactUpdate(object args)
        {
            var button = (args as RoutedEventArgs).OriginalSource as Button;
            var listboxItem = VisualTreeHelperExtensions.FindAncestor<ListBoxItem>(button);
            var contact = listboxItem.DataContext as Contact;
            SupplierContactUpdateRequest.Raise(new SupplierContactCreateUpdateNotif(contact) { Title = "更新联系人" }, r =>
            {
                if (r.Confirmed)
                {
                    contact.Name = r.NewContact.Name;
                    contact.Position = r.NewContact.Position;
                    contact.Phone = r.NewContact.Phone;
                    contact.MobilePhone = r.NewContact.MobilePhone;
                    contact.QQ = r.NewContact.QQ;
                    contact.WeChat = r.NewContact.WeChat;
                    contact.MobilePhone = r.NewContact.MobilePhone;
                    contact.Email = r.NewContact.Email;
                }
            });
        }

        private void RaiseSupplierContactDelete(object args)
        {
            var button = (args as RoutedEventArgs).OriginalSource as Button;
            var listboxItem = VisualTreeHelperExtensions.FindAncestor<ListBoxItem>(button);
            var contact = listboxItem.DataContext as Contact;
            ConfirmationRequest.Raise(new Confirmation
            {
                Title = "确认提示",
                Content = "删除联系人?" + "\n\n" + contact.Name,
            }, r =>
            {
                if (r.Confirmed)
                {
                    NewSupplier.Contacts.Remove(contact);
                }
            });
        }

        private void Save()
        {
            using (var context = new PartnerContext())
            {
                if (!Validate(context)) return;

                var existingSupplier = context.Suppliers
                    .Include(s => s.Contacts)
                    .FirstOrDefault(s => s.Id == NewSupplier.Id);

                if (existingSupplier == null)
                {
                    context.Update(NewSupplier);
                    context.SaveChanges();
                    IsEdit = true;
                }
                else
                {
                    context.Entry(existingSupplier).CurrentValues.SetValues(NewSupplier);
                    foreach (var contact in NewSupplier.Contacts)
                    {
                        var existingContact = existingSupplier.Contacts
                            .FirstOrDefault(c => c.Id == contact.Id);
                        
                        if (existingContact == null)
                        {
                            existingSupplier.Contacts.Add(contact);
                        }
                        else
                        {
                            context.Entry(existingContact).CurrentValues.SetValues(contact);
                        }
                    }

                    foreach (var contact in existingSupplier.Contacts)
                    {
                        if (!NewSupplier.Contacts.Any(c => c.Id == contact.Id))
                        {
                            context.Remove(contact);
                        }
                    }
                }

                context.SaveChanges();
                NotificationRequest.Raise(new Notification { Content = "保存成功", Title = "提示" });
            }
        }

        // Validation
        private bool Validate(PartnerContext context)
        {
            if (string.IsNullOrWhiteSpace(NewSupplier.Number))
            {
                NotificationRequest.Raise(new Notification { Content = "供应商编码为空", Title = "错误提示" });
                return false;
            }
            
            if (!IsEdit && context.Partners.Where(s => s.Number == NewSupplier.Number).FirstOrDefault() != null)
            {
                NotificationRequest.Raise(new Notification { Content = "供应商编码已存在", Title = "错误提示" });
                return false;
            }

            return true;
        }

        // INavigationAware
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }
    }
}
