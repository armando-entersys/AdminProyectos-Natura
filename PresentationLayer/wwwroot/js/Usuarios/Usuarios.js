var estatusActivo = { IdEstatus: true, Estatus: "Activo" };
var estatusInactivo = { IdEstatus: false, Estatus: "Inactivo" };

var catEstatus = [estatusActivo, estatusInactivo];
function AppViewModel() {
    var self = this;

    self.registros = ko.observableArray();

    self.id = ko.observable(0);
    self.nombre = ko.observable().extend({ required: true });
    self.correo = ko.observable().extend({ required: true, email: true });

    self.catEstatus = ko.observableArray();
    self.Estatus = ko.observable().extend({ required: true });
    self.catRoles = ko.observableArray();
    self.rol = ko.observable().extend({ required: true });


    self.inicializar = function () {
        $.ajax({
            url: "/Usuarios/GetAll", // URL del método GetAll en tu API
            type: "GET",
            contentType: "application/json",
            success: function (d) {
                self.registros.removeAll();
                self.registros.push.apply(self.registros, d.datos);
                self.catEstatus.removeAll();
                self.catEstatus.push.apply(self.catEstatus, catEstatus);
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                alert("Error al obtener los datos: " + xhr.responseText);
            }
        });
           
    }
    self.inicializar();

    self.fullName = ko.pureComputed(function () {
        return self.firstName() + " " + self.lastName();
    }, self);
}

// Activar Knockout.js
ko.applyBindings(new AppViewModel());