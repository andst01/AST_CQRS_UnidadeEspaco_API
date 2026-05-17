using AutoMapper;
using UnidadeEspacoSrv.Application.Commnds;
using UnidadeEspacoSrv.Application.Interfaces;
using UnidadeEspacoSrv.Application.Request;
using UnidadeEspacoSrv.Application.ViewModel;
using UnidadeEspacoSrv.Domain.Entities;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces;
using UnidadeEspacoSrv.Domain.Interfaces.MongoDb;

namespace UnidadeEspacoSrv.Application
{
    /// <summary>
    /// EspacoApp é a classe de aplicação responsável por gerenciar as operações relacionadas à entidade Espaco. Ela utiliza o AutoMapper para mapear os objetos de ViewModel para os comandos correspondentes e o MediatR para enviar esses comandos para os handlers apropriados. A classe implementa a interface IEspacoApp, que define os métodos para adicionar, atualizar e excluir espaços. Cada método recebe um objeto EspacoViewModel, mapeia para o comando correspondente e envia o comando usando o MediatR, retornando o resultado da operação.
    /// </summary>
    public class EspacoApp : AppBase<Espaco,
                                    EspacoRequest,
                                    EspacoViewModel,
                                    EspacoCreateCommand,
                                    EspacoUpdateCommand,
                                    EspacoDeleteCommand,
                                    EspacoNotification>, IEspacoApp
    {
        private readonly IEspacoMongoDbRepository _repository;
        private readonly IMapper _mapper;

        public EspacoApp(IMapper mapper, 
                        IMediatorHandler mediator, 
                        IEspacoMongoDbRepository repository) : base(mapper, mediator, repository, "Espaco")
        {
            _repository = repository;
            _mapper = mapper;

        }

        /// <summary>
        /// ObterEspacosComUnidadesAsync é um método assíncrono que retorna uma lista de objetos EspacoViewModel, cada um contendo informações sobre um espaço e suas unidades associadas. Ele utiliza o repositório para obter os dados dos espaços com suas unidades e, em seguida, mapeia esses dados para a estrutura de visualização (ViewModel) usando o AutoMapper. O resultado é uma lista de EspacoViewModel que pode ser utilizada para exibir as informações dos espaços e suas unidades em uma interface de usuário ou para outras finalidades dentro da aplicação.
        /// </summary>
        /// <returns></returns>
        public async Task<List<EspacoViewModel>> ObterEspacosComUnidadesAsync()
        {
            return _mapper.Map<List<EspacoViewModel>>(await _repository.ObterEspacosComUnidadesAsync());        
        }
    }
}
