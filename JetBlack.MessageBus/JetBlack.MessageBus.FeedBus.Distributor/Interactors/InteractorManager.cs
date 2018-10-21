using System;
using log4net;

namespace JetBlack.MessageBus.FeedBus.Distributor.Interactors
{
    public class InteractorManager : IInteractorManager
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IInteractorRepository _repository;

        public InteractorManager()
        {
            _repository = new InteractorRepository();
        }

        public event EventHandler<InteractorClosedEventArgs> ClosedInteractors;
        public event EventHandler<InteractorFaultedEventArgs> FaultedInteractors;

        public void AddInteractor(IInteractor interactor)
        {
            Log.Info($"Adding interactor: {interactor}");

            _repository.Add(interactor);
        }

        public void CloseInteractor(IInteractor interactor)
        {
            Log.Info($"Closing interactor: {interactor}");

            _repository.Remove(interactor);
            ClosedInteractors?.Invoke(this, new InteractorClosedEventArgs(interactor));
        }

        public void FaultInteractor(IInteractor interactor, Exception error)
        {
            Log.Info($"Faulting interactor: {interactor}");

            _repository.Remove(interactor);
            FaultedInteractors?.Invoke(this, new InteractorFaultedEventArgs(interactor, error));
        }

        public void Dispose()
        {
            Log.Debug($"Disposing all interactors.");
            _repository.Dispose();
        }
    }
}
