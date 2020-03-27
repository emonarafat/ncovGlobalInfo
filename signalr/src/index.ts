import "./css/main.css";
import * as signalR from "@microsoft/signalr";

const divMessages: HTMLDivElement = document.querySelector("#divMessages");
const tbMessage: HTMLInputElement = document.querySelector("#tbMessage");
const btnSend: HTMLButtonElement = document.querySelector("#btnSend");
const username = new Date().getTime();

const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:44357/coronahub")
    .build();

connection.on("getAll", (all) => {
let result=all.result;
 var liElement = document.createElement('li');
                liElement.innerHTML = '<strong> Cases :</strong>' + result.cases + '<strong> Deaths :</strong>' + result.deaths + '<strong> Recovered :</strong>' + result.recovered ;
                document.getElementById('discussion').appendChild(liElement);

});
connection.on("getCountries", (all) => {
    let messages = document.createElement("div");


});
connection.start().catch(err => document.write(err));

tbMessage.addEventListener("keyup", (e: KeyboardEvent) => {
    if (e.key === "Enter") {
        send();
    }
});

btnSend.addEventListener("click", send);

function send() {

}