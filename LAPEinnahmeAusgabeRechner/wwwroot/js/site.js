document.getElementById("tableDefaultOpen").click();
document.getElementById("chartDefaultOpen").click();

window.onload = function () {
    showEinträge('einnahme', 10);
    showEinträge('ausgabe', 10);
}


function openTab(evt, tabId, contentClass, linkClass) {
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName(contentClass);
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName(linkClass);
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(tabId).style.display = "block";
    evt.currentTarget.className += " active";
}

function searchTable(inputId, tableId) {
    var input, filter, table, tr, td, i, txtValue;
    input = document.getElementById(inputId);
    filter = input.value.toUpperCase();
    table = document.getElementById(tableId);
    tr = table.getElementsByTagName("tr");

    for (i = 0; i < tr.length; i++) {
        td = tr[i].getElementsByTagName("td")[0];
        if (td) {
            txtValue = td.textContent || td.innerText;
            if (txtValue.toUpperCase().indexOf(filter) > -1) {
                tr[i].style.display = "";
            } else {
                tr[i].style.display = "none";
            }
        }
    }
}

function showEinträge(tablename, entriesToShow) {
    var table = document.getElementById(tablename + 'Table').getElementsByTagName('tbody')[0];
    var rows = table.getElementsByTagName('tr');
    var totalRows = rows.length;
    var displayCount = entriesToShow === 'alle' ? totalRows : parseInt(entriesToShow);

    for (var i = 0; i < totalRows; i++) {
        if (i < displayCount) {
            rows[i].style.display = '';
        } else {
            rows[i].style.display = 'none';
        }
    }
}

