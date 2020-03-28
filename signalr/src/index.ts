import "./css/main.css";
import 'datatables.net-bs4/css/dataTables.bootstrap4.css';
import * as signalR from "@microsoft/signalr";

const $ = require( 'jquery' ); 
// Import datatables and the required plugins, using the bootstrap 4 styling
import 'datatables.net-bs4';

const divMessages: HTMLDivElement = document.querySelector("#divMessages");
const tbMessage: HTMLInputElement = document.querySelector("#tbMessage");
const btnSend: HTMLButtonElement = document.querySelector("#btnSend");
const username = new Date().getTime();

const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:44357/coronahub")
    .build();

connection.on("getAll", (all) => {
	let result=all;
	let spans=$('.maincounter-number span');
	spans[0].innerHTML=result.cases;
	spans[1].innerHTML=result.deaths;
	spans[2].innerHTML=result.recovered;
});
connection.on("getCountries", (all) => {
    let messages = document.createElement("div");

$("#main_table_countries_today").DataTable(
{ data:all,
"retrieve": true,
 "paging":   false,
        "info":     false,
		 "order": [[ 1, "desc" ]],
 "columns": [
            { "data": "country" },
            { "data": "cases" },
            { "data": "todayCases" },
            { "data": "deaths" },
            { "data": "todayDeaths" },
            { "data": "recovered" },
			{ "data": "active" },
            { "data": "critical" },
            { "data": "casesPerOneMillion" },
            { "data": "deathsPerOneMillion" },
			{ "data": "firstCase" }
			
        ]
});
});
connection.start().catch(err => document.write(err));
