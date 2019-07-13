using OnIndustri.MasterData.Views;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnIndustri.MasterData
{
    public class MasterDataModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SupplierCreateUpdate>();
            containerRegistry.RegisterForNavigation<SupplierRetrieve>();
            containerRegistry.RegisterForNavigation<SupplierDetail>();
        }
    }
}
