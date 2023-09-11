var gravarNovoModal;
var myHeaders = new Headers();
var produtoIdField;
var mlbField;
var quantAVendaField;
var estoqueField;
var query = null;
var queryField;

$(document).ready(function () {
    myHeaders.append("Content-Type", "application/json");
    GetData();
    gravarNovoModal = new bootstrap.Modal(document.getElementById("gravarNovoModal"), {
        keyboard: false
    });
    produtoIdField = document.getElementById("produtoIdField");
    mlbField = document.getElementById("skuField");
    quantAVendaField = document.getElementById("quantReservaField");
    estoqueField = document.getElementById("estoqueField");
    queryField = document.getElementById("queryText");
});

async function GetData(pageNum = 1) {
    const requestOptions = {
        method: "GET",
        headers: myHeaders
    };

    const get = await fetch(`/Home/GetDataPaged?recordsPerPage=15&pageNumber=${pageNum}&query=${query}`, requestOptions);
    if (get.status >= 400) {
        alert("Deu ruim");
        return;
    } else {
        const receivedData = JSON.parse(await get.text());
        DrawTabela(receivedData);
    }

}

function setQuery() {
    query = queryField.value;
    GetData();
}

function clearQuery() {
    queryField.value = null;
    query = null;
    GetData();
}

function DrawTabela(data) {
    const tabela = document.getElementById("tableBody");
    tabela.innerHTML = null;
    data.returnedData.map(element => {
        tabela.insertAdjacentHTML("beforeend", `<tr><td>${element.seller}</td><td>${element.mlb}</td><td>${element.descricao}</td><td>${element.quantidadeAVenda}</td><td>${element.estoque}</td><td style="white-space: nowrap;"><button class="btn btn-warning" onclick="editRow(${element.id}, '${element.mlb}', ${element.quantidadeAVenda}, ${element.estoque})">E</button><button class="btn btn-danger" onclick="deleteCurrentRow(${element.id}, '${element.descricao}')">D</button></td></tr>`);
    });
    AtualizaPaginacao(data.currentPage, data.maxPages, data.recordsTotal, data.recordsPerPage);
}


function AtualizaPaginacao(pagAtual, maxPags, maxRecords, recordsPerPage) {
    const paginacaoArea = document.getElementById("paginationButtons");
    const recordNumbers = document.getElementById("recordNumbering");
    recordNumbers.innerHTML = null;
    paginacaoArea.innerHTML = null;
    maxPage = maxPags;
    //recordNumbers.innerText = `Exibindo ${Math.min(maxRecords, recordsPerPage)} resultados de ${maxRecords}`;

    if (pagAtual === 1) {
        paginacaoArea.insertAdjacentHTML("beforeend", '<li class="disabled"><span style="font-size:1.5em;text-decoration:none;cursor:default">&#9198;</span></li>');
        paginacaoArea.insertAdjacentHTML("beforeend", '<li class="disabled"><span style="font-size:1.5em;text-decoration:none;cursor:default">&#9194;</span></li>');
    } else {
        paginacaoArea.insertAdjacentHTML("beforeend", `<li><a href=# onClick=GetData() style="font-size:1.5em;text-decoration:none">&#9198;</a></li>`);
        paginacaoArea.insertAdjacentHTML("beforeend", `<li><a href=# onClick=GetData(${pagAtual - 1}) style="font-size:1.5em;text-decoration:none">&#9194;</a></li>`);
    }
    paginacaoArea.insertAdjacentHTML("beforeend", `<input type="number" style="width:60px" value="${pagAtual}" id="pageSelectorField" />`);
    if (pagAtual === maxPags) {
        paginacaoArea.insertAdjacentHTML("beforeend", '<li class="disabled"><span style="font-size:1.5em;text-decoration:none;cursor:default">&#9193;</span></li>');
        paginacaoArea.insertAdjacentHTML("beforeend", '<li class="disabled"><span style="font-size:1.5em;text-decoration:none;cursor:default">&#9197;</span></li>');
    } else {
        paginacaoArea.insertAdjacentHTML("beforeend", `<li><a href=# onClick=GetData(${pagAtual + 1}) style="font-size:1.5em;text-decoration:none">&#9193;</a></li>`);
        paginacaoArea.insertAdjacentHTML("beforeend", `<li><a href=# onClick=GetData(${maxPags}) style="font-size:1.5em;text-decoration:none">&#9197;</a></li>`);
    }
    //const selector = document.getElementById("pageSelectorField");
    document.addEventListener("keydown", pageSelectorKeyDown);
}

function pageSelectorKeyDown(e) {
    if (e.code === "Enter" || e.code === "NumpadEnter") {
        const selector = document.getElementById("pageSelectorField");
        GetData(Math.min(selector.value, maxPage));
    }
}

async function addNewRow() {
    produtoIdField.value = null;
    mlbField.value = null;
    quantAVendaField.value = null;
    estoqueField.value = null;
    gravarNovoModal.show();
}

function acessarRelatorios() {
    var janela = window.open('Report', '_blank');
}

async function editRow(id, mlb, quantAVenda, estoque) {
    produtoIdField.value = id;
    mlbField.value = mlb;
    quantAVendaField.value = quantAVenda;
    estoqueField.value = estoque;
    gravarNovoModal.show();
}

function editCurrentRow(rowNum) {
    let row = document.querySelector(`#row${rowNum}`);
    let vendedor = document.querySelector(`#rowId${rowNum}`).innerText;
    let mlb = document.querySelector(`#rowMlb${rowNum}`).innerText;
    let des = document.querySelector(`#rowDes${rowNum}`).innerText;
    let qtd = document.querySelector(`#rowQtd${rowNum}`).innerText;
    let est = document.querySelector(`#rowEst${rowNum}`).innerText;

    row.innerHTML = '<div class="divTableCell" id="rowNewId"><input readonly value="' + vendedor + '"/></div><div class="divTableCell" id="rowNewMlb"><input readonly id="newMlb" value="' + mlb + '"/></div><div class="divTableCell" id="rowNewDes"><input readonly value="' + des + '"/></div><div class="divTableCell" id="rowNewQtd"><input id="newQtd" value="' + parseInt(qtd) + '"/></div><div class="divTableCell" id="rowNewEst"><input id="newEst" value="' + parseInt(est) + '"/></div><div class="divTableCell" id="rowNewAct"><a href="#" onclick="saveNew(' + rowNum + ')">Salvar</a></div>';
}

async function deleteCurrentRow(rowNum, descricao) {
    if (confirm(`Deseja mesmo remover o item ${descricao}? Isso não poderá ser desfeito!`)) {
        let requestOpts =
        {
            method: "DELETE",
            headers: myHeaders
        };

        let post = await fetch(`/Home/RemoveMlbEntry?id=${rowNum}`, requestOpts);
        let response = post.text();

        //var receivedData = await response;
        location.reload();
    }
}

async function gravarProduto(id = null) {
    let meuBody = {};
    meuBody.MLB = mlbField.value;
    meuBody.Id = produtoIdField.value.length === 0 ? null : produtoIdField.value;
    meuBody.QuantidadeAVenda = quantAVendaField.value;
    meuBody.Estoque = estoqueField.value;
    meuBody.Ativo = true;
    meuBody.Descricao = null;

    myHeaders.append("Content-Type", "application/json");

    let requestOpts =
    {
        method: "POST",
        headers: myHeaders,
        body: JSON.stringify(meuBody)
    };

    let post = await fetch("/Home/SaveMlbEntry", requestOpts);
    let response = post.text();

    //var receivedData = await response;
    location.reload();
}

