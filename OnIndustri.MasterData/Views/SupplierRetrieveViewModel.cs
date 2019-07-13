using CommonServiceLocator;
using Microsoft.EntityFrameworkCore;
using OnIndustri.Data.Database;
using OnIndustri.Data.Model.MasterData;
using OnIndustri.Infrastructure;
using OnIndustri.Infrastructure.Constants;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace OnIndustri.MasterData.Views
{
    public class SupplierRetrieveViewModel : ViewModelBase, INavigationAware
    {
        private string _title = "供应商维护";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string SearchNumber { get; set; }
        public string SearchName { get; set; }
        //public Expression<Func<Supplier, bool>> SearchClause { get; private set; }
        //public ObservableCollection<Supplier> Suppliers { get; set; }
        private ICollectionView _suppliersView;
        public ICollectionView SuppliersView
        {
            get { return _suppliersView; }
            set { SetProperty(ref _suppliersView, value); }
        }

        // Paging
        public int PageSize { get; set; }                   // 每页记录数
        public int PageIndex { get; set; } = 0;             // 当前页
        public int TotalRecords { get; set; }               // 总记录数
        public int TotalPages { get; set; }                 // 总页数

        // Notifications
        public InteractionRequest<INotification> NotificationRequest { get; set; }
        public InteractionRequest<IConfirmation> ConfirmationRequest { get; set; }

        // Commands
        public DelegateCommand SearchCommand { get; private set; }
        public DelegateCommand DetailCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }

        public SupplierRetrieveViewModel()
        {
            SearchCommand = new DelegateCommand(Search);
            DetailCommand = new DelegateCommand(Detail, CanDetail)
                                .ObservesProperty(() => SuppliersView);
            UpdateCommand = new DelegateCommand(Update, CanUpdate)
                                .ObservesProperty(() => SuppliersView);
            DeleteCommand = new DelegateCommand(Delete, CanDelete)
                                .ObservesProperty(() => SuppliersView);

            NotificationRequest = new InteractionRequest<INotification>();
            ConfirmationRequest = new InteractionRequest<IConfirmation>();
        }
        
        private void Search()
        {
            var clause = BuildSearchExpression(SearchNumber, SearchName);
            
            using (var context = new PartnerContext())
            {
                var suppliers = context.Suppliers.Where(clause).OrderBy(s => s.Number).ToList();
                SuppliersView = CollectionViewSource.GetDefaultView(suppliers);
            }
        }

        private void Update()
        {
            var supplier = SuppliersView.CurrentItem as Supplier;

            using (var context = new PartnerContext())
            {
                var existingSupplier = context.Suppliers.Where(s => s.Id == supplier.Id)
                    .Include(s => s.Contacts).FirstOrDefault();
                if (existingSupplier != null)
                {
                    var parameters = new NavigationParameters
                    {
                        { "supplier", existingSupplier }
                    };

                    IRegionManager regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
                    regionManager.RequestNavigate(RegionNames.MAIN_REGION, "SupplierCreateUpdate", parameters);
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
                }
            }
        }

        private bool CanUpdate()
        {
            return CanExecute();
        }

        private void Delete()
        {

        }

        private bool CanDelete()
        {
            return CanExecute();
        }

        private void Detail()
        {
            var supplier = SuppliersView.CurrentItem as Supplier;

            using (var context = new PartnerContext())
            {
                var existingSupplier = context.Suppliers.Where(s => s.Id == supplier.Id)
                    .Include(s => s.Contacts).FirstOrDefault();

                if (existingSupplier != null)
                {
                    var parameters = new NavigationParameters
                    {
                        { "supplier", existingSupplier }
                    };

                    IRegionManager regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
                    regionManager.RequestNavigate(RegionNames.MAIN_REGION, "SupplierDetail", parameters);
                }
                else
                {
                    NotificationRequest.Raise(
                        new Notification
                        {
                            Content = "供应商 " + supplier.Name + " 不存在。\n可能已删除，请重新查询。",
                            Title = "提示"
                        }
                    );
                }
            }
        }

        private bool CanDetail()
        {
            return CanExecute();
        }

        private bool CanExecute()
        {
            if (SuppliersView == null || SuppliersView.CurrentItem == null)
            {
                return false;
            }

            return true;
        }

        private Expression<Func<Supplier, bool>> BuildSearchExpression(string number, string name)
        {
            Expression<Func<Supplier, bool>> searchClause =
                s => (string.IsNullOrEmpty(number) || s.Number.Contains(number)) && (string.IsNullOrEmpty(name) || s.Name.Contains(name));
            return searchClause;
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
