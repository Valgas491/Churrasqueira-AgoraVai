using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using ExemploChurrasqueira.Module.Helper;

namespace ExemploChurrasqueira.Module;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
public sealed class ExemploChurrasqueiraModule : ModuleBase {
    public ExemploChurrasqueiraModule() {
		// 
		// ExemploChurrasqueiraModule
		// 
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.ValidationModule));
    }
    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
        ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
        return new ModuleUpdater[] { updater };
    }
    public override void Setup(XafApplication application) {
        base.Setup(application);
        // Manage various aspects of the application UI and behavior at the module level.
        application.SetupComplete += ConfiguracaoCompletaAplicacao;
        application.ObjectSpaceCreated += Application_ObjectSpaceCreated1;
    }
    private void ConfiguracaoCompletaAplicacao(object sender, EventArgs e)
    {
        Application.ObjectSpaceCreated += EspacoCriacaoObjetos;
    }

    private void EspacoCriacaoObjetos(object sender, ObjectSpaceCreatedEventArgs e)
    {
        //Ao criar o objectSpace e se for do tipo NonPersistentObjectSpace, trata o evento de get.
        var nonPersistentObjectSpace = e.ObjectSpace as NonPersistentObjectSpace;
        if (nonPersistentObjectSpace != null)
        {
            nonPersistentObjectSpace.ObjectsGetting += EspacoConfiguracaoObjetos;
        }
    }

    private void EspacoConfiguracaoObjetos(object sender, ObjectsGettingEventArgs evento)
    {
        IObjectSpace objSpace = Application.CreateObjectSpace();
        ModuleHelperXaf.ConfigurarSocios(sender, evento, objSpace);
    }

    private void Application_ObjectSpaceCreated1(object sender, ObjectSpaceCreatedEventArgs e)
    {
        CompositeObjectSpace compositeObjectSpace = e.ObjectSpace as CompositeObjectSpace;
        if (compositeObjectSpace != null)
        {
            if (!(compositeObjectSpace.Owner is CompositeObjectSpace))
            {
                compositeObjectSpace.PopulateAdditionalObjectSpaces((XafApplication)sender);
            }
        }
    }
    public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
        base.CustomizeTypesInfo(typesInfo);
        CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
    }
}
