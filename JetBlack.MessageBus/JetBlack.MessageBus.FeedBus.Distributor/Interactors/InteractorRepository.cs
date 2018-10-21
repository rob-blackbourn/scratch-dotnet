using System;
using System.Collections.Generic;

namespace JetBlack.MessageBus.FeedBus.Distributor.Interactors
{
    public class InteractorRepository : IInteractorRepository
    {
        private readonly IDictionary<Guid, IInteractor> _interactors = new Dictionary<Guid, IInteractor>();

        public InteractorRepository()
        {
        }

        public void Add(IInteractor interactor)
        {
            _interactors.Add(interactor.Id, interactor);
        }

        public bool Remove(IInteractor interactor)
        {
            return _interactors.Remove(interactor.Id);
        }

        public void Dispose()
        {
            foreach (var interactor in _interactors.Values)
            {
                interactor.Dispose();
            }

            _interactors.Clear();
        }
    }
}
