using OnIndustri.Infrastructure;
using OnIndustri.Infrastructure.Constants;
using OnIndustri.Infrastructure.Helpers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OnIndustri.Navigation.Views
{
    public class NavigationMenuViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;
        public DelegateCommand<DependencyObject> NavigateCommand { get; private set; }

        public NavigationMenuViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            NavigateCommand = new DelegateCommand<DependencyObject>(Navigate);
        }

        private void Navigate(DependencyObject originalSource)
        {
            var item = VisualTreeHelperExtensions.FindAncestor<TreeViewItem>(originalSource);
            if (item != null && item.Tag != null)
            {
                var navigatePath = item.Tag.ToString();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, navigatePath);
            }
        }
    }
}
