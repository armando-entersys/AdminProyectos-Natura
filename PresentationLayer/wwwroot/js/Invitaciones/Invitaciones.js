function AppViewModel() {
    var self = this;

    self.registros = ko.observableArray();

    self.id = ko.observable(0);
    self.nombre = ko.observable().extend({ required: true });
    self.correo = ko.observable().extend({ required: true, email: true });
    self.apellidoPaterno = ko.observable().extend({ required: true });
    self.apellidoMaterno = ko.observable().extend({ required: true });
    self.contrasena = ko.observable();
   

 

    self.inicializar = function () {
        $.ajax({
            url: "/Invitaciones/GetAll", // URL del método GetAll en tu API
            type: "GET",
            contentType: "application/json",
            success: function (d) {
                self.registros.removeAll();
                self.registros.push.apply(self.registros, d.datos.$values);
               
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                alert("Error al obtener los datos: " + xhr.responseText);
            }
        });
           
    }
    self.inicializar();


    self.Aceptar = function (usuario) {

        $.ajax({
            url: "/Invitaciones/Aceptar/" + usuario.id, // URL del método GetAll en tu API
            type: "GET",
            contentType: "application/json",
            success: function (d) {
                self.inicializar();
                $('#alertMessage').text(d.mensaje);
             
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                $('#alertMessage').text("Error al obtener los datos: " + xhr.responseText);
                $('#alertModalLabel').text("Error");
                $("#alertModal").modal("show");
            }
        });
    }
    self.Rechazar = function (usuario) {
        $.ajax({
            url: "/Invitaciones/Rechazar/" + usuario.id, // URL del método GetAll en tu API
            type: "GET",
            contentType: "application/json",
            success: function (d) {
                self.inicializar();
                $('#alertMessage').text(d.mensaje);
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                $('#alertMessage').text("Error al obtener los datos: " + xhr.responseText);
                $('#alertModalLabel').text("Error");
                $("#alertModal").modal("show");
            }
        });
    }
}

// Activar Knockout.js
ko.applyBindings(new AppViewModel());