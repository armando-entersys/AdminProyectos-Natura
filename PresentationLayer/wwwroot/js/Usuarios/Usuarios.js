    var estatusActivo = { IdEstatus: true, Estatus: "Activo" };
var estatusInactivo = { IdEstatus: false, Estatus: "Inactivo" };

var catEstatus = [estatusActivo, estatusInactivo];
function AppViewModel() {
    var self = this;

    self.registros = ko.observableArray();

    self.id = ko.observable(0);
    self.nombre = ko.observable().extend({ required: true });
    self.correo = ko.observable().extend({ required: true, email: true });
    self.apellidoPaterno = ko.observable().extend({ required: true });
    self.apellidoMaterno = ko.observable().extend({ required: true });
    self.contrasena = ko.observable();
    self.catEstatus = ko.observableArray();
    self.Estatus = ko.observable().extend({ required: true });
    self.catRoles = ko.observableArray();
    self.Rol = ko.observable().extend({ required: true });

 

    self.inicializar = function () {
        $.ajax({
            url: "Usuarios/GetAll", // URL del método GetAll en tu API
            type: "GET",
            contentType: "application/json",
            success: function (d) {
                self.registros.removeAll();
                self.registros.push.apply(self.registros, d.datos);
                self.catEstatus.removeAll();
                self.catEstatus.push.apply(self.catEstatus, catEstatus);
                $("#divEdicion").modal("hide");
                //self.Limpiar();
                $.ajax({
                    url: "Usuarios/GetAllRoles", // URL del método GetAll en tu API
                    type: "GET",
                    contentType: "application/json",
                    success: function (d) {
                        self.catRoles.removeAll();
                        self.catRoles.push.apply(self.catRoles, d.datos);
                        $("#divEdicion").modal("hide");
                        self.Limpiar();
                    },
                    error: function (xhr, status, error) {
                        console.error("Error al obtener los datos: ", error);
                        alert("Error al obtener los datos: " + xhr.responseText);
                    }
                });
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                alert("Error al obtener los datos: " + xhr.responseText);
            }
        });
           
    }
    self.inicializar();

    self.Limpiar = function () {
        self.id(0);
        self.nombre("");
        self.correo("");
        self.apellidoPaterno("");
        self.apellidoMaterno("");
        self.contrasena("");
    }
    self.Agregar = function () {
        self.Limpiar();
        $("#divEdicion").modal("show");

    }
    self.Editar = function (usuario) {
        self.Limpiar();
        $("#divEdicion").modal("show");
        self.id(usuario.id);
        self.nombre(usuario.nombre);
        self.correo(usuario.correo);
        self.contrasena(usuario.contrasena);
        self.apellidoPaterno(usuario.apellidoPaterno);
        self.apellidoMaterno(usuario.apellidoMaterno);
        self.contrasena(usuario.contrasena);
        var rol = self.catRoles().find(function (r) {
            return r.id === usuario.rolId;
        });
        self.Rol(rol);
        var Estatus = self.catEstatus().find(function (r) {
            return r.IdEstatus === usuario.estatus;
        });
        self.Estatus(Estatus);
    }
    self.GuardarEditar = function () {

        var usuario = {
            Id : self.id(),
            Nombre: self.nombre(),
            ApellidoPaterno: self.apellidoPaterno(),
            ApellidoMaterno: self.apellidoMaterno(),
            Correo: self.correo(),
            Estatus: self.Estatus().IdEstatus,
            RolId: self.Rol().id,
            Contrasena: self.contrasena()
        }
        $.ajax({
            url: "Usuarios/Edit", // URL del método GetAll en tu API
            type: "PUT",
            contentType: "application/json",
            data: JSON.stringify(usuario),
            success: function (d) {
                self.inicializar();
                $("#divEdicion").modal("hide");

                $('#alertMessage').text(d.mensaje);
                $('#alertModalLabel').text("Solicitud exitosa");
                $("#alertModal").modal("show");
                self.Limpiar();
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                $('#alertMessage').text("Error al obtener los datos: " + xhr.responseText);
                $('#alertModalLabel').text("Error");
                $("#alertModal").modal("show");
            }
        });
    }
    self.GuardarNuevo = function () {
        if (self.id() != 0) {
        }
        else {
        }
        var usuario = {
            Nombre : self.nombre(),
            ApellidoPaterno : self.apellidoPaterno(),
            ApellidoMaterno : self.apellidoMaterno(),
            Correo: self.correo(),
            Estatus: self.Estatus().IdEstatus,
            RolId: self.Rol().id,
            Contrasena: self.contrasena()
        }
        $.ajax({
            url: "Usuarios/Create", // URL del método GetAll en tu API
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(usuario),
            success: function (d) {
                self.inicializar();
                $("#divEdicion").modal("hide");
                $('#alertMessage').text(d.mensaje);
                $('#alertModalLabel').text("Solicitud exitosa");
                $("#alertModal").modal("show");
                self.Limpiar();
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                $('#alertMessage').text("Error al obtener los datos: " + xhr.responseText);
                $('#alertModalLabel').text("Error");
                $("#alertModal").modal("show");
            }
        });
    }
    self.Guardar = function () {
        if (self.id() === 0) {
            self.GuardarNuevo();
        }
        else {
            self.GuardarEditar();
        }
        
    }
}

// Activar Knockout.js
ko.applyBindings(new AppViewModel());