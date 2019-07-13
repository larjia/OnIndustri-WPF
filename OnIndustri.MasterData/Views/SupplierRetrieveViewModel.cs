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
        public string CurrentSearchNumber { get; private set; }
        public string CurrentSearchName { get; private set; }

        //public Expression<Func<Supplier, bool>> SearchClause { get; private set; }
        //public ObservableCollection<Supplier> Suppliers { get; set; }
        private ICollectionView _suppliersView;
        public ICollectionView SuppliersView
        {
            get { return _suppliersView; }
            set { SetProperty(ref _suppliersView, value); }
        }

        // Paging
        private int _pageSize;
        public int PageSize                                     // 每页记录数
        {
            get { return _pageSize; }
            set { SetProperty(ref _pageSize, value); }
        }

        private int _pageIndex = 0;
        public int PageIndex                                    // 当前页
        {
            get { return _pageIndex; }
            set { SetProperty(ref _pageIndex, value); }
        }

        private int _totalRecords = 0;
        public int TotalRecords                                 // 总记录数
        {
            get { return _totalRecords; }
            set { SetProperty(ref _totalRecords, value); }
        }

        private int _totalPages = 0;
        public int TotalPages                                   // 总页数
        {
            get { return _totalPages; }
            set { SetProperty(ref _totalPages, value); }
        }

        public IList<Supplier> Suppliers { get; private set; }

        // Notifications
        public InteractionRequest<INotification> NotificationRequest { get; set; }
        public InteractionRequest<IConfirmation> ConfirmationRequest { get; set; }

        // Commands
        public DelegateCommand SearchCommand { get; private set; }
        public DelegateCommand DetailCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }
        
        // Commands - paging
        public DelegateCommand MoveToFirstPageCommand { get; private set; }
        public DelegateCommand MoveToPreviousPageCommand { get; private set; }
        public DelegateCommand MoveToNextPageCommand { get; private set; }
        public DelegateCommand MoveToLastPageCommand { get; private set; }
        public DelegateCommand PageSizeSelectionChanged { get; private set; }

        public SupplierRetrieveViewModel()
        {
            SearchCommand = new DelegateCommand(Search);
            DetailCommand = new DelegateCommand(Detail, CanDetail)
                                .ObservesProperty(() => SuppliersView);
            UpdateCommand = new DelegateCommand(Update, CanUpdate)
                                .ObservesProperty(() => SuppliersView);
            DeleteCommand = new DelegateCommand(Delete, CanDelete)
                                .ObservesProperty(() => SuppliersView);

            MoveToFirstPageCommand = new DelegateCommand(MoveToFirstPage, CanMoveToFirstPage)
                                        .ObservesProperty(() => PageIndex);
            MoveToPreviousPageCommand = new DelegateCommand(MoveToPreviousPage, CanMoveToPreviousPage)
                                        .ObservesProperty(() => PageIndex);
            MoveToNextPageCommand = new DelegateCommand(MoveToNextPage, CanMoveToNextPage)
                                        .ObservesProperty(() => PageIndex).ObservesProperty(() => TotalPages);
            MoveToLastPageCommand = new DelegateCommand(MoveToLastPage, CanMoveToLastPage)
                                        .ObservesProperty(() => PageIndex).ObservesProperty(() => TotalPages);
            PageSizeSelectionChanged = new DelegateCommand(PageSizeChanged);

            NotificationRequest = new InteractionRequest<INotification>();
            ConfirmationRequest = new InteractionRequest<IConfirmation>();
        }
        
        private void MoveToFirstPage()
        {
            PageIndex = 1;
            SuppliersView = CollectionViewSource.GetDefaultView(Suppliers.Skip((PageIndex - 1) * PageSize).Take(PageSize));
        }

        private bool CanMoveToFirstPage()
        {
            return PageIndex > 1;
        }

        private void MoveToPreviousPage()
        {
            PageIndex -= 1;
            SuppliersView = CollectionViewSource.GetDefaultView(Suppliers.Skip((PageIndex - 1) * PageSize).Take(PageSize));
        }

        private bool CanMoveToPreviousPage()
        {
            return PageIndex > 1;
        }

        private void MoveToNextPage()
        {
            PageIndex += 1;
            SuppliersView = CollectionViewSource.GetDefaultView(Suppliers.Skip((PageIndex - 1) * PageSize).Take(PageSize));
        }

        private bool CanMoveToNextPage()
        {
            return PageIndex < TotalPages;
        }

        private void MoveToLastPage()
        {
            PageIndex = TotalPages;
            SuppliersView = CollectionViewSource.GetDefaultView(Suppliers.Skip((PageIndex - 1) * PageSize).Take(PageSize));
        }

        private bool CanMoveToLastPage()
        {
            return PageIndex < TotalPages;
        }

        private void PageSizeChanged()
        {
            if (TotalPages == 0)
                return;

            TotalPages = (int)Math.Ceiling(1.0 * TotalRecords / PageSize);
            PageIndex = (PageIndex > TotalPages) ? TotalPages : PageIndex;
            SuppliersView = CollectionViewSource.GetDefaultView(Suppliers.Skip((PageIndex - 1) * PageSize).Take(PageSize));
        }

        private void Search()
        {
            CurrentSearchNumber = SearchNumber;
            CurrentSearchName = SearchName;
            GetData(1);
        }

        private void GetData(int pageindex)
        {
            using (var context = new MasterDataContext())
            {
                var clause = BuildSearchExpression(CurrentSearchNumber, CurrentSearchName);
                var query = context.Suppliers.Where(clause).OrderBy(s => s.Number);

                TotalRecords = query.Count();
                TotalPages = (int)Math.Ceiling(1.0 * TotalRecords / PageSize);
                PageIndex = (TotalPages == 0) ? 0 : (pageindex > TotalPages ? TotalPages : pageindex);
                Suppliers = query.ToList();

                if (PageIndex == 0)
                {
                    SuppliersView = CollectionViewSource.GetDefaultView(Suppliers);
                }
                else
                {
                    SuppliersView = CollectionViewSource.GetDefaultView(Suppliers.Skip((PageIndex - 1) * PageSize).Take(PageSize));
                }
            }
        }

        private void Update()
        {
            var supplier = SuppliersView.CurrentItem as Supplier;

            using (var context = new MasterDataContext())
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
            var supplier = SuppliersView.CurrentItem as Supplier;
            ConfirmationRequest.Raise(
                new Confirmation
                {
                    Content = "删除供应商" + supplier.Name + "?",
                    Title = "确认",
                },
                r =>
                {
                    if (r.Confirmed)
                    {
                        using (var context = new MasterDataContext())
                        {
                            var existingSupplier = context.Suppliers.Where(s => s.Id == supplier.Id)
                                                        .Include(s => s.Contacts).FirstOrDefault();
                            if (existingSupplier == null)
                            {
                                NotificationRequest.Raise(
                                    new Notification
                                    {
                                        Content = "供应商" + supplier.Name + "不存在。\n可能已删除，请重新查询。",
                                        Title = "提示"
                                    }
                                );
                                return;
                            }
                            context.Remove(existingSupplier);
                            context.SaveChanges();
                        }
                        
                        NotificationRequest.Raise(
                            new Notification
                            {
                                Content = "已删除。",
                                Title = "提示"
                            }
                        );
                        Suppliers.Remove(supplier);
                        SuppliersView.Refresh();
                        TotalRecords -= 1;
                        TotalPages = (int)Math.Ceiling(1.0 * TotalRecords / PageSize);

                        if (TotalPages == 0)
                        {
                            PageIndex = 0;
                        }
                        else if (PageIndex > TotalPages)
                        {
                            PageIndex = TotalPages;
                            SuppliersView = CollectionViewSource.GetDefaultView(Suppliers.Skip((PageIndex - 1) * PageSize).Take(PageSize));
                        }
                    }
                }
            );
        }

        private bool CanDelete()
        {
            return CanExecute();
        }

        private void Detail()
        {
            var supplier = SuppliersView.CurrentItem as Supplier;

            using (var context = new MasterDataContext())
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
