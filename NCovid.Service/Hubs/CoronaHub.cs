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
                var countries = await _coronaVirus.GetCountriesData(search);
                await Clients.Caller.SendAsync("getCountries", countries);
            }
            else
            {
                var countries = await _coronaVirus.GetCountriesData();
                await Clients.Caller.SendAsync("getCountries", countries);
            }
        }

        public async Task GetAll()
        {
            var all = await _coronaVirus.GetAllData();
            await Clients.Caller.SendAsync("getAll", all);
        }

        public override async Task OnConnectedAsync()
        {
            
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
