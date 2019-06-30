using OnIndustri.Infrastructure.Helpers;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace OnIndustri.Views
{
    public class CloseTabAction : TriggerAction<Button>
    {
        protected override void Invoke(object parameter)
        {
            var args = parameter as RoutedEventArgs;
            if (args == null)
                return;

            var tabitem = VisualTreeHelperExtensions.FindAncestor<TabItem>(args.OriginalSource as DependencyObject);
            if (tabitem == null)
                return;

            var tabcontrol = VisualTreeHelperExtensions.FindAncestor<TabControl>(tabitem);
            if (tabcontrol == null)
                return;

            IRegion region = RegionManager.GetObservableRegion(tabcontrol).Value;
            if (region == null)
                return;

            //tabcontrol.Items.Remove(tabitem.Content);
            RemoveItemFromRegion(tabitem.Content, region);
        }

        private void RemoveItemFromRegion(object item, IRegion region)
        {
            var navigationtionContext = new NavigationContext(region.NavigationService, null);
            if (CanRemove(item, navigationtionContext))
            {
                region.Remove(item);
            }
        }

        private bool CanRemove(object item, NavigationContext navigationContext)
        {
            bool canRemove = true;

            var confirmRequestItem = item as IConfirmNavigationRequest;
            if (confirmRequestItem != null)
            {
                confirmRequestItem.ConfirmNavigationRequest(navigationContext, result =>
                {
                    canRemove = result;
                });
            }

            var frameworkElement = item as FrameworkElement;
            if (frameworkElement != null && canRemove)
            {
                IConfirmNavigationRequest confirmRequestDataContext = frameworkElement.DataContext as IConfirmNavigationRequest;
                if (confirmRequestDataContext != null)
                {
                    confirmRequestDataContext.ConfirmNavigationRequest(navigationContext, result =>
                    {
                        canRemove = result;
                    });
                }
            }

            return canRemove;
        }
    }
}
