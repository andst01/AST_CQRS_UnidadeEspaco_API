using AutoMapper;
using FluentValidation.Results;
using UnidadeEspacoSrv.Application.Interfaces;
using UnidadeEspacoSrv.Domain;
using UnidadeEspacoSrv.Domain.Events;
using UnidadeEspacoSrv.Domain.Interfaces;
using UnidadeEspacoSrv.Domain.Interfaces.MongoDb;

namespace UnidadeEspacoSrv.Application
{
    /// <summary>
    /// AppBase é uma classe genérica que implementa a interface IAppBase, fornecendo uma implementação base para operações comuns de CRUD (Create, Read, Update, Delete) em entidades do domínio. Ela utiliza o AutoMapper para mapear entre os tipos de request, view model e comandos, e o MediatR para enviar comandos e publicar eventos. A classe é projetada para ser reutilizada por diferentes entidades, permitindo que as operações de CRUD sejam facilmente implementadas em classes específicas de aplicação. Cada método da classe lida com a lógica de mapeamento, envio de comandos e publicação de eventos, garantindo uma separação clara de responsabilidades e facilitando a manutenção do código.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="TCommandCreate"></typeparam>
    /// <typeparam name="TCommandUpdate"></typeparam>
    /// <typeparam name="TCommandDelete"></typeparam>
    /// <typeparam name="TNotification"></typeparam>
    public class AppBase<TEntity,
                              TRequest,
                              TViewModel,
                              TCommandCreate,
                              TCommandUpdate,
                              TCommandDelete,
                              TNotification> :
                 IAppBase<TEntity,
                              TRequest,
                              TViewModel,
                              TCommandCreate,
                              TCommandUpdate,
                              TCommandDelete,
                              TNotification>
        where TEntity : EntityBase
        where TRequest : class
        where TViewModel : class
        where TCommandCreate : BaseCommand<TEntity>
        where TCommandUpdate : BaseCommand<TEntity>
        where TCommandDelete : BaseCommand<ValidationResult>
        where TNotification : EventBase
    {
        protected readonly string _collectionName;
        protected readonly IMapper _mapper;
        private readonly IMediatorHandler _mediator;
        private readonly IMongoDbBaseRepository<TNotification> _repository;

        public AppBase(IMapper mapper,
                       IMediatorHandler mediator,
                       IMongoDbBaseRepository<TNotification> repository,
                       string collectionName)
        {
            _mapper = mapper;
            _mediator = mediator;
            _repository = repository;
            _collectionName = collectionName;
        }

        /// <summary>
        /// Adicionar um novo registro, mapeando o request para o comando de criação e retornando a ViewModel correspondente.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TViewModel> AdicionarAsync(TRequest request)
        {
            //var command = _mapper.Map<TCommandCreate>(request);
           
            var command = (TCommandCreate)Activator.CreateInstance(typeof(TCommandCreate), request);
            
            var result = await _mediator.SendCommand<TCommandCreate, TEntity>(command);

            var response = (TViewModel)Activator.CreateInstance(typeof(TViewModel), result);
           

            return response;
        }

        /// <summary>
        /// Atualizar um registro existente, mapeando o request para o comando de atualização e retornando a ViewModel correspondente.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TViewModel> AtualizarAsync(TRequest request)
        {
            //var command = _mapper.Map<TCommandUpdate>(request);
        
            var command = (TCommandUpdate)Activator.CreateInstance(typeof(TCommandUpdate), request);

            var result = await _mediator.SendCommand<TCommandUpdate, TEntity>(command);

            var response = (TViewModel)Activator.CreateInstance(typeof(TViewModel), result);
            
            return response;
        }

        /// <summary>
        /// Excluir um registro por ID, retornando o resultado da operação de validação.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ValidationResult> ExcluirAsync(int id)
        {
            var command = Activator.CreateInstance<TCommandDelete>();
            command.Id = id;
            var response = await _mediator.SendCommand(command);
          
            return response;
        }

        /// <summary>
        /// Obter um registro por ID, mapeando para a ViewModel correspondente.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TViewModel> ObterPorIdAsync(int id)
        {
            // var response = (TViewModel)(dynamic)(await _repository.GetById(id, _collectionName));
            var response = Activator.CreateInstance<TViewModel>();

            if(response is INotificationMapper<TNotification> notificationMapper)
            {
                var notification = await _repository.GetById(id, _collectionName);
                notificationMapper.MapFromNotification(notification);
            }

            return response;
            //return _mapper.Map<TViewModel>(await _repository.GetById(id, _collectionName));  
        }

        /// <summary>
        /// Obter todos os registros da coleção, mapeando para a ViewModel correspondente.
        /// </summary>
        /// <returns></returns>
        public async Task<List<TViewModel>> ObterTodosAsync()
        {
            //var response = (List<TViewModel>)(dynamic)(await _repository.GetAll(_collectionName));
            //return response;

            //var response = new List<TViewModel>();
            //var notifications = await _repository.GetAll(_collectionName);
            //foreach (var notification in notifications)
            //{
            //    var viewModel = Activator.CreateInstance<TViewModel>();
            //    if (viewModel is INotificationMapper<TNotification> notificationMapper)
            //        notificationMapper.MapFromNotification(notification);
            //    response.Add(viewModel);
            //}

            //return response;

            var result = await _repository.GetAll(_collectionName);
            var response = result.Select(n => (TViewModel)Activator.CreateInstance(typeof(TViewModel), n)).ToList();

            return response;
            //return _mapper.Map<List<TViewModel>>(await _repository.GetAll(_collectionName));
        }
    }
}
