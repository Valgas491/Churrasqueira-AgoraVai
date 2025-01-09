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
    public partial class VerificarDisponibilidadeController : ObjectViewController<DevExpress.ExpressApp.DetailView, ReservaChurrasqueiraData>
    {
        private List<DateTime> datasIndisponiveis = new();
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public VerificarDisponibilidadeController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            CarregarDatasIndisponiveis();

            // Configurar evento de alteração da data
            AoAlterarData();

            ConfigurarBotoes(false);
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
            base.OnDeactivated();
            ConfigurarBotoes(true);
        }
        private void ConfigurarBotoes(bool desabilitar)
        {
            Frame.GetController<ExportController>()?.ExportAction?.Active.SetItemValue("DetailView", desabilitar);
            Frame.GetController<RefreshController>()?.RefreshAction?.Active.SetItemValue("DetailView", desabilitar);
            Frame.GetController<ModificationsController>()?.SaveAndNewAction?.Active.SetItemValue("DetailView", desabilitar);
            Frame.GetController<ModificationsController>()?.SaveAndCloseAction?.Active.SetItemValue("DetailView", desabilitar);
            Frame.GetController<RecordsNavigationController>()?.PreviousObjectAction?.Active.SetItemValue("DetailView", desabilitar);
            Frame.GetController<RecordsNavigationController>()?.NextObjectAction?.Active.SetItemValue("DetailView", desabilitar);

            // Alterar legenda do botão Salvar
            var saveAction = Frame.GetController<ModificationsController>()?.SaveAction;
            if (desabilitar == false)
            {
                if (saveAction != null)
                {
                    saveAction.Caption = "Salvar Reserva";
                }
            }
        }

        private void CarregarDatasIndisponiveis()
        {
            // Busca todas as datas indisponíveis
            //var objectSpace = View.ObjectSpace;
            //datasIndisponiveis = objectSpace.GetObjects<ReservaChurrasqueiraData>()
            //    .Select(r => r.DataReserva.Date) // Apenas a data
            //    .Distinct()
            //    .ToList();
        }

        private void AoAlterarData()
        {
            if (View is DevExpress.ExpressApp.DetailView detailView)
            {
                var date = detailView.FindItem("DataReserva") as PropertyEditor;
                if (date == null)
                {
                    MostrarToast("Campo de data não encontrado.", DevExpress.ExpressApp.InformationType.Error);
                    return;
                }

                date.ControlValueChanged += (sender, evento) =>
                {
                    // Atualiza explicitamente o valor da propriedade no objeto subjacente
                    date.WriteValue();

                    // Pega o valor atualizado da propriedade DataReserva
                    var novaData = date.PropertyValue as DateTime?;
                    if (!novaData.HasValue)
                    {
                        MostrarToast("A data selecionada é inválida.", DevExpress.ExpressApp.InformationType.Error);
                        return;
                    }

                    // Pega o objeto atual
                    var reservaAtual = View.CurrentObject as ReservaChurrasqueiraData;
                    var objectSpace = View.ObjectSpace;

                    // Verifica se já existe reserva para a mesma churrasqueira e data
                    var reservaExistente = objectSpace.GetObjects<ReservaChurrasqueiraData>(
                        CriteriaOperator.Parse("churrasqueira = ? AND DataReserva = ?",
                            reservaAtual.Churrasqueira, novaData.Value)
                    ).FirstOrDefault();

                    if (reservaExistente != null && reservaExistente.Oid != reservaAtual.Oid) // Ignora a reserva atual em edição
                    {
                        MostrarToast("Já contém uma reserva para a churrasqueira na data informada.", DevExpress.ExpressApp.InformationType.Error);
                        return;
                    }

                    // Consulta as churrasqueiras disponíveis
                    var churrasqueirasDisponiveis = ObterChurrasqueirasDisponiveis(objectSpace, novaData.Value);

                    // Atualiza a lista transitória
                    AtualizarListaChurrasqueiras(churrasqueirasDisponiveis);
                    if (churrasqueirasDisponiveis.Any())
                    {
                        MostrarToast($"{churrasqueirasDisponiveis.Count} churrasqueira(s) disponível(is).", DevExpress.ExpressApp.InformationType.Success);
                    }
                    else
                    {
                        MostrarToast("Nenhuma churrasqueira disponível na data selecionada.", DevExpress.ExpressApp.InformationType.Warning);
                    }
                };
            }
        }

        private List<Churrasqueira> ObterChurrasqueirasDisponiveis(IObjectSpace objectSpace, DateTime dataReserva)
        {
            // Busca as churrasqueiras reservadas para a data fornecida
            var churrasqueirasReservadas = objectSpace.GetObjects<ReservaChurrasqueiraData>(
                CriteriaOperator.Parse("DataReserva = ?", dataReserva)
            ).Select(r => r.Churrasqueira).ToList();

            // Busca todas as churrasqueiras
            var todasChurrasqueiras = objectSpace.GetObjects<Churrasqueira>();

            // Filtra as churrasqueiras disponíveis
            var churrasqueirasDisponiveis = todasChurrasqueiras
                .Where(c => !churrasqueirasReservadas.Contains(c))
                .ToList();

            return churrasqueirasDisponiveis;
        }

        private void AtualizarListaChurrasqueiras(List<Churrasqueira> churrasqueirasDisponiveis)
        {
            if (View.CurrentObject is ReservaChurrasqueiraData reservaAtual)
            {
                reservaAtual.ChurrasqueirasDisponiveis.Clear();
                reservaAtual.ChurrasqueirasDisponiveis.AddRange(churrasqueirasDisponiveis);

                View.Refresh(); // Atualiza a interface
            }
        }

        private void MostrarToast(string mensagem, DevExpress.ExpressApp.InformationType tipo)
        {
            var showViewStrategy = Application.ShowViewStrategy as ShowViewStrategyBase;
            showViewStrategy?.ShowMessage(mensagem, tipo); // Tipos: "success", "error", "warning", "info"
        }

    }
}
