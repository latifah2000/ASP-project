
var gid;
function AddNewItems() {
    $('#newproducts').modal('show');
}

function AddNewDemag() {
    $('#demag').modal('show');
}


function update(id) {
    var gid = document.getElementById("Id");
    var name = document.getElementById("Name");
    var description = document.getElementById("Description");

    $.ajax({

        url: '/Home/GetData',
        type: 'POST',
        data: { id: id },

        success: function (result) {
            gid.value = result.id;
            name.value = result.name;
            description.value = result.description;

            console.table(result);
        }


    })


    $('#update').modal('show');
}


function ShowDelMessage(id) {
    gid = id;
    $('#confirm').modal('show');
}

function AddProDtil() {
    $('#addproducts').modal('show');
}



function updateDetail(id) {
    var gid = document.getElementById("Id");
    var ph = document.getElementById("photo");
    var price = document.getElementById("Price");
    var qty = document.getElementById("Qty");
    var color = document.getElementById("Color");
    var name = document.getElementById("ProductId");

    $.ajax({

        url: '/Home/GetDetails',
        type: 'POST',
        data: { id : id },

        success: function (result) {
            gid.value = result.id;
            ph.value = result.ph;
            price.value = result.price;
            qty.value = result.qty;
            color.value = result.color;
            name.value = result.name;

            console.table(result);
        }


    })


    $('#updateDetail').modal('show');
}

function Confirmdel() {
    $.ajax
        ({
            url: '/Home/Delete',
            type: 'POST',
            data: { record: gid },

            success: function (result) {

                window.location.href = '/Home/AddNewitems';
            }

        });
}

function ConfirmDetails() {
    $.ajax
        ({
            url: '/Home/DeleteDetails',
            type: 'POST',
            data: { id: gid },

            success: function (result) {

                window.location.href = '/Home/ProductDetails';
            }

        });
}

