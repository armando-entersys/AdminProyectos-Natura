    var estatusActivo = { IdEstatus: true, Estatus: "Activo" };
var estatusInactivo = { IdEstatus: false, Estatus: "Inactivo" };

var catEstatus = [estatusActivo, estatusInactivo];
var CatCatalogo = [{ Id: 1, Descripcion: "Audiencia" },
    { Id: 2, Descripcion: "TipoBreaf" },
    { Id: 3, Descripcion: "TipoAlerta" },
    { Id: 4, Descripcion: "Prioridad" },
    { Id: 5, Descripcion: "PCN" },
    { Id: 6, Descripcion: "EstatusMaterial" },
    { Id: 6, Descripcion: "EstatusBreaf" },
    { Id: 6, Descripcion: "Formato" },

                   ];
function AppViewModel() {
    var self = this;

    self.registros = ko.observableArray();

    self.id = ko.observable(0);
    self.Descripcion = ko.observable();

    self.catEstatus = ko.observableArray();
    self.Estatus = ko.observable();

    self.CatCatalogo = ko.observableArray();
    self.Catalogo = ko.observable();

    self.inicializar = function () {
       
        self.catEstatus.removeAll();
        self.catEstatus.push.apply(self.catEstatus, catEstatus);

        self.CatCatalogo.removeAll();
        self.CatCatalogo.push.apply(self.CatCatalogo, CatCatalogo);
           
    }
    self.inicializar();
    // Función para manejar el cambio de selección
    self.onCatalogoChange = function () {
        var seleccionado = self.Catalogo(); // Obtiene el objeto seleccionado
        if (seleccionado != null && seleccionado!= undefined ) {
            // Llama al servicio con la información del catálogo seleccionado
            $.ajax({
                url: "Catalogos/GetCatalogoInfo?nombreCatalogo=" + seleccionado.Descripcion, // URL del servicio
                type: "GET",
                contentType: "application/json",
                success: function (d) {
                    self.registros.removeAll();
                    self.registros.push.apply(self.registros, d.datos);
                },
                error: function (xhr, status, error) {
                    console.error("Error en servicio web:", error);
                }
            });
        }
    };
    self.Limpiar = function () {
        self.id(0);
        self.Descripcion("");
        self.Estatus(false);
    }
    self.Agregar = function () {
        self.Limpiar();
        $("#divEdicion").modal("show");

    }
    self.Editar = function (catalogo) {
        self.Limpiar();
        $("#divEdicion").modal("show");
        self.id(catalogo.id);
        self.Descripcion(catalogo.descripcion);
        var Estatus = self.catEstatus().find(function (r) {
            return r.IdEstatus === catalogo.activo;
        });
        self.Estatus(Estatus);
    }
    self.GuardarEditar = function () {
        if (tipo = "Audiencia") {
        }
      
        var Audiencia = {
            Id : self.id(),
            Descripcion: self.Descripcion(),
            Activo: self.Estatus().IdEstatus,
        }
        $.ajax({
            url: "Catalogos/CreateAudiencia", // URL del método GetAll en tu API
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(Audiencia),
            success: function (d) {
                self.onCatalogoChange();
                $("#divEdicion").modal("hide");

                $('#alertMessage').text(d.mensaje);
                $('#alertModalLabel').text("Success");
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
        
        var audiencia = {
            Id: self.id(),
            Descripcion: self.Descripcion(),
            Activo: self.Estatus().IdEstatus,
        }
        $.ajax({
            url: "Catalogos/CreateAudiencia", // URL del método GetAll en tu API
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(audiencia),
            success: function (d) {
                self.onCatalogoChange();
                $("#divEdicion").modal("hide");
                $('#alertMessage').text(d.mensaje);
                $('#alertModalLabel').text("Success");
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
    self.Eliminar = function (elemento) {
        var peticionCatalogos = {
            Id: elemento.id,
            nombreCatalogo:"Audiencia"
        }
        $.ajax({
            url: "Catalogos/Delete", // URL del método GetAll en tu API
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(peticionCatalogos),
            success: function (d) {
                self.onCatalogoChange();
                $("#divEdicion").modal("hide");
                $('#alertMessage').text(d.mensaje);
                $('#alertModalLabel').text("Success");
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
}

// Activar Knockout.js
ko.applyBindings(new AppViewModel());