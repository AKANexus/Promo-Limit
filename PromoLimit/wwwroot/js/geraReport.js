var myHeaders = new Headers();
var dataInicioField;
var dataFimField;
var tableBod;
var gerarButton;
var workingMessage;

$(document).ready(function () {
    myHeaders.append("Content-Type", "application/json");
    dataInicioField = document.getElementById("dataInicioField");
    dataFimField = document.getElementById("dataFimField");
    tableBod = document.getElementById("tableBody");
    gerarButton = document.getElementById("gerarRelatorioButton");
    workingMessage = document.getElementById("workingMessage");
});

async function gerarRelatorio() {
    gerarButton.disabled = true;
    workingMessage.removeAttribute("hidden");
    const requestOptions = {
        method: "GET",
        headers: myHeaders
    };


    const get = await fetch(`/Report/GerarRelatorio?dataInicio=${dataInicioField.value}&dataFim=${dataFimField.value}`, requestOptions);
    if (get.status >= 400) {
        alert("Deu ruim");
        return;
    }
    const receivedData = JSON.parse(await get.text());
    DrawReport(receivedData);
}

function DrawReport(data) {
    tableBod.innerHTML = null;
    if (data.length === 0) {
        alert("Nenhum dado encontrado");
    } else {
        data.map(element => {
            tableBod.insertAdjacentHTML("beforeend",
                `<tr><td>${element.vendedor}</td><td>${element.mlb}</td><td>${element.descricao}</td><td>${element.vendidos}</td></tr>`
            );

        });
    }
    gerarButton.disabled = false;
    workingMessage.setAttribute("hidden", true);
}