using DevExpress.ExpressApp;
using ExemploChurrasqueira.Module.BusinessObjects;
using ExemploChurrasqueira.Module.BusinessObjects.NoPer;
using Newtonsoft.Json;
using RestSharp;

namespace ExemploChurrasqueira.Module.Helper
{
    public class ModuleHelperXaf
    {
        public static void ConfigurarSocios(object sender, ObjectsGettingEventArgs evento, IObjectSpace objectSpace)
        {
            try
            {
                if (evento.ObjectType != typeof(Socio))
                    return;

                var client = new RestClient("https://localhost:7015");
                var requisicao = new RestRequest("/api/Pessoa/socios", Method.Get);
                requisicao.AddHeader("Cache-Control", "no-cache");
                requisicao.AddHeader("Content-Type", "application/json");

                var response = client.Execute(requisicao);

                if (!response.IsSuccessful)
                {           
                    throw new Exception("ERRO");
                }

                var sociosJson = response.Content;
                var socios = JsonConvert.DeserializeObject<List<Socio>>(sociosJson);

                var objects = socios.Select(s => new Socio
                {
                    Id = s.Oid,
                    Nome = s.Nome,
                    Npf = s.Npf
                }).ToList();

                evento.Objects = objects;
            }
            catch (Exception error)
            {
                
                Console.WriteLine(error.Message);
            }
        }

    }
}
