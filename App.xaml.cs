using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SVGRecolorTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Njg1NjkxQDMyMzAyZTMyMmUzMEhBWm1NZ2lOTW16WDdnRHcvcnZXRHgvVUc5dldVV1BmTXpjbURuWkJLOVU9;Njg1NjkyQDMyMzAyZTMyMmUzMEwwN2F6UUJnMTAyTWsyZFhzZUlHV2tSbEJXcDlCSmFBSVdoaEhUeDliaW89");
            base.OnStartup(e);
        }
    }
}
