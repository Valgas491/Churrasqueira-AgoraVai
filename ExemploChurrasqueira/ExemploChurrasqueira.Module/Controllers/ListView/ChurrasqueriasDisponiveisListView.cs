using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace ExemploChurrasqueira.Module.Controllers.ListView
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ChurrasqueriasDisponiveisListView : ViewController<DevExpress.ExpressApp.ListView>
    {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public ChurrasqueriasDisponiveisListView()
        {
            this.TargetViewId = "ReservaChurrasqueira_ChurrasqueirasDisponiveis_ListView";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            Action(false);
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
        private void Action(bool desabilitar)
        {
            var newAction = Frame.GetController<NewObjectViewController>()?.NewObjectAction;
            newAction?.Active.SetItemValue("ListView", false);

            var linkAction = Frame.GetController<LinkUnlinkController>()?.LinkAction;
            linkAction?.Active.SetItemValue("ListView", false);

            var unlinkAction = Frame.GetController<LinkUnlinkController>()?.UnlinkAction;
            unlinkAction?.Active.SetItemValue("ListView", false);

            var deleteAction = Frame.GetController<DeleteObjectsViewController>()?.DeleteAction;
            deleteAction?.Active.SetItemValue("ListView", false);

            var searchAction = Frame.GetController<FilterController>()?.FullTextFilterAction;
            searchAction?.Active.SetItemValue("ListView", false);

            var exportAction = Frame.GetController<ExportController>()?.ExportAction;
            exportAction?.Active.SetItemValue("ListView", false);
        }
    }
}
