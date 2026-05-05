using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace UnidadeEspacoSrv.Domain.Entities
{
    public class HistoricoEventoEntry
    {
        public int Codigo { get; set; }
        public EntityEntry Entry { get; }
        public string NomeTabela { get; set; }
        public Dictionary<string, object> ValoresChaves { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> ValoresAntigos { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> ValoresNovos { get; set; } = new Dictionary<string, object>();
        public List<PropertyEntry> TemporaryProperties { get; set; } = new List<PropertyEntry>();
        public int CodigoUsuario { get; set; }
        public string TipoOperacao { get; set; }

        public bool HasTemporaryProperties => TemporaryProperties.Any();

        public HistoricoEventoEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public HistoricoEvento ToHistoricoEvento()
        {
            var audit = new HistoricoEvento();

            audit.Codigo = Codigo;
            audit.NomeTabela = NomeTabela;
            audit.DataCadastro = DateTime.UtcNow;
            audit.ValoresChaves = JsonConvert.SerializeObject(ValoresChaves);
            audit.ValoresAntigos = ValoresAntigos.Count == 0 ? null : JsonConvert.SerializeObject(ValoresAntigos);
            audit.ValoresNovos = ValoresNovos.Count == 0 ? null : JsonConvert.SerializeObject(ValoresNovos);
            audit.CodigoUsuario = CodigoUsuario;
            audit.TipoOperacao = TipoOperacao;

            return audit;
        }

        public HistoricoEvento ToHistoricoEventoUpdate(int Id)
        {
            var audit = new HistoricoEvento();

            audit.Id = Id;
            audit.Codigo = Codigo;
            audit.NomeTabela = NomeTabela;
            audit.DataCadastro = DateTime.UtcNow;
            audit.ValoresChaves = JsonConvert.SerializeObject(ValoresChaves);
            audit.ValoresAntigos = ValoresAntigos.Count == 0 ? null : JsonConvert.SerializeObject(ValoresAntigos);
            audit.ValoresNovos = ValoresNovos.Count == 0 ? null : JsonConvert.SerializeObject(ValoresNovos);
            audit.CodigoUsuario = CodigoUsuario;
            audit.TipoOperacao = TipoOperacao;

            return audit;
        }
    }
}
