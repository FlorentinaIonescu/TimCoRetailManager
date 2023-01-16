using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        private IEventAggregator _events;
        private SalesViewModel _salesVM;
        private ILoggedInUserModel _user;

        // construction injection
        public ShellViewModel(IEventAggregator events, SalesViewModel salesVM, ILoggedInUserModel user)
        {
            _events = events;
            _salesVM = salesVM;
            _user = user;
            _events.Subscribe(this);

            ActivateItemAsync(IoC.Get<LoginViewModel>());

        }

        public Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            ActivateItemAsync(_salesVM);
            return Task.CompletedTask;
            NotifyOfPropertyChange(() => IsAccountVisible);
        }

        public bool IsLoggedIn()
        {
            get

            {
                bool output = false;

                if (string.IsNullOrWhiteSpace(_user.Token) == false)
                {
                    output = true;
                }

                return output;
            }
        }

        public void LogOut()
        {
            _user.LogOffUser();
            ActivateItemAsync(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsAccountVisible);
        }

        public void ExitApplication()
        {
            TryCloseAsync();
        }
    }
}
