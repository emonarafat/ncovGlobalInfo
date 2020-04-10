namespace NCovid.Service.Hubs
{
    using Microsoft.AspNetCore.SignalR;

    using Services;

    using System;
    using System.Threading.Tasks;

    public class CoronaHub : Hub
    {
        private readonly ICoronaVirusService _coronaVirus;

        public CoronaHub(ICoronaVirusService coronaVirus)
        {
            _coronaVirus = coronaVirus;
            _coronaVirus.SaveInfo();
        }


        public async Task GetCountries(string search = "")
        {
            if (search.IsSet())
            {
                await Clients.Caller.SendAsync("getCountries", await _coronaVirus.GetCountriesData(search));
            }
            else
            {
                await Clients.Caller.SendAsync("getCountries", await _coronaVirus.GetCountriesData());
            }
        }

        public async Task GetAll()
        {
            await Clients.Caller.SendAsync("getAll", await _coronaVirus.GetAllData());
        }

        public override async Task OnConnectedAsync()
        {

            await Clients.Caller.SendAsync("getAll", await _coronaVirus.GetAllData());
            await Clients.Caller.SendAsync("getCountries", await _coronaVirus.GetCountriesData());
            await Groups.AddToGroupAsync(Context.ConnectionId, "Corona Users");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Corona Users");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
