using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace ExemploChurrasqueira.Module.BusinessObjects.Per
{
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class GerenciarChurrasqueira : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public GerenciarChurrasqueira(Session session)
            : base(session)
        {
        }

        Churrasqueira churrasqueira;
        DateTime dataManutencao;


        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        [ModelDefault("Caption", "Data Da Manutenção")]
        [RuleRequiredField(DefaultContexts.Save)]
        public DateTime DataManutencao
        {
            get => dataManutencao;
            set => SetPropertyValue(nameof(DataManutencao), ref dataManutencao, value);
        }

        [NonPersistent]
        [Browsable(false)]
        public List<Churrasqueira> ChurrasqueirasDisponiveis { get; set; } = new List<Churrasqueira>();

        // Associação com a churrasqueira
        [Association("Churrasqueira-GerenciarChurrasqueiras")]
        [DataSourceProperty(nameof(ChurrasqueirasDisponiveis))]
        public Churrasqueira Churrasqueira
        {
            get => churrasqueira;
            set => SetPropertyValue(nameof(Churrasqueira), ref churrasqueira, value);
        }

        [Association("ReservaChurrasqueira-GerenciarChurrasqueiras")]
        [Browsable(false)]
        public XPCollection<ReservaChurrasqueiraData> reservaChurrasqueiras
        {
            get
            {
                return GetCollection<ReservaChurrasqueiraData>(nameof(reservaChurrasqueiras));
            }
        }

        // Status para controle da churrasqueira
        private TaskStatus status;
        public TaskStatus Status
        {
            get { return status; }
            set
            {
                status = value;
            }
        }

        // Método para alterar o status para manutenção
        [Action(ImageName = "State_Validation_Invalid")]
        public void MarkMaintance()
        {
            Status = TaskStatus.Maintance;
        }

        // Enum para o status
        public enum TaskStatus
        {
            [ModelDefault("Caption", "Em Manuntenção")]
            [ImageName("State_Validation_Invalid")]
            Maintance
        }
        // Método auxiliar para verificar se a reserva está sendo apagada
        private bool IsReservaDeleted()
        {
            return IsDeleted || IsDeleted && Churrasqueira == null;
        }
        protected override void OnSaving()
        {
            // Garante o comportamento padrão primeiro
            base.OnSaving();

            if (Churrasqueira != null && DataManutencao > DateTime.MinValue)
            {
                try
                {
                    // Busca uma reserva existente para a data informada
                    var reservaManutencao = Session.FindObject<ReservaChurrasqueiraData>(
                        CriteriaOperator.Parse("Churrasqueira.Oid = ? AND DataReserva = ? AND QtdPessoas = -1",
                        Churrasqueira.Oid, DataManutencao));

                    if (reservaManutencao == null) // Cria nova reserva de manutenção
                    {
                        var novaReserva = new ReservaChurrasqueiraData(Session)
                        {
                            Churrasqueira = Churrasqueira,
                            DataReserva = DataManutencao,
                            QtdPessoas = -1,
                            GerenciarChurrasqueira = this,
                            IsManutencao = true
                        };

                        // Adiciona a reserva na sessão (sem chamar Save diretamente)
                        Session.Save(novaReserva);
                    }
                    else
                    {

                        reservaManutencao.GerenciarChurrasqueira = this;
                        reservaManutencao.IsManutencao = true;
                        Session.Save(reservaManutencao);
                    }



                }
                catch (Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine($"Erro ao salvar reserva: {ex.Message}");
                    throw;
                }
            }
            else
            {
                if (IsReservaDeleted())
                {
                    return; // Ignora a validação, permitindo que o objeto seja removido
                }

                throw new UserFriendlyException("Você deve selecionar uma Data e Uma Churrasqueira.");
            }
        }

        protected override void OnDeleting()
        {
            base.OnDeleting();

            // Busca as reservas associadas
            var reservasAssociadas = new XPCollection<ReservaChurrasqueiraData>(Session,
                CriteriaOperator.Parse("GerenciarChurrasqueira.Oid = ?", Oid));

            foreach (var reserva in reservasAssociadas.ToList())
            {
                reserva.Delete();
            }


        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }
        //private string _PersistentProperty;
        //[XafDisplayName("My display name"), ToolTip("My hint message")]
        //[ModelDefault("EditMask", "(000)-00"), Index(0), VisibleInListView(false)]
        //[Persistent("DatabaseColumnName"), RuleRequiredField(DefaultContexts.Save)]
        //public string PersistentProperty {
        //    get { return _PersistentProperty; }
        //    set { SetPropertyValue(nameof(PersistentProperty), ref _PersistentProperty, value); }
        //}

        //[Action(Caption = "My UI Action", ConfirmationMessage = "Are you sure?", ImageName = "Attention", AutoCommit = true)]
        //public void ActionMethod() {
        //    // Trigger a custom business logic for the current record in the UI (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112619.aspx).
        //    this.PersistentProperty = "Paid";
        //}
    }
}