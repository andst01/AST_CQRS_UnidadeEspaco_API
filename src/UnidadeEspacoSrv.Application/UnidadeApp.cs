using AutoMapper;
using FluentValidation.Results;
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
    /// UnidadeApp é a classe de aplicação responsável por gerenciar as operações relacionadas à entidade Unidade. Ela utiliza o AutoMapper para mapear os objetos de ViewModel para os comandos correspondentes e o MediatR para enviar esses comandos para os handlers apropriados. A classe implementa a interface IUnidadeApp, que define os métodos para adicionar, atualizar e excluir unidades, bem como métodos específicos para obter unidades com seus espaços associados. Cada método recebe um objeto UnidadeViewModel, mapeia para o comando correspondente e envia o comando usando o MediatR, retornando o resultado da operação ou os dados solicitados.
    /// </summary>
    public class UnidadeApp : AppBase<Unidade, 
                                      UnidadeRequest, 
                                      UnidadeViewModel, 
                                      UnidadeCreateCommand, 
                                      UnidadeUpdateCommand, 
                                      UnidadeDeleteCommand, 
                                      UnidadeNotification>, IUnidadeApp
    {
        private readonly IUnidadeMongoDbRepository _repository;
        private readonly IMapper _mapper;   


        public UnidadeApp(IMapper mapper, 
                         IMediatorHandler mediator,
                         IUnidadeMongoDbRepository repository) 
            : base(mapper, mediator, repository, "Unidade")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// ObterTodasUnidadeComEspaco é um método assíncrono que retorna uma lista de objetos UnidadeViewModel, cada um contendo informações sobre uma unidade e seu espaço associado. Ele utiliza o repositório para obter todas as unidades com seus espaços correspondentes, mapeia os resultados para a ViewModel usando o AutoMapper e retorna a lista resultante.
        /// </summary>
        /// <returns></returns>
        public async Task<List<UnidadeViewModel>> ObterTodasUnidadeComEspaco()
        {
            return _mapper.Map<List<UnidadeViewModel>>(await _repository.ObterTodasUnidadeComEspaco());
        }


        /// <summary>
        /// ObterUnidadeComEspacoPorIdAsync é um método assíncrono que recebe um identificador de unidade e retorna um objeto UnidadeViewModel contendo informações sobre a unidade correspondente e seu espaço associado. Ele utiliza o repositório para obter a unidade com seu espaço correspondente com base no identificador fornecido, mapeia o resultado para a ViewModel usando o AutoMapper e retorna o objeto resultante.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<UnidadeViewModel> ObterUnidadeComEspacoPorIdAsync(int id)
        {
            return _mapper.Map<UnidadeViewModel>(await _repository.ObterUnidadeComEspacoPorIdAsync(id));
        }

    }
}
