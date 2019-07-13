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
    public class SupplierCreateUpdateViewModel : ViewModelBase, INavigationAware
    {
        private string _title = "供应商新增";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private Supplier _currentSupplier;
        public Supplier CurrentSupplier
        {
            get { return _currentSupplier; }
            set { SetProperty(ref _currentSupplier, value); }
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
        public DelegateCommand DeleteCommand { get; private set; }

        // Commands - Contact Create
        public InteractionRequest<ISupplierContactCreateUpdateNotif> SupplierContactCreateRequest { get; set; }
        public DelegateCommand SupplierContactCreateCommand { get; private set; }

        // Commands - Contact Update
        public InteractionRequest<ISupplierContactCreateUpdateNotif> SupplierContactUpdateRequest { get; set; }
        public DelegateCommand<object> SupplierContactUpdateCommand { get; private set; }

        // Commands - Contact Delete
        public DelegateCommand<object> SupplierContactDeleteCommand { get; private set; }

        public SupplierCreateUpdateViewModel()
        {
            SaveCommand = new DelegateCommand(Save);
            DeleteCommand = new DelegateCommand(Delete).ObservesCanExecute(() => IsEdit);

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
                    CurrentSupplier.Contacts.Add(r.NewContact);
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
                Title = "确认",
                Content = "删除联系人" + contact.Name + "?",
            }, r =>
            {
                if (r.Confirmed)
                {
                    CurrentSupplier.Contacts.Remove(contact);
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
                    .FirstOrDefault(s => s.Id == CurrentSupplier.Id);

                if (existingSupplier == null)
                {
                    context.Add(CurrentSupplier);
                }
                else
                {
                    context.Entry(existingSupplier).CurrentValues.SetValues(CurrentSupplier);
                    foreach (var contact in CurrentSupplier.Contacts)
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
                        if (!CurrentSupplier.Contacts.Any(c => c.Id == contact.Id))
                        {
                            context.Remove(contact);
                        }
                    }
                }

                context.SaveChanges();
                NotificationRequest.Raise(new Notification { Content = "保存成功", Title = "提示" });
                IsEdit = true;
            }
        }

        private void Delete()
        {
            ConfirmationRequest.Raise(new Confirmation
            {
                Content = "删除供应商" + CurrentSupplier.Number + "?",
                Title = "确认"
            },
            r =>
            {
                if (r.Confirmed)
                {
                    using (var context = new PartnerContext())
                    {
                        context.Suppliers.Remove(CurrentSupplier);
                        context.SaveChanges();
                    }

                    IsEdit = false;
                    NotificationRequest.Raise(new Notification { Content = "已删除", Title = "提示" });
                    CurrentSupplier = new Supplier
                    {
                        InternalType = "Supplier",
                        Contacts = new ObservableCollection<Contact>(),
                    };
                    Title = "供应商新增";
                }
            });
        }

        // Validation
        private bool Validate(PartnerContext context)
        {
            if (string.IsNullOrWhiteSpace(CurrentSupplier.Number))
            {
                NotificationRequest.Raise(new Notification { Content = "供应商编码为空", Title = "提示" });
                return false;
            }
            
            if (!IsEdit && context.Partners.Where(s => s.Number == CurrentSupplier.Number).FirstOrDefault() != null)
            {
                NotificationRequest.Raise(new Notification { Content = "供应商编码已存在", Title = "提示" });
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
            var supplier = navigationContext.Parameters["supplier"] as Supplier;
            if (supplier != null)
            {
                CurrentSupplier = supplier;
                Title = "供应商维护";
                IsEdit = true;
            }
            else
            {
                CurrentSupplier = new Supplier
                {
                    InternalType = "Supplier",
                    Contacts = new ObservableCollection<Contact>()
                };
            }
        }
    }
}
