using OnIndustri.Data.Database;
using OnIndustri.Data.Model.MasterData;
using OnIndustri.Infrastructure;
using OnIndustri.Infrastructure.Helpers;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;


namespace OnIndustri.MasterData.Views
{
    public class SupplierDetailViewModel : ViewModelBase, INavigationAware
    {
        private string _title = "供应商详细";
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

        private bool _isExisting;
        public bool IsExisting
        {
            get { return _isExisting; }
            set { SetProperty(ref _isExisting, value); }
        }

        // Notifications
        public InteractionRequest<INotification> NotificationRequest { get; set; }

        // Commands
        public DelegateCommand RefreshCommand { get; private set; }

        public SupplierDetailViewModel()
        {
            RefreshCommand = new DelegateCommand(Refresh).ObservesCanExecute(() => IsExisting);
            NotificationRequest = new InteractionRequest<INotification>();
        }

        private void Refresh()
        {
            using (var context = new PartnerContext())
            {
                var supplier = context.Suppliers.Where(s => s.Id == CurrentSupplier.Id)
                    .Include(s => s.Contacts).FirstOrDefault();
                if (supplier != null)
                {
                    CurrentSupplier = supplier;
                }
                else
                {
                    NotificationRequest.Raise(
                        new Notification
                        {
                            Content = "供应商" + supplier.Name + "不存在。\n可能已删除，请重新查询。",
                            Title = "提示"
                        }
                    );

                    CurrentSupplier = new Supplier
                    {
                        InternalType = "Supplier",
                        Contacts = new ObservableCollection<Contact>(),
                    };
                    IsExisting = false;
                }
            }
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
                IsExisting = true;
            }
        }
    }
}
