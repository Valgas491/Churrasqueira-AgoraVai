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
using ExemploChurrasqueira.Module.BusinessObjects.Per;

namespace ExemploChurrasqueira.Module.Controllers.DetailView
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class GerenciarChurrsqueiraDetailView : ObjectViewController<DevExpress.ExpressApp.DetailView, GerenciarChurrasqueira>
    {
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public GerenciarChurrsqueiraDetailView()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            AoAlterarData();
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
        private void AoAlterarData()
        {

            if (View is DevExpress.ExpressApp.DetailView detailView)
            {
                var date = detailView.FindItem("DataManutencao") as PropertyEditor;
                if (date == null)
                {
                    MostrarToast("Campo de data não encontrado.", InformationType.Error);
                    return;
                }

                date.ControlValueChanged += (sender, evento) =>
                {
                    // Atualiza o valor no objeto
                    date.WriteValue();

                    // Obtém o valor atualizado da data
                    var novaData = date.PropertyValue as DateTime?;
                    if (!novaData.HasValue)
                    {
                        MostrarToast("Data inválida!", InformationType.Error);
                        return;
                    }

                    // Obtém o objeto atual
                    var gerenciarChurrasqueira = View.CurrentObject as GerenciarChurrasqueira;
                    var objectSpace = View.ObjectSpace;

                    // Verifica churrasqueiras disponíveis
                    var churrasqueirasDisponiveis = ObterChurrasqueirasDisponiveis(objectSpace, novaData.Value);

                    if (churrasqueirasDisponiveis.Any())
                    {
                        // Atualiza a lista transitória
                        gerenciarChurrasqueira.ChurrasqueirasDisponiveis.Clear();
                        gerenciarChurrasqueira.ChurrasqueirasDisponiveis.AddRange(churrasqueirasDisponiveis);
                        MostrarToast($"{churrasqueirasDisponiveis.Count} churrasqueira(s) disponível(is).", InformationType.Success);
                    }
                    else
                    {
                        MostrarToast("Nenhuma churrasqueira disponível na data selecionada.", InformationType.Warning);
                    }

                    // Atualiza a interface
                    View.Refresh();
                };
            }
        }

        private List<Churrasqueira> ObterChurrasqueirasDisponiveis(IObjectSpace objectSpace, DateTime dataReserva)
        {
            // Busca churrasqueiras já reservadas ou em manutenção para a data fornecida
            var churrasqueirasIndisponiveis = objectSpace.GetObjects<ReservaChurrasqueiraData>(
                CriteriaOperator.Parse("(DataReserva = ? AND IsManutencao = false) OR (DataReserva = ? AND IsManutencao = true)",
                dataReserva, dataReserva))
                .Select(r => r.Churrasqueira).Distinct().ToList();

            // Busca todas as churrasqueiras
            var todasChurrasqueiras = objectSpace.GetObjects<Churrasqueira>();

            // Filtra as disponíveis
            var churrasqueirasDisponiveis = todasChurrasqueiras
                .Where(c => !churrasqueirasIndisponiveis.Contains(c))
                .ToList();

            return churrasqueirasDisponiveis;
        }

        private void MostrarToast(string mensagem, DevExpress.ExpressApp.InformationType tipo)
        {

            var showViewStrategy = Application.ShowViewStrategy as ShowViewStrategyBase;
            showViewStrategy?.ShowMessage(mensagem, tipo); // Tipos: "success", "error", "warning", "info"
        }

    }
}
