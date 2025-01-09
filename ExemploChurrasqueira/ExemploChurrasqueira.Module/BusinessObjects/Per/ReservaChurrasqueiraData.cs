using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using ExemploChurrasqueira.Module.BusinessObjects.NoPer;

namespace ExemploChurrasqueira.Module.BusinessObjects.Per
{
    [DefaultClassOptions]
    [ModelDefault("Caption", "Reserva Churrasqueira Pela Data")]
    [ImageName("BO_Scheduler")]
    [Appearance("ManutencaoCor", Criteria = "IsManutencao = true",
    BackColor = "LightYellow", FontColor = "Red", Priority = 1)]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class ReservaChurrasqueiraData : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public ReservaChurrasqueiraData(Session session)
            : base(session)
        {
        }
        Churrasqueira churrasqueira;
        GerenciarChurrasqueira gerenciarChurrasqueira;
        string associado;
        string npf;

        // Propriedades existentes
        DateTime dataReserva;
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy}")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        [RuleRequiredField]
        public DateTime DataReserva
        {
            get => dataReserva;
            set => SetPropertyValue(nameof(DataReserva), ref dataReserva, value);
        }

        int qtdPessoas;
        [ModelDefault("Caption", "Quantidade de Pessoas")]
        public int QtdPessoas
        {
            get => qtdPessoas;
            set => SetPropertyValue(nameof(QtdPessoas), ref qtdPessoas, value);
        }

        [NonPersistent]
        [Browsable(false)]
        public List<Churrasqueira> ChurrasqueirasDisponiveis { get; set; } = new List<Churrasqueira>();

        [Association("Churrasqueira-ReservaChurrasqueiras")]
        [ModelDefault("Caption", "Churrasqueiras Disponíveis")]
        [DataSourceProperty(nameof(ChurrasqueirasDisponiveis))]
        public Churrasqueira Churrasqueira
        {
            get => churrasqueira;
            set => SetPropertyValue(nameof(Churrasqueira), ref churrasqueira, value);
        }

        // Adicionada a associação com gerenciamento de churrasqueira
        [Browsable(false)] // Não exibe na interface
        [Association("ReservaChurrasqueira-GerenciarChurrasqueiras")]
        public GerenciarChurrasqueira GerenciarChurrasqueira
        {
            get => gerenciarChurrasqueira;
            set => SetPropertyValue(nameof(GerenciarChurrasqueira), ref gerenciarChurrasqueira, value);
        }

        // Propriedade para diferenciar reservas de manutenção
        [Browsable(false)] // Não exibe na interface
        public bool IsManutencao
        {
            get => GetPropertyValue<bool>(nameof(IsManutencao));
            set => SetPropertyValue(nameof(IsManutencao), value);
        }

        public string Associado
        {
            get => associado;
            set => SetPropertyValue(nameof(Associado), ref associado, value);
        }

        public string Npf
        {
            get => npf;
            set => SetPropertyValue(nameof(Npf), ref npf, value);
        }

        Socio socio;
        [NonPersistent]
        [XafDisplayName("Tetste")]
        public Socio Socio
        {
            get { return socio; }
            set => SetPropertyValue(nameof(Socio), ref socio, value);

        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if (qtdPessoas == 0)
                throw new UserFriendlyException("Quantidade de Pessoas Invalida");


            // Verificar se já existe reserva na data e churrasqueira especificada
            if (!IsManutencao) // Apenas para reservas normais
            {
                var reservaExistente = Session.FindObject<ReservaChurrasqueiraData>(
                    DevExpress.Data.Filtering.CriteriaOperator.Parse("churrasqueira = ? AND DataReserva = ?", churrasqueira, DataReserva));
                if (reservaExistente != null && reservaExistente.Oid != Oid)
                {
                    throw new UserFriendlyException("Já contém uma reserva para a churrasqueira na data informada.");
                }
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