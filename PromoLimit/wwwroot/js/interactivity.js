async function addNewRow() {
    if (newLineOpen === false) {
        let tabela = document.querySelector("#tableBod");
        tabela.innerHTML +=
            '<div class="divTableRow" id="rowNew"><div class="divTableCell" id="rowNewId"><input readonly></div><div class="divTableCell" id="rowNewMlb"><input id="newMlb"/></div><div class="divTableCell" id="rowNewDes"><input readonly/></div><div class="divTableCell" id="rowNewQtd"><input type="number" id="newQtd"/></div><div class="divTableCell" id="rowNewEst"><input type="number" id="newEst"/></div><div class="divTableCell" id="rowNewAct"><a href="#" onclick="saveNew(null)">Salvar</a></div></div>';
        //let requestOpts = { method: "GET", headers: myHeaders };
        //let get = await fetch(`/Home/GetSellers`, requestOpts);
        //let response = await get.text();
        //debugger;
        //let resposta = JSON.parse(response);
        //let selectElement = document.querySelector("#rowVendedorId");
        //for (var i = 0; i < resposta.length; i++) {
        //    let el = document.createElement("option");
        //    el.text = resposta[i].text;
        //    el.value = resposta[i].value;
        //    selectElement.appendChild(el);
        //}
        newLineOpen = true;
    }
}

function editCurrentRow(rowNum) {
    let row = document.querySelector(`#row${rowNum}`);
    let vendedor = document.querySelector(`#rowId${rowNum}`).innerText;
    let mlb = document.querySelector(`#rowMlb${rowNum}`).innerText;
    let des = document.querySelector(`#rowDes${rowNum}`).innerText;
    let qtd = document.querySelector(`#rowQtd${rowNum}`).innerText;
    let est = document.querySelector(`#rowEst${rowNum}`).innerText;

    row.innerHTML = '<div class="divTableCell" id="rowNewId"><input readonly value="'+vendedor+'"/></div><div class="divTableCell" id="rowNewMlb"><input readonly id="newMlb" value="'+mlb+'"/></div><div class="divTableCell" id="rowNewDes"><input readonly value="'+des+'"/></div><div class="divTableCell" id="rowNewQtd"><input id="newQtd" value="'+parseInt(qtd)+'"/></div><div class="divTableCell" id="rowNewEst"><input id="newEst" value="'+parseInt(est)+'"/></div><div class="divTableCell" id="rowNewAct"><a href="#" onclick="saveNew('+rowNum+')">Salvar</a></div>';
}

var myHeaders = new Headers();
myHeaders.append("Content-Type", "application/json");

async function deleteCurrentRow(rowNum) {


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

async function saveNew(id) {
    debugger;
    let myHeaders = new Headers();
    let meuBody = {};
    meuBody.MLB = document.querySelector("#newMlb").value;
    if (id)
        meuBody.Id = id;
    else {
        //meuBody.Seller = document.querySelector("#rowVendedorId").value;
        meuBody.Id = null;
    }
    meuBody.QuantidadeAVenda = document.querySelector("#newQtd").value;
    meuBody.Estoque = document.querySelector("#newEst").value;
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

var newLineOpen = false;